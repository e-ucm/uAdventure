using System;
using System.Collections.Generic;
using System.Linq;
using uAdventure.Core;
using uAdventure.Editor;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace uAdventure.Geo
{
    public class MapSceneWindowReferences : MapEditorWindow
    {
        private readonly DataControlList mapElements;
        private readonly Texture2D conditionsTex, noConditionsTex, centerTex;
        private int lastSelectedMapScene = -1;
        private MapSceneDataControl workingMapScene;
        private readonly Dictionary<Type, Texture2D> icons;

        public MapSceneWindowReferences(Rect rect, GUIContent content, GUIStyle style, MapEditor componentBasedEditor, params GUILayoutOption[] options) 
            : base(rect, content, style, componentBasedEditor, options)
        {
            conditionsTex = Resources.Load<Texture2D>("EAdventureData/img/icons/conditions-24x24");
            noConditionsTex = Resources.Load<Texture2D>("EAdventureData/img/icons/no-conditions-24x24");
            centerTex = Resources.Load<Texture2D>("EAdventureData/img/icons/center-24x24");

            icons = new Dictionary<Type, Texture2D>()
            {
                { typeof(GeoElementDataControl), Resources.Load<Texture2D>("EAdventureData/img/icons/poi") },
                { typeof(PlayerDataControl),     Resources.Load<Texture2D>("EAdventureData/img/icons/player-old") },
                { typeof(ItemDataControl),       Resources.Load<Texture2D>("EAdventureData/img/icons/item") },
                { typeof(AtrezzoDataControl),    Resources.Load<Texture2D>("EAdventureData/img/icons/atrezzo-1") },
                { typeof(NPCDataControl),        Resources.Load<Texture2D>("EAdventureData/img/icons/npc") }
            };

            new MapReferenceComponent(rect, null, style, null);

            mapElements = new DataControlList
            {
                Columns = new List<ColumnList.Column>
                {
                    new ColumnList.Column
                    {
                        Text = TC.get("ElementList.Title")
                    },
                    new ColumnList.Column
                    {
                        Text = TC.get("Geo.MapScene.ReferenceList.Column.Center"),
                        SizeOptions = new []{GUILayout.Width(24f)},
                    },
                    new ColumnList.Column()
                    {
                        Text = TC.get("Conditions.Title")
                    }
                },
                drawCell = (cellRect, row, column, active, focused) =>
                {
                    var mapElement = mapElements.list[row] as MapElementDataControl;
                    switch (column)
                    {
                        case 0:
                            var iconSpace = new Rect(cellRect);
                            var nameSpace = new Rect(cellRect);
                            iconSpace.size = new Vector2(16, nameSpace.size.y);
                            nameSpace.position += new Vector2(16, 0);
                            nameSpace.size += new Vector2(-16, 0);

                            Texture2D icon = null;
                            var type = mapElement.ReferencedDataControl.GetType();
                            if (icons.ContainsKey(type)) icon = icons[type];
                            if (icon != null)
                            {
                                GUI.Label(iconSpace, icons[type]);
                            }
                            GUI.Label(icon != null ? nameSpace : cellRect, mapElement.ReferencedId);
                            break;
                        case 1:
                            var elementReference = mapElement as ExtElementRefDataControl;

                            if ((elementReference == null || elementReference.TransformManager.PositionManagerName == "WorldPositioned") 
                                && GUI.Button(cellRect, centerTex))
                            {
                                Center(componentBasedEditor, mapElement);
                            }
                            break;
                        case 2:
                            if (GUI.Button(cellRect,
                                mapElement.Conditions.getBlocksCount() > 0 ? conditionsTex : noConditionsTex))
                            {
                                this.mapElements.index = row;
                                var window = ScriptableObject.CreateInstance<ConditionEditorWindow>();
                                window.Init(mapElement.Conditions);
                            }
                            break;
                    }
                }
            };
            mapElements.onSelectCallback += list =>
                {
                    componentBasedEditor.SelectedElement = (DataControl)(mapElements.index == -1 ? null : mapElements.list[mapElements.index]);
                };

            componentBasedEditor.onSelectElement += elem =>
            {
                mapElements.index = mapElements.list.IndexOf(elem);
            };
        }

        private static void Center(MapEditor mapEditor, MapElementDataControl mapElement)
        {
            var extElementReference = mapElement as ExtElementRefDataControl;
            var geoElementReference = mapElement as GeoElementRefDataControl;

            if (extElementReference != null)
            {

                var sprite = ScenesWindowElementReference.ReferenceComponent.GetSprite(extElementReference);
                if (!sprite)
                {
                    mapEditor.Center = (Vector2d)extElementReference.TransformManager["Position"];
                }
                else
                {
                    var texture = sprite.texture;
                    var rect = new RectD(new Vector2d(-0.5f * texture.width, -texture.height),
                        new Vector2d(texture.width, texture.height));

                    var previousTransformManager = mapEditor.PositionManager;
                    mapEditor.PositionManager = extElementReference.TransformManager.GUIMapPositionManager;
                    var relativeRect = mapEditor.ToRelative(rect.ToPoints()).ToRectD();
                    mapEditor.PositionManager = previousTransformManager;
                    var boundingBox = relativeRect.ToPoints()
                        .Select(p => mapEditor.PixelToLatLon(mapEditor.RelativeToAbsolute(p)))
                        .ToArray()
                        .ToRectD();

                    mapEditor.ZoomToBoundingBox(boundingBox);
                    mapEditor.Center = boundingBox.Center;
                }
            }
            else if (geoElementReference != null)
            {
                var geoElement = geoElementReference.ReferencedDataControl as GeoElementDataControl;
                var geometry = geoElement.GMLGeometries[geoElement.SelectedGeometry];
                mapEditor.ZoomToBoundingBox(geometry.BoundingBox);
                mapEditor.Center = geometry.BoundingBox.Center;
            }
        }

        protected override void DrawInspector()
        {
            if (GeoController.Instance.SelectedMapScene != -1 &&
                GeoController.Instance.SelectedMapScene != lastSelectedMapScene)
            {
                lastSelectedMapScene = GeoController.Instance.SelectedMapScene;
                workingMapScene = GeoController.Instance.MapScenes[lastSelectedMapScene];
                mapElements.SetData(workingMapScene.Elements, e => (e as ListDataControl<MapSceneDataControl, MapElementDataControl>).DataControls.Cast<DataControl>().ToList());
            }

            mapElements.DoList(160);
        }

        [EditorComponent(typeof(ExtElementRefDataControl), Name = "Geo.MapElement.Component.Title", Order = 0)]
        private class MapReferenceComponent : AbstractEditorComponent
        {
            private readonly DrawerParametersMenu transformManagerParametersEditor;
            private readonly Texture2D centerTex;

            private readonly int[] orientationValues;
            private readonly string[] orientationTexts;
            
            public MapReferenceComponent(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
            {
                transformManagerParametersEditor = ScriptableObject.CreateInstance<DrawerParametersMenu>();
                centerTex = Resources.Load<Texture2D>("EAdventureData/img/icons/center-24x24");
                var orientations = Enum.GetValues(typeof(Orientation));
                orientationValues = orientations.Cast<int>().ToArray();
                orientationTexts = orientations.Cast<Orientation>().Select(s => "Orientation." + s.ToString()).ToArray();
            }

            public override void Draw(int aID)
            {
                var workingMapElement = Target as MapElementDataControl;
                var extElementReference = workingMapElement as ExtElementRefDataControl;
                if (extElementReference != null)
                {
                    // Trasnform manager descriptor selection
                    var avaliableDescriptors = TransformManagerDescriptorFactory.Instance.AvaliableTransformManagers.ToList();
                    var selected = avaliableDescriptors.FindIndex(d => d.Value == extElementReference.TransformManager.PositionManagerName);
                    var newSelected = EditorGUILayout.Popup(selected, avaliableDescriptors.ConvertAll(d => d.Value).ToArray());

                    if (newSelected != selected)
                    {
                        // In case of change, reinstance a new one
                        extElementReference.TransformManager.PositionManagerName = avaliableDescriptors[newSelected].Value;
                    }

                    if (extElementReference.TransformManager.PositionManagerName == "WorldPositioned" && GUILayout.Button(centerTex))
                    {
                        Center(MapEditor.Current, extElementReference);
                    }

                    using (new EditorGUI.IndentLevelScope(1))
                    {
                        EditorGUILayout.LabelField("Geo.MapElement.Component.Parameters".Traslate());
                        transformManagerParametersEditor.extElementRefDataControl = extElementReference;
                        transformManagerParametersEditor.OnGUI();
                        EditorGUI.BeginChangeCheck();
                        var newScale = EditorGUILayout.FloatField(
                            "Geo.MapElement.Positioner.Attribute.Scale".Traslate(), extElementReference.Scale);
                        if (EditorGUI.EndChangeCheck())
                        {
                            extElementReference.Scale = newScale;
                        }

                        if (extElementReference.UsesOrientation)
                        {
                            EditorGUI.BeginChangeCheck();
                            var orientationLabel = TC.get("ElementReference.Orientation");
                            var translatedTexts = orientationTexts.Select(TC.get).ToArray();
                            var newOrientation = (Orientation)EditorGUILayout.IntPopup(orientationLabel, (int)extElementReference.Orientation, translatedTexts, orientationValues);
                            if (EditorGUI.EndChangeCheck())
                            {
                                extElementReference.Orientation = newOrientation;
                            }
                        }
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Geo.MapElement.Component.NothingToEdit".Traslate(), MessageType.Info);
                }
            }

            public override void OnPreRender()
            {
                var workingMapElement = Target as MapElementDataControl;
                var extElementReference = workingMapElement as ExtElementRefDataControl;
                if (extElementReference != null)
                {
                    MapEditor.Current.PositionManager = extElementReference.TransformManager.GUIMapPositionManager;
                }
            }

            public override void OnPostRender()
            {
                MapEditor.Current.PositionManager = null;
            }

            public override bool Update()
            {
                if (Event.current.type != EventType.MouseDown)
                {
                    return false;
                }

                var workingMapElement = Target as MapElementDataControl;
                var extElementReference = workingMapElement as ExtElementRefDataControl;
                if (extElementReference == null)
                {
                    return false;
                }

                var sprite = ScenesWindowElementReference.ReferenceComponent.GetSprite(extElementReference);
                if (!sprite)
                {
                    return false;
                }

                var texture = sprite.texture;
                var rect = new RectD(new Vector2d(-0.5 * texture.width, -texture.height),
                    new Vector2d(texture.width, texture.height));
                var adaptedRect = ComponentBasedEditor.Generic.ToRelative(rect.ToPoints()).ToRectD();
                var textureContains = false;

                if (adaptedRect.Contains(Event.current.mousePosition.ToVector2d()))
                {
                    double x = (Event.current.mousePosition.x - rect.Min.x) / rect.Width;
                    double y = (Event.current.mousePosition.y - rect.Min.y) / rect.Height;
                    textureContains = true;
                    //textureContains = sprite.texture.GetPixel((int)(x * sprite.texture.width), sprite.texture.height - (int)(y * sprite.texture.height)).a > 0;
                }

                return textureContains || extElementReference.TransformManager.GUIMapPositionManager.IsGizmosSelected(MapEditor.Current, rect.ToPoints());
            }

            public override void OnDrawingGizmosSelected()
            {
                var workingMapElement = Target as MapElementDataControl;
                var extElementReference = workingMapElement as ExtElementRefDataControl;
                if (extElementReference == null)
                {
                    return;
                }

                var sprite = ScenesWindowElementReference.ReferenceComponent.GetSprite(extElementReference);
                if (!sprite)
                {
                    return;
                }

                var texture = sprite.texture;
                var rect = new RectD(new Vector2d(-0.5f * texture.width, -texture.height),
                    new Vector2d(texture.width, texture.height));

                extElementReference.TransformManager.GUIMapPositionManager.OnDrawingGizmosSelected(MapEditor.Current, rect.ToPoints());
            }
        }
    }
}