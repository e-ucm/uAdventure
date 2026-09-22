using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ChangeDocumentationTool : Tool
    {
        private Documented documented;

        private string documentation;

        private string oldDocumentation;

        private Controller controller;

        public ChangeDocumentationTool(Documented documented, string documentation)
        {

            this.documented = documented;
            this.documentation = documentation;
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

            if (!documentation.Equals(documented.getDocumentation()))
            {
                oldDocumentation = documented.getDocumentation();
                documented.setDocumentation(documentation);
                return true;
            }
            return false;
        }


        public override string getToolName()
        {

            return "change docuemntation";
        }


        public override bool redoTool()
        {

            documented.setDocumentation(documentation);
            controller.updatePanel();
            return true;
        }


        public override bool undoTool()
        {

            documented.setDocumentation(oldDocumentation);
            controller.updatePanel();
            return true;
        }


        public override bool combine(Tool other)
        {

            if (other is ChangeDocumentationTool)
            {
                ChangeDocumentationTool cnt = (ChangeDocumentationTool)other;
                if (cnt.documented == documented && cnt.oldDocumentation == documentation)
                {
                    documentation = cnt.documentation;
                    timeStamp = cnt.timeStamp;
                    return true;
                }
            }
            return false;
        }
    }
}