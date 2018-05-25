using UnityEngine;
using System.Collections;
using System;

namespace uAdventure.Runner
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ChapterTargetFactoryAttribute : Attribute
    {
        public ChapterTargetFactoryAttribute(params Type[] types)
        {
            Types = types;
        }

        public Type[] Types { get; private set; }
    }
}


