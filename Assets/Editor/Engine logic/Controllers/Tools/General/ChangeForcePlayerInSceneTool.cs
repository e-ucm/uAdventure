using UnityEngine;
using System.Collections;

public class ChangeForcePlayerInSceneTool : Tool {

    private Controller controller;

    private bool isAllow;

    //private ScenePreviewEditionPanel scenePreviewEditionPanel;

    private SceneDataControl scene;

    public ChangeForcePlayerInSceneTool(bool isAllow, /*ScenePreviewEditionPanel scenePreviewEditionPanel,*/ SceneDataControl scene)
    {

        controller = Controller.getInstance();
        this.isAllow = isAllow;
        //this.scenePreviewEditionPanel = scenePreviewEditionPanel;
        this.scene = scene;
    }

    
    public override bool canRedo()
    {

        return true;
    }

    
    public override bool canUndo()
    {

        return true;
    }

    
    public override bool combine(Tool other)
    {

        return false;
    }

    
    public override bool doTool()
    {

        action(isAllow);
        return true;
    }

    private void action(bool al)
    {

        if (!al)
            scene.setPlayerLayer(Scene.PLAYER_WITHOUT_LAYER);
        else
            scene.setPlayerLayer(0);

        //if it is not allow that the player has layer, delete it in all references container
        if (al)
        {
            scene.addPlayerInReferenceList();
        }
        else {
            scene.deletePlayerInReferenceList();
        }
        // Now, the player is always showed in scenePreviewEditionPanel
        //if (scenePreviewEditionPanel != null){
        //  if (!Controller.getInstance().isPlayTransparent())
        //scenePreviewEditionPanel.addPlayer(scene, scene.getReferencesList().getPlayerImage());
        //scenePreviewEditionPanel.repaint();
        //}

        controller.updatePanel();
        /*looksPanel = new SceneLooksPanel(looksPanel.getSceneDataControl());
        tabPanel.remove(0);
        tabPanel.insertTab( TextConstants.getText( "Scene.LookPanelTitle" ), null, looksPanel, TextConstants.getText( "Scene.LookPanelTip" ), 0 );
        */
    }

    
    public override bool redoTool()
    {

        return doTool();
    }

    
    public override bool undoTool()
    {

        action(!isAllow);
        return true;
    }
}
