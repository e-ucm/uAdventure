using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Editor
{
    public class ResourcesPopup : PopupWindowContent
    {
        private ResourcesEditor resourcesEditor;
        private DataControlWithResources data;
        private bool showResourcesList;

        private static Texture2D resourceInactive, resourceActive;

        public ResourcesPopup(DataControlWithResources data, bool showResourcesList)
        {
            this.data = data;
            this.showResourcesList = showResourcesList;
            resourcesEditor = new ResourcesEditor { Data = data, ShowResourcesList = showResourcesList };
        }

        public static bool ShowAtPosition(DataControlWithResources data, bool showResourcesList, Rect buttonRect)
        {
            buttonRect.height = 0;
            buttonRect.x += buttonRect.width;
            PopupWindow.Show(buttonRect, new ResourcesPopup(data, showResourcesList));
            return true;
        }

        public static void DoResourcesButton(Rect rect, DataControlWithResources data, bool showResourcesList, bool active, GUIStyle style)
        {
            if(!resourceActive || !resourceInactive)
            {
                resourceActive = Resources.Load<Texture2D>("EAdventureData/img/icons/resources");
                resourceInactive = Resources.Load<Texture2D>("EAdventureData/img/icons/resources-inactive");
            }

            if (GUI.Button(rect, active ? resourceActive : resourceInactive, style))
            {
                ShowAtPosition(data, showResourcesList, rect);
            }
        }

        public override void OnGUI(Rect rect)
        {
            EditorGUILayout.LabelField("Resources", EditorStyles.boldLabel);
            var boxRect = EditorGUILayout.BeginVertical("box");
            resourcesEditor.DoLayout();
            EditorGUILayout.EndVertical();
            if (Event.current.type == EventType.Repaint)
            {
                editorWindow.maxSize = editorWindow.minSize = new Vector2(500f, boxRect.y + boxRect.height + 4);
            }
        }
    }
}
