using UnityEngine;
using System.Collections;

public class ConversationWindow : LayoutWindow
{
    private static float windowWidth, windowHeight;

    private static Rect thisRect;

    public ConversationWindow(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
        : base(aStartPos, aContent, aStyle, aOptions)
    {

        windowWidth = aStartPos.width;
        windowHeight = aStartPos.height;

        thisRect = aStartPos;
    }


    public override void Draw(int aID)
    {
        GUILayout.Space(30);
        for (int i = 0; i < Controller.getInstance().getCharapterList().getSelectedChapterData().getConversations().Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Box(Controller.getInstance().getCharapterList().getSelectedChapterData().getConversations()[i].getId(), GUILayout.Width(windowWidth * 0.65f));
            if (GUILayout.Button("Edit conversation", GUILayout.MaxWidth(windowWidth * 0.3f)))
            {
                ConversationEditorWindow window = (ConversationEditorWindow)ScriptableObject.CreateInstance(typeof(ConversationEditorWindow));
                window.Init((GraphConversation) Controller.getInstance().getCharapterList().getSelectedChapterData().getConversations()[i]);
            }

            GUILayout.EndHorizontal();

        }
    }

}