using System.Linq;
using uAdventure.Core;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Editor
{
    public class AnimationField : FileChooser
    {

        const string EMPTY = "assets/special/EmptyAnimation";

        public AnimationField()
        {
            Empty = EMPTY;
        }

        public override void DoLayout(params GUILayoutOption[] options)
        {
            var initialPath = Path;
            EditorGUILayout.BeginHorizontal(options);
            {
                if (!string.IsNullOrEmpty(newFilePath))
                {
                    Path = newFilePath;
                    newFilePath = string.Empty;
                }
                DrawPathLayout();
                DrawSelectLayout();
                DrawView();
                DrawClearLayout();
            }
            EditorGUILayout.EndHorizontal();
            GUI.changed = initialPath != Path || frameChanged;
        }

        protected void DrawView()
        {
            var text = EMPTY.Equals(Path) ? TC.get("Resources.Create") : TC.get("Resources.Edit");
            if (GUILayout.Button(text, GUILayout.Width(GUI.skin.button.CalcSize(new GUIContent(text)).x)))
            {
                // For not-existing cutscene - show new cutscene name dialog
                if (Path.Equals(EMPTY))
                {
                    ScriptableObject.CreateInstance<CutsceneNameInputPopup>().Init(this, "");
                }
                else
                {
                    EditCutscene();
                }
            }
        }

        string newFilePath = string.Empty;
        private bool frameChanged = false;

        public override void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
        {

            if (workingObject is CutsceneNameInputPopup)
            {
                newFilePath = message;
                OnSlidesceneCreated(message);
                EditCutscene();
            }
            else if (workingObject is CutsceneSlidesEditor)
            {
                newFilePath = message;
            }
            else if (workingObject is uAdventure.Core.Animation)
            {
                frameChanged = true;
            }
            else
            {
                base.OnDialogOk(message, workingObject, workingObjectSecond);
                EditCutscene();
            }
        }

        void OnSlidesceneCreated(string val)
        {
            uAdventure.Core.Animation newAnim = new uAdventure.Core.Animation(val.Split('/').Last());
            newAnim.getFrame(0).setUri(EMPTY + "_01.png");
            AnimationWriter.WriteAnimation(val, newAnim);
            AssetDatabase.ImportAsset("Assets/uAdventure/Resources/CurrentGame/" + val);
            Path = val;
        }

        void EditCutscene()
        {
            ScriptableObject.CreateInstance<CutsceneSlidesEditor>().Init(this, Path);
        }
    }
}
