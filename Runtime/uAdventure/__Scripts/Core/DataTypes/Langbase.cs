using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace uAdventure.Core
{
    [XmlRoot(ElementName = "available")]
    public class LangbaseAvailable
    {
        [XmlAttribute(AttributeName = "lang")]
        public string Lang { get; set; }
    }

    [XmlRoot(ElementName = "languages")]
    public class LangbaseLanguages
    {
        [XmlElement(ElementName = "available")]
        public List<LangbaseAvailable> Available { get; set; }
    }

    [XmlRoot(ElementName = "text")]
    public class LangbaseText
    {
        [XmlAttribute(AttributeName = "lang")]
        public string Lang { get; set; }
        [XmlText]
        public string TextValue { get; set; }
    }

    [XmlRoot(ElementName = "label")]
    public class LangbaseLabel
    {
        [XmlElement(ElementName = "text")]
        public List<LangbaseText> Text { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "labels")]
    public class LangbaseLabels
    {
        [XmlElement(ElementName = "label")]
        public List<LangbaseLabel> Label { get; set; }
    }

    /// <summary>
    /// Struktura bazy danych pliku tekstoweog niezbędna do  
    /// deserializacji pliku xml z tłumaczeniami na klasy w języku C#. 
    /// </summary>

    [XmlRoot(ElementName = "root")]
    public class LangbaseRoot
    {
        [XmlElement(ElementName = "languages")]
        public LangbaseLanguages Languages { get; set; }
        [XmlElement(ElementName = "labels")]
        public LangbaseLabels Labels { get; set; }

        public static LangbaseRoot LoadFromText(string text)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(LangbaseRoot));
            return serializer.Deserialize(new StringReader(text)) as LangbaseRoot;
        }
    }
}