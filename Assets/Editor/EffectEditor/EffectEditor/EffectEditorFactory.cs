using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public abstract class EffectEditorFactory {
    
	private static EffectEditorFactory instance;
	public static EffectEditorFactory Intance {
		get{ 
			if(instance == null)
				instance = new EffectEditorFactoryImp();
			return instance; 
		}
	}
	
	public abstract string[] CurrentEffectEditors { get; }
	public abstract EffectEditor createEffectEditorFor (string effectName);
    public abstract EffectEditor createEffectEditorFor (AbstractEffect effect); 
	public abstract int EffectEditorIndex(AbstractEffect effect);
    public void ResetInstance()
    {
        instance = new EffectEditorFactoryImp();
    }
}

public class EffectEditorFactoryImp : EffectEditorFactory {

    private List<System.Type> types;
	private List<EffectEditor> effectEditors;
	private EffectEditor defaultEffectEditor;
	
	public EffectEditorFactoryImp(){
		this.effectEditors = new List<EffectEditor> ();

        if (types == null) {
            types = System.AppDomain.CurrentDomain.GetAssemblies ().SelectMany (s => s.GetTypes ()).Where (p => typeof(EffectEditor).IsAssignableFrom (p)).ToList();
            types.Remove(typeof(EffectEditor));
        }

	    foreach (System.Type t in types)
	    {
	        if (t == typeof (ActivateEffectEditor) || t == typeof (DeactivateEffectEditor))
	        {
                if (Controller.getInstance().getVarFlagSummary().getVarCount() > 0)
                    this.effectEditors.Add((EffectEditor)System.Activator.CreateInstance(t));
            }
            else if (t == typeof (ConsumeObjectEffectEditor) || t == typeof (GenerateObjectEffectEditor) ||
                     t == typeof (HighlightItemEffectEditor) || t == typeof(MoveObjectEffectEditor))
            {
                if (Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItemsIDs().Length > 0)
                    this.effectEditors.Add((EffectEditor)System.Activator.CreateInstance(t));
            }
            else if (t == typeof (IncrementVarEffectEditor) || t == typeof (DecrementVarEffectEditor) ||
                     t == typeof (SetValueEffectEditor))
            {
                if (Controller.getInstance().getVarFlagSummary().getFlagCount()>0)
                    this.effectEditors.Add((EffectEditor)System.Activator.CreateInstance(t));
            }
            else if (t == typeof (MacroReferenceEffectEditor))
            {
                if (Controller.getInstance().getAdvancedFeaturesController().getMacrosListDataControl().getMacrosIDs().Length > 0)
                    this.effectEditors.Add((EffectEditor)System.Activator.CreateInstance(t));

            }
            else if (t == typeof (MoveNPCEffectEditor) || t == typeof(SpeakCharEffectEditor))
            {
                if (Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCsIDs().Length > 0)
                    this.effectEditors.Add((EffectEditor)System.Activator.CreateInstance(t));
            }
            else if (t == typeof (TriggerBookEffectEditor))
            {
                if (Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooksIDs().Length > 0)
                    this.effectEditors.Add((EffectEditor)System.Activator.CreateInstance(t));
            }
            else if (t == typeof(TriggerConversationEffectEditor))
            {
                if (Controller.getInstance().getSelectedChapterDataControl().getConversationsList().getConversationsIDs().Length > 0)
                    this.effectEditors.Add((EffectEditor)System.Activator.CreateInstance(t));
            }
            else if (t == typeof (TriggerCutsceneEffectEditor))
            {
                if (Controller.getInstance().getSelectedChapterDataControl().getCutscenesList().getCutscenesIDs().Length > 0)
                    this.effectEditors.Add((EffectEditor)System.Activator.CreateInstance(t));
            }
            else
            {
                this.effectEditors.Add((EffectEditor)System.Activator.CreateInstance(t));
            }
	    }
	}
	
	public override string[] CurrentEffectEditors {
		get {
			string[] descriptors = new string[effectEditors.Count+1];
			for(int i = 0; i< effectEditors.Count; i++)
				descriptors[i] = effectEditors[i].EffectName;
			return descriptors;
		}
	}
	
	
	public override EffectEditor createEffectEditorFor (string effectName)
	{
        Debug.Log("Create: " + effectName);
		foreach (EffectEditor effectEditor in effectEditors) {
			if(effectEditor.EffectName.ToLower() == effectName.ToLower()){
				return effectEditor.clone();
			}
        }
        return null;
	}

    public override EffectEditor createEffectEditorFor (AbstractEffect effect)
    {
        foreach (EffectEditor effectEditor in effectEditors) 
            if(effectEditor.manages(effect))    
                return effectEditor.clone();
        
        return null;
    }

	public override int EffectEditorIndex(AbstractEffect effect){
		
		int i = 0;
		foreach (EffectEditor effectEditor in effectEditors) 
			if(effectEditor.manages(effect))	
                return i;
		    else 							
                i++;
		
		return 0;
	}
}
