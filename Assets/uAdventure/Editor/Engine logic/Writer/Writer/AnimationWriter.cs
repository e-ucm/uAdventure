using System.Xml;

using uAdventure.Core;
using UnityEngine;
using Animation = uAdventure.Core.Animation;

namespace uAdventure.Editor
{
    public class AnimationWriter
    {


        private AnimationWriter()
        {

        }

        public static bool WriteAnimation(string filename, Animation animation)
        {
            bool dataSaved = false;
            XmlDocument doc = new XmlDocument();
            
            // Declaration, encoding, version, etc
            XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", "no");
            doc.AppendChild(declaration);

            // DTD
            //XmlDocumentType typeDescriptor = doc.CreateDocumentType("animation", "SYSTEM", "animation.dtd", null);
            //doc.AppendChild(typeDescriptor);

            // Main animation node
            XmlElement mainNode = doc.CreateElement("animation");
            mainNode.SetAttribute("id", animation.getId());
            mainNode.SetAttribute("usetransitions", animation.isUseTransitions() ? "yes" : "no");
            mainNode.SetAttribute("slides", animation.isSlides() ? "yes" : "no");

            // Documentation node
            XmlElement documentation = doc.CreateElement("documentation");
            if (animation.getDocumentation() != null && animation.getDocumentation().Length > 0)
            {
                documentation.InnerText = animation.getDocumentation();
            }

            mainNode.AppendChild(documentation);

            // Resources in this animation
            foreach (ResourcesUni resources in animation.getResources())
            {
                // TODO update to domwriter resource
                XmlNode resourcesNode = ResourcesDOMWriter.buildDOM(resources, ResourcesDOMWriter.RESOURCES_ANIMATION);
                doc.ImportNode(resourcesNode, true);
                mainNode.AppendChild(resourcesNode);
            }
            
            // Frames and transitions 
            // TODO update to DOMWriter
            for (int i = 0; i < animation.getFrames().Count; i++)
            {
                mainNode.AppendChild(createTransitionElement(animation.getTransitions()[i], doc));
                mainNode.AppendChild(createFrameElement(animation.getFrames()[i], doc));
            }
            mainNode.AppendChild(createTransitionElement(animation.getEndTransition(), doc));
            doc.ImportNode(mainNode, true);
            doc.AppendChild(mainNode);

            // File saving
            string name = "Assets/uAdventure/Resources/CurrentGame/" + filename;
            if (!name.EndsWith(".eaa.xml"))
            {
                name += ".eaa.xml";
            }

            try
            {
                // Save
                doc.Save(name);
                dataSaved = true;
            }
            catch(System.Exception ex)
            {
                Debug.Log("Couldn't save Animation file \"" + name + "\": "+ ex.Message);
            }

            return dataSaved;
        }

        private static XmlElement createTransitionElement(Transition t, XmlDocument doc)
        {
            var element = doc.CreateElement("transition");
            element.SetAttribute("type", ((int)t.getType()).ToString());
            element.SetAttribute("time", "" + t.getTime());
            return element;
        }

        public static XmlElement createFrameElement(Frame f, XmlDocument doc)
        {

            XmlElement element = doc.CreateElement("frame");

            element.SetAttribute("uri", (f.getUri() != null ? f.getUri() : ""));

            if (f.getType() == Frame.TYPE_IMAGE)
            {
                element.SetAttribute("type", "image");
            }
            else if (f.getType() == Frame.TYPE_VIDEO)
            {
                element.SetAttribute("type", "video");
            }

            element.SetAttribute("time", f.getTime().ToString());

            element.SetAttribute("waitforclick", (f.isWaitforclick() ? "yes" : "no"));

            element.SetAttribute("soundUri", (f.getSoundUri() != null ? f.getSoundUri() : ""));

            element.SetAttribute("maxSoundTime", f.getMaxSoundTime().ToString());

            var documentation = doc.CreateElement("documentation");
            documentation.InnerText = f.getDocumentation();
            element.AppendChild(documentation);

            return element;
        }
    }
}