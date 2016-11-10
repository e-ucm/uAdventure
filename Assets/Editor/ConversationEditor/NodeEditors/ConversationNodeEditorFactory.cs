using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public abstract class ConversationNodeEditorFactory {

	private static ConversationNodeEditorFactory instance;
	public static ConversationNodeEditorFactory Intance {
		get{ 
			if(instance == null)
				instance = new ConversationNodeEditorFactoryImp();
			return instance; 
		}
	}

	public abstract string[] CurrentConversationNodeEditors { get; }
    public abstract ConversationNodeEditor createConversationNodeEditorFor (string nodeName);
    public abstract ConversationNodeEditor createConversationNodeEditorFor (ConversationNode node); 
	public abstract int ConversationNodeEditorIndex(ConversationNode node);
    public void ResetInstance()
    {
        instance = new ConversationNodeEditorFactoryImp();
    }
}

public class ConversationNodeEditorFactoryImp : ConversationNodeEditorFactory {

	private List<System.Type> types;
	private List<ConversationNodeEditor> nodeEditors;
	private ConversationNodeEditor defaultNodeEditor;

	public ConversationNodeEditorFactoryImp(){
        this.nodeEditors = new List<ConversationNodeEditor> ();

		if (types == null) {
			types = System.AppDomain.CurrentDomain.GetAssemblies ().SelectMany (s => s.GetTypes ()).Where (p => typeof(ConversationNodeEditor).IsAssignableFrom (p)).ToList();
			types.Remove(typeof(ConversationNodeEditor));
		}

		foreach (System.Type t in types)
		{
			this.nodeEditors.Add((ConversationNodeEditor)System.Activator.CreateInstance(t));
		}
	}

	public override string[] CurrentConversationNodeEditors {
		get {
			string[] descriptors = new string[nodeEditors.Count+1];
			for(int i = 0; i< nodeEditors.Count; i++)
				descriptors[i] = nodeEditors[i].NodeName;
			return descriptors;
		}
	}


	public override ConversationNodeEditor createConversationNodeEditorFor (string nodeName)
	{
		foreach (ConversationNodeEditor nodeEditor in nodeEditors) {
			if(nodeEditor.NodeName.ToLower() == nodeName.ToLower()){
				return nodeEditor.clone();
			}
		}
		return null;
	}

	public override ConversationNodeEditor createConversationNodeEditorFor (ConversationNode node)
	{
		foreach (ConversationNodeEditor nodeEditor in nodeEditors) 
			if(nodeEditor.manages(node))    
				return nodeEditor.clone();

		return null;
	}

	public override int ConversationNodeEditorIndex(ConversationNode node){

		int i = 0;
		foreach (ConversationNodeEditor nodeEditor in nodeEditors) 
			if(nodeEditor.manages(node))	
				return i;
			else 							
				i++;

		return 0;
	}
}
