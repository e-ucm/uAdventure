using UnityEngine;

public class SetItemsWindow : LayoutWindow
{
    private enum SetItemsWindowType { Appearance, Documentation}

    private static SetItemsWindowType openedWindow = SetItemsWindowType.Appearance;
    private static SetItemsWindowApperance setItemsWindowApperance;
    private static SetItemsWindowDocumentation setItemsWindowDocumentation;

    private static float windowWidth, windowHeight;
    private static Rect thisRect;

    // Flag determining visibility of concrete item information
    private bool isConcreteItemVisible = false;

    private static GUISkin selectedButtonSkin;
    private static GUISkin defaultSkin;

    public SetItemsWindow(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
        : base(aStartPos, aContent, aStyle, aOptions)
    {
        setItemsWindowApperance = new SetItemsWindowApperance(aStartPos, new GUIContent(TC.get("Atrezzo.LookPanelTitle")), "Window");
        setItemsWindowDocumentation = new SetItemsWindowDocumentation(aStartPos, new GUIContent(TC.get("Atrezzo.DocPanelTitle")), "Window");

        windowWidth = aStartPos.width;
        windowHeight = aStartPos.height;

        thisRect = aStartPos;
        selectedButtonSkin = (GUISkin)Resources.Load("Editor/ButtonSelected", typeof(GUISkin));
    }


    public override void Draw(int aID)
    {
        // Show information of concrete item
        if (isConcreteItemVisible)
        {
            /**
             UPPER MENU
            */
            GUILayout.BeginHorizontal();
            if (openedWindow == SetItemsWindowType.Appearance)
                GUI.skin = selectedButtonSkin;
            if (GUILayout.Button(TC.get("Atrezzo.LookPanelTitle")))
            {
                OnWindowTypeChanged(SetItemsWindowType.Appearance);
            }
            if (openedWindow == SetItemsWindowType.Appearance)
                GUI.skin = defaultSkin;

            if (openedWindow == SetItemsWindowType.Documentation)
                GUI.skin = selectedButtonSkin;
            if (GUILayout.Button(TC.get("Atrezzo.DocPanelTitle")))
            {
                OnWindowTypeChanged(SetItemsWindowType.Documentation);
            }
            if (openedWindow == SetItemsWindowType.Documentation)
                GUI.skin = defaultSkin;
            GUILayout.EndHorizontal();

            switch (openedWindow)
            {
                case SetItemsWindowType.Appearance:
                    setItemsWindowApperance.Draw(aID);
                    break;
                case SetItemsWindowType.Documentation:
                    setItemsWindowDocumentation.Draw(aID);
                    break;
            }
        }
        else
        {
            GUILayout.Space(30);
            for (int i = 0; i < Controller.getInstance().getCharapterList().getSelectedChapterData().getAtrezzo().Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Box(Controller.getInstance().getCharapterList().getSelectedChapterData().getAtrezzo()[i].getId(), GUILayout.Width(windowWidth * 0.75f));
                if (GUILayout.Button(TC.get("GeneralText.Edit"), GUILayout.MaxWidth(windowWidth * 0.2f)))
                {
                    ShowItemWindowView(i);
                }

                GUILayout.EndHorizontal();
            }
        }
    }

    void OnWindowTypeChanged(SetItemsWindowType type_)
    {
        openedWindow = type_;
    }

    // Two methods responsible for showing right window content 
    // - concrete item info or base window view
    public void ShowBaseWindowView()
    {
        isConcreteItemVisible = false;
        GameRources.GetInstance().selectedSetItemIndex = -1;
    }

    public void ShowItemWindowView(int o)
    {
        isConcreteItemVisible = true;
        GameRources.GetInstance().selectedSetItemIndex = o;

        setItemsWindowApperance = new SetItemsWindowApperance(thisRect, new GUIContent(TC.get("Atrezzo.LookPanelTitle")), "Window");
        setItemsWindowDocumentation = new SetItemsWindowDocumentation(thisRect, new GUIContent(TC.get("Atrezzo.DocPanelTitle")), "Window");
    }
}