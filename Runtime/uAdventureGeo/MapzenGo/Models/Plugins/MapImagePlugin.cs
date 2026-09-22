using System;
using System.Collections;
using MapzenGo.Models.Plugins;
using UnityEngine;
using MapzenGo.Models;
using uAdventure.Runner;

namespace uAdventure.Geo
{
    public class MapImagePlugin : Plugin
    {
        private ITileMeta defaultTileMeta = new OsmMeta();

        protected override IEnumerator CreateRoutine(Tile tile, Action<bool> finished)
        {
            var mapScene = Game.Instance.CurrentTargetRunner.Data as MapScene;
            var tileMeta = TileProvider.Instance.GetTileMeta(mapScene.TileMetaIdentifier) ?? defaultTileMeta;
            var go = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;
            go.name = "map";
            go.SetParent(tile.transform, true);
            go.localScale = new Vector3((float)tile.Rect.Width, (float)tile.Rect.Width, 1);
            go.localRotation = Quaternion.AngleAxis(90, new Vector3(1, 0, 0));
            go.localPosition = Vector3.zero;
            go.localPosition -= new Vector3(0, 1, 0);
            var rend = go.GetComponent<Renderer>();
            rend.material = tile.Material;
            TileProvider.Instance.GetTile(new Vector3d(tile.TileTms.x, tile.TileTms.y, tile.Zoom), tileMeta, (tilePromise) =>
            {
                var texture = tilePromise.Data as Texture2D;
                if (texture)
                {
                    if (rend)
                    {
                        rend.material.mainTexture = texture;
                        rend.material.color = new Color(1f, 1f, 1f, 1f);

                        finished(true);
                    }
                }
                else
                {
                    finished(false);
                }
            });

            yield return null;
        }
    }
}