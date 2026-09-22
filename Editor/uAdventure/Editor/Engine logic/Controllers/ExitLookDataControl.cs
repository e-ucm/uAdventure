using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ExitLookDataControl : Searchable, HasSound
    {
        private ExitLook exitLook;

        public ExitLookDataControl(NextScene nextScene)
        {

            if (nextScene.getExitLook() == null)
                nextScene.setExitLook(new ExitLook());
            this.exitLook = nextScene.getExitLook();
        }

        public ExitLookDataControl(Exit exit)
        {

            if (exit.getDefaultExitLook() == null)
                exit.setDefaultExitLook(new ExitLook());
            this.exitLook = exit.getDefaultExitLook();
        }

        /**
         * @return the isTextCustomized
         */
        public bool isTextCustomized()
        {

            return exitLook.getExitText() != null;
        }

        public string getCustomizedText()
        {

            string text = null;
            if (exitLook != null && exitLook.getExitText() != null)
                text = exitLook.getExitText();
            return text;
        }

        //v1.4
        public string getSoundPath()
        {

            string text = null;
            if (exitLook != null && exitLook.getSoundPath() != null)
                text = exitLook.getSoundPath();
            return text;
        }

        /**
         * @return the isCursorCustomized
         */
        public bool isCursorCustomized()
        {

            return exitLook.getCursorPath() != null || exitLook.getSoundPath() != null;
        }

        public string getCustomizedCursor()
        {

            string text = null;
            if (exitLook != null && exitLook.getCursorPath() != null)
                text = exitLook.getCursorPath();
            return text;
        }

        public void setExitText(string text)
        {

            Controller.Instance.AddTool(new ChangeStringValueTool(exitLook, text, "getExitText", "setExitText"));
            //this.exitLook.setExitText( text );
        }

        public void editCursorPath()
        {
            Controller.Instance.AddTool(new SelectExitCursorPathTool(exitLook));
        }

        public void invalidCursor()
        {

            Controller.Instance.AddTool(new InvalidExitCursorTool(exitLook));
        }

        public void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

            if (exitLook.getCursorPath() != null && !exitLook.getCursorPath().Equals("") && !assetPaths.Contains(exitLook.getCursorPath()))
            {
                assetPaths.Add(exitLook.getCursorPath());
                assetTypes.Add(AssetsConstants.CATEGORY_CURSOR);
            }

            if (exitLook.getSoundPath() != null && !exitLook.getSoundPath().Equals("") && !assetPaths.Contains(exitLook.getSoundPath()))
            {
                assetPaths.Add(exitLook.getSoundPath());
                assetTypes.Add(AssetsConstants.CATEGORY_AUDIO);
            }

        }

        public int countAssetReferences(string assetPath)
        {

            if (exitLook.getCursorPath() != null && exitLook.getCursorPath().Equals(assetPath))
                return 1;
            else if (exitLook.getSoundPath() != null && exitLook.getSoundPath().Equals(assetPath))
                return 1;
            else
                return 0;

        }

        public void deleteAssetReferences(string assetPath)
        {

            if (exitLook.getCursorPath() != null && exitLook.getCursorPath().Equals(assetPath))
                exitLook.setCursorPath("");

            if (exitLook.getSoundPath() != null && exitLook.getSoundPath().Equals(assetPath))
                exitLook.setSoundPath("");

        }

        public void setSoundPath(string soundPath)
        {
            if (exitLook != null)
            {
                exitLook.setSoundPath(soundPath);
            }
        }

        //DO NOT REMOVE: USED WITH REFLECTION
        public void setCursorPath(string value)
        {

            exitLook.setCursorPath(value);
        }

        public override void recursiveSearch()
        {

            check(getCustomizedCursor(), TC.get("Cursor.exit.Description"));
            check(getCustomizedText(), TC.get("Search.CustomizedText"));
            check(getSoundPath(), TC.get("Animation.Sound"));
        }
        //TODO: check access rights
        protected override List<Searchable> getPath(Searchable dataControl)
        {

            if (dataControl == this)
            {
                List<Searchable> path = new List<Searchable>();
                path.Add(this);
                return path;
            }
            return getPathToDataControl(dataControl);
        }

        public List<Searchable> getPathToDataControl(Searchable dataControl)
        {

            return null;
        }
    }
}