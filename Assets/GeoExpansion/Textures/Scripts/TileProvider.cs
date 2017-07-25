using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;

public enum TileServices
{
    Default,
    Satellite,
    Terrain,
    Toner,
    Watercolor
}

/**
* Tile promise class is used to provide a tile to draw that will be download 
* later on.
*/
public class TilePromise
{
    private static Texture2D loadingTexture;

    // Zoom in X, Tile in Y and Z
    private Vector3d tileZoom;
    private Texture2D texture;

    public TilePromise()
    {
        if (!loadingTexture)
        {
            loadingTexture = Resources.Load<Texture2D>("tile_loading");
        }
    }

    public Vector3d TileZoom { get; set; }
    public Texture2D Texture { get { return texture ? texture : loadingTexture; } set { texture = value; } }
    
}

public class TileProvider {

    private static string[] TileServiceUrls = new string[] {
            "http://b.tile.openstreetmap.org/",
            "http://b.tile.openstreetmap.us/usgs_large_scale/",
            "http://tile.stamen.com/terrain-background/",
            "http://a.tile.stamen.com/toner/",
            "https://stamen-tiles.a.ssl.fastly.net/watercolor/"
        };

    public static TileServices TileService { get; set; }
    
    private static Dictionary<Vector3d, TilePromise> tileCache;

    public static TilePromise GetTile(Vector3d tileZoom, Action<Texture2D> callback)
    {
        if (tileCache == null)
            tileCache = new Dictionary<Vector3d, TilePromise>();

        var url = TileServiceUrls[(int)TileService] + tileZoom.z + "/" + tileZoom.x + "/" + tileZoom.y + ".png";
        if (tileCache.ContainsKey(tileZoom))
        {
            callback(tileCache[tileZoom].Texture);
            return tileCache[tileZoom];
        }
        else
        {
            TilePromise tp = new TilePromise();
            tp.TileZoom = tileZoom;
            tileCache.Add(tileZoom, tp);

            ObservableWWW.GetWWW(url).Subscribe(
            success =>
            {
                var texture = new Texture2D(512, 512, TextureFormat.DXT5, false);
                success.LoadImageIntoTexture(texture);
                tp.Texture = texture;
                callback(texture);
            },
            error =>
            {
                // TODO error texture
                Debug.Log(error);
                callback(null);
            });

            return tp;
        }

        /*if (Application.isPlaying)
        {*/

        //}


    }
}
