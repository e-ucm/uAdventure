using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uAdventure.Geo
{
    public abstract class SimpleTileMeta : ITileMeta
    {
        private readonly string identifier;
        private readonly string name;
        private readonly string description;
        private readonly string[] attributes;
        private readonly Dictionary<string, object> attributeValues;

        protected SimpleTileMeta(string identifier, string name, string description, Dictionary<string, object> attributeValues)
        {
            this.identifier = identifier;
            this.name = name;
            this.description = description;
            this.attributeValues = attributeValues;
            this.attributes = attributeValues.Keys.ToArray();
        }

        public object this[string attribute]
        {
            get { return attributeValues.ContainsKey(attribute) ? attributeValues[attribute] : null; }
        }

        public string Identifier
        {
            get { return identifier; }
        }

        public string Name
        {
            get { return name; }
        }

        public string Description
        {
            get { return description; }
        }

        public string[] Attributes
        {
            get { return attributes; }
        }
    }
}
