using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    /**
     * Edition Tool for Changing a Target ID. Supports undo, redo but not combine
     */
    public class ChangeTargetIdTool : Tool
    {

        /**
         * The new id
         */
        protected string id;

        /**
         * The old id (for undo/redo)
         */
        protected string oldId;

        /**
         * Tells if the tree must be reloaded after do/undo/redo
         */
        protected bool updateTree;

        /**
         * Tells if the panel must be reloaded after do/undo/redo
         */
        protected bool reloadPanel;

        /**
         * The main controller
         */
        protected Controller controller;

        /**
         * The element which contains the targetId
         */
        protected HasTargetId elementWithTargetId;

        /**
         * Default constructor. Does not update neither tree nor panel
         * 
         * @param elementWithTargetId
         * @param newId
         */
        public ChangeTargetIdTool(HasTargetId elementWithTargetId, string newId) : this(elementWithTargetId, newId, false, true)
        {
        }

        public ChangeTargetIdTool(HasTargetId elementWithTargetId, string newId, bool updateTree, bool reloadPanel)
        {

            this.elementWithTargetId = elementWithTargetId;
            this.id = newId;
            this.oldId = elementWithTargetId.getTargetId();
            this.updateTree = updateTree;
            this.reloadPanel = reloadPanel;
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


        public override bool combine(Tool other)
        {

            return false;
        }


        public override bool doTool()
        {

            bool done = false;
            if (elementWithTargetId.getTargetId() == null || !elementWithTargetId.getTargetId().Equals(id))
            {
                elementWithTargetId.setTargetId(id);
                done = true;
                if (updateTree)
                {
                    controller.updateStructure();
                }
                if (reloadPanel)
                {
                    controller.updatePanel();
                }
            }
            return done;
        }


        public override bool redoTool()
        {

            return undoTool();
        }


        public override bool undoTool()
        {

            elementWithTargetId.setTargetId(oldId);
            string temp = oldId;
            oldId = id;
            id = temp;
            if (updateTree)
            {
                controller.updateStructure();
            }
            if (reloadPanel)
            {
                controller.updatePanel();
            }

            return true;
        }
    }
}