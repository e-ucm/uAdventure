using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class AdaptationRuleDataControl //: DataControl
    {
        //private AdaptationRule adaptationRule;

        //private AdaptationProfile profile;

        //public AdaptationRuleDataControl(AdaptationRule adpRule, AdaptationProfile profile)
        //{

        //    this.adaptationRule = adpRule;
        //    this.profile = profile;
        //}


        //public override bool addElement(int type, string id)
        //{

        //    return false;
        //}


        //public override bool canAddElement(int type)
        //{

        //    return false;
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

        //    return false;
        //}


        //public override int countAssetReferences(string assetPath)
        //{

        //    return 0;
        //}


        //public override int countIdentifierReferences(string id)
        //{


        //    if (adaptationRule.getId().Equals(id))
        //    {
        //        return 1;
        //    }
        //    else if (adaptationRule.getAdaptedState() != null && adaptationRule.getAdaptedState().getTargetId() != null &&
        //            adaptationRule.getAdaptedState().getTargetId().Equals(id))
        //    {
        //        return 1;
        //    }
        //    else {
        //        return 0;
        //    }


        //}


        //public override void deleteAssetReferences(string assetPath)
        //{

        //}


        //public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        //{

        //    return false;
        //}


        //public override void deleteIdentifierReferences(string id)
        //{

        //    if (adaptationRule.getAdaptedState() != null && adaptationRule.getAdaptedState().getTargetId() != null &&
        //            adaptationRule.getAdaptedState().getTargetId().Equals(id))
        //        adaptationRule.getAdaptedState().setTargetId(null);
        //}


        //public override int[] getAddableElements()
        //{

        //    return new int[0];
        //}


        //public override System.Object getContent()
        //{

        //    return adaptationRule;
        //}


        //public override bool isValid(string currentPath, List<string> incidences)
        //{

        //    return true;
        //}


        //public override bool moveElementDown(DataControl dataControl)
        //{

        //    return false;
        //}


        //public override bool moveElementUp(DataControl dataControl)
        //{

        //    return false;
        //}


        //public override string renameElement(string name)
        //{

        //    return null;
        //}


        //public override void replaceIdentifierReferences(string oldId, string newId)
        //{

        //    if (adaptationRule.getAdaptedState() != null && adaptationRule.getAdaptedState().getTargetId() != null &&
        //            adaptationRule.getAdaptedState().getTargetId().Equals(oldId))
        //        adaptationRule.getAdaptedState().setTargetId(newId);
        //}


        //public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        //{

        //    foreach (string flag in adaptationRule.getAdaptedState().getFlagsVars())
        //    {
        //        if (isFlag(flag))
        //            varFlagSummary.addFlagReference(flag);
        //        else
        //            varFlagSummary.addVarReference(flag);
        //    }

        //}

        //public string getDescription()
        //{

        //    return adaptationRule.getDescription();
        //}

        //public void setInitialScene(string initScene)
        //{

        //    controller.addTool(new ChangeTargetIdTool(adaptationRule.getAdaptedState(), initScene));
        //    //adaptationRule.getAdaptedState( ).setTargetId( initScene );
        //}

        //public string getInitialScene()
        //{

        //    return adaptationRule.getAdaptedState().getTargetId();
        //}

        //private AdaptedState getGameState()
        //{

        //    return adaptationRule.getAdaptedState();
        //}

        //public bool moveUOLPropertyUp(int selectedRow)
        //{

        //    return controller.addTool(new MoveObjectTool(adaptationRule.getUOLProperties(), selectedRow, MoveObjectTool.MODE_UP));
        //}

        //public bool moveUOLPropertyDown(int selectedRow)
        //{

        //    return controller.addTool(new MoveObjectTool(adaptationRule.getUOLProperties(), selectedRow, MoveObjectTool.MODE_DOWN));
        //}

        //public bool addFlagAction(int selectedRow)
        //{

        //    return controller.addTool(new AddActionTool(adaptationRule.getAdaptedState(), selectedRow));
        //}

        //public void deleteFlagAction(int selectedRow)
        //{

        //    controller.addTool(new DeleteActionTool(adaptationRule, selectedRow));
        //}

        //public int getFlagActionCount()
        //{

        //    return adaptationRule.getAdaptedState().getFlagsVars().Count;
        //}

        //public void setFlag(int rowIndex, string flag)
        //{

        //    controller.addTool(new ChangeActionTool(adaptationRule, rowIndex, flag, ChangeActionTool.SET_ID));
        //}

        //public void change(int rowIndex, string name)
        //{

        //    //profile.getAdaptedState().change(rowIndex, name);
        //    controller.addTool(new ChangeVarFlagTool(adaptationRule.getAdaptedState(), rowIndex, name));
        //}

        //public string getFlag(int rowIndex)
        //{

        //    return this.adaptationRule.getAdaptedState().getFlagVar(rowIndex);
        //}

        //public string getAction(int rowIndex)
        //{

        //    return this.adaptationRule.getAdaptedState().getAction(rowIndex);
        //}

        //public bool isFlag(int rowIndex)
        //{

        //    return this.adaptationRule.getAdaptedState().isFlag(rowIndex);
        //}

        //public bool isFlag(string name)
        //{

        //    return this.adaptationRule.getAdaptedState().isFlag(name);
        //}

        //public string getId()
        //{

        //    return adaptationRule.getId();
        //}

        //public void addBlankUOLProperty(int selectedRow)
        //{

        //    //controller.addTool(new AddUOLPropertyTool(adaptationRule, selectedRow));
        //}

        //public void deleteUOLProperty(int selectedRow)
        //{

        //   // controller.addTool(new DeleteUOLPropertyTool(adaptationRule, selectedRow));
        //}

        //public int getUOLPropertyCount()
        //{

        //    return adaptationRule.getUOLProperties().Count;
        //}

        //public void setUOLPropertyValue(int rowIndex, string s )
        //{

        //   // controller.addTool(new ChangeUOLPropertyTool(adaptationRule, s, rowIndex, ChangeUOLPropertyTool.SET_VALUE));
        //}

        //public void setUOLPropertyId(int rowIndex, string s )
        //{
        //    //if (SCORMConfigData.isArrayAttribute(string))
        //    //{
        //    //    //check if "string" has a previous value of the same kind of selected attribute
        //    //    if (adaptationRule.getUOLProperties().get(rowIndex).getId().startsWith(string))
        //    //        string = adaptationRule.getUOLProperties().get(rowIndex).getId();
        //    //    string = SCORMAttributeDialog.showAttributeDialogForRead(getProfileType(), string);
        //    //}

        //    //if (!SCORMConfigData.isArrayAttribute(string))
        //    //    controller.addTool(new ChangeUOLPropertyTool(adaptationRule, string, rowIndex, ChangeUOLPropertyTool.SET_ID));
        //}

        //public void setUOLPropertyOp(int rowIndex, string s )
        //{

        //    //controller.addTool(new ChangeUOLPropertyTool(adaptationRule, s, rowIndex, ChangeUOLPropertyTool.SET_OP));
        //}

        //public string getUOLPropertyId(int rowIndex)
        //{

        //    return this.adaptationRule.getUOLProperties()[rowIndex].getId();
        //}

        //public string getUOLPropertyValue(int rowIndex)
        //{

        //    return adaptationRule.getUOLProperties()[rowIndex].getValue();
        //}

        //public string getUOLPropertyOp(int rowIndex)
        //{

        //    return adaptationRule.getUOLProperties()[rowIndex].getOperation();
        //}

        //public void setAction(int rowIndex, string s )
        //{

        //    controller.addTool(new ChangeActionTool(adaptationRule, rowIndex, s, ChangeActionTool.SET_VALUE));
        //}

        //public int getValueToSet(int rowIndex)
        //{

        //    if (adaptationRule.getAdaptedState().getValueToSet(rowIndex) == int.MinValue)
        //        return 0;
        //    else
        //        return adaptationRule.getAdaptedState().getValueToSet(rowIndex);

        //}


        //public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        //{

        //    // Do nothing
        //}


        //public override bool canBeDuplicated()
        //{

        //    return true;
        //}


        //public override void recursiveSearch()
        //{

        //    //check(getDescription(), Language.GetText("Search.Description"));
        //    //check(getId(), "ID");
        //    //check(getInitialScene(), Language.GetText("Search.InitialScene"));

        //    //for (int i = 0; i < this.getFlagActionCount(); i++)
        //    //{
        //    //    if (isFlag(i))
        //    //        check(getFlag(i), Language.GetText("Search.Flag"));
        //    //    else
        //    //        check(getFlag(i), Language.GetText("Search.Var"));

        //    //    check(getAction(i), Language.GetText("Search.ActionOverGameState"));
        //    //}
        //    //for (int i = 0; i < this.getUOLPropertyCount(); i++)
        //    //{
        //    //    check(this.getUOLPropertyId(i), Language.GetText("Search.LMSPropertyID"));
        //    //    check(this.getUOLPropertyValue(i), Language.GetText("Search.LMSPropertyValue"));
        //    //}
        //}


        //public int getProfileType()
        //{
        //    //if (profile.isScorm12())
        //    //    return SCORMConfigData.SCORM_V12;
        //    //else if (profile.isScorm2004())
        //    //    return SCORMConfigData.SCORM_2004;
        //    //else
        //        return -1;

        //}

        //public override List<Searchable> getPathToDataControl(Searchable dataControl)
        //{

        //    return null;
        //}
    }
}