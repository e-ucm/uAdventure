using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace uAdventure.Core.XmlUpgrader
{
    internal class Chapter0To1Transformer : AbstractXsltTransformer
    {        
        public override string TargetFile { get { return "chapter*.xml"; } }

        public override int TargetVersion { get { return 0; } }

        public override int DestinationVersion { get { return 1; } }

        protected override string XsltFile { get { return "Upgrader/chapter-v0-to-v1"; } }
    }
}
