using UnityEngine;
using System.Collections;

public class PlayerWindowDocumentation : LayoutWindow
{
    private string documentation, documentationLast;
    private float windowHeight;

    public PlayerWindowDocumentation(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
        : base(aStartPos, aContent, aStyle, aOptions)
    {
        documentation = documentationLast = Controller.getInstance().getCharapterList().getSelectedChapterData().getPlayer().getDocumentation();
        if (documentation == null)
            documentation = documentationLast = "";
        windowHeight = aStartPos.height;
    }


    public override void Draw(int aID)
    {
        GUILayout.Space(20);
        GUILayout.Label(TC.get("NPC.Documentation"));
        GUILayout.Space(20);
        documentation = GUILayout.TextArea(documentation, GUILayout.MinHeight(0.4f * windowHeight));
        if (!documentation.Equals(documentationLast))
            OnDocumentationChanged(documentation);
    }

    private void OnDocumentationChanged(string s)
    {
        Controller.getInstance().getCharapterList().getSelectedChapterData().getPlayer().setDocumentation(s);
        documentationLast = s;
    }

}