using UnityEngine;
using System.Collections;
using TriangleNet.Geometry;
using MapzenGo.Helpers;
using TriangleNet;
using uAdventure.Geo;
using MapzenGo.Models;
using System.Collections.Generic;
using System.Linq;

public class GeoElementMB : MonoBehaviour {

    public Tile Tile { get; set; }
    public GeoElement Element { get; set; }
    public GeoReference Reference { get; set; }

    // Use this for initialization
    void Start () {
        
        var inp = new InputGeometry(Element.Geometry.Points.Count);
        int i = 0;
        foreach (var p in Element.Geometry.Points)
        {
            var dotMerc = GM.LatLonToMeters(p.y, p.x);
            var localMercPos = dotMerc - Tile.Rect.Center;
            inp.AddPoint(localMercPos.x, localMercPos.y);
            inp.AddSegment(i, (i + 1) % Element.Geometry.Points.Count);
            i++;
        }
        
        var md = new MeshData();
        var mesh = GetComponent<MeshFilter>().mesh;
        
        CreateMesh(inp, md);

        //I want object center to be in the middle of object, not at the corner of the tile
        var center = ChangeToRelativePositions(md.Vertices);
        transform.localPosition = center;

        mesh.vertices = md.Vertices.ToArray();
        mesh.triangles = md.Indices.ToArray();
        mesh.SetUVs(0, md.UV);
        mesh.RecalculateNormals();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    private void CreateMesh(InputGeometry corners, MeshData meshdata)
    {
        var mesh = new TriangleNet.Mesh();
        mesh.Behavior.Algorithm = TriangulationAlgorithm.SweepLine;
        mesh.Behavior.Quality = true;
        mesh.Triangulate(corners);

        var vertsStartCount = meshdata.Vertices.Count;
        meshdata.Vertices.AddRange(corners.Points.Select(x => new Vector3((float)x.X, 0, (float)x.Y)).ToList());
        meshdata.UV.AddRange(corners.Points.Select(x => new Vector2((float)x.X, (float)x.Y)).ToList());
        foreach (var tri in mesh.Triangles)
        {
            meshdata.Indices.Add(vertsStartCount + tri.P1);
            meshdata.Indices.Add(vertsStartCount + tri.P0);
            meshdata.Indices.Add(vertsStartCount + tri.P2);
        }
    }

    private Vector3 ChangeToRelativePositions(List<Vector3> landuseCorners)
    {
        var landuseCenter = landuseCorners.Aggregate((acc, cur) => acc + cur) / landuseCorners.Count;
        for (int i = 0; i < landuseCorners.Count; i++)
        {
            //using corner position relative to landuse center
            landuseCorners[i] = landuseCorners[i] - landuseCenter;
        }
        return landuseCenter;
    }
}
