using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public abstract class ConditionEditorFactory
    {

        private static ConditionEditorFactory instance;
        public static ConditionEditorFactory Intance
        {
            get
            {
                if (instance == null)
                    instance = new ConditionEditorFactoryImp();
                return instance;
            }
        }

        public abstract string[] CurrentConditionEditors { get; }
        public abstract List<ConditionEditor> Editors { get; }
        public abstract ConditionEditor getConditionEditorFor(Condition condition);
        public abstract int ConditionEditorIndex(Condition condition);

        public void ResetInstance()
        {
            instance = new ConditionEditorFactoryImp();
        }
    }

    public class ConditionEditorFactoryImp : ConditionEditorFactory
    {

        private List<System.Type> types;
        private List<ConditionEditor> editors;
        private ConditionEditor defaultConditionEditor;

        public ConditionEditorFactoryImp()
        {
            this.editors = new List<ConditionEditor>();

            if (types == null)
            {
                types = System.AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(ConditionEditor).IsAssignableFrom(p)).ToList();
                types.Remove(typeof(ConditionEditor));
            }

            foreach (System.Type t in types)
            {
                Debug.Log(t.Name);
                this.editors.Add((ConditionEditor)System.Activator.CreateInstance(t));
            }
        }

        public override string[] CurrentConditionEditors
        {
            get
            {
                List<string> descriptors = new List<string>();
                foreach (ConditionEditor editor in editors)
                    descriptors.Add(editor.conditionName());
                return descriptors.ToArray();
            }
        }

        public override List<ConditionEditor> Editors
        {
            get
            {
                return editors;
            }
        }


        public override ConditionEditor getConditionEditorFor(Condition condition)
        {
            foreach (ConditionEditor editor in editors)
            {
                if (editor.manages(condition))
                {
                    return editor;
                }
            }
            return null;
        }

        public override int ConditionEditorIndex(Condition condition)
        {

            int i = 0;
            foreach (ConditionEditor editor in editors)
                if (editor.manages(condition))
                    return i;
                else
                    i++;

            return 0;
        }
    }
}