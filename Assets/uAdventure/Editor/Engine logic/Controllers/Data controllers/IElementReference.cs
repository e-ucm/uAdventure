using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uAdventure.Editor
{
    public interface IElementReference
    {
        DataControl ReferencedDataControl { get; }

        string ReferencedId { get; }
    }
}
