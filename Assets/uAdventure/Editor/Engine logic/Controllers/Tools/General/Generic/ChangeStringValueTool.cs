using System.Collections;
using System.Reflection;
using System.Security;
using Object = UnityEngine.Object;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ChangeStringValueTool : Tool
    {

        protected MethodInfo get;

        protected MethodInfo set;

        protected string getName;

        protected string setName;

        protected string oldValue;

        protected string newValue;

        protected System.Object data;

        protected bool updateTree;

        protected bool updatePanel;

        /**
         * Default constructor. Will update panel but not tree
         * 
         * @param data
         *            The object which data is to be modified
         * @param newValue
         *            The new Value (string)
         * @param getMethodName
         *            The name of the get method. Must follow this pattern: public
         *            string getMethodName()
         * @param setMethodName
         *            The name of the set method. Must follow this pattern: public *
         *            setMethodName( string )
         */

        public ChangeStringValueTool(System.Object data, string newValue, string getMethodName, string setMethodName)
            : this(data, newValue, getMethodName, setMethodName, false, true)
        {
        }

        public ChangeStringValueTool(System.Object data, string newValue, string getMethodName, string setMethodName,
            bool updateTree, bool updatePanel)
        {
            this.data = data;
            this.newValue = newValue;
            this.updatePanel = updatePanel;
            this.updateTree = updateTree;

            set = data.GetType().GetMethod(setMethodName);
            get = data.GetType().GetMethod(getMethodName);
            this.getName = getMethodName;
            this.setName = setMethodName;
            if (get.ReturnType != typeof(string))
            {
                get = set = null;
                getName = setName = null;
                //ReportDialog.GenerateErrorReport(new Exception("Get method must return bool value"), false,
                //    Language.GetText("Error.Title"));
            }

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

            if (other is ChangeStringValueTool)
            {
                ChangeStringValueTool cnt = (ChangeStringValueTool)other;
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
            if (newValue != null && oldValue == null || newValue == null && oldValue != null ||
                (newValue != null && oldValue != null && !oldValue.Equals(newValue)))
            {
                // Get the old value
                //try
                //{
                oldValue = (string)get.Invoke(data, null);
                if (newValue != null && oldValue == null || newValue == null && oldValue != null ||
                    (newValue != null && oldValue != null && !oldValue.Equals(newValue)))
                {
                    set.Invoke(data, new object[] { newValue });
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
            set.Invoke(data, new object[] { newValue });
            if (updateTree)
                Controller.Instance.updateStructure();
            if (updatePanel)
                Controller.Instance.updatePanel();
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
            set.Invoke(data, new object[] { oldValue });
            if (updateTree)
                Controller.Instance.updateStructure();
            if (updatePanel)
                Controller.Instance.updatePanel();
            done = true;
            //}
            //catch (Exception e)
            //{
            //    ReportDialog.GenerateErrorReport(e, false, Language.GetText("Error.Title"));
            //}
            return done;
        }
    }
}