using System;
using System.Collections;
using MapzenGo.Models.Plugins;
using UnityEngine;
using MapzenGo.Models;

public class MapImagePlugin : Plugin
{

    public TileServices TileService = TileServices.Default;

    protected override IEnumerator CreateRoutine(Tile tile, Action<bool> finished)
    {

        var go = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;
        go.name = "map";
        go.SetParent(tile.transform, true);
        go.localScale = new Vector3((float)tile.Rect.Width, (float)tile.Rect.Width, 1);
        go.rotation = Quaternion.Euler(new Vector3(0, 0, 0));//.AngleAxis(90, new Vector3(1, 0, 0));
        go.localPosition = Vector3.zero;
        go.localPosition -= new Vector3(0, 1, 0);
        var rend = go.GetComponent<Renderer>();
        rend.material = tile.Material;

        TileProvider.GetTile(new Vector3d(tile.TileTms.x, tile.TileTms.y, tile.Zoom), (texture) =>
        {
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

