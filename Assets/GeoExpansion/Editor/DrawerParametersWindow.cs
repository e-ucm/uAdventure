using UnityEngine;
using System.Collections;
using UnityEditor;

using uAdventure.Geo;
using MapzenGo.Helpers;
using System;

namespace uAdventure.Editor
{

    public class DrawerParametersMenu : EditorWindow {

        public ExtElemReference ExtElemReference;
        
        internal static bool ShowAtPosition(Rect buttonRect)
        {
            long num = DateTime.Now.Ticks / 10000L;
            if (num >= DrawerParametersMenu.s_LastClosedTime + 50L)
            {
                if (Event.current != null)
                {
                    Event.current.Use();
                }
                if (DrawerParametersMenu.s_DrawerParametersMenu == null)
                {
                    DrawerParametersMenu.s_DrawerParametersMenu = ScriptableObject.CreateInstance<DrawerParametersMenu>();
                }
                DrawerParametersMenu.s_DrawerParametersMenu.Init(buttonRect);
                return true;
            }
            return false;
        }

        public static DrawerParametersMenu s_DrawerParametersMenu;
        private static long s_LastClosedTime;

        private void Init(Rect buttonRect)
        {
            buttonRect.position = GUIUtility.GUIToScreenPoint(buttonRect.position);
            float y = 145f;
            Vector2 windowSize = new Vector2(300f, y);
            base.ShowAsDropDown(buttonRect, windowSize);
        }

        protected void OnDisable()
        {
            DrawerParametersMenu.s_LastClosedTime = DateTime.Now.Ticks / 10000L;
            DrawerParametersMenu.s_DrawerParametersMenu = null;
        }

        protected void OnGUI()
        {
            if(ExtElemReference != null && ExtElemReference.TransformManagerDescriptor != null)
            {
                // And then show the required parameters

                foreach (var param in ExtElemReference.TransformManagerDescriptor.ParameterDescription)
                {
                    if (!ExtElemReference.TransformManagerParameters.ContainsKey(param.Key))
                        ExtElemReference.TransformManagerParameters.Add(param.Key, param.Value.DefaultValue);

                    if (param.Value.Type == typeof(float))
                    {
                        ExtElemReference.TransformManagerParameters[param.Key] = EditorGUILayout.FloatField(param.Key, (float)ExtElemReference.TransformManagerParameters[param.Key]);
                    }

                    if (param.Value.Type == typeof(Vector2) )
                    {
                        ExtElemReference.TransformManagerParameters[param.Key] = EditorGUILayout.Vector2Field(param.Key,(Vector2)ExtElemReference.TransformManagerParameters[param.Key]);
                    }

                    if (param.Value.Type == typeof(Vector3))
                    {
                        ExtElemReference.TransformManagerParameters[param.Key] = EditorGUILayout.Vector3Field(param.Key, (Vector3)ExtElemReference.TransformManagerParameters[param.Key]);
                    }

                    if (param.Value.Type == typeof(Vector2d))
                    {
                        ExtElemReference.TransformManagerParameters[param.Key] = EditorGUILayout.Vector2Field(param.Key, ((Vector2d)ExtElemReference.TransformManagerParameters[param.Key]).ToVector2()).ToVector2d();
                    }

                    if (param.Value.Type == typeof(bool))
                    {
                        ExtElemReference.TransformManagerParameters[param.Key] = EditorGUILayout.Toggle(param.Key, (bool)ExtElemReference.TransformManagerParameters[param.Key]);
                    }
                }
            }
        }

    }
}