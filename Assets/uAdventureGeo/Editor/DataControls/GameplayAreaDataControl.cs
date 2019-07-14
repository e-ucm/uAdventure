using System;
using System.Collections.Generic;
using uAdventure.Core;
using uAdventure.Editor;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Geo
{
    public class GameplayAreaDataControl : DataControl
    {
        private class GameplayArea
        {
            public bool UsesGameplayArea { get; set; }
            public RectD BoundingBox { get; set; }
            public string TileMetaIdentifier { get; set; }
        }

        private readonly MapSceneDataControl mapSceneDataControl;
        private readonly MapScene mapScene;
        private readonly GameplayArea gameplayArea;

        public GameplayAreaDataControl(MapSceneDataControl mapSceneDataControl)
        {
            this.mapSceneDataControl = mapSceneDataControl;
            this.mapScene = mapSceneDataControl.getContent() as MapScene;
            this.gameplayArea = new GameplayArea
            {
                UsesGameplayArea = mapScene.UsesGameplayArea,
                BoundingBox = mapScene.GameplayArea,
                TileMetaIdentifier = mapScene.TileMetaIdentifier
            };
        }

        public bool UsesGameplayArea
        {
            get { return gameplayArea.UsesGameplayArea; }
            set
            {
                if (gameplayArea.UsesGameplayArea == value)
                {
                    return;
                }

                if (controller.AddTool(new ChangeValueTool<GameplayArea, bool>(gameplayArea, value, "UsesGameplayArea")) && MapEditor.Current != null && value)
                {
                    var mapCenter = MapEditor.Current.Center;
                    var size = new Vector2d(0.02, 0.02);
                    gameplayArea.BoundingBox = new RectD(mapCenter - size / 2d, size);
                }
            }
        }

        public RectD BoundingBox
        {
            get { return gameplayArea.BoundingBox; }
            set { controller.AddTool(new ChangeValueTool<GameplayArea, RectD>(gameplayArea, value, "BoundingBox")); }
        }
        
        public string TileMetaIdentifier
        {
            get { return gameplayArea.TileMetaIdentifier; }
            set { controller.AddTool(new ChangeValueTool<GameplayArea, string>(gameplayArea, value, "TileMetaIdentifier")); }
        }

        public bool IsModified
        {
            get
            {
                return gameplayArea.UsesGameplayArea != mapScene.UsesGameplayArea ||
                       gameplayArea.BoundingBox != mapScene.GameplayArea ||
                       gameplayArea.TileMetaIdentifier != mapScene.TileMetaIdentifier;
            }
        }

        public void DownloadAndApplyChanges()
        {
            if (!IsModified)
            {
                EditorUtility.DisplayDialog("Geo.NoDownloadRequired.Title".Traslate(),
                    "Geo.NoDownloadRequired.Message".Traslate(), "GeneralText.OK".Traslate());
                return;
            }

            if (!EditorUtility.DisplayDialog("Geo.CantUndo.Title".Traslate(),
                "Geo.CantUndo.Message".Traslate(), "GeneralText.OK".Traslate(), "GeneralText.No".Traslate()))
            {
                return;
            }
            EditorWindowBase.LockWindow(); 
            TileStorage.Instance.StoreTiles(mapSceneDataControl, true, result =>
            {
                if (result)
                { 
                    mapScene.GameplayArea = gameplayArea.BoundingBox;
                    mapScene.UsesGameplayArea = gameplayArea.UsesGameplayArea;
                    mapScene.TileMetaIdentifier = gameplayArea.TileMetaIdentifier;
                    Controller.Instance.Save(); // Force the save after we finish
                    EditorUtility.DisplayDialog("Geo.TileManager.Progress.Title".Traslate(),
                        "Geo.TileManager.Progress.Dones".Traslate(), "GeneralText.OK".Traslate());
                }
                else
                {
                    EditorUtility.DisplayDialog("Geo.TileManager.Progress.Title".Traslate(),
                        "Geo.TileManager.Progress.Failed".Traslate(), "GeneralText.OK".Traslate());
                }

                EditorWindowBase.UnlockWindow();
            });
        }


        public override bool addElement(int type, string id)
        {
            return false;
        }

        public override bool canAddElement(int type)
        {
            return false;
        }

        public override bool canBeDeleted()
        {
            return true;
        }

        public override bool canBeDuplicated()
        {
            return true;
        }

        public override bool canBeMoved()
        {
            return true;
        }

        public override bool canBeRenamed()
        {
            return true;
        }

        public override int countAssetReferences(string assetPath)
        {
            return 0;
        }

        public override int countIdentifierReferences(string id)
        {
            return 0;
        }

        public override void deleteAssetReferences(string assetPath)
        {
        }

        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {
            return false;
        }

        public override void deleteIdentifierReferences(string id)
        {
        }

        public override int[] getAddableElements()
        {
            return null;
        }

        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {
        }

        public override object getContent()
        {
            return mapScene;
        }

        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {
            return null;
        }

        public override bool isValid(string currentPath, List<string> incidences)
        {
            return true;
        }

        public override bool moveElementDown(DataControl dataControl)
        {
            return false;
        }

        public override bool moveElementUp(DataControl dataControl)
        {
            return false;
        }

        public override void recursiveSearch()
        {
        }

        public override string renameElement(string newName)
        {
            return null;
        }

        public override void replaceIdentifierReferences(string oldId, string newId)
        {
        }

        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {
        }
    }
}
