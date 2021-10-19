using System;
using UnityEngine;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;
using System.Linq;

using uAdventure.Core;
using System.Globalization;

namespace uAdventure.Editor
{
    using CIP = ChapterDOMWriter.ChapterTargetIDParam;

    [DOMWriter(typeof(Scene))]
    public class SceneDOMWriter : ParametrizedDOMWriter
    {
        /**
            * Private constructor.
            */

        public SceneDOMWriter()
        {

        }

        protected override string GetElementNameFor(object target)
        {
            return "scene";
        }

        protected override void FillNode(XmlNode node, object target, params IDOMWriterParam[] options)
        {
            var sceneElement = node as XmlElement;
            var scene = target as Scene;

            if (scene != null)
            {
                TrajectoryFixer.fixTrajectory(scene);
            }
            // Create the necessary elements to create the DOM
            XmlDocument doc = Writer.GetDoc();
            // Create the root node
            sceneElement.SetAttribute("id", scene.getId());
            sceneElement.SetAttribute("hideInventory", scene.HideInventory ? "yes" : "no");
            sceneElement.SetAttribute("allowsZoom", scene.AllowsZoom ? "yes" : "no");
            sceneElement.SetAttribute("allowsSavingGame", scene.allowsSavingGame() ? "yes" : "no");

            if (options.Any(o => o is CIP && (o as CIP).TargetId.Equals(scene.getId())))
                sceneElement.SetAttribute("start", "yes");
            else
                sceneElement.SetAttribute("start", "no");

            sceneElement.SetAttribute("playerLayer", scene.getPlayerLayer().ToString());
            sceneElement.SetAttribute("playerScale", scene.getPlayerScale().ToString(CultureInfo.InvariantCulture));

            sceneElement.SetAttribute("class", scene.getXApiClass());
            sceneElement.SetAttribute("type", scene.getXApiType());

            // Append the documentation (if avalaible)
            if (scene.getDocumentation() != null)
            {
                XmlNode sceneDocumentationNode = doc.CreateElement("documentation");
                sceneDocumentationNode.AppendChild(doc.CreateTextNode(scene.getDocumentation()));
                sceneElement.AppendChild(sceneDocumentationNode);
            }

            // Append the resources
            foreach (ResourcesUni resources in scene.getResources())
            {
                XmlNode resourcesNode = ResourcesDOMWriter.buildDOM(resources, ResourcesDOMWriter.RESOURCES_SCENE);
                doc.ImportNode(resourcesNode, true);
                sceneElement.AppendChild(resourcesNode);
            }

            // Append the name
            XmlNode nameNode = doc.CreateElement("name");
            nameNode.AppendChild(doc.CreateTextNode(scene.getName()));
            sceneElement.AppendChild(nameNode);

            // Append the default inital position (if avalaible)
            if (scene.hasDefaultPosition())
            {
                XmlElement initialPositionElement = doc.CreateElement("default-initial-position");
                initialPositionElement.SetAttribute("x", scene.getPositionX().ToString());
                initialPositionElement.SetAttribute("y", scene.getPositionY().ToString());
                sceneElement.AppendChild(initialPositionElement);
            }

            // Append the exits (if there is at least one)
            if (scene.getExits().Count > 0)
            {
                XmlNode exitsElement = doc.CreateElement("exits");

                // Append every single exit
                foreach (Exit exit in scene.getExits())
                {
                    // Create the exit element
                    XmlElement exitElement = doc.CreateElement("exit");
                    exitElement.SetAttribute("rectangular", (exit.isRectangular() ? "yes" : "no"));
                    exitElement.SetAttribute("x", exit.getX().ToString());
                    exitElement.SetAttribute("y", exit.getY().ToString());
                    exitElement.SetAttribute("width", exit.getWidth().ToString());
                    exitElement.SetAttribute("height", exit.getHeight().ToString());
                    exitElement.SetAttribute("hasInfluenceArea", (exit.getInfluenceArea().isExists() ? "yes" : "no"));
                    exitElement.SetAttribute("idTarget", exit.getNextSceneId());
                    exitElement.SetAttribute("destinyY", exit.getDestinyY().ToString());
                    exitElement.SetAttribute("destinyX", exit.getDestinyX().ToString());
                    if (exit.getDestinyScale() >= 0)
                    {
                        exitElement.SetAttribute("destinyScale", exit.getDestinyScale().ToString(CultureInfo.InvariantCulture));
                    }
                    exitElement.SetAttribute("transitionType", exit.getTransitionType().ToString());
                    exitElement.SetAttribute("transitionTime", exit.getTransitionTime().ToString());
                    exitElement.SetAttribute("not-effects", (exit.isHasNotEffects() ? "yes" : "no"));

                    if (exit.getInfluenceArea().isExists())
                    {
                        exitElement.SetAttribute("influenceX", exit.getInfluenceArea().getX().ToString());
                        exitElement.SetAttribute("influenceY", exit.getInfluenceArea().getY().ToString());
                        exitElement.SetAttribute("influenceWidth", exit.getInfluenceArea().getWidth().ToString());
                        exitElement.SetAttribute("influenceHeight", exit.getInfluenceArea().getHeight().ToString());
                    }

                    // Append the documentation (if avalaible)
                    if (exit.getDocumentation() != null)
                    {
                        XmlNode exitDocumentationNode = doc.CreateElement("documentation");
                        exitDocumentationNode.AppendChild(doc.CreateTextNode(exit.getDocumentation()));
                        exitElement.AppendChild(exitDocumentationNode);
                    }

                    //Append the default exit look (if available)
                    ExitLook defaultLook = exit.getDefaultExitLook();
                    if (defaultLook != null)
                    {
                        XmlElement exitLook = doc.CreateElement("exit-look");
                        if (defaultLook.getExitText() != null)
                            exitLook.SetAttribute("text", defaultLook.getExitText());
                        if (defaultLook.getCursorPath() != null)
                            exitLook.SetAttribute("cursor-path", defaultLook.getCursorPath());
                        if (defaultLook.getSoundPath() != null)
                            exitLook.SetAttribute("sound-path", defaultLook.getSoundPath());

                        if (defaultLook.getExitText() != null || defaultLook.getCursorPath() != null)
                            exitElement.AppendChild(exitLook);
                    }

                    // Append the next-scene structures
                    foreach (NextScene nextScene in exit.getNextScenes())
                    {
                        // Create the next-scene element
                        XmlElement nextSceneElement = doc.CreateElement("next-scene");
                        nextSceneElement.SetAttribute("idTarget", nextScene.getTargetId());

                        // Append the destination position (if avalaible)
                        if (nextScene.hasPlayerPosition())
                        {
                            nextSceneElement.SetAttribute("x", nextScene.getPositionX().ToString());
                            nextSceneElement.SetAttribute("y", nextScene.getPositionY().ToString());
                        }

                        nextSceneElement.SetAttribute("transitionTime", nextScene.getTransitionTime().ToString());
                        nextSceneElement.SetAttribute("transitionType", nextScene.getTransitionType().ToString());

                        // Append the conditions (if avalaible)
                        if (!nextScene.getConditions().IsEmpty())
                        {
                            DOMWriterUtility.DOMWrite(nextSceneElement, nextScene.getConditions());
                        }

                        //Append the default exit look (if available)
                        ExitLook look = nextScene.getExitLook();
                        if (look != null)
                        {
                            Debug.Log("SAVE 154: " + look.getExitText());
                            XmlElement exitLook = doc.CreateElement("exit-look");
                            if (look.getExitText() != null)
                                exitLook.SetAttribute("text", look.getExitText());
                            if (look.getCursorPath() != null)
                                exitLook.SetAttribute("cursor-path", look.getCursorPath());
                            if (look.getSoundPath() != null)
                                exitLook.SetAttribute("sound-path", look.getSoundPath());
                            if (look.getExitText() != null || look.getCursorPath() != null)
                                nextSceneElement.AppendChild(exitLook);
                        }

                        OrderedDictionary nextSceneEffects = new OrderedDictionary();

                        if (nextScene.getEffects() != null && !nextScene.getEffects().IsEmpty())
                            nextSceneEffects.Add(EffectsDOMWriter.EFFECTS, nextScene.getEffects());

                        if (nextScene.getPostEffects() != null && !nextScene.getPostEffects().IsEmpty())
                            nextSceneEffects.Add(EffectsDOMWriter.POST_EFFECTS, nextScene.getPostEffects());

                        // Append the next scene
                        DOMWriterUtility.DOMWrite(nextSceneElement, nextSceneEffects, DOMWriterUtility.DontCreateElement());

                        exitElement.AppendChild(nextSceneElement);
                    }

                    if (!exit.isRectangular())
                    {
                        foreach (Vector2 point in exit.getPoints())
                        {
                            XmlElement pointNode = doc.CreateElement("point");
                            pointNode.SetAttribute("x", ((int)point.x).ToString());
                            pointNode.SetAttribute("y", ((int)point.y).ToString());
                            exitElement.AppendChild(pointNode);
                        }
                    }

                    if (exit.getConditions() != null && !exit.getConditions().IsEmpty())
                    {
                        DOMWriterUtility.DOMWrite(exitElement, exit.getConditions());
                    }

                    OrderedDictionary effectsTypes = new OrderedDictionary();

                    if (exit.getEffects() != null && !exit.getEffects().IsEmpty())
                        effectsTypes.Add(EffectsDOMWriter.EFFECTS, exit.getEffects());

                    if (exit.getPostEffects() != null && !exit.getPostEffects().IsEmpty())
                        effectsTypes.Add(EffectsDOMWriter.POST_EFFECTS, exit.getPostEffects());

                    if (exit.getNotEffects() != null && !exit.getNotEffects().IsEmpty())
                        effectsTypes.Add(EffectsDOMWriter.NOT_EFFECTS, exit.getNotEffects());


                    DOMWriterUtility.DOMWrite(exitElement, effectsTypes, DOMWriterUtility.DontCreateElement());

                    // Append the exit
                    exitsElement.AppendChild(exitElement);
                }
                // Append the list of exits
                sceneElement.AppendChild(exitsElement);
            }

            // Add the item references (if there is at least one)
            if (scene.getItemReferences().Count > 0)
            {
                XmlNode itemsNode = doc.CreateElement("objects");

                // Append every single item reference
                foreach (ElementReference itemReference in scene.getItemReferences())
                {
                    // Create the item reference element
                    XmlElement itemReferenceElement = doc.CreateElement("object-ref");
                    itemReferenceElement.SetAttribute("idTarget", itemReference.getTargetId());
                    itemReferenceElement.SetAttribute("x", itemReference.getX().ToString());
                    itemReferenceElement.SetAttribute("y", itemReference.getY().ToString());
                    itemReferenceElement.SetAttribute("scale", itemReference.Scale.ToString(CultureInfo.InvariantCulture));
                    itemReferenceElement.SetAttribute("glow", itemReference.Glow ? "yes" : "no");
                    if (itemReference.getLayer() != -1)
                        itemReferenceElement.SetAttribute("layer", itemReference.getLayer().ToString());
                    if (itemReference.getInfluenceArea().isExists())
                    {
                        itemReferenceElement.SetAttribute("hasInfluenceArea", "yes");
                        InfluenceArea ia = itemReference.getInfluenceArea();
                        itemReferenceElement.SetAttribute("influenceX", ia.getX().ToString());
                        itemReferenceElement.SetAttribute("influenceY", ia.getY().ToString());
                        itemReferenceElement.SetAttribute("influenceWidth", ia.getWidth().ToString());
                        itemReferenceElement.SetAttribute("influenceHeight", ia.getHeight().ToString());
                    }
                    else
                    {
                        itemReferenceElement.SetAttribute("hasInfluenceArea", "no");
                    }

                    // Append the documentation (if avalaible)
                    if (itemReference.getDocumentation() != null)
                    {
                        XmlNode itemDocumentationNode = doc.CreateElement("documentation");
                        itemDocumentationNode.AppendChild(doc.CreateTextNode(itemReference.getDocumentation()));
                        itemReferenceElement.AppendChild(itemDocumentationNode);
                    }

                    // Append the conditions (if avalaible)
                    if (!itemReference.Conditions.IsEmpty())
                    {
                        DOMWriterUtility.DOMWrite(itemReferenceElement, itemReference.Conditions);
                    }

                    // Append the exit
                    itemsNode.AppendChild(itemReferenceElement);
                }
                // Append the list of exits
                sceneElement.AppendChild(itemsNode);
            }

            // Add the character references (if there is at least one)
            if (scene.getCharacterReferences().Count > 0)
            {
                XmlNode charactersNode = doc.CreateElement("characters");

                // Append every single character reference
                foreach (ElementReference characterReference in scene.getCharacterReferences())
                {
                    // Create the character reference element
                    XmlElement npcReferenceElement = doc.CreateElement("character-ref");
                    npcReferenceElement.SetAttribute("idTarget", characterReference.getTargetId());
                    npcReferenceElement.SetAttribute("x", characterReference.getX().ToString());
                    npcReferenceElement.SetAttribute("y", characterReference.getY().ToString());
                    npcReferenceElement.SetAttribute("scale", characterReference.Scale.ToString(CultureInfo.InvariantCulture));
                    npcReferenceElement.SetAttribute("orientation", ((int)characterReference.Orientation).ToString());
                    npcReferenceElement.SetAttribute("glow", characterReference.Glow ? "yes" : "no");
                    if (characterReference.getLayer() != -1)
                        npcReferenceElement.SetAttribute("layer", characterReference.getLayer().ToString());
                    if (characterReference.getInfluenceArea().isExists())
                    {
                        npcReferenceElement.SetAttribute("hasInfluenceArea", "yes");
                        InfluenceArea ia = characterReference.getInfluenceArea();
                        npcReferenceElement.SetAttribute("influenceX", ia.getX().ToString());
                        npcReferenceElement.SetAttribute("influenceY", ia.getY().ToString());
                        npcReferenceElement.SetAttribute("influenceWidth", ia.getWidth().ToString());
                        npcReferenceElement.SetAttribute("influenceHeight", ia.getHeight().ToString());
                    }
                    else
                    {
                        npcReferenceElement.SetAttribute("hasInfluenceArea", "no");
                    }

                    // Append the documentation (if avalaible)
                    if (characterReference.getDocumentation() != null)
                    {
                        XmlNode itemDocumentationNode = doc.CreateElement("documentation");
                        itemDocumentationNode.AppendChild(doc.CreateTextNode(characterReference.getDocumentation()));
                        npcReferenceElement.AppendChild(itemDocumentationNode);
                    }

                    // Append the conditions (if avalaible)
                    if (!characterReference.Conditions.IsEmpty())
                    {
                        DOMWriterUtility.DOMWrite(npcReferenceElement, characterReference.Conditions);
                    }

                    // Append the exit
                    charactersNode.AppendChild(npcReferenceElement);
                }
                // Append the list of exits
                sceneElement.AppendChild(charactersNode);
            }

            // Append the exits (if there is at least one)
            if (scene.getActiveAreas().Count > 0)
            {
                XmlNode aasElement = doc.CreateElement("active-areas");

                // Append every single exit
                foreach (ActiveArea activeArea in scene.getActiveAreas())
                {
                    // Create the active area element
                    XmlElement aaElement = doc.CreateElement("active-area");
                    if (activeArea.getId() != null)
                        aaElement.SetAttribute("id", activeArea.getId());
                    aaElement.SetAttribute("rectangular", (activeArea.isRectangular() ? "yes" : "no"));
                    aaElement.SetAttribute("x", activeArea.getX().ToString());
                    aaElement.SetAttribute("y", activeArea.getY().ToString());
                    aaElement.SetAttribute("width", activeArea.getWidth().ToString());
                    aaElement.SetAttribute("height", activeArea.getHeight().ToString());
                    if (activeArea.getInfluenceArea().isExists())
                    {
                        aaElement.SetAttribute("hasInfluenceArea", "yes");
                        InfluenceArea ia = activeArea.getInfluenceArea();
                        aaElement.SetAttribute("influenceX", ia.getX().ToString());
                        aaElement.SetAttribute("influenceY", ia.getY().ToString());
                        aaElement.SetAttribute("influenceWidth", ia.getWidth().ToString());
                        aaElement.SetAttribute("influenceHeight", ia.getHeight().ToString());
                    }
                    else
                    {
                        aaElement.SetAttribute("hasInfluenceArea", "no");
                    }

                    // Behavior
                    if (activeArea.getBehaviour() == Item.BehaviourType.NORMAL)
                        aaElement.SetAttribute("behaviour", "normal");
                    if (activeArea.getBehaviour() == Item.BehaviourType.ATREZZO)
                        aaElement.SetAttribute("behaviour", "atrezzo");
                    if (activeArea.getBehaviour() == Item.BehaviourType.FIRST_ACTION)
                        aaElement.SetAttribute("behaviour", "first-action");

                    // Append the documentation (if avalaible)
                    if (activeArea.getDocumentation() != null)
                    {
                        XmlNode exitDocumentationNode = doc.CreateElement("documentation");
                        exitDocumentationNode.AppendChild(doc.CreateTextNode(activeArea.getDocumentation()));
                        aaElement.AppendChild(exitDocumentationNode);
                    }

                    // Append the conditions (if avalaible)
                    if (!activeArea.getConditions().IsEmpty())
                    {
                        DOMWriterUtility.DOMWrite(aaElement, activeArea.getConditions());
                    }


                    foreach (Description description in activeArea.getDescriptions())
                    {
                        // Create the description
                        XmlNode descriptionNode = doc.CreateElement("description");

                        // Append the conditions (if available)
                        if (description.getConditions() != null && !description.getConditions().IsEmpty())
                        {
                            DOMWriterUtility.DOMWrite(descriptionNode, description.getConditions());
                        }

                        // Create and append the name, brief description and detailed description
                        XmlElement aaNameNode = doc.CreateElement("name");
                        if (description.getNameSoundPath() != null && !description.getNameSoundPath().Equals(""))
                        {
                            aaNameNode.SetAttribute("soundPath", description.getNameSoundPath());
                        }
                        aaNameNode.AppendChild(doc.CreateTextNode(description.getName()));
                        descriptionNode.AppendChild(aaNameNode);

                        XmlElement aaBriefNode = doc.CreateElement("brief");
                        if (description.getDescriptionSoundPath() != null &&
                            !description.getDescriptionSoundPath().Equals(""))
                        {
                            aaBriefNode.SetAttribute("soundPath", description.getDescriptionSoundPath());
                        }
                        aaBriefNode.AppendChild(doc.CreateTextNode(description.getDescription()));
                        descriptionNode.AppendChild(aaBriefNode);

                        XmlElement aaDetailedNode = doc.CreateElement("detailed");
                        if (description.getDetailedDescriptionSoundPath() != null &&
                            !description.getDetailedDescriptionSoundPath().Equals(""))
                        {
                            aaDetailedNode.SetAttribute("soundPath", description.getDetailedDescriptionSoundPath());
                        }
                        aaDetailedNode.AppendChild(doc.CreateTextNode(description.getDetailedDescription()));
                        descriptionNode.AppendChild(aaDetailedNode);

                        // Append the description
                        aaElement.AppendChild(descriptionNode);
                    }

                    // Append the actions (if there is at least one)
                    if (activeArea.getActions().Count > 0)
                    {
                        DOMWriterUtility.DOMWrite(aaElement, activeArea.getActions());
                    }

                    if (!activeArea.isRectangular())
                    {
                        foreach (Vector2 point in activeArea.getPoints())
                        {
                            XmlElement pointNode = doc.CreateElement("point");
                            pointNode.SetAttribute("x", ((int)point.x).ToString());
                            pointNode.SetAttribute("y", ((int)point.y).ToString());
                            aaElement.AppendChild(pointNode);
                        }
                    }

                    // Append the exit
                    aasElement.AppendChild(aaElement);
                }
                // Append the list of exits
                sceneElement.AppendChild(aasElement);
            }

            // Append the barriers (if there is at least one)
            if (scene.getBarriers().Count > 0)
            {
                XmlNode barriersElement = doc.CreateElement("barriers");

                // Append every single barrier
                foreach (Barrier barrier in scene.getBarriers())
                {
                    // Create the active area element
                    XmlElement barrierElement = doc.CreateElement("barrier");
                    barrierElement.SetAttribute("x", barrier.getX().ToString());
                    barrierElement.SetAttribute("y", barrier.getY().ToString());
                    barrierElement.SetAttribute("width", barrier.getWidth().ToString());
                    barrierElement.SetAttribute("height", barrier.getHeight().ToString());

                    // Append the documentation (if avalaible)
                    if (barrier.getDocumentation() != null)
                    {
                        XmlNode exitDocumentationNode = doc.CreateElement("documentation");
                        exitDocumentationNode.AppendChild(doc.CreateTextNode(barrier.getDocumentation()));
                        barrierElement.AppendChild(exitDocumentationNode);
                    }

                    // Append the conditions (if avalaible)
                    if (!barrier.getConditions().IsEmpty())
                    {
                        DOMWriterUtility.DOMWrite(barrierElement, barrier.getConditions());
                    }

                    // Append the barrier
                    barriersElement.AppendChild(barrierElement);
                }
                // Append the list of exits
                sceneElement.AppendChild(barriersElement);
            }

            // Add the atrezzo item references (if there is at least one)
            if (scene.getAtrezzoReferences().Count > 0)
            {
                XmlNode atrezzoNode = doc.CreateElement("atrezzo");

                // Append every single atrezzo reference
                foreach (ElementReference atrezzoReference in scene.getAtrezzoReferences())
                {
                    // Create the atrezzo reference element
                    XmlElement atrezzoReferenceElement = doc.CreateElement("atrezzo-ref");
                    atrezzoReferenceElement.SetAttribute("idTarget", atrezzoReference.getTargetId());
                    atrezzoReferenceElement.SetAttribute("x", atrezzoReference.getX().ToString());
                    atrezzoReferenceElement.SetAttribute("y", atrezzoReference.getY().ToString());
                    atrezzoReferenceElement.SetAttribute("scale", atrezzoReference.Scale.ToString(CultureInfo.InvariantCulture));
                    atrezzoReferenceElement.SetAttribute("glow", atrezzoReference.Glow ? "yes" : "no");
                    if (atrezzoReference.getLayer() != -1)
                        atrezzoReferenceElement.SetAttribute("layer", atrezzoReference.getLayer().ToString());

                    // Append the documentation (if avalaible)
                    if (atrezzoReference.getDocumentation() != null)
                    {
                        XmlNode itemDocumentationNode = doc.CreateElement("documentation");
                        itemDocumentationNode.AppendChild(doc.CreateTextNode(atrezzoReference.getDocumentation()));
                        atrezzoReferenceElement.AppendChild(itemDocumentationNode);
                    }

                    // Append the conditions (if avalaible)
                    if (!atrezzoReference.Conditions.IsEmpty())
                    {
                        DOMWriterUtility.DOMWrite(atrezzoReferenceElement, atrezzoReference.Conditions);
                    }

                    // Append the atrezzo reference
                    atrezzoNode.AppendChild(atrezzoReferenceElement);
                }
                // Append the list of atrezzo references
                sceneElement.AppendChild(atrezzoNode);
            }

            if (scene.getTrajectory() != null)
            {
                DOMWriterUtility.DOMWrite(sceneElement, scene.getTrajectory());
            }
        }

    }
}