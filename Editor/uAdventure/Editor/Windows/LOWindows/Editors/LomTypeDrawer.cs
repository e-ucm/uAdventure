
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using uAdventure.Core.Metadata;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Editor
{
    [CustomPropertyDrawer(typeof(LomType), true)]
    public class LomTypeDrawer : PropertyDrawer
    {
        TextInfo myTI = new CultureInfo("en-US",false).TextInfo;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            EditorGUIUtility.labelWidth = 50;

            var elem = property.GetPropertyObject<LomType>();
            var fieldInfo = property.GetPropertyFieldInfo();
            var attribute = (LomTypeEnumAttribute)fieldInfo.GetCustomAttributes(typeof(LomTypeEnumAttribute), false).FirstOrDefault();
            if (attribute != null)
            {
                var enumValue = TrimAllWithInplaceCharArray(myTI.ToTitleCase(elem.Value ?? string.Empty));
                var value = (Enum)(string.IsNullOrEmpty(enumValue) ? GetDefault(attribute.EnumType) : Enum.Parse(attribute.EnumType, enumValue));
                EditorGUI.BeginChangeCheck();
                var newValue = EditorGUI.EnumPopup(position, value);
                if (EditorGUI.EndChangeCheck())
                {
                    elem.Source = attribute.Source;
                    elem.Value = Regex.Replace(newValue.ToString(), "([A-Z])", " $1").Trim().ToLower();
                    property.serializedObject.Update();
                }
            }
            else
            {
                // Calculate rects
                var catalogRect = new Rect(position.x, position.y, position.width /2f, position.height);
                var entryRect = new Rect(5 + catalogRect.x + catalogRect.width, position.y, position.width / 2f - 5f, position.height);

                var catalog = property.FindPropertyRelative("source");
                var entry = property.FindPropertyRelative("value");

                EditorGUI.PropertyField(catalogRect, catalog);
                EditorGUI.PropertyField(entryRect, entry);
            }

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
        public static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        public static string TrimAllWithInplaceCharArray(string str)
        {

            var len = str.Length;
            var src = str.ToCharArray();
            int dstIdx = 0;

            for (int i = 0; i < len; i++)
            {
                var ch = src[i];

                switch (ch)
                {

                    case '\u0020':
                    case '\u00A0':
                    case '\u1680':
                    case '\u2000':
                    case '\u2001':

                    case '\u2002':
                    case '\u2003':
                    case '\u2004':
                    case '\u2005':
                    case '\u2006':

                    case '\u2007':
                    case '\u2008':
                    case '\u2009':
                    case '\u200A':
                    case '\u202F':

                    case '\u205F':
                    case '\u3000':
                    case '\u2028':
                    case '\u2029':
                    case '\u0009':

                    case '\u000A':
                    case '\u000B':
                    case '\u000C':
                    case '\u000D':
                    case '\u0085':
                        continue;

                    default:
                        src[dstIdx++] = ch;
                        break;
                }
            }
            return new string(src, 0, dstIdx);
        }
    }
}
