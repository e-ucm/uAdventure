using UnityEngine;
using System.Collections;
using UnityEditor;

public class CharactersWindowDialogConfiguration : LayoutWindow
{
    private Color fontFrontColor, fontBorderColor, bubbleBcgColor, bubbleBorderColor;
    private Color fontFrontColorLast, fontBorderColorLast, bubbleBcgColorLast, bubbleBorderColorLast;

    private bool shouldShowSpeachBubble, shouldShowSpeachBubbleLast;

    private GUISkin skinDefault;
    private GUIStyle previewTextStyle;

    private Texture2D bckImage = null;

    public CharactersWindowDialogConfiguration(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
        : base(aStartPos, aContent, aStyle, aOptions)
    {
        if (GameRources.GetInstance().selectedCharacterIndex >= 0)
        {
            shouldShowSpeachBubble = shouldShowSpeachBubbleLast =
                    Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
                    GameRources.GetInstance().selectedCharacterIndex].getShowsSpeechBubbles();

            fontFrontColor = fontFrontColorLast = ColorConverter.HexToColor(Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
                    GameRources.GetInstance().selectedCharacterIndex].getTextFrontColor());
            fontBorderColor = fontBorderColorLast = ColorConverter.HexToColor(Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
                    GameRources.GetInstance().selectedCharacterIndex].getTextBorderColor());
            bubbleBcgColor = bubbleBcgColorLast = ColorConverter.HexToColor(Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
                    GameRources.GetInstance().selectedCharacterIndex].getBubbleBkgColor());
            bubbleBorderColor = fontFrontColorLast = ColorConverter.HexToColor(Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
                    GameRources.GetInstance().selectedCharacterIndex].getBubbleBorderColor());
        }

        bckImage = (Texture2D)Resources.Load("Editor/TextBubble", typeof(Texture2D));
        previewTextStyle = new GUIStyle();
        previewTextStyle.fontSize = 24;
        previewTextStyle.normal.textColor = fontFrontColor;
        //previewTextStyle.normal.background = bckImage;
    }

    public override void Draw(int aID)
    {
        GUILayout.Label(TC.get("Player.TextColor"));

        GUILayout.Space(20);
        shouldShowSpeachBubble = GUILayout.Toggle(shouldShowSpeachBubble, TC.get("Player.ShowsSpeechBubble"));
        if (shouldShowSpeachBubble != shouldShowSpeachBubbleLast)
            OnShowBubbleChange();

        GUILayout.Space(10);

        GUILayout.Label(TC.get("GeneralText.PreviewText"), previewTextStyle);

        GUILayout.Space(20);

        fontFrontColor = EditorGUILayout.ColorField(TC.get("Player.FrontColor"), fontFrontColor);
        if (fontFrontColor != fontFrontColorLast)
        {
            OnFontFrontChange(fontFrontColor);
        }

        fontBorderColor = EditorGUILayout.ColorField(TC.get("Player.BorderColor"), fontBorderColor);
        if (fontBorderColor != fontBorderColorLast)
        {
            OnFontBorderChange(fontBorderColor);
        }

        if (!shouldShowSpeachBubble)
            GUI.enabled = false;

        bubbleBcgColor = EditorGUILayout.ColorField(TC.get("Player.BubbleBkgColor"), bubbleBcgColor);
        if (bubbleBcgColor != bubbleBcgColorLast)
        {
            OnBubbleBcgChange(bubbleBcgColor);
        }

        bubbleBorderColor = EditorGUILayout.ColorField(TC.get("Player.BubbleBorderColor"), bubbleBorderColor);
        if (bubbleBorderColor != bubbleBorderColorLast)
        {
            OnBubbleBorderChange(bubbleBorderColor);
        }

        GUI.enabled = true;
    }

    void OnShowBubbleChange()
    {
        shouldShowSpeachBubbleLast = shouldShowSpeachBubble;
        Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
               GameRources.GetInstance().selectedCharacterIndex].setShowsSpeechBubbles(shouldShowSpeachBubble);
    }

    void OnFontFrontChange(Color val)
    {
        fontFrontColorLast = val;
        previewTextStyle.normal.textColor = fontFrontColor;
        Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
               GameRources.GetInstance().selectedCharacterIndex].setTextFrontColor(ColorConverter.ColorToHex(val));
    }

    void OnFontBorderChange(Color val)
    {
        fontBorderColorLast = val;
        Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
               GameRources.GetInstance().selectedCharacterIndex].setTextBorderColor(ColorConverter.ColorToHex(val));
    }

    void OnBubbleBcgChange(Color val)
    {
        bubbleBcgColorLast = val;
        Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
               GameRources.GetInstance().selectedCharacterIndex].setBubbleBkgColor(ColorConverter.ColorToHex(val));
    }

    void OnBubbleBorderChange(Color val)
    {
        bubbleBorderColorLast = val;
        Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
               GameRources.GetInstance().selectedCharacterIndex].setBubbleBorderColor(ColorConverter.ColorToHex(val));
    }
}
