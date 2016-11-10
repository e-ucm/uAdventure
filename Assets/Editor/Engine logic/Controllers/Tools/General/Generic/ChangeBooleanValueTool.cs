using System;
using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Security;

/**
 * Generic tool that uses introspection to change a bool value
 * 
 */

public class ChangeBooleanValueTool : Tool
{

    protected MethodInfo get;

    protected MethodInfo set;

    protected string getName;

    protected string setName;

    protected bool oldValue;

    protected bool newValue;

    protected System.Object data;

    protected bool updateTree;

    protected bool updatePanel;

    /**
     * Default constructor. Will update panel but not tree
     * 
     * @param data
     *            The object which data is to be modified
     * @param newValue
     *            The new Value (bool)
     * @param getMethodName
     *            The name of the get method. Must follow this pattern: public
     *            bool getMethodName()
     * @param setMethodName
     *            The name of the set method. Must follow this pattern: public *
     *            setMethodName( bool )
     */

    public ChangeBooleanValueTool(System.Object data, bool newValue, string getMethodName, string setMethodName) :
        this(data, newValue, getMethodName, setMethodName, false, true)
    {
    }

    public ChangeBooleanValueTool(System.Object data, bool newValue, string getMethodName, string setMethodName,
        bool updateTree, bool updatePanel)
    {

        this.data = data;
        this.newValue = newValue;
        this.updatePanel = updatePanel;
        this.updateTree = updateTree;
        //try
        //{
        set = data.GetType().GetMethod(setMethodName);
        get = data.GetType().GetMethod(getMethodName);
        this.getName = getMethodName;
        this.setName = setMethodName;
        if (get.ReturnType != typeof (bool))
        {
            get = set = null;
            getName = setName = null;
            //ReportDialog.GenerateErrorReport(new Exception("Get method must return bool value"), false,
            //    Language.GetText("Error.Title"));
        }
        //}
        //catch( SecurityException e ) {
        //    get = set = null;
        //    getName = setName = null;
        //    ReportDialog.GenerateErrorReport( e, false, Language.GetText( "Error.Title" ) );
        //}
        //catch( NoSuchMethodException e ) {
        //    get = set = null;
        //    getName = setName = null;
        //    ReportDialog.GenerateErrorReport( e, false, Language.GetText( "Error.Title" ) );
        //}

    }

    public override bool canRedo()
    {

        return true;
    }

    public override bool canUndo()
    {

        return true;
    }

    public override bool combine(Tool other)
    {

        if (other is ChangeBooleanValueTool)
        {
            ChangeBooleanValueTool cnt = (ChangeBooleanValueTool) other;
            if (cnt.getName.Equals(getName) && cnt.setName.Equals(setName) && data == cnt.data)
            {
                newValue = cnt.newValue;
                timeStamp = cnt.timeStamp;
                return true;
            }
        }
        return false;
    }

    public override bool doTool()
    {

        bool done = false;
        if (get != null && set != null)
        {
            // Get the old value
            //try
            //{
            oldValue = (bool) get.Invoke(data, null);
            if (newValue != null && oldValue == null || newValue == null && oldValue != null ||
                (newValue != null && oldValue != null && !oldValue.Equals(newValue)))
            {
                set.Invoke(data, new object[] {newValue});
                done = true;
            }
            //}
            //catch (Exception e)
            //{
            //    ReportDialog.GenerateErrorReport(e, false, Language.GetText("Error.Title"));
            //}

        }
        return done;

    }

    public override bool redoTool()
    {

        bool done = false;
        //try
        //{
        set.Invoke(data, new object[] {newValue});
        if (updateTree)
            Controller.getInstance().updateStructure();
        if (updatePanel)
            Controller.getInstance().updatePanel();
        done = true;
        //}
        //catch (Exception e)
        //{
        //    ReportDialog.GenerateErrorReport(e, false, Language.GetText("Error.Title"));
        //}
        return done;
    }

    public override bool undoTool()
    {

        bool done = false;
        //try
        //{
        set.Invoke(data, new object[] {oldValue});
        if (updateTree)
            Controller.getInstance().updateStructure();
        if (updatePanel)
            Controller.getInstance().updatePanel();
        done = true;
        //}
        //catch (Exception e)
        //{
        //    ReportDialog.GenerateErrorReport(e, false, Language.GetText("Error.Title"));
        //}
        return done;
    }
}