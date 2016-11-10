using UnityEngine;
using System.Collections;

public class SetItemsWindowDocumentation : LayoutWindow
{
    private string documentation, documentationLast;
    private float windowHeight;

    public SetItemsWindowDocumentation(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
        : base(aStartPos, aContent, aStyle, aOptions)
    {
        string doc = "";

        if (GameRources.GetInstance().selectedSetItemIndex >= 0)
            doc = Controller.getInstance().getCharapterList().getSelectedChapterData().getAtrezzo()[
                GameRources.GetInstance().selectedSetItemIndex].getDocumentation();
        doc = (doc == null ? "" : doc);
        documentation = documentationLast = doc;
        windowHeight = aStartPos.height;
    }


    public override void Draw(int aID)
    {
        GUILayout.Space(20);
        GUILayout.Label(TC.get("Atrezzo.DocPanelTitle"));
        GUILayout.Space(20);
        documentation = GUILayout.TextArea(documentation, GUILayout.MinHeight(0.4f * windowHeight));
        if (!documentation.Equals(documentationLast))
            OnDocumentationChanged(documentation);
    }

    private void OnDocumentationChanged(string s)
    {
        Controller.getInstance().getCharapterList().getSelectedChapterData().getAtrezzo()[GameRources.GetInstance().selectedSetItemIndex].setDocumentation(s);
        documentationLast = s;
    }
}
