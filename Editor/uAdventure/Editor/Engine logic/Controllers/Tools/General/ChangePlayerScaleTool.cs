using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ChangePlayerScaleTool : Tool
    {
        private Scene scene;

        private float scale;

        private float oldScale;

        public ChangePlayerScaleTool(Scene scene, float scale)
        {

            this.scene = scene;
            this.scale = scale;
            this.oldScale = scene.getPlayerScale();
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

            if (other
            is ChangePlayerScaleTool)
            {
                ChangePlayerScaleTool cpst = (ChangePlayerScaleTool)other;
                if (cpst.scene == scene)
                {
                    scale = cpst.scale;
                    timeStamp = cpst.timeStamp;
                    return true;
                }
            }
            return false;
        }



        public override bool doTool()
        {

            scene.setPlayerScale(scale);
            return true;
        }



        public override bool redoTool()
        {

            scene.setPlayerScale(scale);
            Controller.Instance.updatePanel();
            return true;
        }

        public override bool undoTool()
        {

            scene.setPlayerScale(oldScale);
            Controller.Instance.updatePanel();
            return true;
        }
    }
}