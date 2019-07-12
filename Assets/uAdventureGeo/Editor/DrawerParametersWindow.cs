using UnityEngine;
using System.Collections;
using UnityEditor;

using uAdventure.Geo;
using MapzenGo.Helpers;
using System;
using uAdventure.Core;

namespace uAdventure.Editor
{

    public class DrawerParametersMenu : EditorWindow {

        public ExtElementRefDataControl extElementRefDataControl;
        private FileChooser textureField;
        
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

        protected void OnEnable()
        {
            textureField = new FileChooser { FileType = FileType.SET_ITEM_IMAGE };
        }

        protected void OnDisable()
        {
            DrawerParametersMenu.s_LastClosedTime = DateTime.Now.Ticks / 10000L;
            DrawerParametersMenu.s_DrawerParametersMenu = null;
        }

        public void OnGUI()
        {
            if(extElementRefDataControl != null && extElementRefDataControl.TransformManager != null)
            {
                // And then show the required parameters

                foreach (var param in extElementRefDataControl.TransformManager.ParameterDescription)
                {
                    if (param.Value.Type == typeof(float))
                    {
                        extElementRefDataControl.TransformManager[param.Key] = EditorGUILayout.FloatField(param.Key.Traslate(), (float)extElementRefDataControl.TransformManager[param.Key]);
                    }

                    if (param.Value.Type == typeof(Vector2) )
                    {
                        extElementRefDataControl.TransformManager[param.Key] = EditorGUILayout.Vector2Field(param.Key.Traslate(), (Vector2)extElementRefDataControl.TransformManager[param.Key]);
                    }

                    if (param.Value.Type == typeof(Vector3))
                    {
                        extElementRefDataControl.TransformManager[param.Key] = EditorGUILayout.Vector3Field(param.Key.Traslate(), (Vector3)extElementRefDataControl.TransformManager[param.Key]);
                    }

                    if (param.Value.Type == typeof(Vector2d))
                    {
                        extElementRefDataControl.TransformManager[param.Key] = EditorGUILayout.Vector2Field(param.Key.Traslate(), ((Vector2d)extElementRefDataControl.TransformManager[param.Key]).ToVector2()).ToVector2d();
                    }

                    if (param.Value.Type == typeof(bool))
                    {
                        extElementRefDataControl.TransformManager[param.Key] = EditorGUILayout.Toggle(param.Key.Traslate(), (bool)extElementRefDataControl.TransformManager[param.Key]);
                    }

                    if (param.Value.Type == typeof(string))
                    {
                        textureField.Empty = extElementRefDataControl.TransformManager.ParameterDescription[param.Key]
                            .DefaultValue as string;
                        textureField.Label = param.Key.Traslate();
                        textureField.Path = extElementRefDataControl.TransformManager[param.Key] as string;
                        textureField.DoLayout();
                        extElementRefDataControl.TransformManager[param.Key] = textureField.Path;
                    }
                }
            }
        }

    }
}