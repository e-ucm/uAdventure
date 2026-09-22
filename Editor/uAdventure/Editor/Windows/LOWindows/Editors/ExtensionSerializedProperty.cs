using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace uAdventure.Editor
{
    public static class ExtensionSerializedProperty
    {
        public static T GetPropertyObject<T>(this SerializedProperty property)
        {
            var bindings = System.Reflection.BindingFlags.NonPublic |
                             System.Reflection.BindingFlags.Public |
                             System.Reflection.BindingFlags.Instance;

            var fields = property.propertyPath.Split('.');

            var type = property.serializedObject.targetObject.GetType();
            object obj = property.serializedObject.targetObject;
            System.Reflection.FieldInfo fieldInfo = null;
            bool nextArray = false;
            foreach (var field in fields)
            {
                if (nextArray)
                {
                    var index = field.Substring(field.IndexOf('[') + 1).RemoveFromEnd("]");
                    var arrayIndex = int.Parse(index);
                    if (obj is Array)
                    {
                        obj = ((Array)obj).GetValue(arrayIndex);
                    }
                    else if (obj is IList)
                    {
                        obj = ((IList)obj)[arrayIndex];
                    }

                    type = obj.GetType();
                    nextArray = false;
                    continue;
                }
                else if (field == "Array")
                {
                    nextArray = true;
                    continue;
                }

                fieldInfo = type.GetField(field, bindings);
                if (fieldInfo == null)
                {
                    fieldInfo = type.GetFields(bindings).Concat(type.BaseType.GetFields(bindings)).First(f => f.Name == field);
                }
                type = fieldInfo.FieldType;
                obj = fieldInfo.GetValue(obj);
            }

            return (T)obj;
        }
        public static T GetParentObject<T>(this SerializedProperty property)
        {
            var bindings = System.Reflection.BindingFlags.NonPublic |
                             System.Reflection.BindingFlags.Public |
                             System.Reflection.BindingFlags.Instance;

            var fields = property.propertyPath.Split('.');

            var type = property.serializedObject.targetObject.GetType();
            object obj = property.serializedObject.targetObject;
            System.Reflection.FieldInfo fieldInfo = null;
            bool nextArray = false;
            object parent = null;
            foreach (var field in fields)
            {
                if (nextArray)
                {
                    var index = field.Substring(field.IndexOf('[') + 1).RemoveFromEnd("]");
                    var arrayIndex = int.Parse(index);
                    parent = obj;
                    if (obj is Array)
                    {
                        obj = ((Array)obj).GetValue(arrayIndex);
                    }
                    else if (obj is IList)
                    {
                        obj = ((IList)obj)[arrayIndex];
                    }

                    type = obj.GetType();
                    nextArray = false;
                    continue;
                }
                else if (field == "Array")
                {
                    nextArray = true;
                    continue;
                }

                fieldInfo = type.GetField(field, bindings);
                if (fieldInfo == null)
                {
                    fieldInfo = type.GetFields(bindings).Concat(type.BaseType.GetFields(bindings)).First(f => f.Name == field);
                }
                type = fieldInfo.FieldType;
                parent = obj;
                obj = fieldInfo.GetValue(obj);
            }

            return (T)parent;
        }
        public static Type GetPropertyFieldType(this SerializedProperty property)
        {
            var obj = property.GetPropertyObject<object>();
            if(obj != null)
            {
                return obj.GetType();
            }
            else
            {
                return property.GetPropertyFieldInfo().FieldType;
            }
        }
        public static FieldInfo GetPropertyFieldInfo(this SerializedProperty property)
        {
            var bindings = System.Reflection.BindingFlags.NonPublic |
                             System.Reflection.BindingFlags.Public |
                             System.Reflection.BindingFlags.Instance;

            var fields = property.propertyPath.Split('.');

            var type = property.serializedObject.targetObject.GetType();
            object obj = property.serializedObject.targetObject;
            System.Reflection.FieldInfo fieldInfo = null;
            bool nextArray = false;
            foreach (var field in fields)
            {
                if (nextArray)
                {
                    var index = field.Substring(field.IndexOf('[') + 1).RemoveFromEnd("]");
                    var arrayIndex = int.Parse(index);
                    if (obj is Array)
                    {
                        obj = ((Array)obj).GetValue(arrayIndex);
                    }
                    else if (obj is IList)
                    {
                        obj = ((IList)obj)[arrayIndex];
                    }

                    type = obj.GetType();
                    nextArray = false;
                    continue;
                }
                else if (field == "Array")
                {
                    nextArray = true;
                    continue;
                }

                fieldInfo = type.GetField(field, bindings);
                if (fieldInfo == null)
                {
                    fieldInfo = type.GetFields(bindings).Concat(type.BaseType.GetFields(bindings)).First(f => f.Name == field);
                }
                type = fieldInfo.FieldType;
                obj = fieldInfo.GetValue(obj);
            }

            return fieldInfo;
        }
    }
}
