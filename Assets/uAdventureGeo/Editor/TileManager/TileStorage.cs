using System.Collections.Generic;
using MapzenGo.Helpers;
using UniRx;
using UnityEngine;
using System.Linq;
using System.Threading;
using uAdventure.Core;
using uAdventure.Editor;
using UnityEditor;
using System;
using Action = uAdventure.Core.Action;

namespace uAdventure.Geo
{
    public class ProgressCallback
    {
        private readonly ProgressController progressController;
        private readonly Action<int, ProgressController, bool> callback;

        public ProgressCallback(int todo, Action<int, ProgressController, bool> callback)
        {
            this.progressController = new ProgressController(todo);
            this.callback = callback;
        }

        public void Update(int fase, bool result)
        {
            progressController.Step();
            callback(fase, progressController, result);
        }
    }

    public class TileStorage
    {

        #region Singleton
        // ##################################

        private static TileStorage instance;

        public static TileStorage Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TileStorage();
                }

                return instance;
            }
        }

        // ##################################
        #endregion

        private class StorageProcess
        {
            public ITileMeta toDownloadMeta;
            public ITileMeta toRemoveMeta;
            public List<ITilePromise> toStorePromises;
            public Dictionary<Vector3d, ITileStorer> toDownloadTiles;
            public Dictionary<Vector3d, ITileStorer> toRemoveTiles;
        }

        private readonly List<MapSceneDataControl> auxList;
        private readonly List<ITileStorer> tileStorers;
        private const int MaxDownloadSlots = 15;
        private readonly Dictionary<MapSceneDataControl, bool> updatingStorage;
        private readonly Dictionary<MapSceneDataControl, StorageProcess> storageProcesses;

        protected TileStorage()
        {
            auxList = new List<MapSceneDataControl>(1) { null };
            updatingStorage = new Dictionary<MapSceneDataControl, bool>();
            storageProcesses = new Dictionary<MapSceneDataControl, StorageProcess>();
            tileStorers = new List<ITileStorer>
            {
                new PngTextureStorer()
            };
        }

        public void StoreTiles(MapSceneDataControl mapSceneDataControl, bool displayProgressBar, System.Action<bool> callback)
        {
            StartProcess(mapSceneDataControl);

            if (displayProgressBar)
            {
                EditorUtility.DisplayProgressBar("Geo.TileManager.Progress.Title".Traslate(),
                    "Geo.TileManager.Calculating".Traslate(), 0);
            }

            var allAreas = GetAllAreasExcept(mapSceneDataControl);
            var mapScene = mapSceneDataControl.getContent() as MapScene;
            var previousArea = new AreaMeta(mapScene);
            var newArea = new AreaMeta(mapSceneDataControl);
            var current = new StorageProcess();
            storageProcesses[mapSceneDataControl] = current;

            // Get the tile metas to remove or to download if they exist
            current.toRemoveMeta = TileProvider.Instance.GetTileMeta(mapScene.TileMetaIdentifier);
            current.toDownloadMeta = TileProvider.Instance.GetTileMeta(mapSceneDataControl.GameplayArea.TileMetaIdentifier);

            // We have to remove if there is gameplayArea based on the previous area and we must substract the new area if it's used
            current.toRemoveTiles = GetNonCollidingStorableTiles(mapScene.UsesGameplayArea, allAreas, previousArea,
                mapSceneDataControl.GameplayArea.UsesGameplayArea, newArea, current.toRemoveMeta);

            // We have to download if there is gameplayArea based on the new area and we must substract the old area if it's used
            current.toDownloadTiles = GetNonCollidingStorableTiles(mapSceneDataControl.GameplayArea.UsesGameplayArea, allAreas,
                newArea, mapScene.UsesGameplayArea, previousArea, current.toDownloadMeta);

            // In case any of them is null it means that we have no storer to use for any of the tiles
            if (current.toRemoveTiles == null || current.toDownloadTiles == null)
            {
                FinishProcess(mapSceneDataControl);
                callback(false);
                return;
            }
            
            RemoveAndDownloadTiles(mapSceneDataControl, new ProgressCallback(current.toRemoveTiles.Count + current.toDownloadTiles.Count *2, 
                (fase, progress, result) =>
                {
                    bool cancel = false;
                    if (displayProgressBar)
                    {
                        if (fase == 0)
                        {
                            EditorUtility.DisplayProgressBar("Geo.TileManager.Progress.Title".Traslate(),
                                "Geo.TileManager.Removing".Traslate(), progress.Progress);
                        }
                        else if(fase == 1)
                        {
                            cancel = EditorUtility.DisplayCancelableProgressBar("Geo.TileManager.Progress.Title".Traslate(),
                                "Geo.TileManager.Downloading".Traslate(), progress.Progress);
                        }
                        else if (fase == 2)
                        {
                            cancel = EditorUtility.DisplayCancelableProgressBar("Geo.TileManager.Progress.Title".Traslate(),
                                "Geo.TileManager.Storing".Traslate(), progress.Progress);
                        }
                    }

                    if ((fase != 0 && !result) || cancel)
                    {
                        FinishProcess(mapSceneDataControl);
                        callback(false);
                    }

                    if (progress.Progress >= 1f)
                    {
                        FinishProcess(mapSceneDataControl);
                        callback(true);
                    }
                }).Update);
        }

        private void RemoveAndDownloadTiles(MapSceneDataControl mapSceneDataControl, System.Action<int, bool> update)
        {
            var current = storageProcesses[mapSceneDataControl];

            var toRemove = current.toRemoveTiles.Keys.ToArray();

            AssetDatabase.StartAssetEditing();
            foreach (var tile in toRemove)
            {
                // Remove the tile
                update(0, RemoveTile(current.toRemoveTiles[tile], tile, current.toRemoveMeta));
            }
            AssetDatabase.StopAssetEditing();

            var toDownload = current.toDownloadTiles.Keys.ToArray();
            current.toStorePromises = new List<ITilePromise>();
            if (toDownload.Length > 0)
            {
                var downloadChunk = toDownload.Length / (float)MaxDownloadSlots;
                for (int i = 0; i < MaxDownloadSlots; i++)
                {
                    // Download the tile
                    DownloadWaterfall(current.toDownloadTiles, mapSceneDataControl, toDownload, current.toDownloadMeta, 
                        Mathf.CeilToInt(i * downloadChunk), Mathf.CeilToInt((i + 1) * downloadChunk), 0,
                        (currentMapScene, tileIndex, tilePromise, result) =>
                        {
                            var process = storageProcesses[currentMapScene];
                            var promises = process.toStorePromises;
                            if (!result)
                            {
                                Debug.Log("Failed to download tile " + toDownload[tileIndex]);
                            }
                            update(1, result);
                            promises.Add(tilePromise);
                            if (promises.Count == toDownload.Length)
                            {
                                AssetDatabase.StartAssetEditing();
                                foreach (var promise in promises)
                                {
                                    result = StoreTile(process.toDownloadTiles[promise.Tile], promise.Tile,
                                        promise.Meta, promise);
                                    update(2, result);
                                }
                                AssetDatabase.StopAssetEditing();
                            }
                        });
                }
            }
            else
            {
                update(2, true);
            }
        }

        private bool RemoveTile(ITileStorer tileStorer, Vector3d tile, ITileMeta tileMeta)
        {
            if (!tileStorer.DeleteTile(tile, tileMeta))
            {
                Debug.Log("Cannot delete tile " + tile + " of " + tileMeta.Identifier);
                return false;
            }

            return true;
        }

        private void DownloadWaterfall(Dictionary<Vector3d, ITileStorer> tileStorer, MapSceneDataControl mapScene, Vector3d[] tiles, 
            ITileMeta meta, int index, int endIndex, int tries, System.Action<MapSceneDataControl, int, ITilePromise, bool> finished)
        {
            if (index == endIndex || IsFinished(mapScene)) 
            {
                 return;
            }

            if (tries < 5)
            {
                DownloadTile(tiles[index], meta, tp =>
                {
                    if (tp.Data == null)
                    {
                        Debug.Log("Failed to download the tile" + tiles[index] + ", retrying... (" + tries + ")");
                        DownloadWaterfall(tileStorer, mapScene, tiles, meta, index, endIndex, tries+1, finished);
                    }
                    else
                    {
                        finished(mapScene, index, tp, true);
                        DownloadWaterfall(tileStorer, mapScene, tiles, meta, index + 1, endIndex, 0, finished);
                    }
                });
            }
            else
            {
                finished(mapScene, index, null, false);
            }
        }

        private void DownloadTile(Vector2d tile, ITileMeta meta, System.Action<ITilePromise> result)
        {
            TileProvider.Instance.GetTile(new Vector3d(tile.x, tile.y, 19), meta, (tilePromise) =>
            {
                result(tilePromise);
            });
        }

        private bool StoreTile(ITileStorer tileStorer, Vector3d tile, ITileMeta tileMeta, ITilePromise tilePromise)
        {
            if (!tileStorer.StoreTile(tile, tileMeta, tilePromise))
            {
                Debug.Log("Cannot store tile " + tile + " of " + tileMeta.Identifier);
                return false;
            }

            return true;
        }

        #region Tile Util
        // #########################################
        private List<AreaMeta> GetAllAreasExcept(MapSceneDataControl mapSceneDataControl)
        {
            var mapScenes = GeoController.Instance.MapScenes;

            auxList[0] = mapSceneDataControl;

            var allAreas = mapScenes.DataControls
                .Except(auxList) // Except mapSceneDataControl
                .Where(m => m.GameplayArea.UsesGameplayArea)
                .Select(m => new AreaMeta(m)).ToList();
            return allAreas;
        }

        private Dictionary<Vector3d, ITileStorer> GetNonCollidingStorableTiles(bool useArea, List<AreaMeta> allAreas, AreaMeta area, bool useExtraArea, AreaMeta extraArea, ITileMeta tileMeta)
        {
            var storableTiles = new Dictionary<Vector3d, ITileStorer>();

            if (!useArea)
            {
                return storableTiles;
            }

            var nonCollidingTiles = TileUtil.NonCollidingTiles(area, allAreas, extraArea, useExtraArea).ToList();

            foreach (var tile in nonCollidingTiles)
            {
                var tile3d = new Vector3d(tile.x, tile.y, 19);
                var tileStorer = tileStorers.Find(ts => ts.CanStoreTile(tile3d, tileMeta));
                if (tileStorer == null)
                {
                    Debug.LogError("Cannot find valid storer for tile " + tile3d);
                    return null;
                }

                storableTiles.Add(tile3d, tileStorer);
            }

            return storableTiles;
        }
        // #########################################
        #endregion

        #region Process
        // #########################################

        private void StartProcess(MapSceneDataControl mapSceneDataControl)
        {
            if (updatingStorage.ContainsKey(mapSceneDataControl) && updatingStorage[mapSceneDataControl])
            {
                Debug.Log("The data was already being updated!");
                return;
            }

            updatingStorage[mapSceneDataControl] = true;
        }

        private void FinishProcess(MapSceneDataControl mapSceneDataControl)
        {
            updatingStorage[mapSceneDataControl] = false;
            EditorUtility.ClearProgressBar();
        }

        private bool IsFinished(MapSceneDataControl mapSceneDataControl)
        {
            return !updatingStorage[mapSceneDataControl];
        }

        // #########################################
        #endregion
    }
}
