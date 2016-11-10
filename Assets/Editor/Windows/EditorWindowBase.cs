using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

public class EditorWindowBase : EditorWindow, DialogReceiverInterface
{
    public enum EditorMenuItem
    {
        File,
        Edit,
        Adventure,
        Chapters,
        Run,
        Configuration,
        About
    };

    public enum EditorWindowType
    {
        Chapter,
        Scenes,
        Cutscenes,
        Books,
        Items,
        SetItems,
        Player,
        Characters,
        Conversations,
        AdvancedFeatures,
        AdaptationProfiles,
        AssesmentProfiles,
        completables
    };

    // The position of the window
    private static float windowWidth, windowHeight;
    private static EditorWindow thisWindowReference;
    private static Rect buttonMenuRect, leftMenuRect, windowRect;

    private static WindowMenuContainer fileMenu,
        editMenu,
        adventureMenu,
        chaptersMenu,
        runMenu,
        configurationMenu,
        aboutMenu;

    private static EditorWindowType openedWindow = EditorWindowType.Chapter;

    private LayoutWindow m_Window1 = null;
    private static ChapterWindow chapterWindow;
    private static ScenesWindow scenesWindow;
    private static CutscenesWindow cutscenesWindow;
    private static BooksWindow booksWindow;
    private static ItemsWindow itemsWindow;
    private static SetItemsWindow setItemsWindow;
    private static PlayerWindow playerWindow;
    private static CharactersWindow characterWindow;
    private static ConversationWindow conversationWindow;
    private static AdvencedFeaturesWindow advencedFeaturesWindow;
    private static AdvencedFeaturesWindow completablesWindow;
    //private static AdaptationProfileWindow adapatationProfileWindow;
    private static AssesmentProfileWindow assesmentProfileWindow;

    private static Vector2 scrollPosition;

    private static GUISkin defaultGUISkin;
    private static GUISkin leftSubMenuSkin;
    private static GUISkin leftSubMenuConcreteItemSkin;

    private static Texture2D addTexture = null;
    private static Texture2D deleteImg = null;
    private static Texture2D duplicateImg = null;

    private static Texture2D redoTexture = null;
    private static Texture2D undoTexture = null;


    private static Texture2D sceneTexture = null;
    private static Texture2D cutsceneTexture = null;
    private static Texture2D bookTexture = null;
    private static Texture2D itemTexture = null;
    private static Texture2D setItemTexture = null;
    private static Texture2D playerTexture = null;
    private static Texture2D characterTexture = null;
    private static Texture2D conversationTexture = null;
    private static Texture2D advancedTexture = null;
    private static Texture2D adaptationTexture = null;
    private static Texture2D assessmentTexture = null;
    private static Texture2D completableTexture = null;

    private static GUIContent leftMenuContentScene;
    private static GUIContent leftMenuContentCutscene;
    private static GUIContent leftMenuContentBook;
    private static GUIContent leftMenuContentItem;
    private static GUIContent leftMenuContentSetItem;
    private static GUIContent leftMenuContentPlayer;
    private static GUIContent leftMenuContentCharacter;
    private static GUIContent leftMenuContentConversation;
    private static GUIContent leftMenuContentAdvanced;
    private static GUIContent leftMenuContentAdaptation;
    private static GUIContent leftMenuContentAssessment;

    [MenuItem("eAdventure/Open eAdventure editor")]
    public static void Init()
    {
        if (!Controller.getInstance().Initialized())
        {
            Controller.resetInstance();
            Language.Initialize();
            Controller.getInstance().init();
        }

        thisWindowReference = EditorWindow.GetWindow(typeof (EditorWindowBase));
        windowWidth = EditorWindow.focusedWindow.position.width;
        windowHeight = EditorWindow.focusedWindow.position.height;
        buttonMenuRect = new Rect(0.01f*windowWidth, 0.01f*windowHeight, windowWidth*0.98f, windowHeight*0.10f);
        leftMenuRect = new Rect(0.01f*windowWidth, 0.12f*windowHeight, windowWidth*0.14f, windowHeight*0.87f);
        windowRect = new Rect(0.16f*windowWidth, 0.12f*windowHeight, windowWidth*0.83f, windowHeight*0.85f);

        leftSubMenuSkin = (GUISkin) Resources.Load("Editor/EditorLeftMenuItemSkin", typeof (GUISkin));
        leftSubMenuConcreteItemSkin =
            (GUISkin) Resources.Load("Editor/EditorLeftMenuItemSkinConcreteOptions", typeof (GUISkin));

        redoTexture = (Texture2D) Resources.Load("EAdventureData/img/icons/redo", typeof (Texture2D));
        undoTexture = (Texture2D) Resources.Load("EAdventureData/img/icons/undo", typeof (Texture2D));

        addTexture = (Texture2D) Resources.Load("EAdventureData/img/icons/addNode", typeof (Texture2D));

        deleteImg = (Texture2D) Resources.Load("EAdventureData/img/icons/deleteContent", typeof (Texture2D));
        duplicateImg = (Texture2D) Resources.Load("EAdventureData/img/icons/duplicateNode", typeof (Texture2D));

        sceneTexture = (Texture2D) Resources.Load("EAdventureData/img/icons/scenes", typeof (Texture2D));
        cutsceneTexture = (Texture2D) Resources.Load("EAdventureData/img/icons/cutscenes", typeof (Texture2D));
        bookTexture = (Texture2D) Resources.Load("EAdventureData/img/icons/books", typeof (Texture2D));
        itemTexture = (Texture2D) Resources.Load("EAdventureData/img/icons/items", typeof (Texture2D));
        setItemTexture = (Texture2D) Resources.Load("EAdventureData/img/icons/Atrezzo-List-1", typeof (Texture2D));
        playerTexture = (Texture2D) Resources.Load("EAdventureData/img/icons/player", typeof (Texture2D));
        characterTexture = (Texture2D) Resources.Load("EAdventureData/img/icons/npcs", typeof (Texture2D));
        conversationTexture = (Texture2D) Resources.Load("EAdventureData/img/icons/conversations", typeof (Texture2D));
        advancedTexture = (Texture2D) Resources.Load("EAdventureData/img/icons/advanced", typeof (Texture2D));
        adaptationTexture = (Texture2D) Resources.Load("EAdventureData/img/icons/adaptationProfiles", typeof (Texture2D));
        assessmentTexture = (Texture2D) Resources.Load("EAdventureData/img/icons/assessmentProfiles", typeof (Texture2D));

        thisWindowReference.Show();

        fileMenu = new FileMenu();
        editMenu = new EditMenu();
        adventureMenu = new AdventureMenu();
        chaptersMenu = new ChaptersMenu();
        runMenu = new RunMenu();
        configurationMenu = new ConfigurationMenu();
        aboutMenu = new AboutMenu();

        InitGUI();
    }

    public static void InitGUI()
    {
        chapterWindow = new ChapterWindow(windowRect, new GUIContent(TC.get("Element.Name0")), "Window");
        scenesWindow = new ScenesWindow(windowRect, new GUIContent(TC.get("Element.Name1")), "Window");
        cutscenesWindow = new CutscenesWindow(windowRect, new GUIContent(TC.get("Element.Name9")), "Window");
        booksWindow = new BooksWindow(windowRect, new GUIContent(TC.get("Element.Name11")), "Window");
        itemsWindow = new ItemsWindow(windowRect, new GUIContent(TC.get("Element.Name18")), "Window");
        setItemsWindow = new SetItemsWindow(windowRect, new GUIContent(TC.get("Element.Name59")), "Window");
        playerWindow = new PlayerWindow(windowRect, new GUIContent(TC.get("Element.Name26")), "Window");
        characterWindow = new CharactersWindow(windowRect, new GUIContent(TC.get("Element.Name27")), "Window");
        conversationWindow = new ConversationWindow(windowRect, new GUIContent(TC.get("Element.Name31")),"Window");
        advencedFeaturesWindow = new AdvencedFeaturesWindow(windowRect, new GUIContent(TC.get("AdvancedFeatures.Title")), "Window");
        //adapatationProfileWindow = new AdaptationProfileWindow(windowRect,
        //    new GUIContent(Language.GetText("ADAPTATION_PROFILES")), "Window");
        assesmentProfileWindow = new AssesmentProfileWindow(windowRect, new GUIContent(Language.GetText("ASSESMENT_PROFILES")), "Window");


        // Left menu buttons
        leftMenuContentScene = new GUIContent();
        leftMenuContentScene.image = (Texture2D)sceneTexture;
        leftMenuContentScene.text = TC.get("Element.Name1");

        leftMenuContentCutscene = new GUIContent();
        leftMenuContentCutscene.image = (Texture2D)cutsceneTexture;
        leftMenuContentCutscene.text = TC.get("Element.Name9");

        leftMenuContentBook = new GUIContent();
        leftMenuContentBook.image = (Texture2D)bookTexture;
        leftMenuContentBook.text = TC.get("Element.Name11");

        leftMenuContentItem = new GUIContent();
        leftMenuContentItem.image = (Texture2D)itemTexture;
        leftMenuContentItem.text = TC.get("Element.Name18");

        leftMenuContentSetItem = new GUIContent();
        leftMenuContentSetItem.image = (Texture2D)setItemTexture;
        leftMenuContentSetItem.text = TC.get("Element.Name59");

        leftMenuContentPlayer = new GUIContent();
        leftMenuContentPlayer.image = (Texture2D)playerTexture;
        leftMenuContentPlayer.text = TC.get("Element.Name26");

        leftMenuContentCharacter = new GUIContent();
        leftMenuContentCharacter.image = (Texture2D)characterTexture;
        leftMenuContentCharacter.text = TC.get("Element.Name27");

        leftMenuContentConversation = new GUIContent();
        leftMenuContentConversation.image = (Texture2D)conversationTexture;
        leftMenuContentConversation.text = TC.get("Element.Name31");

        leftMenuContentAdvanced = new GUIContent();
        leftMenuContentAdvanced.image = (Texture2D)advancedTexture;
        leftMenuContentAdvanced.text = TC.get("AdvancedFeatures.Title");

        leftMenuContentAssessment = new GUIContent();
        leftMenuContentAssessment.image = (Texture2D)assessmentTexture;
        leftMenuContentAssessment.text = TC.get("AssessmentFeatures.Title");
    }

    public static void RefreshChapter()
    {
        chapterWindow = new ChapterWindow(windowRect, new GUIContent(TC.get("Element.Name0")), "Window");
        openedWindow = EditorWindowType.Chapter;
    }

    void OnGUI()
    {
        /**
        UPPER MENU
        */
        GUILayout.BeginArea(buttonMenuRect);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(TC.get("MenuFile.Title")))
        {
            fileMenu.menu.ShowAsContext();
        }
        //if (GUILayout.Button(Language.GetText("GeneralText.Edit")))
        //{
        //    editMenu.menu.ShowAsContext();
        //}
        //if (GUILayout.Button(Language.GetText("ADVENTURE")))
        //{
        //    adventureMenu.menu.ShowAsContext();
        //}
        if (GUILayout.Button(TC.get("MenuChapters.Title")))
        {
            chaptersMenu.menu.ShowAsContext();
        }
        //if (GUILayout.Button(Language.GetText("RUN")))
        //{
        //    runMenu.menu.ShowAsContext();
        //}
        if (GUILayout.Button(TC.get("MenuConfiguration.Title")))
        {
            configurationMenu.menu.ShowAsContext();
        }
        if (GUILayout.Button(TC.get("About")))
        {
            aboutMenu.menu.ShowAsContext();
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        /**
        LEFT MENU
        */
        GUILayout.BeginArea(leftMenuRect);
        GUILayout.BeginVertical();

        //GUILayout.BeginHorizontal(GUILayout.MaxWidth(25), GUILayout.MaxHeight(25));
        //if (GUILayout.Button(undoTexture, GUILayout.MaxWidth(25), GUILayout.MaxHeight(25)))
        //{
        //    UndoAction();
        //}

        //GUILayout.Space(5);

        //if (GUILayout.Button(redoTexture, GUILayout.MaxWidth(25), GUILayout.MaxHeight(25)))
        //{
        //    RedoAction();
        //}
        //GUILayout.EndHorizontal();

        //GUILayout.Space(25);

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        // Button event chapter
        if (GUILayout.Button(TC.get("Element.Name0")))
        {
            chapterWindow = new ChapterWindow(windowRect, new GUIContent(TC.get("Element.Name0")), "Window");
            OnWindowTypeChanged(EditorWindowType.Chapter);
        }

        // Button event scene
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(leftMenuContentScene))
        {
            OnWindowTypeChanged(EditorWindowType.Scenes);
            scenesWindow.ShowBaseWindowView();
        }
        //Add button scene
        if (openedWindow == EditorWindowType.Scenes)
        {
            if (GUILayout.Button(addTexture))
            {
                ChapterElementNameInputPopup window =
                    (ChapterElementNameInputPopup)
                        ScriptableObject.CreateInstance(typeof (ChapterElementNameInputPopup));
                window.Init(this, "Scene", EditorWindowType.Scenes);
            }
        }
        GUILayout.EndHorizontal();
        // Item sublist scene
        if (openedWindow == EditorWindowType.Scenes)
        {
            GUI.skin = leftSubMenuSkin;
            for (int i = 0;
                i < Controller.getInstance().getCharapterList().getSelectedChapterData().getScenes().Count;
                i++)
            {
                if (i == GameRources.GetInstance().selectedSceneIndex)
                {
                    GUI.skin = leftSubMenuConcreteItemSkin;
                }

                if (
                    GUILayout.Button(
                        Controller.getInstance().getCharapterList().getSelectedChapterData().getScenes()[i].getId()))
                {
                    scenesWindow.ShowItemWindowView(i);
                }

                if (i == GameRources.GetInstance().selectedSceneIndex)
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Rename"))
                    {
                        Debug.Log("Rename");
                    }
                    if (GUILayout.Button(duplicateImg))
                    {
                        Controller.getInstance()
                            .getCharapterList()
                            .getSelectedChapterDataControl()
                            .getScenesList()
                            .duplicateElement(
                                Controller.getInstance()
                                    .getCharapterList()
                                    .getSelectedChapterDataControl()
                                    .getScenesList()
                                    .getScenes()[i]);

                    }
                    if (GUILayout.Button(deleteImg))
                    {
                        Controller.getInstance()
                            .getCharapterList()
                            .getSelectedChapterDataControl()
                            .getScenesList()
                            .deleteElement(
                                Controller.getInstance()
                                    .getCharapterList()
                                    .getSelectedChapterDataControl()
                                    .getScenesList()
                                    .getScenes()[i], false);
                        scenesWindow.ShowBaseWindowView();
                    }
                    GUILayout.EndHorizontal();
                    GUI.skin = leftSubMenuSkin;
                }
            }
            GUI.skin = defaultGUISkin;
        }



        // Button event cutscene
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(leftMenuContentCutscene))
        {
            OnWindowTypeChanged(EditorWindowType.Cutscenes);
            cutscenesWindow.ShowBaseWindowView();
        }
        //Add button cutscene
        if (openedWindow == EditorWindowType.Cutscenes)
        {
            if (GUILayout.Button(addTexture))
            {
                ChapterElementNewCutsceneInputPopup window =
                    (ChapterElementNewCutsceneInputPopup)
                        ScriptableObject.CreateInstance(typeof (ChapterElementNewCutsceneInputPopup));
                window.Init(this, "Cutscene", EditorWindowType.Cutscenes);
            }
        }
        GUILayout.EndHorizontal();
        // Item sublist cutscene
        if (openedWindow == EditorWindowType.Cutscenes)
        {
            GUI.skin = leftSubMenuSkin;
            for (int i = 0;
                i < Controller.getInstance().getCharapterList().getSelectedChapterData().getCutscenes().Count;
                i++)
            {
                if (i == GameRources.GetInstance().selectedCutsceneIndex)
                {
                    GUI.skin = leftSubMenuConcreteItemSkin;
                }

                if (
                    GUILayout.Button(
                        Controller.getInstance().getCharapterList().getSelectedChapterData().getCutscenes()[i].getId()))
                {
                    cutscenesWindow.ShowItemWindowView(i);
                }

                if (i == GameRources.GetInstance().selectedCutsceneIndex)
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Rename"))
                    {
                        Debug.Log("Rename");
                    }
                    if (GUILayout.Button(deleteImg))
                    {
                        Controller.getInstance()
                            .getCharapterList()
                            .getSelectedChapterDataControl()
                            .getCutscenesList()
                            .deleteElement(
                                Controller.getInstance()
                                    .getCharapterList()
                                    .getSelectedChapterDataControl()
                                    .getCutscenesList()
                                    .getCutscenes()[i], false);
                        scenesWindow.ShowBaseWindowView();
                    }
                    GUILayout.EndHorizontal();
                    GUI.skin = leftSubMenuSkin;
                }
            }
            GUI.skin = defaultGUISkin;
        }



        // Button event book
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(leftMenuContentBook))
        {
            OnWindowTypeChanged(EditorWindowType.Books);
            booksWindow.ShowBaseWindowView();
        }
        //Add button book
        if (openedWindow == EditorWindowType.Books)
        {
            if (GUILayout.Button(addTexture))
            {
                ChapterElementNameInputPopup window =
                    (ChapterElementNameInputPopup)
                        ScriptableObject.CreateInstance(typeof (ChapterElementNameInputPopup));
                window.Init(this, "Book", EditorWindowType.Books);
            }
        }
        GUILayout.EndHorizontal();
        // Item sublist book
        if (openedWindow == EditorWindowType.Books)
        {
            GUI.skin = leftSubMenuSkin;
            for (int i = 0;
                i < Controller.getInstance().getCharapterList().getSelectedChapterData().getBooks().Count;
                i++)
            {
                if (i == GameRources.GetInstance().selectedBookIndex)
                {
                    GUI.skin = leftSubMenuConcreteItemSkin;
                }

                if (
                    GUILayout.Button(
                        Controller.getInstance().getCharapterList().getSelectedChapterData().getBooks()[i].getId()))
                {
                    booksWindow.ShowItemWindowView(i);
                }

                if (i == GameRources.GetInstance().selectedBookIndex)
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Rename"))
                    {
                        Debug.Log("Rename");
                    }
                    if (GUILayout.Button(duplicateImg))
                    {
                        Controller.getInstance()
                            .getCharapterList()
                            .getSelectedChapterDataControl()
                            .getBooksList()
                            .duplicateElement(
                                Controller.getInstance()
                                    .getCharapterList()
                                    .getSelectedChapterDataControl()
                                    .getBooksList()
                                    .getBooks()[i]);

                    }
                    if (GUILayout.Button(deleteImg))
                    {
                        Controller.getInstance()
                            .getCharapterList()
                            .getSelectedChapterDataControl()
                            .getBooksList()
                            .deleteElement(
                                Controller.getInstance()
                                    .getCharapterList()
                                    .getSelectedChapterDataControl()
                                    .getBooksList()
                                    .getBooks()[i], false);
                        scenesWindow.ShowBaseWindowView();
                    }
                    GUILayout.EndHorizontal();
                    GUI.skin = leftSubMenuSkin;
                }
            }
            GUI.skin = defaultGUISkin;
        }



        // Button event item
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(leftMenuContentItem))
        {
            OnWindowTypeChanged(EditorWindowType.Items);
            itemsWindow.ShowBaseWindowView();
        }
        //Add button item
        if (openedWindow == EditorWindowType.Items)
        {
            if (GUILayout.Button(addTexture))
            {
                ChapterElementNameInputPopup window =
                    (ChapterElementNameInputPopup)
                        ScriptableObject.CreateInstance(typeof (ChapterElementNameInputPopup));
                window.Init(this, "Item", EditorWindowType.Items);
            }
        }
        GUILayout.EndHorizontal();
        // Item sublist item
        if (openedWindow == EditorWindowType.Items)
        {
            GUI.skin = leftSubMenuSkin;
            for (int i = 0;
                i < Controller.getInstance().getCharapterList().getSelectedChapterData().getItems().Count;
                i++)
            {
                if (i == GameRources.GetInstance().selectedItemIndex)
                {
                    GUI.skin = leftSubMenuConcreteItemSkin;
                }

                if (
                    GUILayout.Button(
                        Controller.getInstance().getCharapterList().getSelectedChapterData().getItems()[i].getId()))
                {
                    itemsWindow.ShowItemWindowView(i);
                }

                if (i == GameRources.GetInstance().selectedItemIndex)
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Rename"))
                    {
                        Debug.Log("Rename");
                    }
                    if (GUILayout.Button(duplicateImg))
                    {
                        Controller.getInstance()
                            .getCharapterList()
                            .getSelectedChapterDataControl()
                            .getItemsList()
                            .duplicateElement(
                                Controller.getInstance()
                                    .getCharapterList()
                                    .getSelectedChapterDataControl()
                                    .getItemsList()
                                    .getItems()[i]);

                    }
                    if (GUILayout.Button(deleteImg))
                    {
                        Controller.getInstance()
                            .getCharapterList()
                            .getSelectedChapterDataControl()
                            .getItemsList()
                            .deleteElement(
                                Controller.getInstance()
                                    .getCharapterList()
                                    .getSelectedChapterDataControl()
                                    .getItemsList()
                                    .getItems()[i], false);
                        scenesWindow.ShowBaseWindowView();
                    }
                    GUILayout.EndHorizontal();
                    GUI.skin = leftSubMenuSkin;
                }
            }
            GUI.skin = defaultGUISkin;
        }



        // Button event item
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(leftMenuContentSetItem))
        {
            OnWindowTypeChanged(EditorWindowType.SetItems);
            setItemsWindow.ShowBaseWindowView();
        }
        //Add button item
        if (openedWindow == EditorWindowType.SetItems)
        {
            if (GUILayout.Button(addTexture))
            {
                ChapterElementNameInputPopup window =
                    (ChapterElementNameInputPopup)
                        ScriptableObject.CreateInstance(typeof (ChapterElementNameInputPopup));
                window.Init(this, "Atrezzo", EditorWindowType.SetItems);
            }
        }
        GUILayout.EndHorizontal();
        // Item sublist item
        if (openedWindow == EditorWindowType.SetItems)
        {
            GUI.skin = leftSubMenuSkin;
            for (int i = 0;
                i < Controller.getInstance().getCharapterList().getSelectedChapterData().getAtrezzo().Count;
                i++)
            {
                if (i == GameRources.GetInstance().selectedSetItemIndex)
                {
                    GUI.skin = leftSubMenuConcreteItemSkin;
                }

                if (
                    GUILayout.Button(
                        Controller.getInstance().getCharapterList().getSelectedChapterData().getAtrezzo()[i].getId()))
                {
                    setItemsWindow.ShowItemWindowView(i);
                }

                if (i == GameRources.GetInstance().selectedSetItemIndex)
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Rename"))
                    {
                        Debug.Log("Rename");
                    }
                    if (GUILayout.Button(duplicateImg))
                    {
                        Controller.getInstance()
                            .getCharapterList()
                            .getSelectedChapterDataControl()
                            .getAtrezzoList()
                            .duplicateElement(
                                Controller.getInstance()
                                    .getCharapterList()
                                    .getSelectedChapterDataControl()
                                    .getAtrezzoList()
                                    .getAtrezzoList()[i]);

                    }
                    if (GUILayout.Button(deleteImg))
                    {
                        Controller.getInstance()
                            .getCharapterList()
                            .getSelectedChapterDataControl()
                            .getAtrezzoList()
                            .deleteElement(
                                Controller.getInstance()
                                    .getCharapterList()
                                    .getSelectedChapterDataControl()
                                    .getAtrezzoList()
                                    .getAtrezzoList()[i], false);
                        scenesWindow.ShowBaseWindowView();
                    }
                    GUILayout.EndHorizontal();
                    GUI.skin = leftSubMenuSkin;
                }
            }
            GUI.skin = defaultGUISkin;
        }



        // Button event player
        if (GUILayout.Button(leftMenuContentPlayer))
        {
            OnWindowTypeChanged(EditorWindowType.Player);
        }

        // Button event NPC
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(leftMenuContentCharacter))
        {
            OnWindowTypeChanged(EditorWindowType.Characters);
            characterWindow.ShowBaseWindowView();
        }
        //Add button NPC
        if (openedWindow == EditorWindowType.Characters)
        {
            if (GUILayout.Button(addTexture))
            {
                ChapterElementNameInputPopup window =
                    (ChapterElementNameInputPopup)
                        ScriptableObject.CreateInstance(typeof (ChapterElementNameInputPopup));
                window.Init(this, "Character", EditorWindowType.Characters);
            }
        }
        GUILayout.EndHorizontal();
        // Item sublist NPC
        if (openedWindow == EditorWindowType.Characters)
        {
            GUI.skin = leftSubMenuSkin;
            for (int i = 0;
                i < Controller.getInstance().getCharapterList().getSelectedChapterData().getCharacters().Count;
                i++)
            {
                if (i == GameRources.GetInstance().selectedCharacterIndex)
                {
                    GUI.skin = leftSubMenuConcreteItemSkin;
                }

                if (
                    GUILayout.Button(
                        Controller.getInstance().getCharapterList().getSelectedChapterData().getCharacters()[i].getId()))
                {
                    characterWindow.ShowItemWindowView(i);
                }

                if (i == GameRources.GetInstance().selectedCharacterIndex)
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Rename"))
                    {
                        Debug.Log("Rename");
                    }
                    if (GUILayout.Button(duplicateImg))
                    {
                        Controller.getInstance()
                            .getCharapterList()
                            .getSelectedChapterDataControl()
                            .getNPCsList()
                            .duplicateElement(
                                Controller.getInstance()
                                    .getCharapterList()
                                    .getSelectedChapterDataControl()
                                    .getNPCsList()
                                    .getNPCs()[i]);

                    }
                    if (GUILayout.Button(deleteImg))
                    {
                        Controller.getInstance()
                            .getCharapterList()
                            .getSelectedChapterDataControl()
                            .getNPCsList()
                            .deleteElement(
                                Controller.getInstance()
                                    .getCharapterList()
                                    .getSelectedChapterDataControl()
                                    .getNPCsList()
                                    .getNPCs()[i], false);
                        scenesWindow.ShowBaseWindowView();
                    }
                    GUILayout.EndHorizontal();
                    GUI.skin = leftSubMenuSkin;
                }
            }
            GUI.skin = defaultGUISkin;
        }

        // Button event Conversation
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(leftMenuContentConversation))
        {
            OnWindowTypeChanged(EditorWindowType.Conversations);
        }
        //Add button Conversation
        if (openedWindow == EditorWindowType.Conversations)
        {
            if (GUILayout.Button(addTexture))
            {
                ChapterElementNameInputPopup window =
                    (ChapterElementNameInputPopup)
                        ScriptableObject.CreateInstance(typeof (ChapterElementNameInputPopup));
                window.Init(this, "Conversation", EditorWindowType.Conversations);
            }
        }
        GUILayout.EndHorizontal();
        // Item sublist book
        if (openedWindow == EditorWindowType.Conversations)
        {
            GUI.skin = leftSubMenuSkin;
            for (int i = 0;
                i < Controller.getInstance().getCharapterList().getSelectedChapterData().getConversations().Count;
                i++)
            {
                if (i == GameRources.GetInstance().selectedConversationIndex)
                {
                    GUI.skin = leftSubMenuConcreteItemSkin;

                    if (GUILayout.Button(
                            Controller.getInstance().getCharapterList().getSelectedChapterData().getConversations()[i]
                                .getId()))
                    {
                        ConversationEditorWindow window =
                            (ConversationEditorWindow)
                                ScriptableObject.CreateInstance(typeof (ConversationEditorWindow));
                        window.Init(
                            (GraphConversation)
                                Controller.getInstance().getCharapterList().getSelectedChapterData().getConversations()[
                                    i]);
                    }
                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button(duplicateImg))
                    {
                        Controller.getInstance()
                            .getCharapterList()
                            .getSelectedChapterDataControl()
                            .getConversationsList()
                            .duplicateElement(
                                Controller.getInstance()
                                    .getCharapterList()
                                    .getSelectedChapterDataControl()
                                    .getConversationsList()
                                    .getConversations()[i]);

                    }
                    if (GUILayout.Button(deleteImg))
                    {
                        Controller.getInstance()
                            .getCharapterList()
                            .getSelectedChapterDataControl()
                            .getConversationsList()
                            .deleteElement(
                                Controller.getInstance()
                                    .getCharapterList()
                                    .getSelectedChapterDataControl()
                                    .getConversationsList()
                                    .getConversations()[i], false);
                    }
                    GUILayout.EndHorizontal();
                    GUI.skin = leftSubMenuSkin;
                }
                else
                {
                    if (GUILayout.Button(
                           Controller.getInstance().getCharapterList().getSelectedChapterData().getConversations()[i]
                               .getId()))
                    {
                        GameRources.GetInstance().selectedConversationIndex = i;
                    }
                }
            }
            GUI.skin = defaultGUISkin;
        }


        // Button event player
        if (GUILayout.Button(leftMenuContentAdvanced))
        {
            OnWindowTypeChanged(EditorWindowType.AdvancedFeatures);
        }

        // Button event player
        if (GUILayout.Button(leftMenuContentAssessment))
        {
            OnWindowTypeChanged(EditorWindowType.AssesmentProfiles);
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        GUILayout.EndArea();

        /**
        WINDOWS
        */
        BeginWindows();

        switch (openedWindow)
        {
            case EditorWindowType.Chapter:
                m_Window1 = chapterWindow;
                break;
            case EditorWindowType.Scenes:
                m_Window1 = scenesWindow;
                break;
            case EditorWindowType.Cutscenes:
                m_Window1 = cutscenesWindow;
                break;
            case EditorWindowType.Books:
                m_Window1 = booksWindow;
                break;
            case EditorWindowType.Items:
                m_Window1 = itemsWindow;
                break;
            case EditorWindowType.SetItems:
                m_Window1 = setItemsWindow;
                break;
            case EditorWindowType.Player:
                m_Window1 = playerWindow;
                break;
            case EditorWindowType.Characters:
                m_Window1 = characterWindow;
                break;
            case EditorWindowType.Conversations:
                m_Window1 = conversationWindow;
                break;
            case EditorWindowType.AdvancedFeatures:
                m_Window1 = advencedFeaturesWindow;
                break;
            case EditorWindowType.AssesmentProfiles:
                m_Window1 = assesmentProfileWindow;
                break;
        }

        if (m_Window1 != null)
            m_Window1.OnGUI();
        EndWindows();
    }

    void OnWindowTypeChanged(EditorWindowType type_)
    {
        openedWindow = type_;
    }

    void RedoAction()
    {
        Debug.Log("redo clicked");
        Controller.getInstance().redoTool();
    }

    void UndoAction()
    {
        Debug.Log("undo clicked");
        Controller.getInstance().undoTool();
    }

    void AddScene(string name)
    {
        Controller.getInstance().getSelectedChapterDataControl().getScenesList().addElement(Controller.SCENE, name);
        scenesWindow.ShowBaseWindowView();
    }

    void AddCutsceneSlide(string name)
    {
        Debug.Log("Slide");
        Controller.getInstance()
            .getSelectedChapterDataControl()
            .getCutscenesList()
            .addElement(Controller.CUTSCENE_SLIDES, name);
        cutscenesWindow.ShowBaseWindowView();
    }

    void AddCutsceneVideo(string name)
    {
        Debug.Log("Video");
        Controller.getInstance()
            .getSelectedChapterDataControl()
            .getCutscenesList()
            .addElement(Controller.CUTSCENE_VIDEO, name);
        cutscenesWindow.ShowBaseWindowView();
    }

    void AddBook(string name)
    {
        Controller.getInstance().getSelectedChapterDataControl().getBooksList().addElement(Controller.BOOK, name);
        booksWindow.ShowBaseWindowView();
    }

    void AddItem(string name)
    {
        Controller.getInstance().getSelectedChapterDataControl().getItemsList().addElement(Controller.ITEM, name);
        itemsWindow.ShowBaseWindowView();
    }

    void AddSetItem(string name)
    {
        Controller.getInstance().getSelectedChapterDataControl().getAtrezzoList().addElement(Controller.ATREZZO, name);
        setItemsWindow.ShowBaseWindowView();
    }

    void AddNPC(string name)
    {
        Controller.getInstance().getSelectedChapterDataControl().getNPCsList().addElement(Controller.NPC, name);
        characterWindow.ShowBaseWindowView();
    }

    void AddConversation(string name)
    {
        Controller.getInstance()
            .getSelectedChapterDataControl()
            .getConversationsList()
            .addElement(Controller.CONVERSATION_GRAPH, name);
        Debug.Log("ADD conversation");
    }

    public void OnDialogOk(string message, System.Object workingObject = null, object workingObjectSecond = null)
    {
        if (((ChapterElementNameInputPopup) workingObject) != null)
        {
            EditorWindowType callbackType = ((ChapterElementNameInputPopup) workingObject).connectedAsssetType;
            switch (callbackType)
            {
                case EditorWindowType.Scenes:
                    AddScene(message);
                    break;
                case EditorWindowType.Cutscenes:
                    int cutsceneType = ((ChapterElementNewCutsceneInputPopup) workingObject).cutsceneType;

                    if (cutsceneType == Controller.CUTSCENE_SLIDES)
                        AddCutsceneSlide(message);
                    else if (cutsceneType == Controller.CUTSCENE_VIDEO)
                        AddCutsceneVideo(message);

                    break;
                case EditorWindowType.Books:
                    AddBook(message);
                    break;
                case EditorWindowType.Items:
                    AddItem(message);
                    break;
                case EditorWindowType.SetItems:
                    AddSetItem(message);
                    break;
                case EditorWindowType.Characters:
                    AddNPC(message);
                    break;
                case EditorWindowType.Conversations:
                    AddConversation(message);
                    break;
            }
        }

    }

    public void OnDialogCanceled(System.Object workingObject = null)
    {
        Debug.Log("Canceled clicked");
    }
}