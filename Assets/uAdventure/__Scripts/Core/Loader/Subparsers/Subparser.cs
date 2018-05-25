using UnityEngine;
using System.Collections;
using System.Xml;

namespace uAdventure.Core
{
    public abstract class Subparser
    {

        protected Chapter chapter;

        /**
         * Constructor.
         */
        public Subparser(Chapter chapter)
        {
            this.chapter = chapter;
        }

        public abstract void ParseElement(XmlElement element);
    }
}