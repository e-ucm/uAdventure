using System.Xml;

namespace uAdventure.Editor
{
    public interface IDOMWriter
    {
        void BuildDOM(XmlNode parent, object target, params IDOMWriterParam[] options);
    }

    public interface IDOMWriterParam
    {

    }
}