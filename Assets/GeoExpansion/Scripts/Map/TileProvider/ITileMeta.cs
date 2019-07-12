using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uAdventure.Geo
{
    public interface ITileMeta
    {
        string Identifier { get; }
        string Name { get; }
        string Description { get; }
        string[] Attributes { get; }
        object this[string attribute] { get; }
    }
}
