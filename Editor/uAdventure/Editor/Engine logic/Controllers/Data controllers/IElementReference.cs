
using uAdventure.Core;

namespace uAdventure.Editor
{
    public interface IElementReference
    {
        DataControl ReferencedDataControl { get; }

        string ReferencedId { get; }

        bool UsesOrientation { get; }

        Orientation Orientation { get; set; }

        float Scale { get; set; }
    }
}
