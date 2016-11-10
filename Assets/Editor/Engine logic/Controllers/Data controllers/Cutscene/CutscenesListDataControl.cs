using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CutscenesListDataControl : DataControl
{

    /**
     * List of cutscenes.
     */
    private List<Cutscene> cutscenesList;

    /**
     * List of cutscene controllers.
     */
    private List<CutsceneDataControl> cutscenesDataControlList;

    /**
     * Constructor.
     * 
     * @param cutscenesList
     *            List of cutscenes
     */

    public CutscenesListDataControl(List<Cutscene> cutscenesList)
    {

        this.cutscenesList = cutscenesList;

        // Create subcontrollers
        cutscenesDataControlList = new List<CutsceneDataControl>();
        foreach (Cutscene cutscene in cutscenesList)
            cutscenesDataControlList.Add(new CutsceneDataControl(cutscene));
    }

    /**
     * Returns the list of cutscene controllers.
     * 
     * @return Cutscene controllers
     */

    public List<CutsceneDataControl> getCutscenes()
    {

        return cutscenesDataControlList;
    }

    /**
     * Returns the last cutscene controller of the list.
     * 
     * @return Last cutscene controller
     */

    public CutsceneDataControl getLastCutscene()
    {

        return cutscenesDataControlList[cutscenesDataControlList.Count - 1];
    }

    /**
     * Returns the info of the cutscenes contained in the list.
     * 
     * @return Array with the information of the cutscenes. It contains the
     *         identifier of each cutscene, and its type
     */

    public string[][] getCutscenesInfo()
    {

        string[][] cutscenesInfo = null;

        // Create the list for the cutscenes
        cutscenesInfo = new string[cutscenesList.Count][];
        for (int i = 0; i < cutscenesList.Count; i ++)
            cutscenesInfo[i] = new string[2];

        // Fill the array with the info
        for (int i = 0; i < cutscenesList.Count; i++)
        {
            Cutscene cutscene = cutscenesList[i];
            cutscenesInfo[i][0] = cutscene.getId();
            if (cutscene.getType() == GeneralScene.GeneralSceneSceneType.SLIDESCENE)
                cutscenesInfo[i][1] = TC.get("CutscenesList.Slidescene");
            else if (cutscene.getType() == GeneralScene.GeneralSceneSceneType.VIDEOSCENE)
                cutscenesInfo[i][1] = TC.get("CutscenesList.Videoscene");

        }

        return cutscenesInfo;
    }

    public string[] getCutscenesIDs()
    {
        string[] scenesInfo = null;
        scenesInfo = new string[cutscenesList.Count];
        for (int i = 0; i < cutscenesList.Count; i++)
        {
            scenesInfo[i] = cutscenesList[i].getId();
        }

        return scenesInfo;
    }

    public int getCutsceneIndexByID(string id)
    {
        for (int i = 0; i < cutscenesList.Count; i++)
        {
            if (cutscenesList[i].getId().Equals(id))
                return i;
        }
        return -1;
    }

    public override System.Object getContent()
    {

        return cutscenesList;
    }


    public override int[] getAddableElements()
    {

        return new int[] {Controller.CUTSCENE};
    }


    public override bool canAddElement(int type)
    {

        // It can always add new cutscenes
        return type == Controller.CUTSCENE;
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


    public override bool addElement(int type, string cutsceneId)
    {

        bool elementAdded = false;

        ////if (type == Controller.CUTSCENE)
        ////{
        //    //TODO: implement
        //    //CutsceneTypesDialog cutscenesTypesDialog = new CutsceneTypesDialog();
        //    //type = cutscenesTypesDialog.getOptionSelected();
        //    //TODO: tmp, delete
        //    type = Controller.CUTSCENE_SLIDES;

        if (type == Controller.CUTSCENE_SLIDES)
        {

            // Show a dialog asking for the cutscene id
            if (cutsceneId == null || cutsceneId.Equals(""))
                cutsceneId = controller.showInputDialog(TC.get("Operation.AddCutsceneTitle"),
                    TC.get("Operation.AddCutsceneMessage"), TC.get("Operation.AddCutsceneDefaultValue"));

            // If some value was typed and the identifier is valid
            if (cutsceneId != null && controller.isElementIdValid(cutsceneId))
            {
                Cutscene newCutscene = null;

                // Create the new cutscene
                if (type == Controller.CUTSCENE_SLIDES)
                    newCutscene = new Slidescene(cutsceneId);

                // Add the new cutscene
                cutscenesList.Add(newCutscene);
                cutscenesDataControlList.Add(new CutsceneDataControl(newCutscene));
                controller.getIdentifierSummary().addCutsceneId(cutsceneId);
                //controller.dataModified( );
                elementAdded = true;
            }
        }

        else if (type == Controller.CUTSCENE_VIDEO)
        {

            // Show a dialog asking for the cutscene id
            if (cutsceneId == null)
                cutsceneId = controller.showInputDialog(TC.get("Operation.AddCutsceneTitle"),
                    TC.get("Operation.AddCutsceneMessage"), TC.get("Operation.AddCutsceneDefaultValue"));

            // If some value was typed and the identifier is valid
            if (cutsceneId != null && controller.isElementIdValid(cutsceneId))
            {
                Cutscene newCutscene = null;

                // Create the new cutscene
                if (type == Controller.CUTSCENE_VIDEO)
                    newCutscene = new Videoscene(cutsceneId);

                // Add the new cutscene
                cutscenesList.Add(newCutscene);
                cutscenesDataControlList.Add(new CutsceneDataControl(newCutscene));
                controller.getIdentifierSummary().addCutsceneId(cutsceneId);
                //controller.dataModified( );
                elementAdded = true;
            }
        }
        //}

        return elementAdded;
    }


    public override string getDefaultId(int type)
    {

        return TC.get("Operation.AddCutsceneDefaultValue");
    }


    public override bool deleteElement(DataControl dataControl, bool askConfirmation)
    {

        bool elementDeleted = false;

        // Take the number of general scenes in the chapter
        int generalScenesCount = controller.getIdentifierSummary().getGeneralSceneIds().Length;

        // If there are at least two scenes, this one can be deleted
        if (generalScenesCount > 1)
        {
            string cutsceneId = ((CutsceneDataControl) dataControl).getId();
            string references = controller.countIdentifierReferences(cutsceneId).ToString();

            // Ask for confirmation
            if (!askConfirmation ||
                controller.showStrictConfirmDialog(TC.get("Operation.DeleteElementTitle"),
                    TC.get("Operation.DeleteElementWarning", new string[] {cutsceneId, references})))
            {
                if (cutscenesList.Remove((Cutscene) dataControl.getContent()))
                {
                    cutscenesDataControlList.Remove((CutsceneDataControl) dataControl);
                    controller.deleteIdentifierReferences(cutsceneId);
                    controller.getIdentifierSummary().deleteCutsceneId(cutsceneId);
                    //controller.dataModified( );
                    elementDeleted = true;
                }
            }
        }

        // If this is the last scene, it can't be deleted
        else
            controller.showErrorDialog(TC.get("Operation.DeleteElementTitle"), TC.get("Operation.ErrorLastScene"));

        return elementDeleted;
    }


    public override bool moveElementUp(DataControl dataControl)
    {

        bool elementMoved = false;
        int elementIndex = cutscenesList.IndexOf((Cutscene) dataControl.getContent());

        if (elementIndex > 0)
        {
            Cutscene e = cutscenesList[elementIndex];
            CutsceneDataControl c = cutscenesDataControlList[elementIndex];
            cutscenesList.RemoveAt(elementIndex);
            cutscenesDataControlList.RemoveAt(elementIndex);
            cutscenesList.Insert(elementIndex - 1, e);
            cutscenesDataControlList.Insert(elementIndex - 1, c);
            //controller.dataModified( );
            elementMoved = true;
        }

        return elementMoved;
    }


    public override bool moveElementDown(DataControl dataControl)
    {

        bool elementMoved = false;
        int elementIndex = cutscenesList.IndexOf((Cutscene) dataControl.getContent());

        if (elementIndex < cutscenesList.Count - 1)
        {
            Cutscene e = cutscenesList[elementIndex];
            CutsceneDataControl c = cutscenesDataControlList[elementIndex];
            cutscenesList.RemoveAt(elementIndex);
            cutscenesDataControlList.RemoveAt(elementIndex);
            cutscenesList.Insert(elementIndex + 1, e);
            cutscenesDataControlList.Insert(elementIndex + 1, c);
            //controller.dataModified( );
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

        // Iterate through each cutscene
        foreach (CutsceneDataControl cutsceneDataControl in cutscenesDataControlList)
            cutsceneDataControl.updateVarFlagSummary(varFlagSummary);
    }


    public override bool isValid(string currentPath, List<string> incidences)
    {

        bool valid = true;

        // Update the current path
        currentPath += " >> " + TC.getElement(Controller.CUTSCENES_LIST);

        // Iterate through the cutscenes
        foreach (CutsceneDataControl cutsceneDataControl in cutscenesDataControlList)
        {
            string cutscenePath = currentPath + " >> " + cutsceneDataControl.getId();
            valid &= cutsceneDataControl.isValid(cutscenePath, incidences);
        }

        return valid;
    }


    public override int countAssetReferences(string assetPath)
    {

        int count = 0;

        // Iterate through each cutscene
        foreach (CutsceneDataControl cutsceneDataControl in cutscenesDataControlList)
            count += cutsceneDataControl.countAssetReferences(assetPath);

        return count;
    }


    public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
    {

        // Iterate through each cutscene
        foreach (CutsceneDataControl cutsceneDataControl in cutscenesDataControlList)
            cutsceneDataControl.getAssetReferences(assetPaths, assetTypes);
    }


    public override void deleteAssetReferences(string assetPath)
    {

        // Iterate through each cutscene
        foreach (CutsceneDataControl cutsceneDataControl in cutscenesDataControlList)
            cutsceneDataControl.deleteAssetReferences(assetPath);
    }


    public override int countIdentifierReferences(string id)
    {

        int count = 0;

        // Iterate through each cutscene
        foreach (CutsceneDataControl cutsceneDataControl in cutscenesDataControlList)
            count += cutsceneDataControl.countIdentifierReferences(id);

        return count;
    }


    public override void replaceIdentifierReferences(string oldId, string newId)
    {

        // Iterate through each cutscene
        foreach (CutsceneDataControl cutsceneDataControl in cutscenesDataControlList)
            cutsceneDataControl.replaceIdentifierReferences(oldId, newId);
    }


    public override void deleteIdentifierReferences(string id)
    {

        // Spread the call to every cutscene
        foreach (CutsceneDataControl cutsceneDataControl in cutscenesDataControlList)
            cutsceneDataControl.deleteIdentifierReferences(id);
    }


    public override bool canBeDuplicated()
    {

        return false;
    }


    public override void recursiveSearch()
    {

        foreach (DataControl dc in this.cutscenesDataControlList)
            dc.recursiveSearch();
    }


    public override List<Searchable> getPathToDataControl(Searchable dataControl)
    {

        return getPathFromChild(dataControl, cutscenesDataControlList.Cast<Searchable>().ToList());
    }

    public List<CutsceneDataControl> getAllCutsceneDataControls()
    {
        return cutscenesDataControlList;
    }

    public bool existsCutscene(string cutsceneID)
    {

        bool exists = false;

        foreach (Cutscene c in  cutscenesList)
        {
            if (c.getId().Equals(cutsceneID))
            {
                exists = true;
                break;
            }
        }

        return exists;


    }
}