using UnityEngine;
using System.Collections;

public class ItemsWindow : LayoutWindow
{
    private enum ItemsWindowType { Appearance, DescriptionConfig, Actions}

    private static ItemsWindowType openedWindow = ItemsWindowType.Appearance;
    private static ItemsWindowActions itemsWindowActions;
    private static ItemsWindowAppearance itemsWindowAppearance;
    private static ItemsWindowDescription itemsWindowDescription;

    private static float windowWidth, windowHeight;

    private static Rect thisRect;

    // Flag determining visibility of concrete item information
    private bool isConcreteItemVisible = false;

    private static GUISkin selectedButtonSkin;
    private static GUISkin defaultSkin;

    public ItemsWindow(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
        : base(aStartPos, aContent, aStyle, aOptions)
    {
        itemsWindowActions = new ItemsWindowActions(aStartPos, new GUIContent(TC.get("Item.ActionsPanelTitle")), "Window");
        itemsWindowAppearance = new ItemsWindowAppearance(aStartPos, new GUIContent(TC.get("Item.LookPanelTitle")), "Window");
        itemsWindowDescription = new ItemsWindowDescription(aStartPos, new GUIContent(TC.get("Item.DocPanelTitle")), "Window");

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
            if (openedWindow == ItemsWindowType.Appearance)
                GUI.skin = selectedButtonSkin;
            if (GUILayout.Button(TC.get("Item.LookPanelTitle")))
            {
                OnWindowTypeChanged(ItemsWindowType.Appearance);
            }
            if (openedWindow == ItemsWindowType.Appearance)
                GUI.skin = defaultSkin;

            if (openedWindow == ItemsWindowType.Actions)
                GUI.skin = selectedButtonSkin;
            if (GUILayout.Button(TC.get("Item.ActionsPanelTitle")))
            {
                OnWindowTypeChanged(ItemsWindowType.Actions);
            }
            if (openedWindow == ItemsWindowType.Actions)
                GUI.skin = defaultSkin;

            if (openedWindow == ItemsWindowType.DescriptionConfig)
                GUI.skin = selectedButtonSkin;
            if (GUILayout.Button(TC.get("Item.DocPanelTitle")))
            {
                OnWindowTypeChanged(ItemsWindowType.DescriptionConfig);
            }
            if (openedWindow == ItemsWindowType.DescriptionConfig)
                GUI.skin = defaultSkin;

            GUILayout.EndHorizontal();

            switch (openedWindow)
            {
                case ItemsWindowType.Actions:
                    itemsWindowActions.Draw(aID);
                    break;
                case ItemsWindowType.Appearance:
                    itemsWindowAppearance.Draw(aID);
                    break;
                case ItemsWindowType.DescriptionConfig:
                    itemsWindowDescription.Draw(aID);
                    break;
            }
        }
        else
        {
            GUILayout.Space(30);
            for (int i = 0; i < Controller.getInstance().getCharapterList().getSelectedChapterData().getItems().Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Box(Controller.getInstance().getCharapterList().getSelectedChapterData().getItems()[i].getId(), GUILayout.Width(windowWidth * 0.75f));
                if (GUILayout.Button(TC.get("GeneralText.Edit"), GUILayout.MaxWidth(windowWidth * 0.2f)))
                {
                    ShowItemWindowView(i);
                }

                GUILayout.EndHorizontal();

            }
        }
    }

    // Two methods responsible for showing right window content 
    // - concrete item info or base window view
    public void ShowBaseWindowView()
    {
        isConcreteItemVisible = false;
        GameRources.GetInstance().selectedItemIndex = -1;
    }

    public void ShowItemWindowView(int o)
    {
        isConcreteItemVisible = true;
        GameRources.GetInstance().selectedItemIndex = o;

        itemsWindowActions = new ItemsWindowActions(thisRect, new GUIContent(TC.get("Item.ActionsPanelTitle")), "Window");
        itemsWindowAppearance = new ItemsWindowAppearance(thisRect, new GUIContent(TC.get("Item.LookPanelTitle")), "Window");
        itemsWindowDescription = new ItemsWindowDescription(thisRect, new GUIContent(TC.get("Item.DocPanelTitle")), "Window");
    }

    void OnWindowTypeChanged(ItemsWindowType type_)
    {
        openedWindow = type_;
    }
}
