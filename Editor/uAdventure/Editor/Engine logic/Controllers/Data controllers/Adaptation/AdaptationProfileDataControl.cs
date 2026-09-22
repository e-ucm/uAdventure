using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class AdaptationProfileDataControl //: DataControl
    {

        ///**
        //* Data control for each rule
        //*/
        //private List<AdaptationRuleDataControl> dataControls;

        ///**
        // * The profile
        // */
        //private AdaptationProfile profile;

        ////TODO PANEL

        //private int number;

        //public AdaptationProfileDataControl(List<AdaptationRule> adpRules, AdaptedState initialState, string name):this(new AdaptationProfile(adpRules, initialState, name, false, false))
        //{}

        //public AdaptationProfileDataControl(AdaptationProfile profile)
        //{

        //    number = 0;
        //    dataControls = new List<AdaptationRuleDataControl>();
        //    if (profile == null)
        //        profile = new AdaptationProfile();
        //    else
        //        this.profile = profile;

        //    if (profile != null && profile.getRules() != null)
        //        foreach (AdaptationRule rule in profile.getRules())
        //        {
        //            rule.setId(generateId());
        //            dataControls.Add(new AdaptationRuleDataControl(rule, this.profile));
        //        }
        //}

        //private string generateId()
        //{

        //    number++;
        //    return "#" + number;
        //}

        //public override bool addElement(int type, string id)
        //{

        //    bool added = false;

        //    if (type == Controller.ADAPTATION_RULE)
        //    {
        //        //if (id == null)
        //        //    id = controller.showInputDialog(Language.GetText("Operation.AddAdaptationRuleTitle"), Language.GetText("Operation.AddAdaptationRuleMessage"), Language.GetText("Operation.AddAdaptationRuleDefaultValue"));

        //        // If some value was typed and the identifier is valid
        //        if (id != null && controller.isElementIdValid(id))
        //        {
        //            // Add thew new adp rule
        //            AdaptationRule adpRule = new AdaptationRule();
        //            adpRule.setId(id);
        //            profile.addRule(adpRule);
        //            dataControls.Add(new AdaptationRuleDataControl(adpRule, profile));
        //            controller.getIdentifierSummary().addAdaptationRuleId(id, profile.getName());
        //            controller.DataModified();
        //            added = true;
        //        }
        //    }
        //    return added;
        //}

        //public DataControl getLastDatacontrol()
        //{
        //    return dataControls[dataControls.Count - 1];
        //}

        //public bool addElement(int type, string adpRuleId, AdaptationRule adpRule)
        //{

        //    bool added = false;

        //    if (type == Controller.ADAPTATION_RULE)
        //    {
        //        // If some value was typed and the identifier is valid
        //        if (adpRuleId != null && controller.isElementIdValid(adpRuleId))
        //        {
        //            // Add thew new adp rule 
        //            profile.addRule(adpRule);
        //            dataControls.Add(new AdaptationRuleDataControl(adpRule, profile));
        //            controller.getIdentifierSummary().addAdaptationRuleId(adpRuleId, profile.getName());
        //            controller.DataModified();
        //            added = true;
        //        }
        //    }
        //    return added;
        //}

        //public override bool canAddElement(int type)
        //{

        //    return type == Controller.ADAPTATION_RULE;
        //}


        //public override bool canBeDeleted()
        //{

        //    return true;
        //}


        //public override bool canBeMoved()
        //{

        //    return true;
        //}


        //public override bool canBeRenamed()
        //{

        //    return true;
        //}


        //public override int countAssetReferences(string assetPath)
        //{

        //    return 0;
        //}


        //public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        //{

        //    // Do nothing
        //}


        //public override int countIdentifierReferences(string id)
        //{

        //    int count = 0;

        //    foreach (AdaptationRuleDataControl rule in dataControls)
        //    {
        //        count += rule.countIdentifierReferences(id);
        //    }

        //    if (profile != null && profile.getAdaptedState() != null && profile.getAdaptedState().getTargetId() != null &&
        //            profile.getAdaptedState().getTargetId().Equals(id))
        //        count++;

        //    return count;
        //}


        //public override void deleteAssetReferences(string assetPath)
        //{

        //}


        //public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        //{

        //    bool deleted = false;

        //    string adpRuleId = ((AdaptationRuleDataControl)dataControl).getId();
        //    //  string references = string.valueOf( controller.countIdentifierReferences( adpRuleId ) );

        //    // Ask for confirmation
        //    //    if( !askConfirmation || controller.showStrictConfirmDialog( Language.GetText( "Operation.DeleteElementTitle" ), Language.GetText( "Operation.DeleteElementWarning", new string[] { adpRuleId, references } ) ) ) {
        //    if (profile.getRules().Remove(dataControl.getContent()))
        //    {
        //        dataControls.Remove(dataControl);
        //        controller.deleteIdentifierReferences(adpRuleId);
        //        controller.getIdentifierSummary().deleteAdaptationRuleId(adpRuleId, profile.getName());
        //        controller.DataModified();
        //        deleted = true;
        //    }
        //    //  }

        //    return deleted;
        //}


        //public override void deleteIdentifierReferences(string id)
        //{

        //    // profiles identifiers are deleted in adaptationProfilesDataControl
        //    Iterator<AdaptationRuleDataControl> itera = this.dataControls.iterator();

        //    while (itera.hasNext())
        //    {
        //        itera.next().deleteIdentifierReferences(id);
        //        // the rule ID are unique, do not look in rule's IDs
        //    }

        //    if (profile != null && profile.getAdaptedState() != null && profile.getAdaptedState().getTargetId() != null &&
        //            profile.getAdaptedState().getTargetId().Equals(id))
        //    {
        //        profile.getAdaptedState().setTargetId(null);
        //    }

        //}


        //public override int[] getAddableElements()
        //{

        //    return new int[] { Controller.ADAPTATION_RULE };
        //}


        //public override System.Object getContent()
        //{

        //    return profile;
        //}

        //public List<AdaptationRuleDataControl> getAdaptationRules()
        //{

        //    return this.dataControls;
        //}


        //public override bool isValid(string currentPath, List<string> incidences)
        //{

        //    return true;
        //}

        //public bool moveElementDown(AdaptationRuleDataControl dataControl)
        //{

        //    return controller.addTool(new MoveRuleTool(this, dataControl, MoveRuleTool.MODE_DOWN));
        //}

        //public bool moveElementUp(AdaptationRuleDataControl dataControl)
        //{

        //    return controller.addTool(new MoveRuleTool(this, dataControl, MoveRuleTool.MODE_UP));
        //}


        //public override bool moveElementDown(DataControl dataControl)
        //{

        //    bool elementMoved = false;
        //    int elementIndex = profile.getRules().IndexOf(dataControl.getContent());

        //    if (elementIndex < profile.getRules().Count - 1)
        //    {
        //        profile.getRules().Add(elementIndex + 1, profile.getRules().RemoveAt(elementIndex));
        //        dataControls.Add(elementIndex + 1, dataControls.RemoveAt(elementIndex));
        //        controller.DataModified();
        //        elementMoved = true;
        //    }

        //    return elementMoved;
        //}


        //public override bool moveElementUp(DataControl dataControl)
        //{

        //    bool elementMoved = false;
        //    int elementIndex = profile.getRules().indexOf(dataControl.getContent());

        //    if (elementIndex > 0)
        //    {
        //        profile.getRules().add(elementIndex - 1, profile.getRules().remove(elementIndex));
        //        dataControls.add(elementIndex - 1, dataControls.remove(elementIndex));
        //        controller.DataModified();
        //        elementMoved = true;
        //    }

        //    return elementMoved;
        //}

        //public string getFileName()
        //{

        //    return profile.getName().Substring(Mathf.Max(profile.getName().LastIndexOf("/"), profile.getName().LastIndexOf("\\")) + 1);
        //}


        //public override string renameElement(string name)
        //{

        //    bool renamed = false;
        //    string oldName = null;
        //    if (this.profile.getName() != null)
        //    {
        //        oldName = this.profile.getName();
        //    }

        //    // Show confirmation dialog.
        //    if (name != null )//|| controller.showStrictConfirmDialog(Language.GetText("Operation.RenameAssessmentFile"), Language.GetText("Operation.RenameAssessmentFile.Message")))
        //    {

        //        //Prompt for file name:
        //        string fileName = name;
        //        //if (name == null || name.Equals(""))
        //        //    fileName = controller.showInputDialog(Language.GetText("Operation.RenameAssessmentFile.FileName"), Language.GetText("Operation.RenameAssessmentFile.FileName.Message"), getFileName());

        //        if (fileName != null && !fileName.Equals(oldName) && controller.isElementIdValid(fileName))
        //        {
        //            if (!controller.getIdentifierSummary().isAdaptationProfileId(name))
        //            {
        //                controller.DataModified();
        //                profile.setName(fileName);
        //                controller.getIdentifierSummary().renameAdaptationProfile(oldName, fileName);

        //                renamed = true;
        //            }
        //            //else {
        //            //    controller.showErrorDialog(Language.GetText("Operation.CreateAdaptationFile.FileName.ExistValue.Title"), Language.GetText("Operation.CreateAdaptationFile.FileName.ExistValue.Message"));
        //            //}
        //        }

        //    }

        //    if (renamed)
        //        return oldName;

        //    return null;
        //}


        //public override void replaceIdentifierReferences(string oldId, string newId)
        //{


        //    foreach (AdaptationRuleDataControl rule in dataControls)
        //    {
        //        if (rule.getId().Equals(oldId))
        //        {
        //            rule.renameElement(newId);
        //        }
        //        rule.replaceIdentifierReferences(oldId, newId);
        //    }

        //    if (profile != null && profile.getAdaptedState() != null && profile.getAdaptedState().getTargetId() != null)
        //    {
        //        if (profile.getAdaptedState().getTargetId().Equals(oldId))
        //            profile.getAdaptedState().setTargetId(newId);
        //    }
        //}


        //public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        //{

        //    foreach (AdaptationRuleDataControl dataControl in dataControls)
        //        dataControl.updateVarFlagSummary(varFlagSummary);

        //    //Update the initial state
        //    if (profile != null && profile.getAdaptedState() != null)
        //    {
        //        foreach (string flag in profile.getAdaptedState().getFlagsVars())
        //        {
        //            if (profile.getAdaptedState().isFlag(flag))
        //                varFlagSummary.addFlagReference(flag);
        //            else
        //                varFlagSummary.addVarReference(flag);
        //        }
        //    }
        //}


        //public override bool duplicateElement(DataControl dataControl)
        //{

        //    if (!(dataControl is AdaptationRuleDataControl ) )
        //        return false;

        //    //try
        //    //{
        //        // Auto generate the rule id
        //        string adpRuleId = generateId();
        //        AdaptationRule newRule = (AdaptationRule)(((AdaptationRule)(dataControl.getContent())));
        //        newRule.setId(adpRuleId);
        //        dataControls.Add(new AdaptationRuleDataControl(newRule, profile));
        //        profile.addRule(newRule);
        //        controller.getIdentifierSummary().addAdaptationRuleId(newRule.getId(), profile.getName());
        //        return true;
        //    //}
        //    //catch (CloneNotSupportedException e)
        //    //{
        //    //    ReportDialog.GenerateErrorReport(e, true, "Could not clone adaptation rule");
        //    //    return false;
        //    //}
        //}

        ///**
        // * @return the profile.getInitialState()
        // */
        //public AdaptedState getInitialState()
        //{

        //    return profile.getAdaptedState();
        //}

        //public void setInitialScene(string initScene)
        //{

        //    if (profile.getAdaptedState() == null)
        //        profile.setAdaptedState(new AdaptedState());
        //    controller.addTool(new ChangeTargetIdTool(profile.getAdaptedState(), initScene));
        //}

        //public string getInitialScene()
        //{

        //    if (profile.getAdaptedState() == null)
        //        return null;
        //    else
        //        return profile.getAdaptedState().getTargetId();
        //}

        //public bool addFlagAction(int selectedRow)
        //{

        //    return controller.addTool(new AddActionTool(profile.getAdaptedState(), selectedRow));
        //}

        //public void deleteFlagAction(int selectedRow)
        //{

        //    controller.addTool(new DeleteActionTool(profile, selectedRow));
        //}

        //public int getFlagActionCount()
        //{

        //    return profile.getAdaptedState().getFlagsVars().size();
        //}

        //public void setFlag(int rowIndex, string flag)
        //{

        //    controller.addTool(new ChangeActionTool(profile, rowIndex, flag, ChangeActionTool.SET_ID));
        //}

        //public void setAction(int rowIndex, string string )
        //{

        //    controller.addTool(new ChangeActionTool(profile, rowIndex, string, ChangeActionTool.SET_VALUE));
        //}

        //public string getFlag(int rowIndex)
        //{

        //    return profile.getAdaptedState().getFlagVar(rowIndex);
        //}

        //public string getAction(int rowIndex)
        //{

        //    return profile.getAdaptedState().getAction(rowIndex);
        //}

        //public int getValueToSet(int rowIndex)
        //{

        //    if (profile.getAdaptedState().getValueToSet(rowIndex) == int.MIN_VALUE)
        //        return 0;
        //    else
        //        return profile.getAdaptedState().getValueToSet(rowIndex);

        //}

        //public string[][] getAdaptationRulesInfo()
        //{

        //    string[][] info = new string[profile.getRules().Count][4];

        //    for (int i = 0; i < profile.getRules().Count; i++)
        //    {
        //        info[i][0] = profile.getRules()[i].getId();
        //        info[i][1] = profile.getRules()[i].getUOLProperties().Count.ToString();
        //        if (profile.getRules()[i].getAdaptedState().getTargetId() == null)
        //            info[i][2] = "<Not selected>";
        //        else
        //            info[i][2] = profile.getRules()[i].getAdaptedState().getTargetId();
        //        info[i][3] = profile.getRules()[i].getAdaptedState().getFlagsVars().Count.ToString();
        //    }
        //    return info;
        //}

        ///**
        // * @return the profile.getName()
        // */
        //public string getName()
        //{

        //    return profile.getName();
        //}


        //public override bool canBeDuplicated()
        //{

        //    return true;
        //}


        //public override void recursiveSearch()
        //{

        //    foreach (DataControl dc in this.dataControls)
        //    {
        //        dc.recursiveSearch();
        //    }
        //    //check("" + number, Language.GetText("Search.Number"));
        //    //check(getFileName(), Language.GetText("Search.Name"));
        //    //check(getInitialScene(), Language.GetText("Search.InitialScene"));
        //    //for (int i = 0; i < this.getFlagActionCount(); i++)
        //    //{
        //    //    if (isFlag(i))
        //    //        check(getFlag(i), Language.GetText("Search.Flag"));
        //    //    else
        //    //        check(getFlag(i), Language.GetText("Search.Var"));

        //    //    check(getAction(i), Language.GetText("Search.ActionOverGameState"));
        //    //}
        //    //check(getName(), TextConstants.getText("Search.Path"));
        //}

        //public void change(int rowIndex, string name)
        //{

        //    //profile.getAdaptedState().change(rowIndex, name);

        //    controller.addTool(new ChangeVarFlagTool(profile.getAdaptedState(), rowIndex, name));
        //}

        //public bool isFlag(int rowIndex)
        //{

        //    return this.profile.getAdaptedState().isFlag(rowIndex);
        //}

        ////public bool isScorm2004()
        ////{

        ////    return profile.isScorm2004();
        ////}

        ////public bool isScorm12()
        ////{

        ////    return profile.isScorm12();
        ////}

        ////public void changeToScorm2004Profile()
        ////{

        ////    controller.addTool(new ChangeAdaptationProfileTypeTool(profile, ChangeAdaptationProfileTypeTool.SCORM2004, profile.isScorm12(), profile.isScorm2004()));
        ////}

        ////public void changeToScorm12Profile()
        ////{

        ////    controller.addTool(new ChangeAdaptationProfileTypeTool(profile, ChangeAdaptationProfileTypeTool.SCORM12, profile.isScorm12(), profile.isScorm2004()));
        ////}

        ////public void changeToNormalProfile()
        ////{

        ////    controller.addTool(new ChangeAdaptationProfileTypeTool(profile, ChangeAdaptationProfileTypeTool.NORMAL, profile.isScorm12(), profile.isScorm2004()));
        ////}


        //public override List<Searchable> getPathToDataControl(Searchable dataControl)
        //{

        //    return getPathFromChild(dataControl, dataControls);
        //}

        ///**
        // * @return the dataControls
        // */
        //public List<AdaptationRuleDataControl> getDataControls()
        //{

        //    return dataControls;
        //}

        ///**
        // * @param dataControls
        // *            the dataControls to set
        // */
        //public void setDataControlsAndData(List<AdaptationRuleDataControl> dataControls)
        //{

        //    this.dataControls = dataControls;
        //    List<AdaptationRule> rules = new List<AdaptationRule>();
        //    foreach (AdaptationRuleDataControl dataControl in dataControls)
        //        rules.Add((AdaptationRule)dataControl.getContent());

        //    this.profile.setRules(rules);

        //}
    }
}