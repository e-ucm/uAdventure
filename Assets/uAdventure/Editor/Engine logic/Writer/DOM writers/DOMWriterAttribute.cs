using UnityEngine;
using System.Collections;
using System;

namespace uAdventure.Editor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DOMWriterAttribute : Attribute
    {
        public DOMWriterAttribute(params Type[] types)
        {
            this.Types = types;
        }

        public Type[] Types { get; private set; }
    }
}