using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using uAdventure.Editor;

namespace uAdventure.Core.Metadata
{
    public static class MetadataUtility
    {
        public static XmlElement CleanXMLGarbage(XmlDocument document, XmlElement xmlElement)
        {
            string finalText = string.Empty;
            using (var sw = new StringWriter())
            using(var xmlWriter = XmlWriter.Create(sw))
            {
                xmlElement.WriteTo(xmlWriter);
                xmlWriter.Flush();
                sw.Flush();
                finalText = sw.ToString();
            }

            var lomTypeRegex = @"[\s\n\r]*<imsmd:[a-z]*[A-Z]*>[\s\n\r]*<imsmd:source>[\s\n\r]*<imsmd:langstring xml:lang=\x22\x22>[\s\n\r]*<\/imsmd:langstring>[\s\n\r]*<\/imsmd:source>[\s\n\r]*<imsmd:value>[\s\n\r]*<imsmd:langstring xml:lang=\x22\x22>[\s\n\r]*<\/imsmd:langstring>[\s\n\r]*<\/imsmd:value>[\s\n\r]*<\/imsmd:[a-z]*[A-Z]*>";
            var emptyLangStringRegex = @"[\s\n\r]*<imsmd:langstring xml:lang=\x22[a-zA-Z]*\x22>[\s\n\r]*<\/imsmd:langstring>";
            var emptyFieldRegex = @"[\s\n\r]*<imsmd:[a-zA-Z]* \/>";
            var emptyFieldOpenRegex = @"[\s\n\r]*<imsmd:[a-zA-Z]*>[\s\n\r]*<\/imsmd:[a-zA-Z]*>";

            foreach (Match match in Regex.Matches(finalText, lomTypeRegex))
            {
                finalText = finalText.Replace(match.Value, "");
            }
            foreach (Match match in Regex.Matches(finalText, emptyLangStringRegex))
            {
                finalText = finalText.Replace(match.Value, "");
            }
            foreach (Match match in Regex.Matches(finalText, emptyFieldRegex))
            {
                finalText = finalText.Replace(match.Value, "");
            }
            string beforeStart;
            do
            {
                beforeStart = finalText;
                foreach (Match match in Regex.Matches(finalText, emptyFieldOpenRegex))
                {
                    finalText = finalText.Replace(match.Value, "");
                }
            } while (beforeStart != finalText);


            XmlDocument doc = new XmlDocument();
            doc.LoadXml(finalText); 
            return (XmlElement)document.ImportNode((XmlElement)doc.DocumentElement, true);
        }



        public static string GenerateManifestIdentifier()
        {
            return "Manifest-" + Controller.Instance.AdventureData.getApplicationIdentifier()
                                    .Replace(".", "-")
                                    .Replace(" ", "");
        }

        public static XmlElement SerializeToXmlElement(object o)
        {
            XmlDocument doc = new XmlDocument();

            using (XmlWriter writer = doc.CreateNavigator().AppendChild())
            {
                var serializer = new XmlSerializer(o.GetType());
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("imsmd", "http://www.imsglobal.org/xsd/imsmd_v1p2");
                serializer.Serialize(writer, o, ns);
            }
            var attr = doc.DocumentElement.GetAttributeNode("xmlns:imsmd");
            doc.DocumentElement.RemoveAttributeNode(attr);
            return doc.DocumentElement;
        }

    }
}
