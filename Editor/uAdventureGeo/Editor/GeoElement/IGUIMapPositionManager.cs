using System;
using uAdventure.Editor;
using UnityEngine;

namespace uAdventure.Geo
{
    /// <summary>
    /// GeoReferenceTransformManager interface for creating new positioning types
    /// </summary>
    public interface IGuiMapPositionManager : IObserver<DataControl>
    {
        Type ForType { get; }

        /// <summary>
        /// Texture of the referenced element to update
        /// </summary>
        Texture2D Texture { get; set; }

        /// <summary>
        /// Configures the Transform Manager
        /// </summary>
        /// <param name="parameters">Dictionary of parameters to extract</param>
        void Configure(TransformManagerDataControl transformManagerDataControl);

        /// <summary>
        /// Transforms a local point to a screen point
        /// </summary>
        Vector2d ToScreenPoint(MapEditor mapEditor, Vector2d point);

        /// <summary>
        /// Transforms a screen point to a local point
        /// </summary>
        Vector2d FromScreenPoint(MapEditor mapEditor, Vector2d point);

        /// <summary>
        /// Checks if the controls might get selected
        /// </summary>
        bool IsGizmosSelected(MapEditor mapEditor, Vector2d[] points);

        /// <summary>
        /// Draws the neccesary controls
        /// </summary>
        void OnDrawingGizmosSelected(MapEditor mapEditor, Vector2d[] points);
    }
}
