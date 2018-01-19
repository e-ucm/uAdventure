using UnityEngine;

using uAdventure.Core;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;
using System;

namespace uAdventure.Editor
{
    [EditorWindowExtension(70, typeof(Conversation))]
    public class ConversationWindow : PreviewDataControlExtension
    {
        //private ConversationEditorWindow convEditor;

        private Dictionary<Conversation, ConversationEditorWindow> conversationWindows;

        public ConversationWindow(Rect aStartPos, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, new GUIContent(TC.get("Element.Name31")), aStyle, aOptions)
        {
            var c = new GUIContent();
            c.image = (Texture2D)Resources.Load("EAdventureData/img/icons/conversations", typeof(Texture2D));
            c.text = TC.get("Element.Name31");
            ButtonContent = c;

            conversationWindows = new Dictionary<Conversation, ConversationEditorWindow>();
        }

        public ConversationEditorWindow GetConversationEditor(int conversationIndex, bool createIfNotExists)
        {
            GameRources.GetInstance().selectedConversationIndex = conversationIndex;
            if (conversationIndex < 0
                || conversationIndex >= Controller.Instance.ChapterList.getSelectedChapterDataControl()
                    .getConversationsList()
                    .getConversations().Count)
                return null;

            var conversation = Controller
                .Instance.ChapterList.getSelectedChapterDataControl()
                .getConversationsList()
                .getConversations()[conversationIndex]
                .getConversation();

            if (conversationWindows.ContainsKey(conversation) && conversationWindows[conversation] == null)
                conversationWindows.Remove(conversation);

            if (!conversationWindows.ContainsKey(conversation) && createIfNotExists)
            {
                ConversationEditorWindow convEditor = EditorWindow.GetWindow<ConversationEditorWindow>();
                convEditor.Init(conversation as GraphConversation);
                conversationWindows.Add(conversation, convEditor);
            }

            return conversationWindows.ContainsKey(conversation) ? conversationWindows[conversation] : null;
        }

        ///////////////////////////////

        protected override void OnSelect(ReorderableList r)
        {
            GameRources.GetInstance().selectedConversationIndex = r.index;
            GetConversationEditor(r.index, true);
        }
        protected override void OnButton()
        {
            dataControlList.index = -1;
            dataControlList.SetData(Controller.Instance.SelectedChapterDataControl.getConversationsList(),
                sceneList => (sceneList as ConversationsListDataControl).getConversations().Cast<DataControl>().ToList());
        }

        protected override void OnDrawMainPreview(Rect rect, int index)
        {
            GUI.Label(rect, "Preview Unavaliable");
        }
    }
}