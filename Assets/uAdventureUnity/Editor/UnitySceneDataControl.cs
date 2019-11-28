using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using uAdventure.Editor;
using uAdventure.Core;

namespace uAdventure.Unity
{
    public class UnitySceneDataControl : DataControl
    {
        private UnityScene unityScene;

        public string Scene
        {
            get { return unityScene.Scene; }
            set { controller.AddTool(new ChangeValueTool<UnityScene, string>(unityScene, value, "Scene")); }
        }

        public UnitySceneDataControl(UnityScene unityScene)
        {
            this.unityScene = unityScene;
        }


        public override bool addElement(int type, string id)
        {
            return false;
        }

        public override bool canAddElement(int type)
        {
            return false;
        }

        public override bool canBeDeleted()
        {
            return true;
        }

        public override bool canBeDuplicated()
        {
            return false;
        }

        public override bool canBeMoved()
        {
            return true;
        }

        public override bool canBeRenamed()
        {
            return true;
        }

        public override int countAssetReferences(string assetPath)
        {
            return 0;
        }

        public override int countIdentifierReferences(string id)
        {
            return unityScene.Id == id ? 1: 0;
        }

        public override void deleteAssetReferences(string assetPath)
        {
        }

        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {
            return false;
        }

        public override void deleteIdentifierReferences(string id)
        {
        }

        public override int[] getAddableElements()
        {
            return null;
        }

        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {
        }

        public override object getContent()
        {
            return unityScene;
        }

        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {
            return null;

        }

        public override bool isValid(string currentPath, List<string> incidences)
        {
            return true;
        }

        public override bool moveElementDown(DataControl dataControl)
        {
            return false;
        }

        public override bool moveElementUp(DataControl dataControl)
        {
            return false;
        }

        public override void recursiveSearch()
        {
        }

        public override string renameElement(string newName)
        {
            if (!Controller.Instance.isElementIdValid(newName))
            {
                newName = Controller.Instance.makeElementValid(newName);
            }
            if(controller.AddTool(new ChangeValueTool<UnityScene, string>(unityScene, newName, "Id")))
            {
                return newName;
            }
            return null;
        }

        public override void replaceIdentifierReferences(string oldId, string newId)
        {
        }

        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {
        }
    }
}
