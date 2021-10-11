using System.Collections.Generic;
using System.Linq;
using uAdventure.Core;
using uAdventure.Editor;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Geo
{
    [EditorComponent(typeof(GeoElementDataControl), typeof(ExtElementRefDataControl), Name = "Geo.GeoElementWindow.Actions.Title", Order = 20)]
    public class GeoElementWindowActions : AbstractEditorComponent
    {
        private readonly DataControlList geoActionsList;
        private readonly Texture2D conditionsTex, noConditionsTex;

        public GeoElementWindowActions(Rect aStartPos, GUIContent aContent, GUIStyle aStyle,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            conditionsTex = Resources.Load<Texture2D>("EAdventureData/img/icons/conditions-24x24");
            noConditionsTex = Resources.Load<Texture2D>("EAdventureData/img/icons/no-conditions-24x24");

            var actionsColumns = new List<ColumnList.Column>
            {
                new ColumnList.Column
                {
                    Text = "Geo.GeoElementWindow.Actions.Column.Name".Traslate(),
                    SizeOptions = new[] { GUILayout.Width(150)}
                },
                new ColumnList.Column
                {
                    Text = "Geo.GeoElementWindow.Actions.Column.Parameters".Traslate()
                },
                new ColumnList.Column
                {
                    Text = "Geo.GeoElementWindow.Actions.Column.Effects".Traslate(),
                    SizeOptions = new[] { GUILayout.Width(150)}
                },
                new ColumnList.Column
                {
                    Text = "Geo.GeoElementWindow.Actions.Column.Conditions".Traslate(),
                    SizeOptions = new[] { GUILayout.Width(150)}
                }
            };

            geoActionsList = new DataControlList
            {
                elementHeight = 60,
                Columns = actionsColumns,
                drawCell = DrawActionColumn
            };
        }


        public override void Draw(int aID)
        {
            var t = Target;
            if (t != null) // Is in the inspector
            {
                m_Rect.height = 300;
            }
            else
            {
                t = GeoController.Instance.GeoElements.DataControls[GeoController.Instance.SelectedGeoElement];
            }

            if (t is ExtElementRefDataControl)
            {
                var extElemRefDataControl = t as ExtElementRefDataControl;
                var geoActions = extElemRefDataControl.GeoActions;

                if (extElemRefDataControl.TransformManager.PositionManagerName != GeopositionedDescriptor.GeopositionedName)
                {
                    GUILayout.Label("The selected transform manager is incompatible with GeoActions. Only World Positioned elements are compatible.");
                }
                else
                {
                    // -------------
                    // Actions
                    // -------------
                    geoActionsList.SetData(geoActions, ge => (ge as ListDataControl<ExtElementRefDataControl, GeoActionDataControl>).DataControls.Cast<DataControl>().ToList());
                    geoActionsList.DoList(m_Rect.height - 60f);
                }
            }
            else if (t is GeoElementDataControl)
            {
                // -------------
                // Actions
                // -------------
                var geoActions = (t as GeoElementDataControl).GeoActions;
                geoActionsList.SetData(geoActions, ge => (ge as ListDataControl<GeoElementDataControl, GeoActionDataControl>).DataControls.Cast<DataControl>().ToList());
                geoActionsList.DoList(m_Rect.height - 60f);
            }
        }

        private void DrawActionColumn(Rect rect, int row, int column, bool active, bool focused)
        {
            var action = geoActionsList.list[row] as GeoActionDataControl;

            switch (column)
            {
                case 0:
                    GUI.Label(rect, TC.get("Geo.GeoElementWindow.Actions." + action.getType() + ".Name"));
                    break;
                case 1:

                    switch (action.getType())
                    {
                        case "Enter": DoEnterActionParameters(rect, action); break;
                        case "Exit": DoExitActionParameters(rect, action); break;
                        case "LookTo": DoLookToActionParameters(rect, action); break;
                        case "Inspect": DoInspectActionParameters(rect, action); break;
                        default:
                            GUI.Label(rect, "Geo.GeoElementWindow.Actions.Unknown".Traslate(action.getType()));
                            break;
                    }

                    break;
                case 2:
                    if (GUI.Button(rect, "Element.Effects".Traslate()))
                    {
                        var effectEditorWindow = ScriptableObject.CreateInstance<EffectEditorWindow>();
                        effectEditorWindow.Init(action.Effects);
                        effectEditorWindow.Show();
                    }

                    break;
                case 3:
                    if (GUI.Button(rect, action.Conditions.getBlocksCount() > 0 ? conditionsTex : noConditionsTex))
                    {
                        ConditionEditorWindow.ShowAtPosition(action.Conditions, rect);
                    }

                    break;
            }
        }

        private static void DoInspectActionParameters(Rect rect, GeoActionDataControl action)
        {
            EditorGUI.BeginChangeCheck();
            var newInside = GUI.Toggle(rect, (bool)action["Inside"],
                "Geo.GeoElementWindow.Actions.Parameter.Inside".Traslate());
            if (EditorGUI.EndChangeCheck())
            {
                action["Inside"] = newInside;
            }
        }

        private static void DoLookToActionParameters(Rect rect, GeoActionDataControl action)
        {
            var horiz = rect.Divide(2);
            var vert = horiz[0].Divide(1,2);
            EditorGUI.BeginChangeCheck();
            var newInside = GUI.Toggle(vert[0, 0], (bool)action["Inside"],
                "Geo.GeoElementWindow.Actions.Parameter.Inside".Traslate());
            if (EditorGUI.EndChangeCheck())
            {
                action["Inside"] = newInside;
            }

            EditorGUI.BeginChangeCheck();
            var newCenter = GUI.Toggle(vert[1, 0], (bool)action["Center"],
                "Geo.GeoElementWindow.Actions.Parameter.Center".Traslate());
            if (EditorGUI.EndChangeCheck())
            {
                action["Center"] = newCenter;
            }

            if (!newCenter)
            {
                EditorGUI.BeginChangeCheck();
                var newDirection = RadialSlider.Do(horiz[1], 
                    "Geo.GeoElementWindow.Actions.Parameter.Direction".Traslate(), (Vector2)action["Direction"]);
                if (EditorGUI.EndChangeCheck())
                {
                    action["Direction"] = newDirection;
                }
            }
        }

        private static void DoExitActionParameters(Rect rect, GeoActionDataControl action)
        {
            EditorGUI.BeginChangeCheck();
            var newFromInside = GUI.Toggle(rect, (bool)action["OnlyFromInside"],
                "Geo.GeoElementWindow.Actions.Parameter.OnlyFromInside".Traslate());
            if (EditorGUI.EndChangeCheck())
            {
                action["OnlyFromInside"] = newFromInside;
            }
        }

        private static void DoEnterActionParameters(Rect rect, GeoActionDataControl action)
        {
            EditorGUI.BeginChangeCheck();
            var newFromOutside = GUI.Toggle(rect, (bool)action["OnlyFromOutside"],
                "Geo.GeoElementWindow.Actions.Parameter.OnlyFromOutside".Traslate());
            if (EditorGUI.EndChangeCheck())
            {
                action["OnlyFromOutside"] = newFromOutside;
            }
        }
    }
}
