using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class NPCsListDataControl : DataControl
    {
        /**
        * List of characters.
        */
        private List<NPC> npcsList;

        /**
         * List of character controller.
         */
        private List<NPCDataControl> npcsDataControlList;

        /**
         * Constructor.
         * 
         * @param npcsList
         *            List of characters
         */
        public NPCsListDataControl(List<NPC> npcsList)
        {

            this.npcsList = npcsList;

            // Create the subcontrollers
            npcsDataControlList = new List<NPCDataControl>();
            foreach (NPC npc in npcsList)
                npcsDataControlList.Add(new NPCDataControl(npc));
        }

        /**
         * Returns the list of NPC controllers.
         * 
         * @return NPC controllers
         */
        public List<NPCDataControl> getNPCs()
        {

            return npcsDataControlList;
        }

        /**
         * Returns the last NPC controller of the list.
         * 
         * @return Last NPC controller
         */
        public NPCDataControl getLastNPC()
        {

            return npcsDataControlList[npcsDataControlList.Count - 1];
        }

        /**
         * Returns the info of the characters contained in the list.
         * 
         * @return Array with the information of the characters. It contains the
         *         identifier of each character, and the number of conversations
         */
        public string[][] getNPCsInfo()
        {

            string[][] npcsInfo = null;

            // Create the list for the characters
            npcsInfo = new string[npcsList.Count][];
            for (int i = 0; i < npcsList.Count; i++)
                npcsInfo[i] = new string[2];

            // Fill the array with the info
            for (int i = 0; i < npcsList.Count; i++)
            {
                NPC npc = npcsList[i];
                npcsInfo[i][0] = npc.getId();
                npcsInfo[i][1] = TC.get("NPCsList.ActionsNumber", npc.getActions().Count.ToString());
            }

            return npcsInfo;
        }

        public string[] getNPCsIDs()
        {
            string[] scenesInfo = null;
            scenesInfo = new string[npcsList.Count];
            for (int i = 0; i < npcsList.Count; i++)
            {
                scenesInfo[i] = npcsList[i].getId();
            }

            return scenesInfo;
        }

        public int getNPCIndexByID(string id)
        {
            for (int i = 0; i < npcsList.Count; i++)
            {
                if (npcsList[i].getId().Equals(id))
                    return i;
            }
            return -1;
        }

        public override System.Object getContent()
        {

            return npcsList;
        }


        public override int[] getAddableElements()
        {

            return new int[] { Controller.NPC };
        }


        public override bool canAddElement(int type)
        {

            // It can always add new characters
            return type == Controller.NPC;
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


        public override bool addElement(int type, string npcId)
        {

            bool elementAdded = false;

            if (type == Controller.NPC)
            {

                // Show a dialog asking for the character id
                if (string.IsNullOrEmpty(npcId))
                    controller.ShowInputIdDialog(TC.get("Operation.AddNPCTitle"), TC.get("Operation.AddNPCMessage"),
                        Controller.Instance.makeElementValid(TC.get("Operation.AddNPCDefaultValue")), performAddElement);
                else
                {
                    controller.DataModified();
                    performAddElement(null, npcId);
                    elementAdded = true;
                }
            }

            return elementAdded;
        }


        private void performAddElement(object sender, string npcId)
        {
            // If some value was typed and the identifier is valid
            if (!controller.isElementIdValid(npcId))
                npcId = controller.makeElementValid(npcId);

            // Add thew new character
            NPC newNPC = new NPC(npcId);
            npcsList.Add(newNPC);
            npcsDataControlList.Add(new NPCDataControl(newNPC));
            controller.IdentifierSummary.addId<NPC>(npcId);
        }


        public override bool duplicateElement(DataControl dataControl)
        {

            if (!(dataControl is NPCDataControl))
                return false;


            NPC newElement = (NPC)(((NPC)(dataControl.getContent())).Clone());
			string id = newElement.getId();

			if (!controller.isElementIdValid (id))
				id = controller.makeElementValid (id);
			
            newElement.setId(id);
            npcsList.Add(newElement);
            npcsDataControlList.Add(new NPCDataControl(newElement));
            controller.IdentifierSummary.addId<NPC>(id);
            return true;
        }


        public override string getDefaultId(int type)
        {

            return TC.get("Operation.AddNPCDefaultValue");
        }


        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {

            bool elementDeleted = false;
            string npcId = ((NPCDataControl)dataControl).getId();
            string references = controller.countIdentifierReferences(npcId).ToString();

            // Ask for confirmation
            if (!askConfirmation || controller.ShowStrictConfirmDialog(TC.get("Operation.DeleteElementTitle"), TC.get("Operation.DeleteElementWarning", new string[] { npcId, references })))
            {
                if (npcsList.Remove((NPC)dataControl.getContent()))
                {
                    npcsDataControlList.Remove((NPCDataControl)dataControl);
                    controller.deleteIdentifierReferences(npcId);
                    controller.IdentifierSummary.deleteId<NPC>(npcId);
                    controller.updateVarFlagSummary();
                    controller.DataModified();
                    elementDeleted = true;
                }
            }

            return elementDeleted;
        }


        public override bool moveElementUp(DataControl dataControl)
        {

            bool elementMoved = false;
            int elementIndex = npcsList.IndexOf((NPC)dataControl.getContent());

            if (elementIndex > 0)
            {
                NPC e = npcsList[elementIndex];
                NPCDataControl c = npcsDataControlList[elementIndex];
                npcsList.RemoveAt(elementIndex);
                npcsDataControlList.RemoveAt(elementIndex);
                npcsList.Insert(elementIndex - 1, e);
                npcsDataControlList.Insert(elementIndex - 1, c);
                controller.DataModified();
                elementMoved = true;
            }

            return elementMoved;
        }


        public override bool moveElementDown(DataControl dataControl)
        {

            bool elementMoved = false;
            int elementIndex = npcsList.IndexOf((NPC)dataControl.getContent());

            if (elementIndex < npcsList.Count - 1)
            {
                NPC e = npcsList[elementIndex];
                NPCDataControl c = npcsDataControlList[elementIndex];
                npcsList.RemoveAt(elementIndex);
                npcsDataControlList.RemoveAt(elementIndex);
                npcsList.Insert(elementIndex + 1, e);
                npcsDataControlList.Insert(elementIndex + 1, c);
                controller.DataModified();
                elementMoved = true;
            }

            return elementMoved;
        }


        public override string renameElement(string name)
        {

            return null;
        }


        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {

            // Iterate through each character
            foreach (NPCDataControl npcDataControl in npcsDataControlList)
                npcDataControl.updateVarFlagSummary(varFlagSummary);
        }


        public override bool isValid(string currentPath, List<string> incidences)
        {

            bool valid = true;

            // Update the current path
            currentPath += " >> " + TC.getElement(Controller.NPCS_LIST);

            // Iterate through the characters
            foreach (NPCDataControl npcDataControl in npcsDataControlList)
            {
                string npcPath = currentPath + " >> " + npcDataControl.getId();
                valid &= npcDataControl.isValid(npcPath, incidences);
            }

            return valid;
        }


        public override int countAssetReferences(string assetPath)
        {

            int count = 0;

            // Iterate through each character
            foreach (NPCDataControl npcDataControl in npcsDataControlList)
                count += npcDataControl.countAssetReferences(assetPath);

            return count;
        }


        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

            // Iterate through each item
            foreach (NPCDataControl npcDataControl in npcsDataControlList)
                npcDataControl.getAssetReferences(assetPaths, assetTypes);
        }


        public override void deleteAssetReferences(string assetPath)
        {

            // Iterate through each character
            foreach (NPCDataControl npcDataControl in npcsDataControlList)
                npcDataControl.deleteAssetReferences(assetPath);
        }


        public override int countIdentifierReferences(string id)
        {

            int count = 0;

            // Iterate through each character
            foreach (NPCDataControl npcDataControl in npcsDataControlList)
                count += npcDataControl.countIdentifierReferences(id);

            return count;
        }


        public override void replaceIdentifierReferences(string oldId, string newId)
        {

            // Iterate through each character
            foreach (NPCDataControl npcDataControl in npcsDataControlList)
                npcDataControl.replaceIdentifierReferences(oldId, newId);
        }


        public override void deleteIdentifierReferences(string id)
        {

            // Spread the call to every character
            foreach (NPCDataControl npcDataControl in npcsDataControlList)
                npcDataControl.deleteIdentifierReferences(id);
        }


        public override bool canBeDuplicated()
        {

            return false;
        }


        public override void recursiveSearch()
        {

            foreach (DataControl dc in this.npcsDataControlList)
            {
                dc.recursiveSearch();
            }
        }


        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {

            return getPathFromChild(dataControl, npcsDataControlList.Cast<Searchable>().ToList());
        }

        public NPC getNPC(string id)
        {
            foreach (NPC npc in npcsList)
            {
                if (npc.getId().Equals(id))
                    return npc;
            }
            return null;

        }

        public List<NPCDataControl> getAllNPCDataControls()
        {
            return npcsDataControlList;

        }
    }
}