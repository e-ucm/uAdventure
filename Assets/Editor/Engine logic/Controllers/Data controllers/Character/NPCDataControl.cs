using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NPCDataControl : DataControlWithResources
{
    /**
         * Constant for the empty animation
         */
    private const string EMPTY_ANIMATION = "assets/special/EmptyAnimation";
    
    /**
     * Contained NPC data.
     */
    private NPC npc;

    /**
     * Actions list controller.
     */
    private ActionsListDataControl actionsListDataControl;

    /**
     * Controller for descriptions
     */
    private DescriptionsController descriptionController;

    /**
     * Constructor
     * 
     * @param npc
     *            Contained NPC data
     */
    public NPCDataControl(NPC npc)
    {

        this.npc = npc;
        this.resourcesList = npc.getResources();

        selectedResources = 0;

        // Add a new resource if the list is empty
        if (resourcesList.Count == 0)
            resourcesList.Add(new ResourcesUni());

        // Create the subcontrollers
        resourcesDataControlList = new List<ResourcesDataControl>();
        foreach (ResourcesUni resources in resourcesList)
            resourcesDataControlList.Add(new ResourcesDataControl(resources, Controller.NPC));

        actionsListDataControl = new ActionsListDataControl(npc.getActions(), this);

        descriptionController = new DescriptionsController(npc.getDescriptions());

    }

    /**
     * Returns the actions list controller.
     * 
     * @return Actions list controller
     */
    public ActionsListDataControl getActionsList()
    {

        return actionsListDataControl;
    }

    /**
     * Returns the path to the selected preview image.
     * 
     * @return Path to the image, null if not present
     */
    public string getPreviewImage()
    {

        string previewImagePath = getExistingPreviewImagePath();
        // Add the extension of the frame
        if (previewImagePath != null && !previewImagePath.ToLower().EndsWith(".eaa")
            //HACK?
            && previewImagePath.Equals(EMPTY_ANIMATION))
            previewImagePath += "_01.png";
        else if (previewImagePath != null)
        {
            return Loader.loadAnimation(AssetsController.InputStreamCreatorEditor.getInputStreamCreator(), previewImagePath, new EditorImageLoader()).getFrame(0).getUri();
        }
        return previewImagePath;
    }

    /**
     * Look for one image path. If there no one, return empty animation path
     * 
     */
    // Modified v1.5 to fix a bug with empty animations in eaa format
    private string getExistingPreviewImagePath()
    {

        string path = null;
        foreach (ResourcesDataControl resource in resourcesDataControlList)
        {
            if (resource != null && resource.getAssetPath("standright") != null &&
                    !resource.getAssetPath("standright").Equals(EMPTY_ANIMATION) &&
                    !resource.getAssetPath("standright").Equals(EMPTY_ANIMATION + ".eaa"))
                path = resource.getAssetPath("standright");
            else
                path = resource.getAssetPath("standleft");
            if (path != null)
            {
                return path;
            }

            for (int i = 0; i < resource.getAssetCount(); i++)
            {
                path = resource.getAssetPath(resource.getAssetName(i));
                if (path != null)
                {
                    return path;
                }
            }
        }

        return EMPTY_ANIMATION;
    }

    /**
     * Returns the id of the NPC.
     * 
     * @return NPC's id
     */
    public string getId()
    {

        return npc.getId();
    }

    /**
     * Returns the documentation of the NPC.
     * 
     * @return NPC's documentation
     */
    public string getDocumentation()
    {

        return npc.getDocumentation();
    }

    public void setDocumentation(string val)
    {
        npc.setDocumentation(val);
    }
    /**
     * Returns the text front color for the player strings.
     * 
     * @return Text front color
     */
    public string getTextFrontColor()
    {
        return npc.getTextFrontColor();
    }

    /**
     * Returns the text border color for the player strings.
     * 
     * @return Text front color
     */
    public string getTextBorderColor()
    {
        return npc.getTextBorderColor();
    }

    /**
     * Check if the engine must synthesizer all current npc conversation lines
     * 
     * @return if npc must synthesizer all his lines
     */
    public bool isAlwaysSynthesizer()
    {

        return npc.isAlwaysSynthesizer();
    }

    /**
     * Sets the text front color for the player strings.
     * 
     * @param textFrontColor
     *            New text front color
     */
    public void setTextFrontColor(string textFrontColor)
    {
        npc.setTextFrontColor(textFrontColor);
    }

    /**
     * Sets the text border color for the player strings.
     * 
     * @param textBorderColor
     *            New text border color
     */
    public void setTextBorderColor(string textBorderColor)
    {

        npc.setTextBorderColor(textBorderColor);
    }

    public void setBubbleBkgColor(string bubbleBkgColor)
    {
        npc.setBubbleBkgColor(bubbleBkgColor);
    }

    public void setBubbleBorderColor(string bubbleBorderColor)
    {
        npc.setBubbleBorderColor(bubbleBorderColor);
    }

    /**
     * Set the possibility to all conversation lines to be read by synthesizer
     * 
     * @param always
     *            bool value
     */
    public void setAlwaysSynthesizer(bool always)
    {

        controller.addTool(new ChangeBooleanValueTool(npc, always, "isAlwaysSynthesizer", "setAlwaysSynthesizer"));
    }

    /**
     * Sets the new voice for the character
     * 
     * @param voice
     *            a string with the valid voice
     */
    public void setVoice(string voice)
    {

        controller.addTool(new ChangeStringValueTool(npc, voice, "getVoice", "setVoice"));
    }

    /**
     * Gets the voice associated to character
     * 
     * @return string representing character voice
     */
    public string getVoice()
    {

        return npc.getVoice();
    }

    
    public override System.Object getContent()
    {

        return npc;
    }

    
    public override int[] getAddableElements()
    {

        //return new int[] { Controller.RESOURCES };
        return new int[] { };
    }

    
    public override bool canAddElement(int type)
    {

        // It can always add new resources
        //return type == Controller.RESOURCES;
        return false;
    }

    
    public override bool canBeDeleted()
    {

        return true;
    }

    
    public override bool canBeMoved()
    {

        return true;
    }

    
    public override bool canBeRenamed()
    {

        return true;
    }

    
    public override bool addElement(int type, string id)
    {

        bool elementAdded = false;

        if (type == Controller.RESOURCES)
        {
            elementAdded = Controller.getInstance().addTool(new AddResourcesBlockTool(resourcesList, resourcesDataControlList, Controller.NPC, this));
        }

        return elementAdded;
    }

    
    public override bool moveElementUp(DataControl dataControl)
    {

        bool elementMoved = false;
        int elementIndex = resourcesList.IndexOf((ResourcesUni)dataControl.getContent());

        if (elementIndex > 0)
        {
            ResourcesUni e = resourcesList[elementIndex];
            ResourcesDataControl c = resourcesDataControlList[elementIndex];
            resourcesList.RemoveAt(elementIndex);
            resourcesDataControlList.RemoveAt(elementIndex);
            resourcesList.Insert(elementIndex - 1, e);
            resourcesDataControlList.Insert(elementIndex - 1, c);
            //controller.dataModified( );
            elementMoved = true;
        }

        return elementMoved;
    }

    
    public override bool moveElementDown(DataControl dataControl)
    {

        bool elementMoved = false;
        int elementIndex = resourcesList.IndexOf((ResourcesUni)dataControl.getContent());

        if (elementIndex < resourcesList.Count - 1)
        {
            ResourcesUni e = resourcesList[elementIndex];
            ResourcesDataControl c = resourcesDataControlList[elementIndex];
            resourcesList.RemoveAt(elementIndex);
            resourcesDataControlList.RemoveAt(elementIndex);
            resourcesList.Insert(elementIndex + 1, e);
            resourcesDataControlList.Insert(elementIndex + 1, c);
            //controller.dataModified( );
            elementMoved = true;
        }

        return elementMoved;
    }

    
    public override string renameElement(string name)
    {

        bool elementRenamed = false;
        string oldNPCId = npc.getId();
        string references = controller.countIdentifierReferences(oldNPCId).ToString();

        // Ask for confirmation
        if (name != null || controller.showStrictConfirmDialog(TC.get("Operation.RenameNPCTitle"), TC.get("Operation.RenameElementWarning", new string[] { oldNPCId, references })))
        {

            // Show a dialog asking for the new npc id
            string newNPCId = name;
            if (name == null)
                newNPCId = controller.showInputDialog(TC.get("Operation.RenameNPCTitle"), TC.get("Operation.RenameNPCMessage"), oldNPCId);

            // If some value was typed and the identifiers are different
            if (newNPCId != null && !newNPCId.Equals(oldNPCId) && controller.isElementIdValid(newNPCId))
            {
                npc.setId(newNPCId);
                controller.replaceIdentifierReferences(oldNPCId, newNPCId);
                controller.getIdentifierSummary().deleteNPCId(oldNPCId);
                controller.getIdentifierSummary().addNPCId(newNPCId);
                //controller.dataModified( );
                elementRenamed = true;
            }
        }

        if (elementRenamed)
            return oldNPCId;
        else
            return null;
    }

    
    public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
    {

        actionsListDataControl.updateVarFlagSummary(varFlagSummary);
        descriptionController.updateVarFlagSummary(varFlagSummary);
        // Iterate through the resources
        foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
            resourcesDataControl.updateVarFlagSummary(varFlagSummary);

    }

    
    public override bool isValid(string currentPath, List<string> incidences)
    {

        bool valid = true;

        // Iterate through the resources
        for (int i = 0; i < resourcesDataControlList.Count; i++)
        {
            string resourcesPath = currentPath + " >> " + TC.getElement(Controller.RESOURCES) + " #" + (i + 1);
            valid &= resourcesDataControlList[i].isValid(resourcesPath, incidences);
        }

        // Spread the call to the actions
        valid &= actionsListDataControl.isValid(currentPath, incidences);
        //1.4
        valid &= descriptionController.isValid(currentPath, incidences);

        return valid;
    }

    
    public override int countAssetReferences(string assetPath)
    {

        int count = 0;

        // Iterate through the resources
        foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
            count += resourcesDataControl.countAssetReferences(assetPath);

        // Add the references in the actions
        count += actionsListDataControl.countAssetReferences(assetPath);

        //1.4
        count += descriptionController.countAssetReferences(assetPath);

        return count;
    }

    
    public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
    {

        // Iterate through the resources
        foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
            resourcesDataControl.getAssetReferences(assetPaths, assetTypes);

        // Add the references in the actions
        actionsListDataControl.getAssetReferences(assetPaths, assetTypes);

        descriptionController.getAssetReferences(assetPaths, assetTypes);
    }

    
    public override void deleteAssetReferences(string assetPath)
    {

        // Iterate through the resources
        foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
            resourcesDataControl.deleteAssetReferences(assetPath);

        // Delete the references from the actions
        actionsListDataControl.deleteAssetReferences(assetPath);
        //1.4
        descriptionController.deleteAssetReferences(assetPath);
    }

    
    public override int countIdentifierReferences(string id)
    {

        int count = 0;
        // Iterate through the resources
        foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
            resourcesDataControl.countIdentifierReferences(id);

        count += actionsListDataControl.countIdentifierReferences(id);
        //1.4
        count += descriptionController.countIdentifierReferences(id);
        return count;
    }

    
    public override void replaceIdentifierReferences(string oldId, string newId)
    {

        // Iterate through the resources
        foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
            resourcesDataControl.replaceIdentifierReferences(oldId, newId);

        actionsListDataControl.replaceIdentifierReferences(oldId, newId);
        descriptionController.replaceIdentifierReferences(oldId, newId);
    }

    
    public override void deleteIdentifierReferences(string id)
    {

        // Iterate through the resources
        foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
            resourcesDataControl.deleteIdentifierReferences(id);

        actionsListDataControl.deleteIdentifierReferences(id);
        //1.4
        descriptionController.deleteIdentifierReferences(id);
    }

    public virtual bool buildResourcesTab()
    {

        return true;
    }

    
    public override bool canBeDuplicated()
    {

        return true;
    }

    
    public override void recursiveSearch()
    {

        check(this.getDocumentation(), TC.get("Search.Documentation"));
        check(this.getId(), "ID");
        check(this.getVoice(), TC.get("Search.NPCVoice"));
        check(this.getPreviewImage(), TC.get("Search.PreviewImage"));
        descriptionController.recursiveSearch();
        getActionsList().recursiveSearch();
        foreach (ResourcesDataControl r in resourcesDataControlList)
        {
            r.recursiveSearch();
        }
    }

    public string getAnimationPath(string animation)
    {

        return resourcesDataControlList[selectedResources].getAssetPath(animation);
    }

    public void addAnimationPath(string animation, string path)
    {
        resourcesDataControlList[selectedResources].addAsset(animation, path);
    }


    public string getAnimationPathPreview(string animation)
    {

        string previewImagePath = resourcesDataControlList[selectedResources].getAssetPath(animation);

        // Add the extension of the frame
        if (previewImagePath != null && !previewImagePath.ToLower().EndsWith(".eaa"))
            previewImagePath += "_01.jpg";
        else if (previewImagePath != null)
        {
            return Loader.loadAnimation(AssetsController.InputStreamCreatorEditor.getInputStreamCreator(), previewImagePath, new EditorImageLoader()).getFrame(0).getUri();
        }

        return previewImagePath;
    }

    public string getBubbleBorderColor()
    {
        return npc.getBubbleBorderColor();
    }

    public string getBubbleBkgColor()
    {
        return npc.getBubbleBkgColor();
    }

    public bool getShowsSpeechBubbles()
    {

        return npc.getShowsSpeechBubbles();
    }

    public void setShowsSpeechBubbles(bool showsSpeechBubbles)
    {

        controller.addTool(new ChangeBooleanValueTool(npc, showsSpeechBubbles, "getShowsSpeechBubbles", "setShowsSpeechBubbles"));
    }

    
    public override List<Searchable> getPathToDataControl(Searchable dataControl)
    {

        List<Searchable> path = getPathFromChild(dataControl, resourcesDataControlList.Cast<Searchable>().ToList());
        if (path != null)
            return path;
        path = getPathFromChild(dataControl, descriptionController);
        if (path != null)
            return path;
        return getPathFromChild(dataControl, actionsListDataControl);
    }


    public DescriptionsController getDescriptionController()
    {

        return descriptionController;
    }
}
