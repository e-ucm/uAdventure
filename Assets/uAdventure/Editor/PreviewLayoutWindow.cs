using MoreLinq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using uAdventure.Core;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Editor
{
    public abstract class PreviewLayoutWindow : LayoutWindow
    {

        /** Corner enumerate represent the possible positions for the preview inspector window */
        private enum Corner { TopRight, TopLeft, BottomRight, BottomLeft }

        // ##################### CONSTRUCTOR & DESTRUCTOR ######################

        /** Constructor */
        public PreviewLayoutWindow(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
        {
            sceneSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);

            DoUpdate = false;
        }
        
        /** Callback for window destroy */
        void OnDestroy()
        {
            DoUpdate = false;
        }

        // ######################### ATTRIBUTES ##########################

        /** Skin used to store the scene window GUISkin from Unity */
        private GUISkin sceneSkin;

        /** Callback delegate for the update */
        private EditorApplication.CallbackFunction callback = null;
        
        private static Rect windowRect; 
        private static Rect previewRect;

        private Vector3 velocity;
        private float time = 1f;

        // ######################## PROPERTIES ###########################
        
        /** Position of the preview inspector window */
        private Vector3 PreviewInspectorPosition
        {
            get { return windowRect.position; }
            set { windowRect.position = value; }
        }

        /** Target position of the preview inspector window (where it should be) */
        private Vector3 TargetPreviewInspectorPosition { get; set; }


        /** True if this window is receiving Update callbacks to perform the preview's inspector movement */
        protected bool DoUpdate
        {
            get
            {
                return callback != null;
            }
            set
            {
                if (value != DoUpdate)
                {
                    if (value)
                    {
                        // Register the update
                        callback = new EditorApplication.CallbackFunction(this.ApplicationUpdate);
                        EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, callback);
                    }
                    else
                    {
                        // De register the update
                        EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, callback);
                        callback = null;
                    }
                }
            }
        }

        private bool d = false;
        private bool Dragging { get { return d; } set { d = value; } }

        // ######################## MAIN FUNCTIONS ########################

        /** Update is called from the UnityEditor and is used to move (animating) the preview inspector inside of the window */
        private int checkMouse = 0;
        protected virtual void ApplicationUpdate()
        {
            if (Dragging)
            {
                // Fix for releasing mouse button outside of the window
                if (GUIUtility.hotControl == 0)
                {
                    Dragging = false;
                }
            }
            else
            {
                if (Vector3.Distance(PreviewInspectorPosition, TargetPreviewInspectorPosition) > 1.5f)
                {
                    PreviewInspectorPosition = Vector3.SmoothDamp(PreviewInspectorPosition, TargetPreviewInspectorPosition, ref velocity, time, 50f, Time.deltaTime);
                }
                else
                {
                    DoUpdate = false;
                    velocity = Vector3.zero;
                }
            }
        }

        bool windowInited = false;

        public override void OnGUI()
        {
            base.OnGUI();
        }

        private Rect areaRect;

        /** Called to draw the main Extension window content */
        public override void Draw(int aID)
        {
            DrawInspector();

            DrawPreviewHeader();
            var auxRect = EditorGUILayout.BeginVertical("preBackground", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {

                if (Event.current.type == EventType.Repaint)
                {
                    areaRect = auxRect;

                    auxRect.position = Vector2.zero;
                    auxRect.x += 30; auxRect.width -= 60;
                    auxRect.y += 30; auxRect.height -= 60;

                    previewRect = auxRect;
                    //previewRect.position += areaRect.position;

                    if (!windowInited)
                    {
                        // If no initial position, we move it to the bottom right
                        windowRect.position = GetCorners(windowRect, previewRect)[Corner.BottomRight];
                        windowInited = true;
                    }
                }

                GUILayout.BeginArea(areaRect);
                GUILayout.BeginHorizontal();
                {
                    BeginWindows();

                    var viewport = new Rect(previewRect);


                    if (HasToDrawPreviewInspector())
                    {
                        var corner = GetDesiredCorner(windowRect, previewRect);
                        switch (corner.Key)
                        {
                            case Corner.BottomLeft:
                            case Corner.TopLeft:
                                {
                                    var reduction = (windowRect.width + 10) * Mathf.Clamp01(1f - (Vector2.Distance(windowRect.position, corner.Value) / (previewRect.width / 3f)));

                                    viewport.x += reduction;
                                    viewport.width -= reduction;
                                }
                                break;

                            case Corner.BottomRight:
                            case Corner.TopRight:
                                {
                                    var reduction = (windowRect.width + 10) * Mathf.Clamp01(1f - (Vector2.Distance(windowRect.position, corner.Value) / (previewRect.width / 3f)));
                                    viewport.width -= reduction;
                                }
                                break;
                        }
                    }

                    DrawPreview(viewport);

                    if (HasToDrawPreviewInspector())
                    {
                        // Change the skin to the scene skin to draw it black
                        var prevSkin = GUI.skin;
                        GUI.skin = sceneSkin;

                        var prevType = Event.current.type;
                        var maxHeight = viewport.height- 25f;

                        windowRect.height = Mathf.Clamp(windowRect.height, 0, maxHeight);
                        // Create the preview inspector window
                        var newWindowRect = GUILayout.Window(9999, windowRect, (i) =>
                        {
                            if (Event.current.type == EventType.Layout) useScroll = windowRect.height >= maxHeight;

                            if (useScroll) scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(maxHeight));
                            DrawPreviewInspector();
                            if (useScroll) EditorGUILayout.EndScrollView();

                            var prevHot = GUIUtility.hotControl;
                            GUI.DragWindow();
                            if(prevHot != GUIUtility.hotControl)
                            {
                                Dragging = true;
                                DoUpdate = true;
                            }

                        }, "Properties", sceneSkin.window)
                        .TrapInside(previewRect);


                        // And then, we update the position
                        windowRect = newWindowRect;

                        // We obtain the corner it should be
                        var corner = GetDesiredCorner(windowRect, previewRect);
                        TargetPreviewInspectorPosition = corner.Value;

                        GUI.BringWindowToFront(9999);

                        // If its doing any update its because its being animated
                        if (DoUpdate)
                        {
                            this.OnRequestRepaint();
                        }
                        else if (!Dragging)
                        {
                            // Otherwise, we just fix the window in the desired corner until it's moved
                            windowRect.position = corner.Value;
                        }

                        // Restore the previous skin
                        GUI.skin = prevSkin;
                    }
                    
                    EndWindows();
                }
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
            
            EditorGUILayout.EndVertical();
            
        }

        private Vector2 scroll;

        private bool useScroll = false;
        
        /** Called to draw the content before the preview area */
        protected virtual void DrawInspector() { }

        /** Called to draw the preview header */
        protected virtual void DrawPreviewHeader()
        {
            GUILayout.Space(10);
            GUILayout.Label(TC.get("ImageAssets.Preview"), "preToolbar", GUILayout.ExpandWidth(true));
        }

        /** Called to draw the preview content */
        public virtual void DrawPreview(Rect rect) { }

        /** If false the inspector preview wont be shown */
        protected virtual bool HasToDrawPreviewInspector()
        {
            return false;
        }

        /** Called to fill up the preview inspector window */
        protected virtual void DrawPreviewInspector() { }


        // ######################## AUX FUNCTIONS ########################

        private Dictionary<Corner, Vector3> GetCorners(Rect windowRect, Rect holderRect)
        {
            var bottomRight = new Vector2(holderRect.width - windowRect.width, holderRect.height - windowRect.height);
            return new Dictionary<Corner, Vector3>()
            {
                {Corner.TopLeft, holderRect.position},
                {Corner.TopRight, holderRect.position + new Vector2(bottomRight.x, 0) },
                {Corner.BottomLeft, holderRect.position + new Vector2(0, bottomRight.y)},
                {Corner.BottomRight, holderRect.position + bottomRight }
            };
        }

        private KeyValuePair<Corner, Vector3> GetDesiredCorner(Rect windowRect, Rect holderRect)
        {
            return GetCorners(windowRect, holderRect).ToList().MinBy(corner => Vector2.Distance(windowRect.position, corner.Value));
        }

    }
}