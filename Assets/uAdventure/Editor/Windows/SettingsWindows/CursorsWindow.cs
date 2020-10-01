
using System.Collections.Generic;
using System.Linq;
using uAdventure.Core;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Editor
{
    public class CursorsWindow : DefaultButtonMenuEditorWindowExtension
    {
        private Vector2 scroll; 
        private class CursorInfo
        {
            private string loadedNormal;
            private Texture2D normal;

            public FileChooser chooser;
            private string identifier;
            private string camelCaseIdentifier;
            public string Label { get { return ("Cursors." + camelCaseIdentifier + ".Title").Traslate(); } }
            public string Identifier { get { return identifier; } }

            public CursorInfo(string identifier)
            {
                this.identifier = identifier;
                camelCaseIdentifier = identifier.Split('-').Select(w => w.Substring(0, 1).ToUpper() + w.Substring(1)).Aggregate((w1, w2) => w1 + w2);
                chooser = new FileChooser
                {
                    Label = "Normal",
                    FileType = FileType.CURSOR,
                    Path = Controller.Instance.AdventureData.getCursorPath(identifier),
                    Empty = Controller.Instance.AdventureData.getAdventureData().getDefaultCursorPath(identifier)
                };
            }

            public Texture2D GetTexture()
            {
                var texturePath = chooser.Path;
                var loadedPath = loadedNormal;
                if (texturePath == null)
                {
                    texturePath = chooser.Empty;
                }

                if (texturePath != null && texturePath != loadedPath)
                {
                    normal = Controller.ResourceManager.getImage(texturePath);
                    loadedNormal = texturePath;
                }

                return normal;
            }
        }

        private List<CursorInfo> cursors;

        public CursorsWindow(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
            : base(rect, content, style)
        {
            Options = options;
            ButtonContent = new GUIContent("Cursors");
            cursors = new List<CursorInfo>();
            foreach (var cursor in DescriptorData.getCursorTypes())
            {
                cursors.Add(new CursorInfo(cursor));
            }
        }

        public override void Draw(int aID)
        {
            using (var scope = new GUILayout.ScrollViewScope(scroll))
            {
                scroll = scope.scrollPosition; 
                GUILayout.Label("GeneralText.Information".Traslate(), EditorStyles.boldLabel);
                GUILayout.Label("Cursors.Information".Traslate());
                GUILayout.Space(10);
                foreach (var cursorInfo in cursors)
                {
                    GUILayout.Label(cursorInfo.Label, EditorStyles.boldLabel);
                    using (new GUILayout.HorizontalScope())
                    {
                        using (new GUILayout.VerticalScope())
                        {
                            EditorGUI.BeginChangeCheck();
                            var path = Controller.Instance.AdventureData.getCursorPath(cursorInfo.Identifier);
                            cursorInfo.chooser.Path = path;
                            cursorInfo.chooser.DoLayout();
                            if (EditorGUI.EndChangeCheck())
                            {
                                Controller.Instance.AdventureData.editCursorPath(cursorInfo.Identifier, cursorInfo.chooser.Path);
                            }
                        }
                        GUILayout.Box("", GUILayout.Width(100), GUILayout.ExpandHeight(true));
                        var lastRect = GUILayoutUtility.GetLastRect();

                        var texture = cursorInfo.GetTexture();
                        lastRect.position += new Vector2(5, 5);
                        lastRect.size -= new Vector2(10, 10);
                        GUI.DrawTexture(lastRect, texture, ScaleMode.ScaleToFit);
                    }
                }
                Repaint();
            }
                
        }

        protected override void OnButton()
        {
        }
    }
}
