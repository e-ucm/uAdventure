using UnityEngine;
using System.Collections;
using UnityEditor;

using uAdventure.Geo;
using MapzenGo.Helpers;

namespace uAdventure.Editor
{

    public class DrawerParametersWindow : EditorWindow {

        public ExtElemReference ExtElemReference;

        void OnGUI()
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
                        ExtElemReference.TransformManagerParameters[param.Key] = EditorGUILayout.FloatField((float)ExtElemReference.TransformManagerParameters[param.Key]);
                    }

                    if (param.Value.Type == typeof(Vector2) )
                    {
                        ExtElemReference.TransformManagerParameters[param.Key] = EditorGUILayout.Vector2Field("",(Vector2)ExtElemReference.TransformManagerParameters[param.Key]);
                    }

                    if (param.Value.Type == typeof(Vector2d))
                    {
                        ExtElemReference.TransformManagerParameters[param.Key] = EditorGUILayout.Vector2Field("",((Vector2d)ExtElemReference.TransformManagerParameters[param.Key]).ToVector2()).ToVector2d();
                    }

                    if (param.Value.Type == typeof(float))
                    {
                        ExtElemReference.TransformManagerParameters[param.Key] = EditorGUILayout.FloatField((float)ExtElemReference.TransformManagerParameters[param.Key]);
                    }
                }
            }
        }

    }
}