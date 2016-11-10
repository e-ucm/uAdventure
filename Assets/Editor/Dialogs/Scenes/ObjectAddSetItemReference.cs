using UnityEngine;
using UnityEditor;
using System;

public class ObjectAddSetItemReference : BaseChooseObjectPopup
{
    private bool okActive = true;

    public override void Init(DialogReceiverInterface e)
    {
        elements = Controller.getInstance().getSelectedChapterDataControl().getAtrezzoList().getItemIDs();
        if (elements == null)
        {
            elements = new string[1];
            elements[0] = "None";
            okActive = false;
        }
        selectedElementID = elements[0];

        base.Init(e);
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField(TC.get("Operation.AddAtrezzoReferenceMessage"), EditorStyles.boldLabel);

        GUILayout.Space(20);

        selectedElementID = elements[EditorGUILayout.Popup(Array.IndexOf(elements, selectedElementID), elements)];

        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        if (!okActive)
            GUI.enabled = false;
        if (GUILayout.Button("OK"))
        {
            reference.OnDialogOk(selectedElementID, this);
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
