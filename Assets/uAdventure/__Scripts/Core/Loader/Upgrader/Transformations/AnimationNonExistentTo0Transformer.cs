using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using uAdventure.Runner;
using UnityEngine;
using UnityFx.Async;

namespace uAdventure.Core.XmlUpgrader
{
    public class AnimationNonExistentTo0Transformer : ITransformer
    {
        public string TargetFile
        {
            get
            {
                return "*animation/*";
            }
        }

        public int TargetVersion { get { return -1; } }

        public int DestinationVersion { get { return 0; } }

        public string Upgrade(string input, string path, ResourceManager resourceManager)
        {
            var id = path.Split('/').Last();

            var frames = new List<string>();
            var frameTypes = new List<int>();

            int num = 1;
            string ruta;
            Texture2D img;

            do
            {
                ruta = path + "_" + IntToStr(num);
                img = resourceManager.getImage(ruta);
                if (img)
                {
                    frames.Add(ruta);
                    frameTypes.Add(Frame.TYPE_IMAGE);
                    num++;
                }

            } while (img);

            var animationXml = CreateAnimationXml(id, frames, frameTypes);
            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                animationXml.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();
                return stringWriter.GetStringBuilder().ToString();
            }
        }

        /*public IAsyncOperation<string> UpgradeAsync(string input, string path, ResourceManager resourceManager)
        {
            var id = path.Split('/').Last();

            var frames = new List<string>();
            var frameTypes = new List<int>();

            int num = 1;
            string ruta;
            Texture2D img;

            do
            {
                ruta = path + "_" + IntToStr(num);
                img = resourceManager.getImage(ruta);
                if (img)
                {
                    frames.Add(ruta);
                    frameTypes.Add(Frame.TYPE_IMAGE);
                    num++;
                }

            } while (img);

            var animationXml = CreateAnimationXml(id, frames, frameTypes);
            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                animationXml.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();
                return stringWriter.GetStringBuilder().ToString();
            }
        }*/


        public static XmlDocument CreateAnimationXml(string id, List<string> frames, List<int> frameTypes)
        {
            var doc = new XmlDocument();

            // Declaration, encoding, version, and dtd
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", "no"));
            //doc.AppendChild(doc.CreateDocumentType("animation", "SYSTEM", "animation.dtd", null));

            // Main animation node
            XmlElement mainNode = doc.CreateElement("animation");
            mainNode.SetAttribute("version", "0");
            mainNode.SetAttribute("id", id);
            mainNode.SetAttribute("usetransitions", "no");
            mainNode.SetAttribute("slides", "no");

            // Documentation node
            XmlElement documentation = doc.CreateElement("documentation");
            documentation.InnerText = "Animation created by upgrader";
            mainNode.AppendChild(documentation);

            // Frames
            for (int i = 0; i < frames.Count; i++)
            {
                mainNode.AppendChild(CreateEmptyTransitionElement(doc));
                mainNode.AppendChild(CreateFrameElement(frames[i], frameTypes[i], doc));
            }
            mainNode.AppendChild(CreateEmptyTransitionElement(doc));

            doc.ImportNode(mainNode, true);
            doc.AppendChild(mainNode);

            return doc;
        }

        public static XmlElement CreateFrameElement(string frameUri, int frameType, XmlDocument doc)
        {
            XmlElement element = doc.CreateElement("frame");

            element.SetAttribute("uri", frameUri);

            if (frameType == Frame.TYPE_IMAGE)
            {
                element.SetAttribute("type", "image");
            }
            else if (frameType == Frame.TYPE_VIDEO)
            {
                element.SetAttribute("type", "video");
            }

            element.SetAttribute("time", "100");
            element.SetAttribute("waitforclick",  "no");
            element.SetAttribute("soundUri",  "");

            var documentation = doc.CreateElement("documentation");
            documentation.InnerText = "Created automatically by the upgrader";
            element.AppendChild(documentation);

            return element;
        }

        public static XmlElement CreateEmptyTransitionElement(XmlDocument doc)
        {
            XmlElement element = doc.CreateElement("transition");

            element.SetAttribute("type", "none");
            element.SetAttribute("time", "0");

            return element;
        }

        private static string IntToStr(int number)
        {
            return number.ToString("D2");
        }
    }
}
