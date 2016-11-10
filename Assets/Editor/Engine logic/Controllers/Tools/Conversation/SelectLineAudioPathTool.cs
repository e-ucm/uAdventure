using UnityEngine;
using System.Collections;

public class SelectLineAudioPathTool : SelectResourceTool
{
    protected const string AUDIO_STR = "audio";

    protected ConversationLine line;

    protected static AssetInformation[] createAssetInfoArray()
    {

        AssetInformation[] array = new AssetInformation[1];
        array[0] = new AssetInformation("", AUDIO_STR, true, AssetsConstants.CATEGORY_AUDIO,
            AssetsController.FILTER_NONE);
        return array;
    }

    protected static ResourcesUni createResources(ConversationLine line)
    {

        ResourcesUni resources = new ResourcesUni();
        if (line.getAudioPath() != null)
            resources.addAsset(AUDIO_STR, line.getAudioPath());
        return resources;
    }

    public SelectLineAudioPathTool(ConversationLine line)
        : base(createResources(line), createAssetInfoArray(), Controller.CONVERSATION_GRAPH, 0)

    {
        this.line = line;
    }


    public bool undoTool()
    {

        bool done = base.undoTool();
        if (!done)
            return false;
        else
        {
            line.setAudioPath(resources.getAssetPath(AUDIO_STR));
            controller.updatePanel();
            return true;
        }

    }


    public bool redoTool()
    {

        bool done = base.redoTool();
        if (!done)
            return false;
        else
        {
            line.setAudioPath(resources.getAssetPath(AUDIO_STR));
            controller.updatePanel();
            return true;
        }
    }


    public bool doTool()
    {

        bool done = base.doTool();
        if (!done)
            return false;
        else
        {
            line.setAudioPath(resources.getAssetPath(AUDIO_STR));
            return true;
        }
    }
}