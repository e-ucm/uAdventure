using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace uAdventure.Core.XmlUpgrader
{
    internal class Chapter1To2Transformer : AbstractXsltTransformer
    {        
        public override string TargetFile { get { return "chapter*.xml"; } }

        public override int TargetVersion { get { return 1; } }

        public override int DestinationVersion { get { return 2; } }

        protected override string XsltFile { get { return "Upgrader/chapter-v1-to-v2"; } }
    }
}
