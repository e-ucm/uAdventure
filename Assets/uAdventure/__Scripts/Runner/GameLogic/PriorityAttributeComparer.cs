using System.Collections.Generic;

namespace uAdventure.Runner
{
    internal class PriorityAttributeComparer : IComparer<GameExtension>
    {
        private string methodName;
        public PriorityAttributeComparer(string methodName)
        {
            this.methodName = methodName;
        }

        public int Compare(GameExtension x, GameExtension y)
        {
            return GetPriority(y) - GetPriority(x);
        }

        private int GetPriority(GameExtension ex)
        {
            var methodInfo = ex.GetType().GetMethod(methodName);
            if (methodInfo != null)
            {
                var attributes = (PriorityAttribute[])methodInfo.GetCustomAttributes(typeof(PriorityAttribute), false);
                if (attributes.Length > 0)
                {
                    var priority = attributes[0];
                    return priority.Priority;
                }
            }
            
            return 0;
        }
    }
}