using System;
using System.Collections.Generic;
using System.Linq;

namespace uAdventure.Runner
{
    public class PriorityAttribute : Attribute
    {
        private int priority;

        public PriorityAttribute(int priority)
        {
            this.priority = priority;
        }

        public int Priority { get => priority; set => priority = value; }

        public static List<GameExtension> OrderExtensionsByMethod(string methodName, List<GameExtension> extension)
        {
            var listClone = extension.ToList();
            listClone.Sort(new PriorityAttributeComparer(methodName));
            return listClone;
        }
    }
}
