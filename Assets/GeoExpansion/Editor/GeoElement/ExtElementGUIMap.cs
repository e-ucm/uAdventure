using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using uAdventure.Geo;
using System;
using MapzenGo.Helpers;
using UnityEditor;

namespace uAdventure.Editor
{


    public class ExtElemReferenceGUIMapPositionManagerFactory
    {
        private static ExtElemReferenceGUIMapPositionManagerFactory instance;
        public static ExtElemReferenceGUIMapPositionManagerFactory Instance
        {
            get
            {
                if (instance == null)
                    instance = new ExtElemReferenceGUIMapPositionManagerFactory();
                return instance;
            }
        }
        private List<ExtElemReferenceGUIMapPositionManager> guiMapPositionManagers;
        private ExtElemReferenceGUIMapPositionManagerFactory()
        {
            guiMapPositionManagers = new List<ExtElemReferenceGUIMapPositionManager>();

            // Add guimap position managers here
            guiMapPositionManagers.Add(new GeoPositionedGUIMapPositionManager());
            guiMapPositionManagers.Add(new ScreenPositionedGUIMapPositionManager());
            guiMapPositionManagers.Add(new RadialCenterGUIMapPositionManager());
        }

        public ExtElemReferenceGUIMapPositionManager CreateInstance(ExtElemReferenceTransformManagerDescriptor element, ExtElemReference reference)
        {
            var elem = (ExtElemReferenceGUIMapPositionManager)Activator.CreateInstance(guiMapPositionManagers.Find(g => g.ForType == element.Type).GetType());
            elem.Configure(reference);
            return elem;
        }
    }

    /// <summary>
    /// GeoReferenceTransformManager interface for creating new positioning types
    /// </summary>
    public interface ExtElemReferenceGUIMapPositionManager
    {
        Type ForType { get; }

        /// <summary>
        /// Transform of the referenced element to update
        /// </summary>
        Texture2D Texture { get; set; }

        /// <summary>
        /// Allows the ExtElemReference to reposition the element
        /// </summary>
        void Draw(GUIMap map, Rect area);

        /// <summary>
        /// Configures the Transform Manager
        /// </summary>
        /// <param name="parameters">Dictionary of parameters to extract</param>
        void Configure(ExtElemReference reference);

        /// <summary>
        /// Updates the element positioning based on the map and the scene rect
        /// </summary>
        /// <param name="map">Map</param>
        /// <param name="area">MapScene rect</param>
        void Repositionate(GUIMap map, Rect area);
    }

    public static class GUIMapPositionManagerUtility
    {
        public static void InsertDefaults(ExtElemReferenceTransformManagerDescriptor descriptor, Dictionary<string, object> into, bool overriding)
        {
            foreach(var param in descriptor.ParameterDescription)
            {
                if (!into.ContainsKey(param.Key))
                {
                    into.Add(param.Key, param.Value.DefaultValue);
                }
                else if (overriding)
                {
                    into[param.Key] = param.Value.DefaultValue;
                }
            }
        }
    }


    public class GeoPositionedGUIMapPositionManager : ExtElemReferenceGUIMapPositionManager
    {
        private Vector2d position;
        private Vector3 scale;
        private float rotation;
        private ExtElemReference reference;
        private float interactionRange;

        public Texture2D Texture { get; set; }
        public Type ForType { get { return typeof(GeoPositionedTransformManager); } }

        public void Configure(ExtElemReference reference)
        {
            this.reference = reference;
            GUIMapPositionManagerUtility.InsertDefaults(reference.TransformManagerDescriptor, reference.TransformManagerParameters, false);
            UpdateValues();
        }

        private void UpdateValues()
        {

            var parameters = reference.TransformManagerParameters;

            object position;
            parameters.TryGetValue("position", out position);
            if (position == null) parameters.TryGetValue("Position", out position);

            if (position != null)
            {
                if (position is Vector2d) this.position = (Vector2d)position;
                else if (position is Vector2) this.position = ((Vector2)position).ToVector2d();
            }


            object scale;
            parameters.TryGetValue("scale", out scale);
            if (scale == null) parameters.TryGetValue("Scale", out scale);

            if (scale != null)
            {
                if (scale is float) this.scale = Vector3.one * ((float)scale);
                else if (scale is Vector2) this.scale = new Vector3(((Vector2)scale).x, ((Vector2)scale).y, ((Vector2)scale).x);
                else if (scale is Vector3) this.scale = (Vector3)scale;
            }

            object rotation;
            parameters.TryGetValue("rotation", out rotation);
            if (rotation == null) parameters.TryGetValue("Rotation", out rotation);

            if (rotation != null)
            {
                if (rotation is float) this.rotation = ((float)rotation);
            }

            object interactionRange;
            parameters.TryGetValue("interactionRange", out interactionRange);
            if (interactionRange == null) parameters.TryGetValue("InteractionRange", out interactionRange);

            if (interactionRange != null)
            {
                if (interactionRange is float) this.interactionRange = ((float)interactionRange);
            }
        }

        public void Draw(GUIMap map, Rect area)
        {
            UpdateValues();
            var center = map.PixelToRelative(GM.MetersToPixels(GM.LatLonToMeters(position.x, position.y), map.Zoom)).ToVector2();
            if(Texture != null)
            {
                var size = new Vector2(Texture.width, Texture.height);

                // Scale of 1 means 1 pixel is 1 map pixel in scale 19
                // Scale of 2 means 1 pixel is 0.5 map pixel in scale 18
                // and so on...
                var pixelsSize = GM.MetersToPixels(GM.PixelsToMeters(new Vector2d(size.x * scale.x, size.y * scale.y), 19), map.Zoom);
                var midWidthHeight = pixelsSize.ToVector2() / 2f;
                var textRect = ExtensionRect.FromCorners(center - midWidthHeight, center + midWidthHeight);

                /*var areaRect = textRect.Intersection(area);
                GUI.DrawTextureWithTexCoords(areaRect, i, textRect.ToTexCoords(areaRect));*/
                GUI.DrawTexture(textRect, Texture);

                Handles.BeginGUI();

                var influencePixels = map.PixelToRelative(GM.MetersToPixels(GM.LatLonToMeters(position.x, position.y) + new Vector2d(interactionRange, 0), map.Zoom)).ToVector2() - center;

                Handles.color = Color.black;
                Handles.DrawWireArc(center, Vector3.back, Vector2.up, 360, influencePixels.magnitude);
                Handles.EndGUI();
            }
        }

        public void Repositionate(GUIMap map, Rect area)
        {
            if (!reference.TransformManagerParameters.ContainsKey("Position"))
                reference.TransformManagerParameters.Add("Position", Vector2d.zero);

            reference.TransformManagerParameters["Position"] = map.GeoMousePosition;
        }
    }

    public class ScreenPositionedGUIMapPositionManager : ExtElemReferenceGUIMapPositionManager
    {
        private const float gameWidth = 800f;
        private const float gameHeight = 600f;

        private Vector2 position;
        private Vector3 scale;
        private float rotation;
        private ExtElemReference reference;

        public Texture2D Texture { get; set; }
        public Type ForType { get { return typeof(ScreenPositionedTransformManager); } }

        private void UpdateValues()
        {
            var parameters = reference.TransformManagerParameters;
            object position;
            parameters.TryGetValue("position", out position);
            if (position == null) parameters.TryGetValue("Position", out position);

            if (position != null)
            {
                if (position is Vector2d) this.position = ((Vector2d)position).ToVector2();
                else if (position is Vector2) this.position = (Vector2)position;
            }


            object scale;
            parameters.TryGetValue("scale", out scale);
            if (scale == null) parameters.TryGetValue("Scale", out scale);

            if (scale != null)
            {
                if (scale is float) this.scale = Vector3.one * ((float)scale);
                else if (scale is Vector2) this.scale = new Vector3(((Vector2)scale).x, ((Vector2)scale).y, ((Vector2)scale).x);
                else if (scale is Vector3) this.scale = (Vector3)scale;
            }

            object rotation;
            parameters.TryGetValue("rotation", out rotation);
            if (rotation == null) parameters.TryGetValue("Rotation", out rotation);

            if (rotation != null)
            {
                if (rotation is float) this.rotation = ((float)rotation);
            }
        }

        public void Configure(ExtElemReference reference)
        {
            this.reference = reference;
            GUIMapPositionManagerUtility.InsertDefaults(reference.TransformManagerDescriptor, reference.TransformManagerParameters, false);
            UpdateValues();
        }

        public void Draw(GUIMap map, Rect area)
        {
            UpdateValues();
            var corner = area.position + new Vector2(position.x * area.width / gameWidth, area.height - (position.y * area.height / gameHeight));
            if (Texture != null)
            {
                var size = new Vector2(Texture.width * scale.x, Texture.height * scale.y);
                GUI.DrawTexture(new Rect(corner - new Vector2(size.x / 2f, size.y), size), Texture);
            }
        }
        
        public void Repositionate(GUIMap map, Rect area)
        {
            if (!reference.TransformManagerParameters.ContainsKey("Position"))
                reference.TransformManagerParameters.Add("Position", Vector2.zero);

            var elemMapPosition = Event.current.mousePosition - area.position;
            
            // save position based on texture starting coord, by placing it on mouse center
            reference.TransformManagerParameters["Position"] = new Vector2d(
                elemMapPosition.x * gameWidth / area.width,
                gameHeight - (elemMapPosition.y * gameHeight / area.height + (Texture.height * scale.y)/2f));
        }
    }

    public class RadialCenterGUIMapPositionManager : ExtElemReferenceGUIMapPositionManager
    {
        private float degree;
        private float distance;
        private Vector3 scale;
        private float rotation;
        private bool rotateAround;
        private ExtElemReference reference;

        public Texture2D Texture { get; set; }
        public Type ForType { get { return typeof(RadialCenterTransformManager); } }

        private void UpdateValues()
        {
            /**
             * 
            ParameterDescription = new Dictionary<string, Geo.ParameterDescription>();
            ParameterDescription.Add("Degree", new Geo.ParameterDescription(typeof(float), 0f));
            ParameterDescription.Add("Scale", new Geo.ParameterDescription(typeof(Vector3), Vector3.one));
            ParameterDescription.Add("Rotation", new Geo.ParameterDescription(typeof(float), 0f));
            ParameterDescription.Add("RotateAround", new Geo.ParameterDescription(typeof(bool), true));
             * 
             * */

            var parameters = reference.TransformManagerParameters;

            object degree;
            parameters.TryGetValue("degree", out degree);
            if (degree == null) parameters.TryGetValue("Degree", out degree);

            if (degree != null)
            {
                if (degree is float) this.degree = ((float)degree);
            }

            object distance;
            parameters.TryGetValue("distance", out distance);
            if (distance == null) parameters.TryGetValue("Distance", out distance);

            if (distance != null)
            {
                if (distance is float) this.distance = ((float)distance);
            }
            
            object scale;
            parameters.TryGetValue("scale", out scale);
            if (scale == null) parameters.TryGetValue("Scale", out scale);

            if (scale != null)
            {
                if (scale is float) this.scale = Vector3.one * ((float)scale);
                else if (scale is Vector2) this.scale = new Vector3(((Vector2)scale).x, ((Vector2)scale).y, ((Vector2)scale).x);
                else if (scale is Vector3) this.scale = (Vector3)scale;
            }

            object rotation;
            parameters.TryGetValue("rotation", out rotation);
            if (rotation == null) parameters.TryGetValue("Rotation", out rotation);

            if (rotation != null)
            {
                if (rotation is float) this.rotation = ((float)rotation);
            }

            object rotateAround;
            parameters.TryGetValue("rotateAround", out rotateAround);
            if (rotateAround == null) parameters.TryGetValue("RotateAround", out rotateAround);

            if (rotateAround != null)
            {
                if (rotateAround is bool) this.rotateAround = ((bool)rotateAround);
            }
        }

        public void Configure(ExtElemReference reference)
        {
            this.reference = reference;
            GUIMapPositionManagerUtility.InsertDefaults(reference.TransformManagerDescriptor, reference.TransformManagerParameters, false);
            UpdateValues();
        }

        public void Draw(GUIMap map, Rect area)
        {
            UpdateValues();

            var degree = this.degree * Mathf.Deg2Rad;

            var center = (map.Center + GM.MetersToLatLon(new Vector2d(distance * Mathd.Sin(degree), distance * Mathd.Cos(degree)))).ToVector2();
            center = map.PixelToRelative(GM.MetersToPixels(GM.LatLonToMeters(center.x, center.y), map.Zoom)).ToVector2();
            if (Texture != null)
            {
                var size = new Vector2(Texture.width, Texture.height);

                // Scale of 1 means 1 pixel is 1 map pixel in scale 19
                // Scale of 2 means 1 pixel is 0.5 map pixel in scale 18
                // and so on...
                var pixelsSize = GM.MetersToPixels(GM.PixelsToMeters(new Vector2d(size.x * scale.x, size.y * scale.y), 19), map.Zoom);
                var midWidthHeight = pixelsSize.ToVector2() / 2f;
                var textRect = ExtensionRect.FromCorners(center - midWidthHeight, center + midWidthHeight);

                /*var areaRect = textRect.Intersection(area);
                GUI.DrawTextureWithTexCoords(areaRect, i, textRect.ToTexCoords(areaRect));*/
                GUI.DrawTexture(textRect, Texture);
            }
        }

        public void Repositionate(GUIMap map, Rect area)
        {
            var centerMouseVector = (map.GeoMousePosition - map.Center).normalized;
            float angle = Vector3.Angle(new Vector3(0.0f, 1.0f, 0.0f), new Vector3((float)centerMouseVector.x, (float)centerMouseVector.y, 0.0f));
            if (centerMouseVector.x < 0.0f) angle = 360.0f - angle;

            reference.TransformManagerParameters["Degree"] = angle;
        }
    }
}

