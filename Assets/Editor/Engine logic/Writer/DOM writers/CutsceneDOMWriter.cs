using System;
using UnityEngine;
using System.Collections;
using System.Xml;

public class CutsceneDOMWriter
{
    /**
        * Private constructor.
        */

    private CutsceneDOMWriter()
    {

    }

    public static XmlNode buildDOM(Cutscene cutscene, bool initialScene)
    {

        XmlElement cutsceneElement = null;


        // Create the necessary elements to create the DOM
        XmlDocument doc = Writer.GetDoc();

        // Create the root node
        if (cutscene.getType() == GeneralScene.GeneralSceneSceneType.SLIDESCENE)
            cutsceneElement = doc.CreateElement("slidescene");
        else if (cutscene.getType() == GeneralScene.GeneralSceneSceneType.VIDEOSCENE)
        {
            cutsceneElement = doc.CreateElement("videoscene");
            if (((Videoscene) cutscene).isCanSkip())
                cutsceneElement.SetAttribute("canSkip", "yes");
            else
                cutsceneElement.SetAttribute("canSkip", "no");
        }

        // Set the attributes
        cutsceneElement.SetAttribute("id", cutscene.getId());
        if (initialScene)
            cutsceneElement.SetAttribute("start", "yes");
        else
            cutsceneElement.SetAttribute("start", "no");

        if (cutscene.getNext() == Cutscene.NEWSCENE)
        {
            cutsceneElement.SetAttribute("idTarget", cutscene.getTargetId());

            cutsceneElement.SetAttribute("destinyX", cutscene.getPositionX().ToString());
            cutsceneElement.SetAttribute("destinyY", cutscene.getPositionY().ToString());

            cutsceneElement.SetAttribute("transitionTime", cutscene.getTransitionTime().ToString());
            cutsceneElement.SetAttribute("transitionType", ((int) cutscene.getTransitionType()).ToString());
        }

        if (cutscene.getNext() == Cutscene.GOBACK)
            cutsceneElement.SetAttribute("next", "go-back");
        else if (cutscene.getNext() == Cutscene.ENDCHAPTER)
            cutsceneElement.SetAttribute("next", "end-chapter");
        else if (cutscene.getNext() == Cutscene.NEWSCENE)
            cutsceneElement.SetAttribute("next", "new-scene");

        cutsceneElement.SetAttribute("class", cutscene.getXApiClass());
        cutsceneElement.SetAttribute("type", cutscene.getXApiType());

        // Append the documentation (if avalaible)
        if (cutscene.getDocumentation() != null)
        {
            XmlNode cutsceneDocumentationNode = doc.CreateElement("documentation");
            cutsceneDocumentationNode.AppendChild(doc.CreateTextNode(cutscene.getDocumentation()));
            cutsceneElement.AppendChild(cutsceneDocumentationNode);
        }

        if (!cutscene.getEffects().isEmpty())
        {
            XmlNode effectsNode = EffectsDOMWriter.buildDOM(EffectsDOMWriter.EFFECTS, cutscene.getEffects());
            doc.ImportNode(effectsNode, true);
            cutsceneElement.AppendChild(effectsNode);
        }

        // Append the resources
        foreach (ResourcesUni resources in cutscene.getResources())
        {
            XmlNode resourcesNode = ResourcesDOMWriter.buildDOM(resources, ResourcesDOMWriter.RESOURCES_CUTSCENE);
            doc.ImportNode(resourcesNode, true);
            cutsceneElement.AppendChild(resourcesNode);
        }

        // Append the name
        XmlNode nameNode = doc.CreateElement("name");
        nameNode.AppendChild(doc.CreateTextNode(cutscene.getName()));
        cutsceneElement.AppendChild(nameNode);

        return cutsceneElement;
    }
}