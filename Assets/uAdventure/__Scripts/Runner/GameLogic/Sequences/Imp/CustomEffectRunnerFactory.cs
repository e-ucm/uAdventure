using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using uAdventure.Core;

namespace uAdventure.Runner
{
    public class CustomEffectRunnerFactory : MonoBehaviour
    {
        // -------------
        // SINGLETON
        // -----------
        private CustomEffectRunnerFactory() { LoadRunners(); }
        private static CustomEffectRunnerFactory instance;
        public static CustomEffectRunnerFactory Instance { get { return instance == null ? instance = new GameObject().AddComponent<CustomEffectRunnerFactory>() : instance; } }

        // ------------------
        // Runner methods
        // ----------------

        private Dictionary<Type, Type> knownRunners; 

        public CustomEffectRunner CreateRunnerFor(IEffect effect)
        {
            CustomEffectRunner r = null;
            if (knownRunners.ContainsKey(effect.GetType()))
            {
                r = (CustomEffectRunner) Activator.CreateInstance(knownRunners[effect.GetType()]);
                r.Effect = effect;
            }

            return  r;
        }

        private void LoadRunners()
        {
            knownRunners = new Dictionary<Type, Type>();
            
            // Make sure is a DOMWriter
            var runnerTypes = GetTypesWith<CustomEffectRunnerAttribute>(true).Where(t => t.GetInterfaces().Contains(typeof(CustomEffectRunner)));
            foreach (var runnerType in runnerTypes)
            {
                foreach (var attr in runnerType.GetCustomAttributes(typeof(CustomEffectRunnerAttribute), true))
                {
                    var dwattr = attr as CustomEffectRunnerAttribute;
                    // Try create an instance with the Activator
                    foreach (var writterType in dwattr.Types)
                        if (!knownRunners.ContainsKey(writterType))
                            knownRunners.Add(writterType, runnerType);
                }
            }
        }

        // -----------------
        // Aux methods
        // --------------
        static IEnumerable<Type> GetTypesWith<TAttribute>(bool inherit)
                              where TAttribute : System.Attribute
        {
            return from a in AppDomain.CurrentDomain.GetAssemblies()
                   from t in a.GetTypes()
                   where t.IsDefined(typeof(TAttribute), inherit)
                   select t;
        }
    }
}