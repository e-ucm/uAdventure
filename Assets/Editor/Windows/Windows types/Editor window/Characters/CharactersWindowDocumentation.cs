using UnityEngine;
using System.Collections;

public class CharactersWindowDocumentation : LayoutWindow, DialogReceiverInterface
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
    private static Rect descriptionRect, descriptionTableRect, rightPanelRect, settingsTable;

    private static GUISkin defaultSkin;
    private static GUISkin noBackgroundSkin;
    private static GUISkin selectedAreaSkin;

    private Vector2 scrollPosition;

    private int selectedDescription;
    /*
* DESCRIPTORS
*/
    private string fullCharacterDescription = "", fullCharacterDescriptionLast = "";

    private string descriptionName = "", briefDescription = "", detailedDescription = "";
    private string descriptionNameLast = "", briefDescriptionLast = "", detailedDescriptionLast = "";

    private string descriptionSound = "", briefDescriptionSound = "", detailedDescriptionSound = "";

    private Texture2D noAudioTexture;
    private Texture2D audioTexture;
    private Texture2D audioTextureTmp;

    public CharactersWindowDocumentation(Rect aStartPos, GUIContent aContent, GUIStyle aStyle,
        params GUILayoutOption[] aOptions)
        : base(aStartPos, aContent, aStyle, aOptions)
    {
        clearTex = (Texture2D)Resources.Load("EAdventureData/img/icons/deleteContent", typeof(Texture2D));
        addTex = (Texture2D)Resources.Load("EAdventureData/img/icons/addNode", typeof(Texture2D));
        duplicateTex = (Texture2D)Resources.Load("EAdventureData/img/icons/duplicateNode", typeof(Texture2D));

        conditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/conditions-24x24", typeof(Texture2D));
        noConditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/no-conditions-24x24", typeof(Texture2D));

        windowWidth = aStartPos.width;
        windowHeight = aStartPos.height;

        noBackgroundSkin = (GUISkin)Resources.Load("Editor/EditorNoBackgroundSkin", typeof(GUISkin));
        selectedAreaSkin = (GUISkin)Resources.Load("Editor/EditorLeftMenuItemSkinConcreteOptions", typeof(GUISkin));

        noAudioTexture = (Texture2D)Resources.Load("EAdventureData/img/icons/noAudio", typeof(Texture2D));
        audioTexture = (Texture2D)Resources.Load("EAdventureData/img/icons/audio", typeof(Texture2D));

        descriptionRect = new Rect(0, 0.1f * windowHeight, windowWidth, 0.2f * windowHeight);
        rightPanelRect = new Rect(0.9f * windowWidth, 0.3f * windowHeight, 0.08f * windowWidth, 0.15f * windowHeight);
        descriptionTableRect = new Rect(0f, 0.3f * windowHeight, 0.9f * windowWidth, 0.15f * windowHeight);
        settingsTable = new Rect(0f, 0.45f * windowHeight, windowWidth, windowHeight * 0.5f);

        if (GameRources.GetInstance().selectedCharacterIndex >= 0)
        {
            fullCharacterDescription =
                fullCharacterDescriptionLast =
                    Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
                        GameRources.GetInstance().selectedCharacterIndex].getDocumentation();
            if (fullCharacterDescription == null)
                fullCharacterDescription =
                    fullCharacterDescriptionLast = "";
        }
        selectedDescription = -1;
    }

    public override void Draw(int aID)
    {
        GUILayout.BeginArea(descriptionRect);
        GUILayout.Label(TC.get("NPC.Documentation"));
        fullCharacterDescription = GUILayout.TextArea(fullCharacterDescription, GUILayout.MinHeight(0.2f * windowHeight));
        if (!fullCharacterDescription.Equals(fullCharacterDescriptionLast))
            OnCharacterDescriptionChanged(fullCharacterDescription);
        GUILayout.EndArea();



        /*
        * Desciptor table
        */
        GUILayout.BeginArea(descriptionTableRect);
        GUILayout.BeginHorizontal();
        GUILayout.Box(TC.get("DescriptionList.Descriptions"), GUILayout.Width(windowWidth * 0.44f));
        GUILayout.Box(TC.get("Conditions.Title"), GUILayout.Width(windowWidth * 0.44f));
        GUILayout.EndHorizontal();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        for (int i = 0;
            i <
            Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
                GameRources.GetInstance().selectedCharacterIndex].getDescriptionController().getDescriptionCount();
            i++)
        {
            if (i == selectedDescription)
                GUI.skin = selectedAreaSkin;
            else
                GUI.skin = noBackgroundSkin;

            tmpTex = (Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
                GameRources.GetInstance().selectedCharacterIndex].getDescriptionController()
                .getDescriptionController(i)
                .getConditionsController()
                .getBlocksCount() > 0
                ? conditionsTex
                : noConditionsTex);

            GUILayout.BeginHorizontal();

            if (i == selectedDescription)
            {
                if (GUILayout.Button(Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
                    GameRources.GetInstance().selectedCharacterIndex].getDescriptionController()
                    .getDescriptionController(i)
                    .getName(), GUILayout.Width(windowWidth * 0.44f)))
                {
                    OnDescriptionSelectionChange(i);
                }
                if (GUILayout.Button(tmpTex, GUILayout.Width(windowWidth * 0.44f)))
                {
                    //TODO???: condition editor 
                }
            }
            else
            {
                if (GUILayout.Button(Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
                    GameRources.GetInstance().selectedCharacterIndex].getDescriptionController()
                    .getDescriptionController(i)
                    .getName(), GUILayout.Width(windowWidth * 0.44f)))
                {
                    OnDescriptionSelectionChange(i);
                }
                if (GUILayout.Button(tmpTex, GUILayout.Width(windowWidth * 0.44f)))
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
        if (GUILayout.Button(addTex, GUILayout.MaxWidth(0.08f * windowWidth)))
        {
            Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
                GameRources.GetInstance().selectedCharacterIndex].getDescriptionController().addElement();
        }
        if (GUILayout.Button(duplicateTex, GUILayout.MaxWidth(0.08f * windowWidth)))
        {
            Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
                GameRources.GetInstance().selectedCharacterIndex].getDescriptionController().duplicateElement();
        }
        if (GUILayout.Button(clearTex, GUILayout.MaxWidth(0.08f * windowWidth)))
        {
            Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
                GameRources.GetInstance().selectedCharacterIndex].getDescriptionController().deleteElement();
        }
        GUI.skin = defaultSkin;
        GUILayout.EndArea();



        /*
        * Properties panel
        */
        GUILayout.BeginArea(settingsTable);


        GUILayout.Label("Name");
        GUILayout.BeginHorizontal();
        descriptionName = GUILayout.TextField(descriptionName, GUILayout.MaxWidth(0.6f * windowWidth));
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

        GUILayout.Label(TC.get("NPC.Description"));
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

        GUILayout.Label(TC.get("NPC.DetailedDescription"));
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


        GUILayout.EndArea();
    }

    private void OnCharacterDescriptionChanged(string val)
    {
        fullCharacterDescriptionLast = val;
        Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
            GameRources.GetInstance().selectedCharacterIndex].setDocumentation(val);
    }


    private void OnDescriptionNameChanged(string val)
    {
        descriptionNameLast = val;
        Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
            GameRources.GetInstance().selectedCharacterIndex].getDescriptionController()
            .getSelectedDescriptionController()
            .getDescriptionData()
            .setName(val);
    }

    private void OnBriefDescriptionChanged(string val)
    {
        briefDescriptionLast = val;
        Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
            GameRources.GetInstance().selectedCharacterIndex].getDescriptionController()
            .getSelectedDescriptionController()
            .getDescriptionData()
            .setDescription(val);
    }

    private void OnDetailedDescriptionChanged(string val)
    {
        detailedDescriptionLast = val;
        Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
            GameRources.GetInstance().selectedCharacterIndex].getDescriptionController()
            .getSelectedDescriptionController()
            .getDescriptionData()
            .setDetailedDescription(val);
    }

    private void OnDescriptionSelectionChange(int i)
    {
        selectedDescription = i;
        RefreshDescriptionInformation();
    }

    private void OnDescriptorNameSoundChange(string val)
    {
        descriptionSound = val;
        Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
            GameRources.GetInstance().selectedCharacterIndex].getDescriptionController()
            .getSelectedDescriptionController().getDescriptionData().setNameSoundPath(val);
    }

    private void OnDescriptorBriefSoundChange(string val)
    {
        briefDescriptionSound = val;
        Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
            GameRources.GetInstance().selectedCharacterIndex].getDescriptionController()
            .getSelectedDescriptionController()
            .getDescriptionData()
            .setDescriptionSoundPath(briefDescriptionSound);
    }

    private void OnDescriptorDetailedSoundChange(string val)
    {
        detailedDescriptionSound = val;
        Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
            GameRources.GetInstance().selectedCharacterIndex].getDescriptionController()
            .getSelectedDescriptionController()
            .getDescriptionData()
            .setDetailedDescriptionSoundPath(detailedDescriptionSound);
    }

    private void RefreshDescriptionInformation()
    {
        Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
            GameRources.GetInstance().selectedCharacterIndex].getDescriptionController()
            .setSelectedDescription(selectedDescription);

        descriptionName =
            descriptionNameLast = Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
                GameRources.GetInstance().selectedCharacterIndex].getDescriptionController()
                .getSelectedDescriptionController()
                .getDescriptionData()
                .getName();

        descriptionSound = Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
            GameRources.GetInstance().selectedCharacterIndex].getDescriptionController()
            .getSelectedDescriptionController()
            .getDescriptionData().getNameSoundPath();

        briefDescription =
            briefDescriptionLast = Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
                GameRources.GetInstance().selectedCharacterIndex].getDescriptionController()
                .getSelectedDescriptionController()
                .getDescriptionData()
                .getDescription();

        briefDescriptionSound = Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
            GameRources.GetInstance().selectedCharacterIndex].getDescriptionController()
            .getSelectedDescriptionController()
            .getDescriptionData().getDescriptionSoundPath();

        detailedDescription =
            detailedDescriptionLast =
                Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
                    GameRources.GetInstance().selectedCharacterIndex].getDescriptionController()
                    .getSelectedDescriptionController()
                    .getDescriptionData()
                    .getDetailedDescription();

        detailedDescriptionSound = Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCs()[
            GameRources.GetInstance().selectedCharacterIndex].getDescriptionController()
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
                nameDialog.Init(this, BaseFileOpenDialog.FileType.NPC_DESCRIPTION_NAME_SOUND);
                break;
            case AssetType.BRIEF_DESCRIPTION_SOUND:
                MusicFileOpenDialog briefDialog =
                (MusicFileOpenDialog)ScriptableObject.CreateInstance(typeof(MusicFileOpenDialog));
                briefDialog.Init(this, BaseFileOpenDialog.FileType.NPC_DESCRIPTION_BRIEF_SOUND);
                break;
            case AssetType.DETAILED_DESCRIPTION_SOUND:
                MusicFileOpenDialog detailedOverDialog =
                (MusicFileOpenDialog)ScriptableObject.CreateInstance(typeof(MusicFileOpenDialog));
                detailedOverDialog.Init(this, BaseFileOpenDialog.FileType.NPC_DESCRIPTION_DETAILED_SOUND);
                break;
        }

    }

    public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
    {
        if (workingObject is BaseFileOpenDialog.FileType)
        {
            switch ((BaseFileOpenDialog.FileType)workingObject)
            {
                case BaseFileOpenDialog.FileType.NPC_DESCRIPTION_NAME_SOUND:
                    OnDescriptorNameSoundChange(message);
                    break;
                case BaseFileOpenDialog.FileType.NPC_DESCRIPTION_BRIEF_SOUND:
                    OnDescriptorBriefSoundChange(message);
                    break;
                case BaseFileOpenDialog.FileType.NPC_DESCRIPTION_DETAILED_SOUND:
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