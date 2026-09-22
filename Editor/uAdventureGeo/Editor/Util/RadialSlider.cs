using UnityEditor;
using UnityEngine;

namespace uAdventure.Editor
{
    public class RadialSliderTexture
    {
        public Texture2D texture2D;

        public RadialSliderTexture()
        {
            texture2D = new Texture2D(1, 1);
            texture2D.SetPixel(0,0, Color.red);
        }
    }

    public class RadialSlider
    {
        public static Vector2 Do(Rect rect, string label, Vector2 direction)
        {

            var realRect = new Rect();
            realRect.width = Mathf.Min(rect.width, rect.height);
            realRect.height = realRect.width;
            realRect.center = rect.center;

            var myId = GUIUtility.GetControlID(FocusType.Passive, realRect);

            var radialSliderTexture = GUIUtility.GetStateObject(typeof(RadialSliderTexture), myId) as RadialSliderTexture;


            Handles.BeginGUI();
            var radius = realRect.width / 2f;
            Handles.color = Color.white;
            Handles.DrawSolidArc(realRect.center, Vector3.back, Vector3.up, 360f, radius);
            Handles.color = Color.black;
            Handles.DrawWireArc(realRect.center, Vector3.back, Vector3.up, 360f, radius);
            Handles.color = Color.red;
            Handles.DrawAAPolyLine(radialSliderTexture.texture2D, 2, realRect.center, realRect.center + direction * radius);
            Handles.EndGUI();

            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    if (Vector2.Distance(Event.current.mousePosition, realRect.center) <= radius)
                    {
                        GUIUtility.hotControl = myId;
                        var newDirection = GetDirection(realRect.center, Event.current.mousePosition);
                        if(newDirection != direction)
                        {
                            direction = newDirection;
                            GUI.changed = true;
                        }
                        Event.current.Use();

                    }
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == myId)
                    {
                        var newDirection = GetDirection(realRect.center, Event.current.mousePosition);
                        if (newDirection != direction)
                        {
                            direction = newDirection;
                            GUI.changed = true;
                        }
                        Event.current.Use();
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == myId)
                    {
                        GUIUtility.hotControl = -1;
                        Event.current.Use();
                    }
                    break;
            }

            return direction;

        }

        private static Vector2 GetDirection(Vector2 center, Vector2 point)
        {
            return (point - center).normalized;
        }
    }
}
