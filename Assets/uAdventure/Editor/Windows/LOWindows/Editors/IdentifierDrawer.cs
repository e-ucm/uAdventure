
using IMS.MD.v1p2;
using IMS.MD.v1p3p2;
using uAdventure.Core.Metadata;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Editor
{
    [CustomPropertyDrawer(typeof(Identifier))]
    [CustomPropertyDrawer(typeof(IdentifierType))]
    [CustomPropertyDrawer(typeof(cataloglangType))]
    public class IdentifierDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var langFieldSize = position.width / 2f;

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            EditorGUIUtility.labelWidth = 50;

            // Calculate rects
            var catalogRect = new Rect(position.x, position.y, position.width - langFieldSize, position.height);
            var entryRect = new Rect(5 + catalogRect.x + catalogRect.width, position.y, langFieldSize - 5, position.height);

            var catalog = property.FindPropertyRelative("catalog");
            var entry = property.FindPropertyRelative("entry");

            EditorGUI.PropertyField(catalogRect, catalog);
            EditorGUI.PropertyField(entryRect, entry);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }

}