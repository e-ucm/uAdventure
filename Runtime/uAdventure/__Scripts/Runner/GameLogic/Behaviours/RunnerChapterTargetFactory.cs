using UnityEngine;
using System.Collections.Generic;
using System;

using uAdventure.Core;
using System.Linq;

namespace uAdventure.Runner
{
    public class RunnerChapterTargetFactory
    {
        private static RunnerChapterTargetFactory instance;

        // Private constructor
        private RunnerChapterTargetFactory()
        {

        }

        // Singleton
        public static RunnerChapterTargetFactory Instance {
            get { return instance != null ? instance : instance = new RunnerChapterTargetFactory(); }
        }

        // Main method

        public IRunnerChapterTarget Instantiate(IChapterTarget target)
        {
            return getFactories()[target.GetType()].Instantiate(target);
        }

        private GameObject factoryHolder;
        private Dictionary<Type, IChapterTargetFactory> factories;
        private Dictionary<Type, IChapterTargetFactory> getFactories()
        {
            if(factories == null)
            {
                factoryHolder = new GameObject("FactoryHolder");
                GameObject.DontDestroyOnLoad(factoryHolder);

                factories = new Dictionary<Type, IChapterTargetFactory>();
                foreach(var type in GetTypesWith<ChapterTargetFactoryAttribute>(true))
                {
                    if (!type.GetInterfaces().Contains(typeof(IChapterTargetFactory)))
                        continue;

                    // Is something out there??
                    IChapterTargetFactory runningInstance = (IChapterTargetFactory) GameObject.FindObjectOfType(type);
                    if (runningInstance == null)
                    {
                        if (typeof(MonoBehaviour).IsAssignableFrom(type))
                        {// Chance monobehaviour
                            runningInstance = (IChapterTargetFactory) factoryHolder.AddComponent(type);
                        }
                        else
                        { // Last chance...
                            runningInstance = (IChapterTargetFactory)Activator.CreateInstance(type);
                        }

                    }

                    foreach (var a in type.GetCustomAttributes(typeof(ChapterTargetFactoryAttribute), true))
                    {
                        var cta = a as ChapterTargetFactoryAttribute;
                        foreach (var canCreate in cta.Types)
                        {
                            factories.Add(canCreate, runningInstance);
                            // According to IChapterTargetFactory it has to have Instance method
                            //factories.Add(canCreate, (IChapterTargetFactory)type.GetProperty("Instance").GetGetMethod().Invoke(null, null));
                        }
                    }
                }
            }

            return factories;
        }

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


