using UnityEngine;
using System.Collections;
using System;

namespace uAdventure.Editor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EditorWindowExtensionAttribute : Attribute
    {
        public int Priority { get; private set; }
        public Type[] Types { get; private set; }

        public EditorWindowExtensionAttribute(int order, params Type[] types)
        {
            Priority = order;
            Types = types;
        }

    }

}
