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

        public static object DOMParse(XmlNode el)
        {
            throw new NotImplementedException();
        }

        // ------------------
        // Aux methods
        // ------------------

        private static Dictionary<Type, Subparser_> knownTypeParsers;
        private static Dictionary<string, Subparser_> knownNameParsers;

        private static Subparser_ GetParserFor(string nodename)
        {
            init();

            if (!knownNameParsers.ContainsKey(nodename))
            {
                fillParsers();
            }

            return knownNameParsers.ContainsKey(nodename) ? knownNameParsers[nodename] : null;
        }

        private static Subparser_ GetParserFor(object o)
        {
            init();

            if (!knownTypeParsers.ContainsKey(o.GetType()))
            {
                fillParsers();
            }

            return knownTypeParsers.ContainsKey(o.GetType()) ? knownTypeParsers[o.GetType()] : null;
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
                knownTypeParsers = new Dictionary<Type, Subparser_>();

            if (knownNameParsers == null)
                knownNameParsers = new Dictionary<string, Subparser_>();
        }

        private static void fillParsers()
        {
            // Make sure is a DOMWriter
            var writers = GetTypesWith<DOMParserAttribute>(true).Where(t => t.IsAssignableFrom(typeof(Subparser_)));
            foreach (var writer in writers)
            {
                foreach (var attr in writer.GetCustomAttributes(typeof(DOMParserAttribute), true))
                {
                    var dwattr = attr as DOMParserAttribute;
                    // Try create an instance with the Activator
                    var instance = (Subparser_)Activator.CreateInstance(writer);

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