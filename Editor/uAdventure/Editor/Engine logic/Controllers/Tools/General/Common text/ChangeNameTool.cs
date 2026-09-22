using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ChangeNameTool : Tool
    {

        private Named named;

        private string name;

        private string oldName;

        private Controller controller;

        public ChangeNameTool(Named named, string name)
        {

            this.named = named;
            this.name = name;
            this.controller = Controller.Instance;
        }

        public override bool canRedo()
        {

            return true;
        }


        public override bool canUndo()
        {

            return true;
        }


        public override bool doTool()
        {

            if (!name.Equals(named.getName()))
            {
                oldName = named.getName();
                named.setName(name);
                return true;
            }
            return false;
        }


        public override bool redoTool()
        {

            named.setName(name);
            controller.updatePanel();
            return true;
        }


        public override bool undoTool()
        {

            named.setName(oldName);
            controller.updatePanel();
            return true;
        }


        public override bool combine(Tool other)
        {

            if (other is ChangeNameTool)
            {
                ChangeNameTool cnt = (ChangeNameTool)other;
                if (cnt.named == named && cnt.oldName == name)
                {
                    name = cnt.name;
                    timeStamp = cnt.timeStamp;
                    return true;
                }
            }
            return false;
        }
    }
}