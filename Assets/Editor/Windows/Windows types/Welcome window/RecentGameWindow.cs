using UnityEngine;
using UnityEditor;
using System.Collections;

public class RecentGameWindow : LayoutWindow
{
    public RecentGameWindow(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
        : base(aStartPos, aContent, aStyle, aOptions)
    {
        fileInfoRect = new Rect(0.01f * m_Rect.width, 0.05f * m_Rect.height, 0.48f * m_Rect.width, 0.98f * m_Rect.height);
        adventureInfoMainRect = new Rect(0.51f * m_Rect.width, 0.05f * m_Rect.height, 0.48f * m_Rect.width, 0.98f * m_Rect.height);
        adventureInfoTitleRect = new Rect(0.53f * m_Rect.width, 0.15f * m_Rect.height, 0.44f * m_Rect.width, 0.15f * m_Rect.height);
        adventureInfoPathRect = new Rect(0.53f * m_Rect.width, 0.30f * m_Rect.height, 0.44f * m_Rect.width, 0.15f * m_Rect.height);
        adventureInfoDescriptionRect = new Rect(0.53f * m_Rect.width, 0.45f * m_Rect.height, 0.44f * m_Rect.width, 0.25f * m_Rect.height);
        adventureInfoPlayerModeRect = new Rect(0.53f * m_Rect.width, 0.7f * m_Rect.height, 0.44f * m_Rect.width, 0.25f * m_Rect.height);
    }

    private Rect fileInfoRect;
    private Rect adventureInfoMainRect;
    private Rect adventureInfoTitleRect;
    private Rect adventureInfoPathRect;
    private Rect adventureInfoDescriptionRect;
    private Rect adventureInfoPlayerModeRect;


    public override void Draw(int aID)
    {
        //GUILayout.BeginVertical();
        //{
        //    GUILayout.BeginArea(fileInfoRect);
        //    {
        //        GUILayout.Label(Language.GetText("FILES"));
        //        GUILayout.Box(Language.GetText("EMPTY_STRING"));
        //    }
        //    GUILayout.EndArea();

        //    GUILayout.BeginArea(adventureInfoMainRect);
        //    {
        //        GUILayout.Label(Language.GetText("ADVENTURE"));

        //        //GUILayout.BeginArea(adventureInfoTitleRect);
        //        //{
        //        GUILayout.Label(Language.GetText("ADVENTURE_TITLE"));
        //        GUILayout.Box(Language.GetText("EMPTY_STRING"));
        //        //}
        //        //GUILayout.EndArea();

        //        //GUILayout.BeginArea(adventureInfoPathRect);
        //        //{
        //        GUILayout.Label(Language.GetText("ADVENTURE_PATH"));
        //        GUILayout.Box(Language.GetText("EMPTY_STRING"));
        //        //}
        //        //GUILayout.EndArea();

        //        //GUILayout.BeginArea(adventureInfoDescriptionRect);
        //        //{
        //        GUILayout.Label(Language.GetText("ADVENTURE_DESCRIPTION"));
        //        GUILayout.Box(Language.GetText("EMPTY_STRING"));
        //        //}
        //        //GUILayout.EndArea();

        //        //GUILayout.BeginArea(adventureInfoPlayerModeRect);
        //        //{
        //        GUILayout.Label(Language.GetText("ADVENTURE_MODE"));
        //        GUILayout.Box(Language.GetText("ADVENTURE_MODE_1ST"));
        //        //}
        //        //GUILayout.EndArea();
        //    }
        //    GUILayout.EndArea();
        //}
        //GUILayout.EndHorizontal();
    }

}
