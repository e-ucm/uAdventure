using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace uAdventure.Editor
{
    public class EditorWindowBaseExtensionFactory 
    {

        private static EditorWindowBaseExtensionFactory instance;
        public static EditorWindowBaseExtensionFactory Instance { get { return instance == null ? instance = new EditorWindowBaseExtensionFactory() : instance; } }

        protected EditorWindowBaseExtensionFactory()
        {
        }



        public EditorWindowExtension CreateExtensionFor(Type type, Rect windowRect, GUIStyle style, params GUILayoutOption[] options)
        {
            LoadExtensions();
            return (EditorWindowExtension) Activator.CreateInstance(knownExtensions[type], windowRect, style, options);
        }

        public List<EditorWindowExtension> CreateAllExistingExtensions(Rect windowRect, GUIStyle style, params GUILayoutOption[] options)
        {
            LoadExtensions();
            var r = new List<EditorWindowExtension>();
            foreach (var t in knownExtensions.Values.Distinct().OrderBy(t => priorities[t]))
            {
                r.Add((EditorWindowExtension)Activator.CreateInstance(t, windowRect, style, options));
            }

            return r;
        }

        public EditorWindowExtension GetExistingExtensionFor(Type type, List<EditorWindowExtension> existingExtensions)
        {
            if (knownExtensions.ContainsKey(type))
            {
                return existingExtensions.Find(e => e.GetType() == knownExtensions[type]);
            }
            else
            {
                return null;
            }
        }

        private static Dictionary<Type, Type> knownExtensions;
        private static Dictionary<Type, int> priorities;

        private static void LoadExtensions()
        {
            if (knownExtensions == null)
            {
                knownExtensions = new Dictionary<Type, Type>();
                priorities = new Dictionary<Type, int>();

            }
            else
            {
                knownExtensions.Clear();
                priorities.Clear();
            }

            // Make sure is a DOMWriter
            var extensions = GetTypesWith<EditorWindowExtensionAttribute>(true).Where(t => typeof(EditorWindowExtension).IsAssignableFrom(t));
            foreach (var extension in extensions)
            {
                foreach (var attr in extension.GetCustomAttributes(typeof(EditorWindowExtensionAttribute), true))
                {
                    var ewea = attr as EditorWindowExtensionAttribute;
                    priorities.Add(extension, ewea.Priority);
                    foreach (var extensionType in ewea.Types)
                        if (!knownExtensions.ContainsKey(extensionType))
                            knownExtensions.Add(extensionType, extension);
                }
            }
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