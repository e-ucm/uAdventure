using UnityEngine;
using System.Collections;
using System.Reflection;

using uAdventure.Core;
using System;
using Action = uAdventure.Core.Action;

namespace uAdventure.Editor
{
    public class ChangeValueTool<O, T> : Tool
    {
        public delegate void VoidCall();

        protected O data;
        protected Func<T> get;
        protected Action<T> set;
        protected VoidCall changed;
        protected T oldValue;
        protected T newValue;
        protected bool updateTree;
        protected bool updatePanel;

        public ChangeValueTool(O data, T newValue, string propertyName) :
            this(data, newValue, propertyName, false, true, null)
        {
        }

        public ChangeValueTool(O data, T newValue, string propertyName, VoidCall changed) :
            this(data, newValue, propertyName, false, true, changed)
        {
        }
        public ChangeValueTool(O data, T newValue, string propertyName, bool updateTree,
            bool updatePanel) : this(data, newValue, propertyName, updateTree, updatePanel, null)
        {

        }

        public ChangeValueTool(O data, T newValue, string propertyName, bool updateTree,
            bool updatePanel, VoidCall changed)

        {
            var type = data != null ? data.GetType() : typeof(O);
            var property = type.GetProperty(propertyName);

            if (property != null && typeof(T).IsAssignableFrom(property.PropertyType))
            {
                Init(data, newValue, property.GetGetMethod(), property.GetSetMethod(), updateTree, updatePanel, changed);
            }
        }

        public ChangeValueTool(O data, T newValue, string getMethodName, string setMethodName) :
            this(data, newValue, getMethodName, setMethodName, false, true, null)
        {
        }
        public ChangeValueTool(O data, T newValue, string getMethodName, string setMethodName, VoidCall changed) :
            this(data, newValue, getMethodName, setMethodName, false, true, changed)
        {
        }

        public ChangeValueTool(O data, T newValue, string getMethodName, string setMethodName, bool updateTree,
            bool updatePanel) :
            this(data, newValue, getMethodName, setMethodName, updateTree, updatePanel, null)
        {
        }

        public ChangeValueTool(O data, T newValue, string getMethodName, string setMethodName, bool updateTree,
            bool updatePanel, VoidCall changed)
        {
            var type = data != null ? data.GetType() : typeof(O);

            var getter = type.GetMethod(getMethodName);
            var setter = type.GetMethod(setMethodName);

            if (typeof(T).IsAssignableFrom(getter.ReturnType) && setter.GetParameters().Length == 1 && typeof(T).IsAssignableFrom(setter.GetParameters()[0].ParameterType))
            {
                Init(data, newValue, getter, setter, updateTree, updatePanel, changed);
            }
        }

        protected void Init(O data, T newValue, MethodInfo getter, MethodInfo setter, bool updateTree, bool updatePanel, VoidCall changed)
        {
            this.data = data;
            this.newValue = newValue;
            this.updatePanel = updatePanel;
            this.updateTree = updateTree;

            this.get = (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), data, getter);
            this.set = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), data, setter);
            this.changed = changed;

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
            return false;
        }

        public override bool doTool()
        {
            bool done = false;
            if (get != null && set != null)
            {
                oldValue = get();
                var isNull = oldValue == null;
                if (isNull && newValue != null || !isNull && !oldValue.Equals(newValue))
                {
                    set(newValue);
                    Update(updateTree, updatePanel, changed);
                    done = true;
                }

            }
            return done;

        }

        public override bool redoTool()
        {
            bool done = false;
            if (get != null && set != null)
            {
                set(newValue);
                Update(updateTree, updatePanel, changed);
                done = true;
            }
            return done;
        }

        public override bool undoTool()
        {

            bool done = false;
            if (get != null && set != null)
            {
                set(oldValue);
                Update(updateTree, updatePanel, changed);
                done = true;
            }
            return done;
        }

        private static void Update(bool updateTree, bool updatePanel, VoidCall changed)
        {
            if (changed != null)
            {
                changed();
            }
            if (updateTree)
            {
                Controller.Instance.updateStructure();
            }
            if (updatePanel)
            {
                Controller.Instance.updatePanel();
            }
        }
    }

    public class ChangeIntegerValueTool : ChangeValueTool<object, int>
    {
        public ChangeIntegerValueTool(object data, int newValue, string propertyName)
            : base(data, newValue, propertyName)
        {
        }

        public ChangeIntegerValueTool(object data, int newValue, string propertyName, bool updateTree,
            bool updatePanel) : base(data, newValue, propertyName, updateTree, updatePanel)
        {
        }

        public ChangeIntegerValueTool(object data, int newValue, string getMethodName, string setMethodName) 
            : base(data, newValue, getMethodName, setMethodName)
        {
        }

        public ChangeIntegerValueTool(object data, int newValue, string getMethodName, string setMethodName, bool updateTree,
            bool updatePanel) : base(data, newValue, getMethodName, setMethodName, updateTree, updatePanel)
        {
        }
    }

    public class ChangeFloatValueTool : ChangeValueTool<object, float>
    {
        public ChangeFloatValueTool(object data, float newValue, string propertyName)
            : base(data, newValue, propertyName)
        {
        }

        public ChangeFloatValueTool(object data, float newValue, string propertyName, bool updateTree,
            bool updatePanel) : base(data, newValue, propertyName, updateTree, updatePanel)
        {
        }

        public ChangeFloatValueTool(object data, float newValue, string getMethodName, string setMethodName)
            : base(data, newValue, getMethodName, setMethodName)
        {
        }

        public ChangeFloatValueTool(object data, float newValue, string getMethodName, string setMethodName, bool updateTree,
            bool updatePanel) : base(data, newValue, getMethodName, setMethodName, updateTree, updatePanel)
        {
        }
    }

    public static class ChangeEnumValueTool
    {
        public static ChangeEnumValueTool<E> Create<E>(object data, E newValue, string propertyName)
        {
            return new ChangeEnumValueTool<E>(data, newValue, propertyName);
        }

        public static ChangeEnumValueTool<E> Create<E>(object data, E newValue, string getMethodName, string setMethodName)
        {
            return new ChangeEnumValueTool<E>(data, newValue, getMethodName, setMethodName);
        }

        public static ChangeEnumValueTool<E> Create<E>(object data, E newValue, string propertyName, bool updateTree,
            bool updatePanel)
        {
            return new ChangeEnumValueTool<E>(data, newValue, propertyName, updateTree, updatePanel);
        }

        public static ChangeEnumValueTool<E> Create<E>(object data, E newValue, string getMethodName, string setMethodName, 
            bool updateTree, bool updatePanel)
        {
            return new ChangeEnumValueTool<E>(data, newValue, getMethodName, setMethodName, updateTree, updatePanel);
        }
    }

    public class ChangeEnumValueTool<E> : ChangeValueTool<object, E>
    {
        public ChangeEnumValueTool(object data, E newValue, string propertyName)
            : base(data, newValue, propertyName)
        {
        }

        public ChangeEnumValueTool(object data, E newValue, string propertyName, bool updateTree,
            bool updatePanel) : base(data, newValue, propertyName, updateTree, updatePanel)
        {
        }

        public ChangeEnumValueTool(object data, E newValue, string getMethodName, string setMethodName)
            : base(data, newValue, getMethodName, setMethodName)
        {
        }

        public ChangeEnumValueTool(object data, E newValue, string getMethodName, string setMethodName, bool updateTree,
            bool updatePanel) : base(data, newValue, getMethodName, setMethodName, updateTree, updatePanel)
        {
        }
    }
}