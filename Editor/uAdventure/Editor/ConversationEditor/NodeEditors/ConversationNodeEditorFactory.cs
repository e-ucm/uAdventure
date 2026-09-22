using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public abstract class ConversationNodeEditorFactory
    {

        private static ConversationNodeEditorFactory instance;
        public static ConversationNodeEditorFactory Intance
        {
            get
            {
                if (instance == null)
                    instance = new ConversationNodeEditorFactoryImp();
                return instance;
            }
        }

        public abstract string[] CurrentConversationNodeEditors { get; }
        public abstract ConversationNodeEditor createConversationNodeEditorFor(ConversationDataControl conversation, string nodeName);
        public abstract ConversationNodeEditor createConversationNodeEditorFor(ConversationDataControl conversation, ConversationNodeDataControl node);
        public abstract int ConversationNodeEditorIndex(ConversationNodeDataControl node);
        public void ResetInstance()
        {
            instance = new ConversationNodeEditorFactoryImp();
        }
    }

    public class ConversationNodeEditorFactoryImp : ConversationNodeEditorFactory
    {

        private List<System.Type> types;
        private List<ConversationNodeEditor> nodeEditors;
        private ConversationNodeEditor defaultNodeEditor;

        public ConversationNodeEditorFactoryImp()
        {
            this.nodeEditors = new List<ConversationNodeEditor>();

            if (types == null)
            {
                types = System.AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(ConversationNodeEditor).IsAssignableFrom(p)).ToList();
                types.Remove(typeof(ConversationNodeEditor));
            }

            foreach (System.Type t in types)
            {
                this.nodeEditors.Add((ConversationNodeEditor)System.Activator.CreateInstance(t));
            }
        }

        public override string[] CurrentConversationNodeEditors
        {
            get
            {
                string[] descriptors = new string[nodeEditors.Count + 1];
                for (int i = 0; i < nodeEditors.Count; i++)
                {
                    descriptors[i] = nodeEditors[i].NodeName;
                }
                return descriptors;
            }
        }


        public override ConversationNodeEditor createConversationNodeEditorFor(ConversationDataControl conversation, string nodeName)
        {
            foreach (ConversationNodeEditor nodeEditor in nodeEditors)
            {
                if (nodeEditor.NodeName.ToLower() == nodeName.ToLower())
                {
                    return nodeEditor.clone();
                }
            }
            return null;
        }

        public override ConversationNodeEditor createConversationNodeEditorFor(ConversationDataControl conversation, ConversationNodeDataControl node)
        {
            return nodeEditors
                .Where(ne => ne.manages(node))
                .Select(ne => ne.clone())
                .FirstOrDefault();
        }

        public override int ConversationNodeEditorIndex(ConversationNodeDataControl node)
        {
            return nodeEditors.FindIndex(ne => ne.manages(node));
        }
    }
}