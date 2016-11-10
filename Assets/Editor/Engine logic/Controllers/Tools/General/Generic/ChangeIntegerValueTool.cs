using UnityEngine;
using System.Collections;
using System.Reflection;

/**
 * Generic tool that uses introspection to change an int value
 */

public class ChangeIntegerValueTool : Tool
{

    protected MethodInfo get;

    protected MethodInfo set;

    protected string getName;

    protected string setName;

    protected int oldValue;

    protected int newValue;

    protected System.Object data;

    protected bool updateTree;

    protected bool updatePanel;

    /**
     * Default constructor. Will update panel but not tree
     * 
     * @param data
     *            The System.Object which data is to be modified
     * @param newValue
     *            The new Value (int)
     * @param getMethodName
     *            The name of the get method. Must follow this pattern: public
     *            int getMethodName()
     * @param setMethodName
     *            The name of the set method. Must follow this pattern: public *
     *            setMethodName( int )
     */

    public ChangeIntegerValueTool(System.Object data, int newValue, string getMethodName, string setMethodName) :
        this(data, newValue, getMethodName, setMethodName, false, true)
    {
    }

    public ChangeIntegerValueTool(System.Object data, int newValue, string getMethodName, string setMethodName, bool updateTree,
        bool updatePanel)
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
        if (get.ReturnType != typeof (int))
        {
            get = set = null;
            getName = setName = null;
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
            oldValue = (int) get.Invoke(data, null);
            if (newValue != null && oldValue == null || newValue == null && oldValue != null ||
                (newValue != null && oldValue != null && !oldValue.Equals(newValue)))
            {
                set.Invoke(data, new System.Object[] {newValue});
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
        set.Invoke(data, new System.Object[] {newValue});
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
        set.Invoke(data, new System.Object[] {oldValue});
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