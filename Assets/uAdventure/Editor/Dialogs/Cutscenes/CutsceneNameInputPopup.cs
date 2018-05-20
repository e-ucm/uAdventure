using UnityEngine;
using System.Collections;
using UnityEditor;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class CutsceneNameInputPopup : BaseInputPopup
    {
        private bool isCharacterCutscene = false;
        private CharactersWindowAppearance.CharacterAnimationType type;
        private string value;

        public void Init(DialogReceiverInterface e, string startTextContent, System.Object characterAnimType = null)
        {
            if (characterAnimType is CharactersWindowAppearance.CharacterAnimationType)
            {
                isCharacterCutscene = true;
                type = (CharactersWindowAppearance.CharacterAnimationType)characterAnimType;
            }
            this.value = startTextContent;

            base.Init(e);
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField(TC.get("Animation.AskFilename"),
                EditorStyles.wordWrappedLabel);

            GUILayout.Space(20);

            value = GUILayout.TextField(value);

            GUILayout.FlexibleSpace();

            if (!Controller.Instance.isElementIdValid(value, false))
            {
                GUI.enabled = false;
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("OK"))
            {
                value = "assets/animation/" + value + ".eaa.xml";
                if (isCharacterCutscene)
                    reference.OnDialogOk(value, this, type);
                else
                    reference.OnDialogOk(value, this);

                this.Close();
            }
            GUI.enabled = true;
            if (GUILayout.Button(TC.get("GeneralText.Cancel")))
            {
                reference.OnDialogCanceled();
                this.Close();
            }
            GUILayout.EndHorizontal();
        }
    }
}