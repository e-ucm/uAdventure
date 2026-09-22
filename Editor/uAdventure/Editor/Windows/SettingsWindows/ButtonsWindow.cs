
using System.Collections.Generic;
using System.Linq;
using uAdventure.Core;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Editor
{
    public class ButtonsWindow : DefaultButtonMenuEditorWindowExtension
    {
        private Vector2 scroll;
        private class ButtonInfo 
        {
            private string loadedNormal;
            private string loadedHightlighted;
            private Texture2D normal;
            private Texture2D highlighted;

            public FileChooser normalButton, highlightedButton;
            private string identifier;
            private string camelCaseIdentifier;
            public string Label { get { return ("Buttons." + camelCaseIdentifier + ".Title").Traslate(); } }
            public string Identifier { get { return identifier; } }
        

            public ButtonInfo(string identifier)
            {
                this.identifier = identifier;
                camelCaseIdentifier = identifier.Split('-').Select(w => w.Substring(0, 1).ToUpper() + w.Substring(1)).Aggregate((w1, w2) => w1 + w2);
                normalButton = new FileChooser
                {
                    Label = "Normal",
                    FileType = FileType.BUTTON,
                    Path = Controller.Instance.AdventureData.getButtonPath(identifier, DescriptorData.NORMAL_BUTTON),
                    Empty = Controller.Instance.AdventureData.getAdventureData().getDefaultButtonPath(identifier, DescriptorData.NORMAL_BUTTON)
                };
                highlightedButton = new FileChooser
                {
                    Label = "Highlighted",
                    FileType = FileType.BUTTON,
                    Path = Controller.Instance.AdventureData.getButtonPath(identifier, DescriptorData.HIGHLIGHTED_BUTTON),
                    Empty = Controller.Instance.AdventureData.getAdventureData().getDefaultButtonPath(identifier, DescriptorData.HIGHLIGHTED_BUTTON)
                };
            }

            public Texture2D GetTexture(bool over)
            {
                var texturePath = over ? highlightedButton.Path : normalButton.Path;
                var loadedPath = over ? loadedHightlighted : loadedNormal;
                if (texturePath == null)
                {
                    texturePath = over ? highlightedButton.Empty : normalButton.Empty;
                }
                var texture = over ? highlighted : normal;

                if (texturePath != null && texturePath != loadedPath)
                {
                    texture = Controller.ResourceManager.getImage(texturePath);
                    if (over)
                    {
                        loadedHightlighted = loadedPath;
                        highlighted = texture;
                    }
                    else
                    {
                        loadedNormal = loadedPath;
                        normal = texture;
                    }
                }

                return texture;
            }
        }

        private List<ButtonInfo> buttons;

        public ButtonsWindow(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
            : base(rect, content, style)
        {
            Options = options;
            ButtonContent = new GUIContent("Buttons");
            buttons = new List<ButtonInfo>();
            foreach (var button in DescriptorData.getActionTypes())
            {
                buttons.Add(new ButtonInfo(button));
            }
        }

        public override void Draw(int aID)
        {
            using (var scope = new GUILayout.ScrollViewScope(scroll))
            {
                scroll = scope.scrollPosition;
                GUILayout.Label("GeneralText.Information".Traslate(), EditorStyles.boldLabel);
                GUILayout.Label("Buttons.Information".Traslate());
                GUILayout.Space(10);
                foreach (var buttonInfo in buttons)
                {
                    GUILayout.Label(buttonInfo.Label, EditorStyles.boldLabel);
                    using (new GUILayout.HorizontalScope())
                    {
                        using (new GUILayout.VerticalScope())
                        {
                            var adc = Controller.Instance.AdventureData;
                            var ad = Controller.Instance.AdventureData.getAdventureData();
                            EditorGUI.BeginChangeCheck();
                            var normalPath = ad.getButtonPathFromEngine(buttonInfo.Identifier, DescriptorData.NORMAL_BUTTON);
                            if (string.IsNullOrEmpty(normalPath))
                            {
                                normalPath = buttonInfo.normalButton.Empty;
                            }
                            buttonInfo.normalButton.Path = normalPath;
                            buttonInfo.normalButton.DoLayout();
                            if (EditorGUI.EndChangeCheck())
                            {
                                adc.editButtonPath(buttonInfo.Identifier, DescriptorData.NORMAL_BUTTON, buttonInfo.normalButton.Path);
                            }

                            EditorGUI.BeginChangeCheck();
                            var highlightedPath = ad.getButtonPathFromEngine(buttonInfo.Identifier, DescriptorData.HIGHLIGHTED_BUTTON);
                            if (string.IsNullOrEmpty(highlightedPath))
                            {
                                highlightedPath = buttonInfo.highlightedButton.Empty;
                            }
                            buttonInfo.highlightedButton.Path = highlightedPath;
                            buttonInfo.highlightedButton.DoLayout();
                            if (EditorGUI.EndChangeCheck())
                            {
                                adc.editButtonPath(buttonInfo.Identifier, DescriptorData.HIGHLIGHTED_BUTTON, buttonInfo.highlightedButton.Path);
                            }
                        }
                        GUILayout.Box("", GUILayout.Width(100), GUILayout.ExpandHeight(true));
                        var lastRect = GUILayoutUtility.GetLastRect();

                        var texture = buttonInfo.GetTexture(lastRect.Contains(Event.current.mousePosition));
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
