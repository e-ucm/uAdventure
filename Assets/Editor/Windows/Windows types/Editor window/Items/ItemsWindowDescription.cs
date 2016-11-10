using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEditor;

public class ItemsWindowDescription : LayoutWindow, DialogReceiverInterface
{
    private enum AssetType
    {
        NAME_SOUND,
        BRIEF_DESCRIPTION_SOUND,
        DETAILED_DESCRIPTION_SOUND
    };

    private Texture2D addTex = null;
    private Texture2D duplicateTex = null;
    private Texture2D clearTex = null;

    private Texture2D conditionsTex = null;
    private Texture2D noConditionsTex = null;
    private Texture2D tmpTex = null;

    private static float windowWidth, windowHeight;
    private static Rect descriptionRect, descriptionTableRect, settingsTable, rightPanelRect;

    private static GUISkin defaultSkin;
    private static GUISkin noBackgroundSkin;
    private static GUISkin selectedAreaSkin;

    private Vector2 scrollPosition;

    private int selectedDescription;
    /*
 * DESCRIPTORS
 */
    private string fullItemDescription = "", fullItemDescriptionLast = "";

    private string descriptionName = "", briefDescription = "", detailedDescription = "";
    private string descriptionNameLast = "", briefDescriptionLast = "", detailedDescriptionLast = "";

    private string descriptionSound = "", briefDescriptionSound = "", detailedDescriptionSound = "";

    private Texture2D noAudioTexture;
    private Texture2D audioTexture;
    private Texture2D audioTextureTmp;
    /*
    * SETTINGS fields
    */
    private bool dragdropToogle, dragdropToogleLast;

    private string[] behaviourTypes = {TC.get("Behaviour.Normal"), TC.get("Behaviour.FirstAction") };

    private string[] behaviourTypesDescription =
    {
       TC.get("Behaviour.Selection.Normal"), TC.get("Behaviour.Selection.FirstAction")
    };

    private int selectedBehaviourType, selectedBehaviourTypeLast;

    private string transitionTime, transitionTimeLast;

    public ItemsWindowDescription(Rect aStartPos, GUIContent aContent, GUIStyle aStyle,
        params GUILayoutOption[] aOptions)
        : base(aStartPos, aContent, aStyle, aOptions)
    {
        clearTex = (Texture2D) Resources.Load("EAdventureData/img/icons/deleteContent", typeof (Texture2D));
        addTex = (Texture2D) Resources.Load("EAdventureData/img/icons/addNode", typeof (Texture2D));
        duplicateTex = (Texture2D) Resources.Load("EAdventureData/img/icons/duplicateNode", typeof (Texture2D));

        conditionsTex = (Texture2D) Resources.Load("EAdventureData/img/icons/conditions-24x24", typeof (Texture2D));
        noConditionsTex = (Texture2D) Resources.Load("EAdventureData/img/icons/no-conditions-24x24", typeof (Texture2D));

        windowWidth = aStartPos.width;
        windowHeight = aStartPos.height;

        noBackgroundSkin = (GUISkin) Resources.Load("Editor/EditorNoBackgroundSkin", typeof (GUISkin));
        selectedAreaSkin = (GUISkin) Resources.Load("Editor/EditorLeftMenuItemSkinConcreteOptions", typeof (GUISkin));

        noAudioTexture = (Texture2D)Resources.Load("EAdventureData/img/icons/noAudio", typeof(Texture2D));
        audioTexture = (Texture2D)Resources.Load("EAdventureData/img/icons/audio", typeof(Texture2D));

        descriptionRect = new Rect(0f, 0.1f*windowHeight, windowWidth, 0.1f*windowHeight);
        rightPanelRect = new Rect(0.9f*windowWidth, 0.2f*windowHeight, 0.08f*windowWidth, 0.15f*windowHeight);
        descriptionTableRect = new Rect(0f, 0.2f*windowHeight, 0.9f*windowWidth, 0.15f*windowHeight);
        settingsTable = new Rect(0f, 0.35f*windowHeight, windowWidth, windowHeight*0.65f);

        if (GameRources.GetInstance().selectedItemIndex >= 0)
        {
            fullItemDescription =
                fullItemDescriptionLast =
                    Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                        GameRources.GetInstance().selectedItemIndex].getDocumentation();
            if (fullItemDescription == null)
                fullItemDescription =
                    fullItemDescriptionLast = "";
            dragdropToogle = dragdropToogleLast =
                Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                    GameRources.GetInstance().selectedItemIndex].isReturnsWhenDragged();
            selectedBehaviourType = selectedBehaviourTypeLast =
                (int) Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                    GameRources.GetInstance().selectedItemIndex].getBehaviour();
            transitionTime = transitionTimeLast =
                Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                    GameRources.GetInstance().selectedItemIndex].getResourcesTransitionTime().ToString();
        }
        selectedDescription = -1;
    }


    public override void Draw(int aID)
    {
        GUILayout.BeginArea(descriptionRect);
        GUILayout.Label(TC.get("Item.Documentation"));
        fullItemDescription = GUILayout.TextField(fullItemDescription);
        if (!fullItemDescription.Equals(fullItemDescriptionLast))
            OnItemDescriptionChanged(fullItemDescription);
        GUILayout.EndArea();



        /*
        * Desciptor table
        */
        GUILayout.BeginArea(descriptionTableRect);
        GUILayout.BeginHorizontal();
        GUILayout.Box(TC.get("DescriptionList.Descriptions"), GUILayout.Width(windowWidth*0.44f));
        GUILayout.Box(TC.get("Conditions.Title"), GUILayout.Width(windowWidth*0.44f));
        GUILayout.EndHorizontal();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        for (int i = 0;
            i <
            Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                GameRources.GetInstance().selectedItemIndex].getDescriptionController().getDescriptionCount();
            i++)
        {
            if (i == selectedDescription)
                GUI.skin = selectedAreaSkin;
            else
                GUI.skin = noBackgroundSkin;

            tmpTex = (Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                GameRources.GetInstance().selectedItemIndex].getDescriptionController()
                .getDescriptionController(i)
                .getConditionsController()
                .getBlocksCount() > 0
                ? conditionsTex
                : noConditionsTex);

            GUILayout.BeginHorizontal();

            if (i == selectedDescription)
            {
                if (GUILayout.Button(Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                    GameRources.GetInstance().selectedItemIndex].getDescriptionController()
                    .getDescriptionController(i)
                    .getName(), GUILayout.Width(windowWidth*0.44f)))
                {
                    OnDescriptionSelectionChange(i);
                }
                if (GUILayout.Button(tmpTex, GUILayout.Width(windowWidth*0.44f)))
                {
                    ConditionEditorWindow window =
                        (ConditionEditorWindow) ScriptableObject.CreateInstance(typeof (ConditionEditorWindow));
                    window.Init(
                        Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                            GameRources.GetInstance().selectedItemIndex].getDescriptionController()
                            .getDescriptionController(i)
                            .getConditionsController());
                }
            }
            else
            {
                if (GUILayout.Button(Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                    GameRources.GetInstance().selectedItemIndex].getDescriptionController()
                    .getDescriptionController(i)
                    .getName(), GUILayout.Width(windowWidth*0.44f)))
                {
                    OnDescriptionSelectionChange(i);
                }
                if (GUILayout.Button(tmpTex, GUILayout.Width(windowWidth*0.44f)))
                {
                    OnDescriptionSelectionChange(i);
                }
            }
            GUILayout.EndHorizontal();
            GUI.skin = defaultSkin;
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();



        /*
        * Right panel
        */
        GUILayout.BeginArea(rightPanelRect);
        GUI.skin = noBackgroundSkin;
        if (GUILayout.Button(addTex, GUILayout.MaxWidth(0.08f*windowWidth)))
        {
            Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                GameRources.GetInstance().selectedItemIndex].getDescriptionController().addElement();
        }
        if (GUILayout.Button(duplicateTex, GUILayout.MaxWidth(0.08f*windowWidth)))
        {
            Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                GameRources.GetInstance().selectedItemIndex].getDescriptionController().duplicateElement();
        }
        if (GUILayout.Button(clearTex, GUILayout.MaxWidth(0.08f*windowWidth)))
        {
            Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                GameRources.GetInstance().selectedItemIndex].getDescriptionController().deleteElement();
        }
        GUI.skin = defaultSkin;
        GUILayout.EndArea();



        /*
        * Properties panel
        */
        GUILayout.BeginArea(settingsTable);


        GUILayout.Label(TC.get("Item.Name"));
        GUILayout.BeginHorizontal();
        descriptionName = GUILayout.TextField(descriptionName, GUILayout.MaxWidth(0.6f*windowWidth));
        if (!descriptionName.Equals(descriptionNameLast))
            OnDescriptionNameChanged(descriptionName);
        if (!string.IsNullOrEmpty(descriptionSound))
            audioTextureTmp = audioTexture;
        else
            audioTextureTmp = noAudioTexture;
        GUILayout.Label(audioTextureTmp);
        GUILayout.Label(descriptionSound);
        if (GUILayout.Button(TC.get("Buttons.Select")))
        {
            ShowAssetChooser(AssetType.NAME_SOUND);
        }
        if (GUILayout.Button(clearTex))
        {
            OnDescriptorNameSoundChange("");
        }
        GUILayout.EndHorizontal();

        GUILayout.Label(TC.get("Item.Description"));
        GUILayout.BeginHorizontal();
        briefDescription = GUILayout.TextField(briefDescription, GUILayout.MaxWidth(0.6f * windowWidth));
        if (!briefDescription.Equals(briefDescriptionLast))
            OnBriefDescriptionChanged(briefDescription);
        if (!string.IsNullOrEmpty(briefDescriptionSound))
            audioTextureTmp = audioTexture;
        else
            audioTextureTmp = noAudioTexture;
        GUILayout.Label(audioTextureTmp);
        GUILayout.Label(briefDescriptionSound);
        if (GUILayout.Button(TC.get("Buttons.Select")))
        {
            ShowAssetChooser(AssetType.BRIEF_DESCRIPTION_SOUND);
        }
        if (GUILayout.Button(clearTex))
        {
            OnDescriptorBriefSoundChange("");
        }
        GUILayout.EndHorizontal();

        GUILayout.Label(TC.get("Item.DetailedDescription"));
        GUILayout.BeginHorizontal();
        detailedDescription = GUILayout.TextField(detailedDescription, GUILayout.MaxWidth(0.6f * windowWidth));
        if (!detailedDescription.Equals(detailedDescriptionLast))
            OnDetailedDescriptionChanged(detailedDescription);
        if (!string.IsNullOrEmpty(detailedDescriptionSound))
            audioTextureTmp = audioTexture;
        else
            audioTextureTmp = noAudioTexture;
        GUILayout.Label(audioTextureTmp);
        GUILayout.Label(detailedDescriptionSound);
        if (GUILayout.Button(TC.get("Buttons.Select")))
        {
            ShowAssetChooser(AssetType.DETAILED_DESCRIPTION_SOUND);
        }
        if (GUILayout.Button(clearTex))
        {
            OnDescriptorDetailedSoundChange("");
        }
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        GUILayout.Label(TC.get("Item.ReturnsWhenDragged.Title"));
        GUILayout.Box(TC.get("Item.ReturnsWhenDragged.Description"));
        dragdropToogle = GUILayout.Toggle(dragdropToogle, TC.get("Item.ReturnsWhenDragged"));
        if (dragdropToogle != dragdropToogleLast)
            OnDragAndDropToogleValueChange(dragdropToogle);

        GUILayout.FlexibleSpace();

        GUILayout.Label(TC.get("Behaviour"));
        GUILayout.BeginHorizontal();
        selectedBehaviourType = EditorGUILayout.Popup(selectedBehaviourType, behaviourTypes,
            GUILayout.MaxWidth(0.2f*windowWidth));
        if (selectedBehaviourType != selectedBehaviourTypeLast)
            OnBehaviourChange(selectedBehaviourType);
        GUILayout.Box(behaviourTypesDescription[selectedBehaviourType]);
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        GUILayout.Label(TC.get("Resources.TransitionTime"));
        GUILayout.Box(TC.get("Resources.TransitionTime.Description"));
        transitionTime = GUILayout.TextField(transitionTime);
        transitionTime = (Regex.Match(transitionTime, "^[0-9]{1,3}$").Success ? transitionTime : transitionTimeLast);
        if (!transitionTime.Equals(transitionTimeLast))
            OnTransitionTimeChange(transitionTime);


        GUILayout.EndArea();

    }

    private void OnItemDescriptionChanged(string val)
    {
        fullItemDescriptionLast = val;
        Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
            GameRources.GetInstance().selectedItemIndex].setDocumentation(fullItemDescription);
    }


    private void OnDescriptionNameChanged(string val)
    {
        descriptionNameLast = val;
        Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
            GameRources.GetInstance().selectedItemIndex].getDescriptionController()
            .getSelectedDescriptionController()
            .getDescriptionData()
            .setName(val);
    }

    private void OnBriefDescriptionChanged(string val)
    {
        briefDescriptionLast = val;
        Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
            GameRources.GetInstance().selectedItemIndex].getDescriptionController()
            .getSelectedDescriptionController()
            .getDescriptionData()
            .setDescription(val);
    }

    private void OnDetailedDescriptionChanged(string val)
    {
        detailedDescriptionLast = val;
        Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
            GameRources.GetInstance().selectedItemIndex].getDescriptionController()
            .getSelectedDescriptionController()
            .getDescriptionData()
            .setDetailedDescription(val);
    }

    private void OnDescriptionSelectionChange(int i)
    {
        selectedDescription = i;
        RefreshDescriptionInformation();
    }

    private void OnDragAndDropToogleValueChange(bool val)
    {
        dragdropToogleLast = val;
        Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
            GameRources.GetInstance().selectedItemIndex].setReturnsWhenDragged(val);
    }

    private void OnBehaviourChange(int val)
    {
        selectedBehaviourTypeLast = val;

        Item.BehaviourType type = (val == 0 ? Item.BehaviourType.NORMAL : Item.BehaviourType.FIRST_ACTION);
        Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
            GameRources.GetInstance().selectedItemIndex].setBehaviour(type);
    }

    private void OnDescriptorNameSoundChange(string val)
    {
        descriptionSound = val;
        Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
            GameRources.GetInstance().selectedItemIndex].getDescriptionController()
            .getSelectedDescriptionController().getDescriptionData().setNameSoundPath(val);
    }

    private void OnDescriptorBriefSoundChange(string val)
    {
        briefDescriptionSound = val;
        Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
            GameRources.GetInstance().selectedItemIndex].getDescriptionController()
            .getSelectedDescriptionController()
            .getDescriptionData()
            .setDescriptionSoundPath(briefDescriptionSound);
    }

    private void OnDescriptorDetailedSoundChange(string val)
    {
        detailedDescriptionSound = val;
        Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
            GameRources.GetInstance().selectedItemIndex].getDescriptionController()
            .getSelectedDescriptionController()
            .getDescriptionData()
            .setDetailedDescriptionSoundPath(detailedDescriptionSound);
    }

    private void OnTransitionTimeChange(string val)
    {
        transitionTimeLast = val;
        Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
            GameRources.GetInstance().selectedItemIndex].setResourcesTransitionTime(int.Parse(val));
    }

    private void RefreshDescriptionInformation()
    {
        Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
            GameRources.GetInstance().selectedItemIndex].getDescriptionController()
            .setSelectedDescription(selectedDescription);

        descriptionName =
            descriptionNameLast = Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                GameRources.GetInstance().selectedItemIndex].getDescriptionController()
                .getSelectedDescriptionController()
                .getDescriptionData()
                .getName();

        descriptionSound = Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
            GameRources.GetInstance().selectedItemIndex].getDescriptionController()
            .getSelectedDescriptionController()
            .getDescriptionData().getNameSoundPath();

        briefDescription =
            briefDescriptionLast = Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                GameRources.GetInstance().selectedItemIndex].getDescriptionController()
                .getSelectedDescriptionController()
                .getDescriptionData()
                .getDescription();

        briefDescriptionSound = Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
            GameRources.GetInstance().selectedItemIndex].getDescriptionController()
            .getSelectedDescriptionController()
            .getDescriptionData().getDescriptionSoundPath();

        detailedDescription =
            detailedDescriptionLast =
                Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
                    GameRources.GetInstance().selectedItemIndex].getDescriptionController()
                    .getSelectedDescriptionController()
                    .getDescriptionData()
                    .getDetailedDescription();

        detailedDescriptionSound = Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
            GameRources.GetInstance().selectedItemIndex].getDescriptionController()
            .getSelectedDescriptionController()
            .getDescriptionData().getDetailedDescriptionSoundPath();

    }



    void ShowAssetChooser(AssetType type)
    {
        switch (type)
        {
            case AssetType.NAME_SOUND:
                MusicFileOpenDialog nameDialog =
                (MusicFileOpenDialog)ScriptableObject.CreateInstance(typeof(MusicFileOpenDialog));
                nameDialog.Init(this, BaseFileOpenDialog.FileType.ITEM_DESCRIPTION_NAME_SOUND);
                break;
            case AssetType.BRIEF_DESCRIPTION_SOUND:
                MusicFileOpenDialog briefDialog =
                (MusicFileOpenDialog)ScriptableObject.CreateInstance(typeof(MusicFileOpenDialog));
                briefDialog.Init(this, BaseFileOpenDialog.FileType.ITEM_DESCRIPTION_BRIEF_SOUND);
                break;
            case AssetType.DETAILED_DESCRIPTION_SOUND:
                MusicFileOpenDialog detailedOverDialog =
                (MusicFileOpenDialog)ScriptableObject.CreateInstance(typeof(MusicFileOpenDialog));
                detailedOverDialog.Init(this, BaseFileOpenDialog.FileType.ITEM_DESCRIPTION_DETAILED_SOUND);
                break;
        }

    }

    public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
    {
        if (workingObject is BaseFileOpenDialog.FileType)
        {
            switch ((BaseFileOpenDialog.FileType) workingObject)
            {
                case BaseFileOpenDialog.FileType.ITEM_DESCRIPTION_NAME_SOUND:
                    OnDescriptorNameSoundChange(message);
                    break;
                case BaseFileOpenDialog.FileType.ITEM_DESCRIPTION_BRIEF_SOUND:
                    OnDescriptorBriefSoundChange(message);
                    break;
                case BaseFileOpenDialog.FileType.ITEM_DESCRIPTION_DETAILED_SOUND:
                    OnDescriptorDetailedSoundChange(message);
                    break;
                default:
                    break;
            }
        }
    }

    public void OnDialogCanceled(object workingObject = null)
    {
        Debug.Log("Wiadomość nie OK");
    }
}