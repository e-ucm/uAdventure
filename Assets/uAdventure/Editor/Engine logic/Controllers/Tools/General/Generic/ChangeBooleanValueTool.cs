using System;
using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Security;

using uAdventure.Core;

namespace uAdventure.Editor
{

    public class ChangeBooleanValueTool : ChangeValueTool<object, bool>
    {
        public ChangeBooleanValueTool(object data, bool newValue, string propertyName) :
            base(data, newValue, propertyName, false, true)
        {
        }

        public ChangeBooleanValueTool(object data, bool newValue, string propertyName,
            bool updateTree, bool updatePanel) :
            base(data, newValue, propertyName, updateTree, updatePanel)
        {
        }

        public ChangeBooleanValueTool(object data, bool newValue, string getMethodName, string setMethodName) :
            base(data, newValue, getMethodName, setMethodName, false, true)
        {
        }

        public ChangeBooleanValueTool(object data, bool newValue, string getMethodName, string setMethodName,
            bool updateTree, bool updatePanel):
            base(data, newValue, getMethodName, setMethodName, updateTree, updatePanel)
        {
        }
    }
}