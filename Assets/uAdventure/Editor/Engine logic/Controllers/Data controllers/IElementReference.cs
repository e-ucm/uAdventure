
using uAdventure.Core;

namespace uAdventure.Editor
{
    public interface IElementReference
    {
        DataControl ReferencedDataControl { get; }

        string ReferencedId { get; }

        Orientation Orientation { get; set; }
    }
}
