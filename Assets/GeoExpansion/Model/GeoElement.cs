using UnityEngine;
using System.Collections;

public class GeoElement  {

    public GeoElement(string id)
    {
        Id = id;
        Geometry = new GMLGeometry();
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public GMLGeometry Geometry { get; set; }

    public GeoElementDrawer Drawer { get; set; }
    public float Influence { get; set; }
    public string Image { get; set; }
}

public interface GeoElementDrawer
{
    void Init(GeoElement element);
    void Update();
}
