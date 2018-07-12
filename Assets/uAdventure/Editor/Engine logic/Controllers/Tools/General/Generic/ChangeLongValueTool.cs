using UnityEngine;
using System.Collections;
using System.Reflection;


using uAdventure.Core;

namespace uAdventure.Editor
{
    /**
     * Generic tool that uses introspection to change an integer value
     */
    public class ChangeLongValueTool : ChangeValueTool<object, long>
    {
        public ChangeLongValueTool(object data, long newValue, string propertyName) :
            base(data, newValue, propertyName, false, true)
        {
        }

        public ChangeLongValueTool(object data, long newValue, string propertyName,
            bool updateTree, bool updatePanel) :
            base(data, newValue, propertyName, updateTree, updatePanel)
        {
        }

        public ChangeLongValueTool(object data, long newValue, string getMethodName, string setMethodName) :
            base(data, newValue, getMethodName, setMethodName, false, true)
        {
        }

        public ChangeLongValueTool(object data, long newValue, string getMethodName, string setMethodName,
            bool updateTree, bool updatePanel):
            base(data, newValue, getMethodName, setMethodName, updateTree, updatePanel)
        {
        }
    }
}