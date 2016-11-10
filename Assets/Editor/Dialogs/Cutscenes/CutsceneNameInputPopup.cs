using UnityEngine;
using System.Collections;
using UnityEditor;

public class CutsceneNameInputPopup : BaseInputPopup
{
    private bool isCharacterCutscene = false;
    private CharactersWindowAppearance.CharacterAnimationType type;

    public void Init(DialogReceiverInterface e, string startTextContent, System.Object characterAnimType = null)
    {
        if (characterAnimType is CharactersWindowAppearance.CharacterAnimationType)
        {
            isCharacterCutscene = true;
            type = (CharactersWindowAppearance.CharacterAnimationType)characterAnimType;
        }

        base.Init(e, startTextContent);
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField(TC.get("Animation.AskFilename"),
            EditorStyles.wordWrappedLabel);

        GUILayout.Space(30);

        textContent = GUILayout.TextField(textContent);

        GUILayout.Space(30);

        if (!Controller.getInstance().isElementIdValid(textContent, false))
        {
            GUI.enabled = false;
        }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("OK"))
        {
            textContent = "assets/animation/" + textContent + ".eaa";
            if (isCharacterCutscene)
                reference.OnDialogOk(textContent, this, type);
            else
                reference.OnDialogOk(textContent, this);

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