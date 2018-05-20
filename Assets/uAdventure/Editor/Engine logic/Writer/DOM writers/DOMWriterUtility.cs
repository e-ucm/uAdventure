using System;
using System.Xml;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Editor
{
    public class DOMWriterUtility
    {
        // ----------------
        // Param classes
        // --------------
        public class NameParam : IDOMWriterParam
        {
            private string name;
            public NameParam(string name)
            {
                this.name = name;
            }

            public string Name { get { return name; } }
        }

        public class DontCreateElementParam : IDOMWriterParam { }


        // --------------------
        // Param creators
        // -------------------
        public static IDOMWriterParam Name(string name)
        {
            return new NameParam(name);
        }


        public static IDOMWriterParam DontCreateElement()
        {
            return new DontCreateElementParam();
        }


        // -----------------
        //  Operations
        // -----------------

        public static void DOMWrite(XmlNode parent, object o, params IDOMWriterParam[] options)
        {
            var writter = GetWritterFor(o);

            if(writter != null)
            {
                writter.BuildDOM(parent, o, options);
            }
            else if(o is IDictionary)
            {
                DOMWriteDictionary(parent, o as IDictionary, options);
            }
            else if (o is IEnumerable)
            {
                DOMWriteEnumerable(parent, o as IEnumerable, options);
            }
        }

        private static void DOMWriteDictionary(XmlNode parent, IDictionary d, params IDOMWriterParam[] options)
        {
            foreach (var e in d.Keys)
            {
                var child = Writer.GetDoc().CreateElement(e.ToString());
                parent.AppendChild(child);
                DOMWrite(child, d[e], options);
            }
        }

        private static void DOMWriteEnumerable(XmlNode parent, IEnumerable e, params IDOMWriterParam[] options)
        {
            foreach (var o in e)
            {
                DOMWrite(parent, o, options);
            }
        }

        // ------------------
        // Aux methods
        // ------------------

        private static Dictionary<Type, IDOMWriter> knownWritters;

        private static IDOMWriter GetWritterFor(object o)
        {
            if (knownWritters == null)
                knownWritters = new Dictionary<Type, IDOMWriter>();

            if (!knownWritters.ContainsKey(o.GetType()))
            {
                // Make sure is a DOMWriter
                var writers = GetTypesWith<DOMWriterAttribute>(true).Where(t => t.GetInterfaces().Contains(typeof(IDOMWriter)));
                foreach(var writer in writers)
                {
                    foreach (var attr in writer.GetCustomAttributes(typeof(DOMWriterAttribute), true))
                    {
                        var dwattr = attr as DOMWriterAttribute;
                        // Try create an instance with the Activator
                        var instance = (IDOMWriter)Activator.CreateInstance(writer);
                        foreach (var writterType in dwattr.Types)
                            if(!knownWritters.ContainsKey(writterType))
                                knownWritters.Add(writterType, instance);
                    }
                }
            }
            return knownWritters.ContainsKey(o.GetType()) ? knownWritters[o.GetType()] : null;
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
