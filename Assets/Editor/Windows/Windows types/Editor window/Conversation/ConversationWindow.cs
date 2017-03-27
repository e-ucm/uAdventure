using UnityEngine;

using uAdventure.Core;
using UnityEditor;
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
			var editor = GetConversationEditor(GameRources.GetInstance ().selectedConversationIndex, false);

			//if (editor == null) {

			GUILayout.Space (30);
			for (int i = 0; i < Controller.getInstance ().getCharapterList ().getSelectedChapterDataControl ().getConversationsList ().getConversations ().Count; i++) {
				GUILayout.BeginHorizontal ();
				GUILayout.Box (Controller.getInstance ().getCharapterList ().getSelectedChapterDataControl ().getConversationsList ().getConversations () [i].getId (), GUILayout.Width (windowWidth * 0.65f));
				if (GUILayout.Button ("Edit conversation", GUILayout.MaxWidth (windowWidth * 0.3f))) {
					GameRources.GetInstance ().selectedConversationIndex = i;
					GetConversationEditor (GameRources.GetInstance ().selectedConversationIndex, true);
				}

				GUILayout.EndHorizontal ();
			}
			//} else {
				//editor.OnGUI ();
			//}

        }

		public ConversationEditorWindow GetConversationEditor(int conversationIndex, bool createIfNotExists)
        {
            GameRources.GetInstance().selectedConversationIndex = conversationIndex;
			if (conversationIndex < 0 
				|| conversationIndex >= Controller.getInstance()
					.getCharapterList()
					.getSelectedChapterDataControl()
					.getConversationsList ()
					.getConversations().Count)
                return null;

			var conversation = Controller
				.getInstance()
				.getCharapterList()
				.getSelectedChapterDataControl()
				.getConversationsList ()
				.getConversations()[conversationIndex]
				.getConversation ();

			if (conversationWindows.ContainsKey (conversation) && conversationWindows [conversation] == null)
				conversationWindows.Remove (conversation);

			if (!conversationWindows.ContainsKey(conversation) && createIfNotExists)
            {
				ConversationEditorWindow convEditor = EditorWindow.GetWindow<ConversationEditorWindow>();
				convEditor.Init(conversation as GraphConversation);
				conversationWindows.Add (conversation, convEditor);
            }

			return conversationWindows.ContainsKey(conversation) ? conversationWindows [conversation] : null;
        }

        ///////////////////////////////

        protected override void OnElementNameChanged(ReorderableList r, int index, string newName)
        {
			Controller.getInstance().getCharapterList().getSelectedChapterDataControl().getConversationsList().getConversations ()[index].renameElement(newName);
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
			GameRources.GetInstance ().selectedConversationIndex = r.index;
        }

        protected override void OnReorder(ReorderableList r)
        {
			var dataControlList = Controller.getInstance ()
				.getCharapterList ().getSelectedChapterDataControl ().getConversationsList ();

			var toPos = r.index;
			var fromPos = dataControlList.getConversations ().FindIndex (i => i.getId () == r.list [r.index] as string);

			dataControlList.MoveElement (dataControlList.getConversations ()[fromPos], fromPos, toPos);
        }

        protected override void OnButton()
        {
            reorderableList.index = -1;
        }

        protected override void OnUpdateList(ReorderableList r)
        {
			Elements = Controller.getInstance().getCharapterList().getSelectedChapterDataControl().getConversationsList ().getConversations().ConvertAll(s => s.getId());
        }
    }
}