using UnityEngine;
using UnityEditor;
using System.Collections;

public class PlayerWindowDialogConfiguration : LayoutWindow
{
    private Color fontFrontColor, fontBorderColor, bubbleBcgColor, bubbleBorderColor;
    private Color fontFrontColorLast, fontBorderColorLast, bubbleBcgColorLast, bubbleBorderColorLast;

    private bool shouldShowSpeachBubble, shouldShowSpeachBubbleLast;

    private GUISkin skinDefault;
    private GUIStyle previewTextStyle;

    private Texture2D bckImage = null;

    public PlayerWindowDialogConfiguration(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
        : base(aStartPos, aContent, aStyle, aOptions)
    {
        shouldShowSpeachBubble = shouldShowSpeachBubbleLast =
            Controller.getInstance().getCharapterList().getSelectedChapterData().getPlayer().getShowsSpeechBubbles();

        fontFrontColor = fontFrontColorLast = ColorConverter.HexToColor(Controller.getInstance().getCharapterList().getSelectedChapterData().getPlayer().getTextFrontColor());
        fontBorderColor = fontBorderColorLast = ColorConverter.HexToColor(Controller.getInstance().getCharapterList().getSelectedChapterData().getPlayer().getTextBorderColor());
        bubbleBcgColor = bubbleBcgColorLast = ColorConverter.HexToColor(Controller.getInstance().getCharapterList().getSelectedChapterData().getPlayer().getBubbleBkgColor());
        bubbleBorderColor = fontFrontColorLast = ColorConverter.HexToColor(Controller.getInstance().getCharapterList().getSelectedChapterData().getPlayer().getBubbleBorderColor());

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
        if(shouldShowSpeachBubble != shouldShowSpeachBubbleLast)
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
        Controller.getInstance().getCharapterList().getSelectedChapterData().getPlayer().setShowsSpeechBubbles(shouldShowSpeachBubble);
    }

    void OnFontFrontChange(Color val)
    {
        fontFrontColorLast = val;
        previewTextStyle.normal.textColor = fontFrontColor;
        Controller.getInstance().getCharapterList().getSelectedChapterData().getPlayer().setTextFrontColor(ColorConverter.ColorToHex(val));
        Debug.Log(ColorConverter.ColorToHex(val));
    }

    void OnFontBorderChange(Color val)
    {
        fontBorderColorLast = val;
        Controller.getInstance().getCharapterList().getSelectedChapterData().getPlayer().setTextBorderColor(ColorConverter.ColorToHex(val));
    }

    void OnBubbleBcgChange(Color val)
    {
        bubbleBcgColorLast = val;
        Controller.getInstance().getCharapterList().getSelectedChapterData().getPlayer().setBubbleBkgColor(ColorConverter.ColorToHex(val));
    }

    void OnBubbleBorderChange(Color val)
    {
        bubbleBorderColorLast = val;
        Controller.getInstance().getCharapterList().getSelectedChapterData().getPlayer().setBubbleBorderColor(ColorConverter.ColorToHex(val));
    }
}