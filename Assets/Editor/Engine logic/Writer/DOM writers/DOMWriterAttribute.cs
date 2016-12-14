using UnityEngine;
using System.Collections;
using System;

namespace uAdventure.Editor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DOMWriterAttribute : Attribute
    {
        private Type[] types;

        public DOMWriterAttribute(params Type[] types)
        {
            this.types = types;
        }
        
        public Type[] Types
        {
            get
            {
                return types;
            }
        }
    }
}