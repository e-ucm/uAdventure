using UnityEngine;
using System.Collections;

public class GeoElement  {

    public string Name { get; set; }
    public string Description { get; set; }
    public GMLGeometry Geometry { get; set; }

    public GeoElementDrawer Drawer { get; set; }
}

public interface GeoElementDrawer
{
    void Init(GeoElement element);
    void Update();
}
