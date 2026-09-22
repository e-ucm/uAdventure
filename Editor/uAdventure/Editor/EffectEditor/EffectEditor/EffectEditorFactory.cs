using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public abstract class EffectEditorFactory
    {

        private static EffectEditorFactory instance;
        public static EffectEditorFactory Intance
        {
            get
            {
                if (instance == null)
                    instance = new EffectEditorFactoryImp();
                return instance;
            }
        }

        public abstract string[] CurrentEffectEditors { get; }
        public abstract IEffectEditor createEffectEditorFor(string effectName);
        public abstract IEffectEditor createEffectEditorFor(IEffect effect);
        public abstract int EffectEditorIndex(IEffect effect);
        public void ResetInstance()
        {
            instance = new EffectEditorFactoryImp();
        }
    }

    public class EffectEditorFactoryImp : EffectEditorFactory
    {
        private readonly List<System.Type> types;
        private readonly List<IEffectEditor> effectEditors;

        private readonly IEnumerable<IEffectEditor> usableEffects;
        private readonly IEnumerable<string> usableEffectNames;

        public EffectEditorFactoryImp()
        {
            var effectEditorType = typeof(IEffectEditor);
            types = System.AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => !p.IsInterface && effectEditorType.IsAssignableFrom(p) && !p.IsAbstract)
                .ToList();

            effectEditors = types.ConvertAll(t => (IEffectEditor)System.Activator.CreateInstance(t));
            usableEffects = effectEditors.Where(e => e.Usable);
            usableEffectNames = usableEffects.Select(e => e.EffectName);
        }

        public override string[] CurrentEffectEditors
        {
            get
            {
                return usableEffectNames.ToArray();
            }
        }


        public override IEffectEditor createEffectEditorFor(string effectName)
        {
            var effectEditor = effectEditors.Find(e => e.EffectName.Equals(effectName, System.StringComparison.InvariantCultureIgnoreCase));
            if (effectEditor != null)
            {
                return effectEditor.clone();
            }
            return null;
        }

        public override IEffectEditor createEffectEditorFor(IEffect effect)
        {
            var effectEditor = effectEditors.Find(e => e.manages(effect));
            if (effectEditor != null)
            {
                return effectEditor.clone();
            }

            return null;
        }

        public override int EffectEditorIndex(IEffect effect)
        {
            return Mathf.Max(0, usableEffects
                .Select((e,i) => new { Editor = e, Index = i})
                .First(e => e.Editor.manages(effect))
                .Index);
        }
    }
}