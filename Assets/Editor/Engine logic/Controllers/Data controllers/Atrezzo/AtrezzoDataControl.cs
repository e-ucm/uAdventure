using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AtrezzoDataControl : DataControlWithResources
{
    /**
         * Contained atrezzo item.
         */
    private Atrezzo atrezzo;

    /**
     * Constructor.
     * 
     * @param atrezzo
     *            Contained atrezzo item
     */
    public AtrezzoDataControl(Atrezzo atrezzo)
    {

        this.atrezzo = atrezzo;
        this.resourcesList = atrezzo.getResources();

        selectedResources = 0;

        // Add a new resource if the list is empty
        if (resourcesList.Count == 0)
            resourcesList.Add(new ResourcesUni());

        // Create the subcontrollers
        resourcesDataControlList = new List<ResourcesDataControl>();
        foreach (ResourcesUni resources in resourcesList)
            resourcesDataControlList.Add(new ResourcesDataControl(resources, Controller.ATREZZO));

    }

    /**
     * Returns the path to the selected preview image.
     * 
     * @return Path to the image, null if not present
     */
    public string getPreviewImage()
    {

        return resourcesDataControlList[selectedResources].getAssetPath("image");
    }

    public void setImage(string path)
    {
        resourcesDataControlList[selectedResources].addAsset("image", path);
    }

    /**
     * Returns the id of the atrezzo item.
     * 
     * @return Atrezzo's id
     */
    public string getId()
    {

        return atrezzo.getId();
    }

    /**
     * Returns the documentation of the atrezzo item.
     * 
     * @return Atrezzo's documentation
     */
    public string getDocumentation()
    {

        return atrezzo.getDocumentation();
    }

    
    public override System.Object getContent()
    {

        return atrezzo;
    }

    
    public override int[] getAddableElements()
    {

        //return new int[] { Controller.RESOURCES };
        return new int[] { };
    }

    
    public override bool canAddElement(int type)
    {

        // It can always add new resources
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
            elementAdded = Controller.getInstance().addTool(new AddResourcesBlockTool(resourcesList, resourcesDataControlList, Controller.ATREZZO, this));
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
        string oldAtrezzoId = atrezzo.getId();
        string references = controller.countIdentifierReferences(oldAtrezzoId).ToString();

        // Ask for confirmation 
        if (name != null || controller.showStrictConfirmDialog(TC.get("Operation.RenameAtrezzoTitle"), TC.get("Operation.RenameElementWarning", new string[] { oldAtrezzoId, references })))
        {

            // Show a dialog asking for the new atrezzo item id
            string newAtrezzoId = name;
            if (name == null)
                newAtrezzoId = controller.showInputDialog(TC.get("Operation.RenameAtrezzoTitle"), TC.get("Operation.RenameAtrezzoMessage"), oldAtrezzoId);

            // If some value was typed and the identifiers are different
            if (newAtrezzoId != null && !newAtrezzoId.Equals(oldAtrezzoId) && controller.isElementIdValid(newAtrezzoId))
            {
                atrezzo.setId(newAtrezzoId);
                controller.replaceIdentifierReferences(oldAtrezzoId, newAtrezzoId);
                controller.getIdentifierSummary().deleteAtrezzoId(oldAtrezzoId);
                controller.getIdentifierSummary().addAtrezzoId(newAtrezzoId);
                //controller.dataModified( );
                elementRenamed = true;
            }
        }

        if (elementRenamed)
            return oldAtrezzoId;
        else
            return null;
    }

    
    public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
    {

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

        return valid;
    }

    
    public override int countAssetReferences(string assetPath)
    {

        int count = 0;

        // Iterate through the resources
        foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
            count += resourcesDataControl.countAssetReferences(assetPath);

        return count;
    }

    
    public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
    {

        // Iterate through the resources
        foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
            resourcesDataControl.getAssetReferences(assetPaths, assetTypes);

    }

    
    public override void deleteAssetReferences(string assetPath)
    {

        // Iterate through the resources
        foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
            resourcesDataControl.deleteAssetReferences(assetPath);
    }

    
    public override int countIdentifierReferences(string id)
    {

        int count = 0;
        // Iterate through the resources
        foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
            count += resourcesDataControl.countIdentifierReferences(id);
        return count;
    }

    
    public override void replaceIdentifierReferences(string oldId, string newId)
    {

        // Iterate through the resources
        foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
            resourcesDataControl.replaceIdentifierReferences(oldId, newId);
    }

    
    public override void deleteIdentifierReferences(string id)
    {

        // Iterate through the resources
        foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
            resourcesDataControl.deleteIdentifierReferences(id);
    }

    
    public override bool canBeDuplicated()
    {

        return true;
    }

    
    public override void recursiveSearch()
    {

        // atrezzo only has name
        // check( this.getBriefDescription( ), TC.get( "Search.BriefDescription" ) );
        // check( this.getDetailedDescription( ), TC.get( "Search.DetailedDescription" ) );
        check(this.getDocumentation(), TC.get("Search.Documentation"));
        check(this.getId(), "ID");
        check(this.getPreviewImage(), TC.get("Search.PreviewImage"));
        foreach (ResourcesDataControl r in resourcesDataControlList)
        {
            r.recursiveSearch();
        }
    }

    
    public override List<Searchable> getPathToDataControl(Searchable dataControl)
    {

        return getPathFromChild(dataControl, resourcesDataControlList.Cast<Searchable>().ToList());
    }
}
