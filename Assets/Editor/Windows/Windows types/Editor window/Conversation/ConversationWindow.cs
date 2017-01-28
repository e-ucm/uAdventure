using UnityEngine;

using uAdventure.Core;
using UnityEditorInternal;
using System.Collections.Generic;

namespace uAdventure.Editor
{
    [EditorWindowExtension(70, typeof(Conversation))]
    public class ConversationWindow : ReorderableListEditorWindowExtension
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


        public override void Draw(int aID)
        {
            var windowWidth = m_Rect.width;

            GUILayout.Space(30);
            for (int i = 0; i < Controller.getInstance().getCharapterList().getSelectedChapterData().getConversations().Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Box(Controller.getInstance().getCharapterList().getSelectedChapterData().getConversations()[i].getId(), GUILayout.Width(windowWidth * 0.65f));
                if (GUILayout.Button("Edit conversation", GUILayout.MaxWidth(windowWidth * 0.3f)))
                    OpenConversationEditor(i);

                GUILayout.EndHorizontal();

            }
        }

        public void OpenConversationEditor(int conversationIndex)
        {
            GameRources.GetInstance().selectedConversationIndex = conversationIndex;
            if (conversationIndex < 0 || conversationIndex >= Controller.getInstance().getCharapterList().getSelectedChapterData().getConversations().Count)
                return;

            var conversation = Controller.getInstance().getCharapterList().getSelectedChapterData().getConversations()[conversationIndex];

            if (conversationWindows.ContainsKey(conversation))
            {
                conversationWindows[conversation].Show();
            }
            else
            {
                ConversationEditorWindow convEditor = (ConversationEditorWindow)ScriptableObject.CreateInstance(typeof(ConversationEditorWindow));
                convEditor.Init(conversation as GraphConversation);
            }
        }

        ///////////////////////////////

        protected override void OnElementNameChanged(ReorderableList r, int index, string newName)
        {
            Controller.getInstance().getCharapterList().getSelectedChapterData().getConversations()[index].setId(newName);
        }

        protected override void OnAdd(ReorderableList r)
        {
            if (r.index != -1 && r.index < r.list.Count)
            {
                Controller.getInstance()
                           .getCharapterList()
                           .getSelectedChapterDataControl()
                           .getConversationsList()
                           .duplicateElement(
                               Controller.getInstance()
                                   .getCharapterList()
                                   .getSelectedChapterDataControl()
                                   .getConversationsList()
                                   .getConversations()[r.index]);
            }
            else
            {
                Controller.getInstance().getSelectedChapterDataControl().getConversationsList().addElement(Controller.CONVERSATION_GRAPH, "newConversation");
            }

        }

        protected override void OnAddOption(ReorderableList r, string option) { }

        protected override void OnRemove(ReorderableList r)
        {
            if (r.index != -1)
            {
                var conv = Controller.getInstance()
                                      .getCharapterList()
                                      .getSelectedChapterDataControl()
                                      .getConversationsList()
                                      .getConversations()[r.index];

                if (conversationWindows.ContainsKey(conv.getConversation()))
                {
                    conversationWindows[conv.getConversation()].Close();
                    conversationWindows.Remove(conv.getConversation());
                }

                Controller.getInstance()
                              .getCharapterList()
                              .getSelectedChapterDataControl()
                              .getConversationsList()
                              .deleteElement(conv, false);
                
            }
        }

        protected override void OnSelect(ReorderableList r)
        {
            OpenConversationEditor(r.index);
        }

        protected override void OnReorder(ReorderableList r)
        {
            List<Conversation> previousList = Controller.getInstance()
                              .getCharapterList()
                              .getSelectedChapterData()
                              .getConversations();

            List<Conversation> reordered = new List<Conversation>();
            foreach (string name in r.list)
                reordered.Add(previousList.Find(s => s.getId() == name));


            previousList.Clear();
            previousList.AddRange(reordered);
        }

        protected override void OnButton()
        {
            reorderableList.index = -1;
        }

        protected override void OnUpdateList(ReorderableList r)
        {
            Elements = Controller.getInstance().getCharapterList().getSelectedChapterData().getConversations().ConvertAll(s => s.getId());
        }
    }
}