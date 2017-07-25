using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEditor;

using uAdventure.Core;
using System;

namespace uAdventure.Editor
{
    public class ItemsWindowDescription : LayoutWindow
    {

        private ItemDataControl workingItem;

        private GUIContent dragdropLabel, transitionLabel;

        /*
        * SETTINGS fields
        */
        private bool dragdropToogle, dragdropToogleLast;

        private string[] behaviourTypes = { TC.get("Behaviour.Normal"), TC.get("Behaviour.FirstAction") };

        private string[] behaviourTypesDescription =
        {
           TC.get("Behaviour.Selection.Normal"), TC.get("Behaviour.Selection.FirstAction")
        };

        private int selectedBehaviourType, selectedBehaviourTypeLast;

        private string transitionTime, transitionTimeLast;

        private DescriptionsEditor descriptionsEditor;

        public ItemsWindowDescription(Rect aStartPos, GUIContent aContent, GUIStyle aStyle,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            descriptionsEditor = ScriptableObject.CreateInstance<DescriptionsEditor>();

            dragdropLabel = new GUIContent(TC.get("Item.ReturnsWhenDragged"), TC.get("Item.ReturnsWhenDragged.Description"));
            transitionLabel = new GUIContent(TC.get("Resources.TransitionTime"), TC.get("Resources.TransitionTime.Description"));
        }


        public override void Draw(int aID)
        {
            workingItem = Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[GameRources.GetInstance().selectedItemIndex];


            // -------------
            // Documentation
            // -------------

            GUILayout.Label(TC.get("Item.Documentation"));
            EditorGUI.BeginChangeCheck();
            var fullItemDescription = GUILayout.TextArea(workingItem.getDocumentation() ?? string.Empty);
            if (EditorGUI.EndChangeCheck())
                workingItem.setDocumentation(fullItemDescription);


            // -------------
            // Descriptions
            // -------------
            descriptionsEditor.Descriptions = workingItem.getDescriptionController();
            descriptionsEditor.OnInspectorGUI();
            GUILayout.Space(20);

            // -------------
            // Drag & drop
            // -------------

            EditorGUILayout.LabelField(TC.get("Item.ReturnsWhenDragged.Title"));
            EditorGUI.BeginChangeCheck();
            var dragdropToogle = EditorGUILayout.Toggle(dragdropLabel, workingItem.isReturnsWhenDragged());
            if (EditorGUI.EndChangeCheck())
                workingItem.setReturnsWhenDragged(dragdropToogle);
            GUILayout.Space(20);

            // -------------
            // Behaviour
            // -------------
            EditorGUI.BeginChangeCheck();
            var selectedBehaviourType = EditorGUILayout.Popup(TC.get("Behaviour"), (int)workingItem.getBehaviour(), behaviourTypes);
            Item.BehaviourType type = (selectedBehaviourType == 0 ? Item.BehaviourType.NORMAL : Item.BehaviourType.FIRST_ACTION);
            if (EditorGUI.EndChangeCheck())
                workingItem.setBehaviour(type);
            EditorGUILayout.HelpBox(behaviourTypesDescription[selectedBehaviourType], MessageType.Info);
            GUILayout.Space(20);

            // -------------
            // Transition time
            // -------------
            EditorGUI.BeginChangeCheck();
            var transitionTime = Math.Max(EditorGUILayout.LongField(transitionLabel, workingItem.getResourcesTransitionTime()), 0);
            if (EditorGUI.EndChangeCheck())
                workingItem.setResourcesTransitionTime(transitionTime);

        }
    }
}