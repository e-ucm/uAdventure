using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Msagl.Core.Layout.ProximityOverlapRemoval.ConjugateGradient;
using uAdventure.Editor;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Editor
{
    public class SceneEditor : ComponentBasedEditor<SceneEditor>
    {
        private SceneDataControl workingScene;
        private Texture2D backgroundPreview;
        private Texture2D foregroundPreview;
        private int lastSelectedResources = -1;

        protected readonly Stack<Matrix4x4> matrices;
        public Rect Viewport { get; protected set; }

        public Matrix4x4 Matrix
        {
            get
            {
                return matrices.Peek();
            }
            set
            {
                matrices.Pop();
                matrices.Push(value);
            }
        }


        public Vector2 Size
        {
            get
            {
                var size = new Vector2(800f, 600f);
                if (backgroundPreview)
                {
                    size.x = backgroundPreview.width;
                    size.y = backgroundPreview.height;
                }
                return size;
            }
        }

        public SceneDataControl Scene
        {
            get
            {
                return workingScene;
            }
            set
            {
                if (value != workingScene)
                {
                    workingScene = value;
                    SelectedElement = null;
                    RefreshSceneResources(workingScene);
                }
            }
        }

        public SceneEditor() : base()
        {
            matrices = new Stack<Matrix4x4>();
            matrices.Push(Matrix4x4.identity);
        }

        protected override void BeforeDrawElements(Rect rect)
        {
            PushMatrix();
            Viewport = rect.AdjustToRatio(Size.x / Size.y);

            Matrix *= Matrix4x4.TRS(Viewport.position, Quaternion.identity,
                new Vector3(Viewport.width / Size.x, Viewport.height / Size.y, 1f));

            if (workingScene.getSelectedResources() != lastSelectedResources)
            {
                RefreshSceneResources(workingScene);
            }

            // Background
            if (backgroundPreview)
            {
                GUI.DrawTexture(Viewport, backgroundPreview, ScaleMode.ScaleToFit);
            }
        }

        protected override void AfterDrawElements(Rect rect)
        {

            if (foregroundPreview)
            {
                GUI.DrawTexture(Viewport, foregroundPreview, ScaleMode.ScaleToFit);
            }
            PopMatrix();
        }

        public void RefreshSceneResources(DataControlWithResources resources)
        {
            var scene = resources as SceneDataControl;

            if (scene == null)
            {
                foregroundPreview = null;
                lastSelectedResources = -1;
                return;
            }

            var back = scene.getPreviewBackground();
            var fore = scene.getPreviewForeground();

            backgroundPreview = Controller.ResourceManager.getImage(back);
            foregroundPreview = Controller.ResourceManager.getImage(fore);

            if (backgroundPreview && foregroundPreview)
            {
                foregroundPreview = CreateMask(backgroundPreview, foregroundPreview);
            }

            lastSelectedResources = scene.getSelectedResources();
        }

        private Texture2D CreateMask(Texture2D background, Texture2D foreground)
        {
            Texture2D toReturn = new Texture2D(background.width, background.height, background.format, false);

            /* TODO: Fix the foreground mask creation 
             *  var foregroundPixels = foreground.GetPixels();
             *  toReturn.SetPixels(background.GetPixels().Select((color, i) => new Color(color.r, color.g, color.b, foregroundPixels[i].r)).ToArray());
             */

            toReturn.Apply();
            return toReturn;
        }


        public void PushMatrix()
        {
            matrices.Push(matrices.Peek() * Matrix4x4.identity);
        }

        public void PopMatrix()
        {
            matrices.Pop();
        }

        public override Vector2d[] ToRelative(Vector2d[] points)
        {
            return points.Select(p => Matrix.MultiplyPoint(new Vector3((float)p.x, (float)p.y, 0))).Select(v => new Vector2d(v.x, v.y)).ToArray();
        }

        public override Vector2d[] FromRelative(Vector2d[] points)
        {
            var inverse = Matrix.inverse;
            return points.Select(p => inverse.MultiplyPoint(new Vector3((float)p.x, (float)p.y, 0))).Select(v => new Vector2d(v.x, v.y)).ToArray();
        }
    }
}