using UnityEngine;

using uAdventure.Core;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;
using System;

namespace uAdventure.Editor
{
    [EditorWindowExtension(70, typeof(ConversationDataControl), typeof(GraphConversationDataControl))]
    public class ConversationWindow : PreviewDataControlExtension
    {
        private enum ConversationWindowTabs { Preview };

        private readonly Dictionary<ConversationDataControl, ConversationEditorWindow> conversationWindows;

        public ConversationWindow(Rect aStartPos, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, new GUIContent(TC.get("Element.Name31")), aStyle, aOptions)
        {
            ButtonContent = new GUIContent()
            {
                image = Resources.Load<Texture2D>("EAdventureData/img/icons/conversations"),
                text = "Element.Name31"
            };

            conversationWindows = new Dictionary<ConversationDataControl, ConversationEditorWindow>();

            DefaultOpenedWindow = ConversationWindowTabs.Preview;
            OpenedWindow = ConversationWindowTabs.Preview;
            AddTab("Preview", ConversationWindowTabs.Preview, new ConversationWindowEdit(aStartPos, new GUIContent("Preview"), "Window"));
        }

        public ConversationEditorWindow GetConversationEditor(int conversationIndex, bool createIfNotExists)
        {
            GameRources.GetInstance().selectedConversationIndex = conversationIndex;
            if (conversationIndex < 0
                || conversationIndex >= Controller.Instance.ChapterList.getSelectedChapterDataControl()
                    .getConversationsList()
                    .getConversations().Count)
            {
                return null;
            }

            var conversation = Controller
                .Instance.ChapterList.getSelectedChapterDataControl()
                .getConversationsList()
                .getConversations()[conversationIndex];

            if (conversationWindows.ContainsKey(conversation) && conversationWindows[conversation] == null)
            {
                conversationWindows.Remove(conversation);
            }

            if (!conversationWindows.ContainsKey(conversation) && createIfNotExists)
            {
                ConversationEditorWindow convEditor = EditorWindow.GetWindow<ConversationEditorWindow>();
                convEditor.Init(conversation);
                conversationWindows.Add(conversation, convEditor);
            }

            return conversationWindows.ContainsKey(conversation) ? conversationWindows[conversation] : null;
        }

        ///////////////////////////////

        protected override void OnSelect(ReorderableList r)
        {
            GameRources.GetInstance().selectedConversationIndex = r.index;
            EditorWindowBase.WantsMouseMove = true;
        }
        protected override void OnButton()
        {
            if(!this.Selected)
                EditorWindowBase.WantsMouseMove = false;

            dataControlList.index = -1;
            dataControlList.SetData(Controller.Instance.SelectedChapterDataControl.getConversationsList(),
                sceneList => (sceneList as ConversationsListDataControl).getConversations().Cast<DataControl>().ToList());
        }

        protected override void OnDrawMainPreview(Rect rect, int index)
        {
            GUI.Label(rect, "Preview Unavaliable");
        }
    }

    public class ConversationWindowEdit : PreviewLayoutWindow
    {
        private ConversationEditor conversationEditor;
        private ConversationDataControl workingConversation;

        public ConversationWindowEdit(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
        {
        }

        protected override void DrawInspector()
        {
            EditorWindowBase.WantsMouseMove = true;
            base.DrawInspector();
            var prevWorkingConversation = workingConversation;
            workingConversation = Controller.Instance.ChapterList.getSelectedChapterDataControl()
                    .getConversationsList().getConversations()[GameRources.GetInstance().selectedConversationIndex];
            if(workingConversation != null && prevWorkingConversation != workingConversation)
            {
                conversationEditor = ConversationEditor.CreateInstance<ConversationEditor>();
                conversationEditor.BeginWindows = () => BeginWindows();
                conversationEditor.EndWindows = () => EndWindows();
                conversationEditor.Repaint = () => Repaint();
                conversationEditor.Init(workingConversation);
            }
            
            EditorGUI.BeginChangeCheck();
            var newId = EditorGUILayout.TextField(TC.get("Conversation.Title"), workingConversation.getId());
            if (EditorGUI.EndChangeCheck())
                workingConversation.getConversation().setId(newId);
        }

        public override void Draw(int aID)
        {
            DrawInspector();
            DrawPreviewHeader();
            DrawPreview(GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)));
        }

        protected override void DrawPreviewHeader()
        {
            conversationEditor.DoToolbar();
        }

        public override void DrawPreview(Rect rect)
        {
            conversationEditor.DoGraph(rect);
        }
    }
}