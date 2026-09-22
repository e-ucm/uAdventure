using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class MacroListDataControl : DataControl
    {/**
     * List of macros.
     */
        private List<Macro> macrosList;

        /**
         * List of macro controllers.
         */
        private List<MacroDataControl> macrosDataControlList;

        /**
         * Constructor.
         * 
         * @param macrosList
         *            List of macros
         */
        public MacroListDataControl(List<Macro> macrosList)
        {

            this.macrosList = macrosList;

            // Create subcontrollers
            macrosDataControlList = new List<MacroDataControl>();
            foreach (Macro macro in macrosList)
                macrosDataControlList.Add(new MacroDataControl(macro));
        }

        /**
         * Returns the list of macro controllers.
         * 
         * @return Macro controllers
         */
        public List<MacroDataControl> getMacros()
        {

            return macrosDataControlList;
        }

        /**
         * Returns the last macro controller from the list.
         * 
         * @return Last macro controller
         */
        public MacroDataControl getLastMacro()
        {

            return macrosDataControlList[macrosDataControlList.Count - 1];
        }

        /**
         * Returns the info of the macros contained in the list.
         * 
         * @return Array with the information of the macros. It contains the
         *         identifier of each macro, and the number of actions
         */
        public string[][] getMacrosInfo()
        {

            string[][] macrosInfo = null;

            // Create the list for the macros
            macrosInfo = new string[macrosList.Count][];
            for (int i = 0; i < macrosList.Count; i++)
                macrosInfo[i] = new string[2];

            // Fill the array with the info
            for (int i = 0; i < macrosList.Count; i++)
            {
                Macro macro = macrosList[i];
                macrosInfo[i][0] = macro.getId();
                macrosInfo[i][1] = Controller.Instance.countIdentifierReferences(macro.getId()).ToString();
            }

            return macrosInfo;
        }

        public string[] getMacrosIDs()
        {

            string[] macrosInfo = null;

            // Create the list for the macros
            macrosInfo = new string[macrosList.Count];

            // Fill the array with the info
            for (int i = 0; i < macrosList.Count; i++)
            {
                Macro macro = macrosList[i];
                macrosInfo[i] = macro.getId();
            }

            return macrosInfo;
        }

        public override System.Object getContent()
        {

            return macrosList;
        }


        public override int[] getAddableElements()
        {

            return new int[] { Controller.MACRO };
        }


        public override bool canAddElement(int type)
        {

            // It can always add new macros
            return type == Controller.MACRO;
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


        public override bool addElement(int type, string macroId)
        {
            bool elementAdded = false;

            if (type == Controller.MACRO)
            {
                // Show a dialog asking for the macro id
                if (macroId == null)
                    controller.ShowInputIdDialog(TC.get("Operation.AddMacroTitle"), TC.get("Operation.AddMacroMessage"),
                        Controller.Instance.makeElementValid(TC.get("Operation.AddMacroDefaultValue")), performAddElement);
                else
                {
                    performAddElement(null, macroId);
                    elementAdded = true;
                }
            }

            return elementAdded;
        }

        private void performAddElement(object sender, string macroId)
        {
            if (!controller.isElementIdValid(macroId))
                macroId = controller.makeElementValid(macroId);

            // Add thew new macro
            Macro newMacro = new Macro(macroId);
            macrosList.Add(newMacro);
            macrosDataControlList.Add(new MacroDataControl(newMacro));
            controller.IdentifierSummary.addId<Macro>(macroId);
            controller.DataModified();
        }


        public override bool duplicateElement(DataControl dataControl)
        {

            if (!(dataControl is MacroDataControl))
                return false;

            Macro newElement = (Macro)(((Macro)(dataControl.getContent())).Clone());
            string id = newElement.getId();
            int i = 1;
            do
            {
                id = newElement.getId() + i;
                i++;
            } while (!controller.isElementIdValid(id, false));
            newElement.setId(id);
            macrosList.Add(newElement);
            macrosDataControlList.Add(new MacroDataControl(newElement));
            controller.IdentifierSummary.addId<Macro>(id);
            controller.updateVarFlagSummary();
            controller.DataModified();
            return true;

        }


        public override string getDefaultId(int type)
        {

            return TC.get("Operation.AddMacroDefaultValue");
        }


        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {

            bool elementDeleted = false;
            string macroId = ((MacroDataControl)dataControl).getId();
            string references = controller.countIdentifierReferences(macroId).ToString();

            // Ask for confirmation
            if (!askConfirmation || controller.ShowStrictConfirmDialog(TC.get("Operation.DeleteElementTitle"), TC.get("Operation.DeleteElementWarning", new string[] { macroId, references })))
            {
                if (macrosList.Remove((Macro)dataControl.getContent()))
                {
                    macrosDataControlList.Remove((MacroDataControl)dataControl);
                    controller.deleteIdentifierReferences(macroId);
                    controller.IdentifierSummary.deleteId<Macro>(macroId);
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
            int elementIndex = macrosList.IndexOf((Macro)dataControl.getContent());

            if (elementIndex > 0)
            {
                Macro e = macrosList[elementIndex];
                MacroDataControl c = macrosDataControlList[elementIndex];
                macrosList.RemoveAt(elementIndex);
                macrosDataControlList.RemoveAt(elementIndex);
                macrosList.Insert(elementIndex - 1, e);
                macrosDataControlList.Insert(elementIndex - 1, c);
                controller.DataModified();
                elementMoved = true;
            }

            return elementMoved;
        }


        public override bool moveElementDown(DataControl dataControl)
        {

            bool elementMoved = false;
            int elementIndex = macrosList.IndexOf((Macro)dataControl.getContent());

            if (elementIndex < macrosList.Count - 1)
            {
                Macro e = macrosList[elementIndex];
                MacroDataControl c = macrosDataControlList[elementIndex];
                macrosList.RemoveAt(elementIndex);
                macrosDataControlList.RemoveAt(elementIndex);
                macrosList.Insert(elementIndex + 1, e);
                macrosDataControlList.Insert(elementIndex + 1, c);
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

            // Iterate through each macro
            foreach (MacroDataControl macroDataControl in macrosDataControlList)
                macroDataControl.updateVarFlagSummary(varFlagSummary);
        }


        public override bool isValid(string currentPath, List<string> incidences)
        {

            bool valid = true;

            // Update the current path
            currentPath += " >> " + TC.getElement(Controller.GLOBAL_STATE_LIST);

            // Iterate through the macros
            foreach (MacroDataControl macroDataControl in macrosDataControlList)
            {
                string macroPath = currentPath + " >> " + macroDataControl.getId();
                valid &= macroDataControl.isValid(macroPath, incidences);
            }

            return valid;
        }


        public override int countAssetReferences(string assetPath)
        {

            int count = 0;

            // Iterate through each macro
            foreach (MacroDataControl macroDataControl in macrosDataControlList)
                count += macroDataControl.countAssetReferences(assetPath);

            return count;
        }


        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

            // Iterate through each macro
            foreach (MacroDataControl macroDataControl in macrosDataControlList)
                macroDataControl.getAssetReferences(assetPaths, assetTypes);
        }


        public override void deleteAssetReferences(string assetPath)
        {

            // Iterate through each macro
            foreach (MacroDataControl macroDataControl in macrosDataControlList)
                macroDataControl.deleteAssetReferences(assetPath);
        }


        public override int countIdentifierReferences(string id)
        {

            int count = 0;

            // Iterate through each macro
            foreach (MacroDataControl macroDataControl in macrosDataControlList)
                count += macroDataControl.countIdentifierReferences(id);

            return count;
        }


        public override void replaceIdentifierReferences(string oldId, string newId)
        {

            // Iterate through each macro
            foreach (MacroDataControl macroDataControl in macrosDataControlList)
                macroDataControl.replaceIdentifierReferences(oldId, newId);
        }


        public override void deleteIdentifierReferences(string id)
        {

            // Spread the call to every macro
            foreach (MacroDataControl macroDataControl in macrosDataControlList)
                macroDataControl.deleteIdentifierReferences(id);
        }


        public override bool canBeDuplicated()
        {

            return false;
        }


        public override void recursiveSearch()
        {

            foreach (DataControl dc in this.macrosDataControlList)
                dc.recursiveSearch();
        }


        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {

            return getPathFromChild(dataControl, macrosDataControlList.Cast<Searchable>().ToList());
        }

        public List<Macro> getMacrosList()
        {

            return this.macrosList;
        }
    }
}