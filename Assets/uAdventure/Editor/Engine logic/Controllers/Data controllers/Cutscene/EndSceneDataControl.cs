using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class EndSceneDataControl : DataControl
    {


        public override System.Object getContent()
        {

            return Controller.END_SCENE;
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

            return true;
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

            return false;
        }


        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {

            return false;
        }


        public override bool moveElementUp(DataControl dataControl)
        {

            return false;
        }


        public override bool moveElementDown(DataControl dataControl)
        {

            return false;
        }


        public override string renameElement(string name)
        {

            return null;
        }


        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {

            // Do nothing
        }


        public override bool isValid(string currentPath, List<string> incidences)
        {

            return true;
        }


        public override int countAssetReferences(string assetPath)
        {

            return 0;
        }


        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

            // Do nothing
        }


        public override void deleteAssetReferences(string assetPath)
        {

            // Do nothing
        }


        public override int countIdentifierReferences(string id)
        {

            return 0;
        }


        public override void replaceIdentifierReferences(string oldId, string newId)
        {

            // Do nothing
        }


        public override void deleteIdentifierReferences(string id)
        {

            // Do nothing
        }


        public override bool canBeDuplicated()
        {

            return false;
        }


        public override void recursiveSearch()
        {

        }


        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {

            return null;
        }
    }
}