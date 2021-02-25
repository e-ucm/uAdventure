
using IMS.MD.v1p2;
using IMS.MD.v1p3p2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Editor
{
    [CustomPropertyDrawer(typeof(interactivitytypeType))]
    [CustomPropertyDrawer(typeof(learningresourcetypeType))]
    [CustomPropertyDrawer(typeof(interactivitylevelType))]
    [CustomPropertyDrawer(typeof(semanticdensityType))]
    [CustomPropertyDrawer(typeof(intendedenduserroleType))]
    [CustomPropertyDrawer(typeof(contextType))]
    [CustomPropertyDrawer(typeof(difficultyType))]
    [CustomPropertyDrawer(typeof(structureType))]
    [CustomPropertyDrawer(typeof(aggregationlevelType))]
    [CustomPropertyDrawer(typeof(roleType))]
    [CustomPropertyDrawer(typeof(copyrightandotherrestrictionsType))]
    [CustomPropertyDrawer(typeof(costType))]
    [CustomPropertyDrawer(typeof(typeType))]
    [CustomPropertyDrawer(typeof(nameType))]
    [CustomPropertyDrawer(typeof(kindType))]
    [CustomPropertyDrawer(typeof(purposeType))]
    [CustomPropertyDrawer(typeof(statusType))]
    public class imsmd_v1p2_enum_drawer : PropertyDrawer
    {
        public enum learningResourceTypeTypeValue
        {

            /// <remarks/>
            exercise,

            /// <remarks/>
            simulation,

            /// <remarks/>
            exam,

            /// <remarks/>
            [System.Xml.Serialization.XmlEnumAttribute("self assessment")]
            selfassessment,

            /// <remarks/>
            lecture,
        }
        public enum aggregationLevelTypeValue
        {

            /// <remarks/>
            [System.Xml.Serialization.XmlEnumAttribute("1")]
            _1,

            /// <remarks/>
            [System.Xml.Serialization.XmlEnumAttribute("2")]
            _2,

            /// <remarks/>
            [System.Xml.Serialization.XmlEnumAttribute("3")]
            _3,

            /// <remarks/>
            [System.Xml.Serialization.XmlEnumAttribute("4")]
            _4,
        }
        public enum roleTypeValue
        {

            /// <remarks/>
            creator,

            /// <remarks/>
            validator,
        }


        private Dictionary<System.Type, System.Type> typeEnum = new Dictionary<System.Type, System.Type>
        {
            { typeof(interactivitytypeType), typeof(InteractivityTypeTypeValue) },
            { typeof(learningresourcetypeType), typeof(learningResourceTypeTypeValue) },
            { typeof(interactivitylevelType), typeof(InteractivityLevelTypeValue) },
            { typeof(semanticdensityType), typeof(SemanticDensityTypeValue) },
            { typeof(intendedenduserroleType), typeof(IntendedEndUserRoleTypeValue) },
            { typeof(contextType), typeof(ContextTypeValue) },
            { typeof(difficultyType), typeof(DifficultyTypeValue) },
            { typeof(structureType), typeof(StructureTypeValue) },
            { typeof(aggregationlevelType), typeof(aggregationLevelTypeValue) },
            { typeof(roleType), typeof(roleTypeValue) },
            { typeof(copyrightandotherrestrictionsType), typeof(CopyrightAndOtherRestrictionsTypeValue) },
            { typeof(costType), typeof(CostTypeValue) },
            { typeof(typeType), typeof(TypeTypeValue) },
            { typeof(nameType), typeof(NameTypeValue) },
            { typeof(kindType), typeof(KindTypeValue) },
            { typeof(purposeType), typeof(PurposeTypeValue) },
            { typeof(statusType), typeof(StatusTypeValue) }
        };

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

            var elemType = property.GetPropertyFieldType();
            var value = property.FindPropertyRelative("value.langstring.Value");
            var source = property.FindPropertyRelative("source.langstring.Value");

            if (typeEnum.ContainsKey(elemType))
            {
                var noSpaces = TrimAllWithInplaceCharArray(value == null ? string.Empty : value.stringValue);
                var values = Enum.GetNames(typeEnum[elemType]).ToList().ConvertAll(v => v.Replace("_", ""));
                values.Insert(0, "-None-");
                var index = (int)(string.IsNullOrEmpty(noSpaces) ? 0 : values.IndexOf(Enum.Parse(typeEnum[elemType], noSpaces).ToString()));
                EditorGUI.BeginChangeCheck();
                var newValue = EditorGUI.Popup(position, index, values.ToArray());

                if(newValue == 0)
                {
                    var field = property.GetPropertyFieldInfo();
                    var parent = property.GetParentObject<object>();
                    var changed = EditorGUI.EndChangeCheck();
                    if (changed || field.GetValue(parent) != null)
                    {
                        field.SetValue(parent, null);
                        if (changed)
                        {
                            property.serializedObject.Update();
                        }
                    }
                }
                else
                {
                    if (EditorGUI.EndChangeCheck())
                    {
                        var field = property.GetPropertyFieldInfo();
                        var parent = property.GetParentObject<object>();
                        field.SetValue(parent, GetDefault(field.GetType()));
                        property.serializedObject.Update();

                        var sourcelang = property.FindPropertyRelative("source.langstring.lang");
                        var valuelang = property.FindPropertyRelative("value.langstring.lang");

                        source.stringValue = "LOMv1.0";
                        sourcelang.stringValue = "x-none";

                        XmlSerializer s = new XmlSerializer(typeEnum[elemType]);
                        using (var sw = new StringWriter())
                        {
                            s.Serialize(sw, newValue - 1);
                            var xml = sw.ToString();
                            var doc = XDocument.Parse(xml);
                            value.stringValue = doc.Root.Value;
                            valuelang.stringValue = "x-none";
                        }

                    }
                }
            }
            else
            {
                // Calculate rects
                var sourceRect = new Rect(position.x, position.y, position.width / 2f, position.height);
                var valueRect = new Rect(5 + sourceRect.x + sourceRect.width, position.y, position.width / 2f - 5f, position.height);

                EditorGUI.PropertyField(sourceRect, source);
                EditorGUI.PropertyField(valueRect, value);
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