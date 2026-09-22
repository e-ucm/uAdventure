using System;
using System.Linq;

namespace uAdventure.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GroupableTypeAttribute : Attribute
    {
        public Type GetTaggedType(Type type)
        {
            var parent = type;
            do
            {
                if(parent.GetCustomAttributes(typeof(GroupableTypeAttribute), false).Any(this.Equals))
                {
                    return parent;
                }

                parent = type.BaseType;
            } while (parent != null);

            return null;
        }

        public static Type GetGroupType(Type t)
        {
            var groupableTypeAttribute = t.GetCustomAttributes(typeof(GroupableTypeAttribute), true).LastOrDefault() as GroupableTypeAttribute;

            if (groupableTypeAttribute != null)
            {
                return groupableTypeAttribute.GetTaggedType(t);
            }

            return t;
        }
    }
}

