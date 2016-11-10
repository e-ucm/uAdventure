using UnityEngine;
using System.Collections;

public class PlayerDataControl : NPCDataControl
{

    /**
       * Contained player data.
       */
    private Player player;

    /**
     * Constructor.
     * 
     * @param player
     *            Contained player data
     */
    public PlayerDataControl(Player player) : base(player)
    {
        this.player = player;
    }

    /**
     * Notify to all scenes that the player image has been changed
     */
    public void playerImageChange()
    {

        string preview = getPreviewImage();
        if (preview != null)
        {
            foreach (SceneDataControl scene in Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes())
            {
                scene.imageChangeNotify(preview);
            }
        }
    }

    
    public override System.Object getContent()
    {

        return player;
    }

    
    public override int[] getAddableElements()
    {

        return new int[] { };
    }

    
    public override bool canAddElement(int type)
    {

        return false;
    }

    
    public override bool canBeDeleted()
    {

        return false;
    }

    
    public override bool canBeMoved()
    {

        return false;
    }

    
    public override bool canBeRenamed()
    {

        return false;
    }

    
    public override bool addElement(int type, string id)
    {

        bool elementAdded = false;

        if (type == Controller.RESOURCES && !Controller.getInstance().isPlayTransparent())
        {
            return base.addElement(type, id);
        }

        return elementAdded;
    }

    
    public override bool buildResourcesTab()
    {

        return !Controller.getInstance().isPlayTransparent();
    }

    
    public override string renameElement(string name)
    {

        return null;
    }


    
    public override bool canBeDuplicated()
    {

        return false;

    }
}
