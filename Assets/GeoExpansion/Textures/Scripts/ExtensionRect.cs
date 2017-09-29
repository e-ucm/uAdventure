using UnityEngine;

/// <summary>
/// The ExtensionRect class hods up multiple util operations for rectangles.
/// </summary>
public static class ExtensionRect
{
    /// <summary>
    /// Traps a rect inside another rect. If its ouside it moves it to the closest point inside
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static Rect TrapInside(this Rect rect, Rect other)
    {
        return new Rect(
            Mathf.Clamp(rect.x, other.x, other.x + other.width - rect.width),
            Mathf.Clamp(rect.y, other.y, other.y + other.height - rect.height),
            rect.width, rect.height);
    }


    public static Rect AdjustToRatio(this Rect rect, float width, float height)
    {
        return rect.AdjustToRatio(width / height);
    }

    public static Rect AdjustToRatio(this Rect rect, float ratio)
    {
        var r = new Rect(rect);
        var originalCenter = r.center;

        var myRatio = r.width / r.height;
        if(myRatio > ratio) r.width = r.height * ratio;
        else r.height = r.width / ratio;

        r.center = originalCenter;
        return r;
    }

    public static Rect AdjustToViewport(this Rect rect, float originalWidth, float originalHeight, Rect viewport)
    {
        return new Rect(
                (rect.x / originalWidth) * viewport.width + viewport.x,
                (rect.y / originalHeight) * viewport.height + viewport.y,
                (rect.width / originalWidth) * viewport.width,
                (rect.height / originalHeight) * viewport.height
            );
    }

    public static Rect ViewportToScreen(this Rect rect, float screenWidth, float screenHeight, Rect viewport)
    {
        return new Rect(
                ((rect.x - viewport.x) / viewport.width) * screenWidth,
                ((rect.y - viewport.y) / viewport.height) * screenHeight,
                (rect.width / viewport.width) * screenWidth,
                (rect.height / viewport.height) * screenHeight
            );
    }

    /// <summary>
    /// Performs the intersection of two Rects.
    /// </summary>
    /// <param name="rect">First rect</param>
    /// <param name="other">Second rect</param>
    /// <returns>Intersected rect</returns>
    public static Rect Intersection(this Rect rect, Rect other)
    {
        Vector2 r0o = rect.position,
            r0e = rect.position + rect.size,
            r1o = other.position,
            r1e = other.position + other.size;

        return FromCorners(
            new Vector2(Mathf.Max(r0o.x, r1o.x), Mathf.Max(r0o.y, r1o.y)),
            new Vector2(Mathf.Min(r0e.x, r1e.x), Mathf.Min(r0e.y, r1e.y)));

    }

    /// <summary>
    /// Transforms a rect to TexCoords using another rect that represents the drawing region.
    /// </summary>
    /// <param name="rect">Rect that occupies the whole texture</param>
    /// <param name="slice">Slice to get TexCoords of</param>
    /// <returns>TexCoords</returns>
    public static Rect ToTexCoords(this Rect rect, Rect slice)
    {
        return new Rect(
            (slice.x - rect.x) / rect.width,
            1f - (((slice.y + slice.height) - (rect.y + rect.height)) / rect.height),
            slice.width / rect.width,
            slice.height / rect.height
            );
    }

    /// <summary>
    /// Creates a rectangle using two points.
    /// </summary>
    /// <param name="o">Origin</param>
    /// <param name="e">End</param>
    /// <returns>New Rect</returns>
    public static Rect FromCorners(Vector2 o, Vector2 e)
    {
        return new Rect(o, e - o);
    }

    /// <summary>
    /// Moves a rectangle position the passed offset.
    /// </summary>
    /// <param name="target">Rect to move</param>
    /// <param name="move">Distance to move</param>
    /// <returns>Moved rect</returns>
    public static Rect Move(this Rect target, Vector2 move)
    {
        return new Rect(move.x + target.x, move.y + target.y, target.width, target.height);
    }

    /// <summary>
    /// Adapts a rect to the GUI, moving it from absolute coords, to relative and clapping the overflows.
    /// </summary>
    /// <param name="rect">The rect to move</param>
    /// <param name="guiSpace">The region of the parent component</param>
    /// <returns>Rect adapted to relative GUI space</returns>
    public static Rect GUIAdapt(this Rect rect, Rect guiSpace)
    {
        return rect.Move(guiSpace.position).Intersection(guiSpace);
    }
}
