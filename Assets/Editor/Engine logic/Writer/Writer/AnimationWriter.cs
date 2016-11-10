using System;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;

public class AnimationWriter
{


    private AnimationWriter()
    {

    }

    public static bool writeAnimation(string filename, Animation animation)
    {

        bool dataSaved = false;
        XmlDocument doc = doc = new XmlDocument();
        XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", "no");
        XmlDocumentType typeDescriptor = doc.CreateDocumentType("animation", "SYSTEM", "animation.dtd", null);
        doc.AppendChild(declaration);
        doc.AppendChild(typeDescriptor);
        XmlElement mainNode = doc.CreateElement("animation");
        //mainNode.AppendChild(doc.createAttribute("id").setNodeValue(animation.getId()));
        mainNode.SetAttribute("id", animation.getId());
        mainNode.SetAttribute("usetransitions", animation.isUseTransitions() ? "yes" : "no");
        mainNode.SetAttribute("slides", animation.isSlides() ? "yes" : "no");
        XmlElement documentation = doc.CreateElement("documentation");
        if (animation.getDocumentation() != null && animation.getDocumentation().Length > 0)
            documentation.InnerText = animation.getDocumentation();
        mainNode.AppendChild(documentation);

        foreach (ResourcesUni resources in animation.getResources())
        {
            XmlNode resourcesNode = ResourcesDOMWriter.buildDOM(resources, ResourcesDOMWriter.RESOURCES_ANIMATION);
            doc.ImportNode(resourcesNode, true);
            mainNode.AppendChild(resourcesNode);
        }

        for (int i = 0; i < animation.getFrames().Count; i++)
        {
            mainNode.AppendChild(createTransitionElement(animation.getTransitions()[i], doc));
            mainNode.AppendChild(createFrameElement(animation.getFrames()[i], doc));
        }
        mainNode.AppendChild(createTransitionElement(animation.getEndTransition(), doc));

        doc.ImportNode(mainNode, true);
        doc.AppendChild(mainNode);
        string name = "Assets/Resources/CurrentGame/" + filename;
        if (!name.EndsWith(".eaa"))
            name += ".eaa";
        doc.Save(name);
        System.IO.File.Copy(name, name.Substring(0, name.LastIndexOf(".")) + ".xml", true);
        //TODO: implementation?
        //transformer = tf.newTransformer();
        //transformer.setOutputProperty(OutputKeys.DOCTYPE_SYSTEM, "animation.dtd");

        //try
        //{
        //    fout = new FileOutputStream(filename);
        //}
        //catch (FileNotFoundException e)
        //{
        //    fout = new FileOutputStream(Controller.getInstance().getProjectFolder() + "/" + filename);
        //}

        //writeFile = new OutputStreamWriter(fout, "UTF-8");
        //transformer.transform(new DOMSource(doc), new StreamResult(writeFile));
        //writeFile.close();
        //fout.close();

        dataSaved = true;

        return dataSaved;
    }

    private static XmlElement createTransitionElement(Transition t, XmlDocument doc)
    {

        XmlElement element = doc.CreateElement("transition");

        if (t.getType() == Transition.TYPE_NONE)
            element.SetAttribute("type", "none");
        else if (t.getType() == Transition.TYPE_FADEIN)
            element.SetAttribute("type", "fadein");
        else if (t.getType() == Transition.TYPE_HORIZONTAL)
            element.SetAttribute("type", "horizontal");
        else if (t.getType() == Transition.TYPE_VERTICAL)
            element.SetAttribute("type", "vertical");

        element.SetAttribute("time", "" + t.getTime());

        return element;
    }

    public static XmlElement createFrameElement(Frame f, XmlDocument doc)
    {

        XmlElement element = doc.CreateElement("frame");

        element.SetAttribute("uri", (f.getUri() != null ? f.getUri() : ""));

        if (f.getType() == Frame.TYPE_IMAGE)
            element.SetAttribute("type", "image");
        else if (f.getType() == Frame.TYPE_VIDEO)
            element.SetAttribute("type", "video");

        element.SetAttribute("time", f.getTime().ToString());

        element.SetAttribute("waitforclick", (f.isWaitforclick() ? "yes" : "no"));

        element.SetAttribute("soundUri", (f.getSoundUri() != null ? f.getSoundUri() : ""));

        element.SetAttribute("maxSoundTime", f.getMaxSoundTime().ToString());

        return element;
    }
}