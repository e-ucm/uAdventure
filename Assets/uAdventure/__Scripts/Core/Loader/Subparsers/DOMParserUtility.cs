using UnityEngine;
using System.Collections;
using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;

namespace uAdventure.Core
{
    public class DOMParserUtility
	{
		/// <summary>
		/// Parses the element based on the avaliable parsers for the node name.
		/// </summary>
		/// <param name="el">Element to extract info from</param>
		/// <returns>An object returned by a IDOMParser that can handle the node</returns>
		public static object DOMParse(XmlNode el, params object[] parameters)
		{
			if (el is XmlElement)
				return DOMParse (el as XmlElement, parameters);

			return null;
		}

        /// <summary>
        /// Parses the element based on the avaliable parsers for the node name.
        /// </summary>
        /// <param name="el">Element to extract info from</param>
        /// <returns>An object returned by a IDOMParser that can handle the node</returns>
        public static object DOMParse(XmlElement el, params object[] parameters)
        {
			if (el == null)
				return null;

            var parser = GetParserFor(el.Name);
            if (parser != null) return parser.DOMParse(el, parameters);
            else
            {
                if (el.Name == "adaptation" || el.Name == "assessment")
                    return null;

                Debug.LogWarning("Parser not found for the desired name: " + el.Name);
                return null;
            }
        }

        /// <summary>
        /// DOMParse all the elements by just using the element name
        /// </summary>
        /// <param name="nl">Nodes to parse</param>
        /// <returns>A list of elements of any type</returns>
        public static IList DOMParse(XmlNodeList nl, params object[] parameters)
        {
            ArrayList parsed = new ArrayList();
            foreach(var o in nl)
            {
                parsed.Add(DOMParse(o as XmlElement, parameters));
            }
            return parsed;
        }

		/// <summary>
		/// Parses the element based on the avaliable parsers for the node name.
		/// </summary>
		/// <param name="el">Element to extract info from</param>
		/// <returns>An object returned by a IDOMParser that can handle the node</returns>
		public static T DOMParse<T>(XmlNode el, params object[] parameters)
		{
			return DOMParse<T>(el as XmlElement, parameters);
		}

        /// <summary>
        /// DOMParse by using only parsers of type T
        /// </summary>
        /// <typeparam name="T">Type of the desired output object and parser used</typeparam>
        /// <param name="el">node that contains the data to parse from</param>
        /// <returns>Parsed element</returns>
		public static T DOMParse<T>(XmlElement el, params object[] parameters)
        {
			if (el == null)
				return default(T);
			
            var parser = GetParserFor(typeof(T));
            if (parser != null) return (T)parser.DOMParse(el, parameters);
            else throw new Exception("Parser not found for the desired type: "+ typeof(T));
        }

        /// <summary>
        /// DOMParses a list of nodes returning the same type of elements
        /// </summary>
        /// <typeparam name="T">Type of the desired output object list and elements to select the parser</typeparam>
        /// <param name="nl">list of nodes to parse from</param>
        /// <returns>A list of elements of type T parsed from xml</returns>
        public static IList<T> DOMParse<T>(XmlNodeList nl, params object[] parameters)
        {
            List<T> parsed = new List<T>();
            foreach(var o in nl)
            {
                parsed.Add(DOMParse<T>(o as XmlElement, parameters));
            }

            return parsed;
        }

        // ------------------
        // Aux methods
        // ------------------

        private static Dictionary<Type, IDOMParser> knownTypeParsers;
        private static Dictionary<string, IDOMParser> knownNameParsers;

        private static IDOMParser GetParserFor(string nodename)
        {
            init();

            if (!knownNameParsers.ContainsKey(nodename))
            {
                fillParsers();
            }

            return knownNameParsers.ContainsKey(nodename) ? knownNameParsers[nodename] : null;
        }

        private static IDOMParser GetParserFor(Type t)
        {
            init();

            if (!knownTypeParsers.ContainsKey(t))
            {
                fillParsers();
            }

            return knownTypeParsers.ContainsKey(t) ? knownTypeParsers[t] : null;
        }

        static IEnumerable<Type> GetTypesWith<TAttribute>(bool inherit)
                              where TAttribute : System.Attribute
        {
            return from a in AppDomain.CurrentDomain.GetAssemblies()
                   from t in a.GetTypes()
                   where t.IsDefined(typeof(TAttribute), inherit)
                   select t;
        }

        private static void init()
        {
            if (knownTypeParsers == null)
                knownTypeParsers = new Dictionary<Type, IDOMParser>();

            if (knownNameParsers == null)
                knownNameParsers = new Dictionary<string, IDOMParser>();
        }

        private static void fillParsers()
        {
            // Make sure is a DOMWriter
            var parsers = GetTypesWith<DOMParserAttribute>(true).Where(t => t.GetInterfaces().Contains(typeof(IDOMParser)));
            foreach (var parser in parsers)
            {
                // Try create an instance with the Activator
                var instance = (IDOMParser)Activator.CreateInstance(parser);

                foreach (var attr in parser.GetCustomAttributes(typeof(DOMParserAttribute), true))
                {
                    var dwattr = attr as DOMParserAttribute;
                    
                    foreach (var writterType in dwattr.Types)
                        if (!knownTypeParsers.ContainsKey(writterType))
                            knownTypeParsers.Add(writterType, instance);

                    foreach (var writterType in dwattr.Names)
                        if (!knownNameParsers.ContainsKey(writterType))
                            knownNameParsers.Add(writterType, instance);
                }
            }
        }
    }
}