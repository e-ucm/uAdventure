using UnityEngine;
using System.Collections;
using UnityEditor;

using uAdventure.Geo;
using MapzenGo.Helpers;
using System;
using uAdventure.Core;

namespace uAdventure.Editor
{

    public class DrawerParametersMenu {
        
        private static FileChooser textureField;

        public static void DrawParametersFor(ExtElementRefDataControl extElementRefDataControl)
        {
            if(textureField == null)
            {
                textureField = new FileChooser { FileType = FileType.SET_ITEM_IMAGE };
            }

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