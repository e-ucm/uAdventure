using System;
using UnityEngine;
using System.Collections;
using System.Xml;

public class SceneDOMWriter
{
    /**
        * Private constructor.
        */

    private SceneDOMWriter()
    {

    }

    public static XmlNode buildDOM(Scene scene, bool initialScene)
    {

        XmlElement sceneElement = null;

        if (scene != null)
        {
            TrajectoryFixer.fixTrajectory(scene);
        }
        // Create the necessary elements to create the DOM
        XmlDocument doc = Writer.GetDoc();
        // Create the root node
        sceneElement = doc.CreateElement("scene");
        sceneElement.SetAttribute("id", scene.getId());
        if (initialScene)
            sceneElement.SetAttribute("start", "yes");
        else
            sceneElement.SetAttribute("start", "no");

        sceneElement.SetAttribute("playerLayer", scene.getPlayerLayer().ToString());
        sceneElement.SetAttribute("playerScale", scene.getPlayerScale().ToString());

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
                    if (!nextScene.getConditions().isEmpty())
                    {
                        XmlNode conditionsNode = ConditionsDOMWriter.buildDOM(nextScene.getConditions());
                        doc.ImportNode(conditionsNode, true);
                        nextSceneElement.AppendChild(conditionsNode);
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

                    // Append the effects (if avalaible)
                    if (!nextScene.getEffects().isEmpty())
                    {
                        XmlNode effectsNode = EffectsDOMWriter.buildDOM(EffectsDOMWriter.EFFECTS,
                            nextScene.getEffects());
                        doc.ImportNode(effectsNode, true);
                        nextSceneElement.AppendChild(effectsNode);
                    }

                    // Append the post-effects (if avalaible)
                    if (!nextScene.getPostEffects().isEmpty())
                    {
                        XmlNode postEffectsNode = EffectsDOMWriter.buildDOM(EffectsDOMWriter.POST_EFFECTS,
                            nextScene.getPostEffects());
                        doc.ImportNode(postEffectsNode, true);
                        nextSceneElement.AppendChild(postEffectsNode);
                    }

                    // Append the next scene
                    exitElement.AppendChild(nextSceneElement);
                }

                if (!exit.isRectangular())
                {
                    foreach (Vector2 point in exit.getPoints())
                    {
                        XmlElement pointNode = doc.CreateElement("point");
                        pointNode.SetAttribute("x", ((int) point.x).ToString());
                        pointNode.SetAttribute("y", ((int) point.y).ToString());
                        exitElement.AppendChild(pointNode);
                    }
                }

                if (exit.getConditions() != null && !exit.getConditions().isEmpty())
                {
                    XmlNode conditionsNode = ConditionsDOMWriter.buildDOM(exit.getConditions());
                    doc.ImportNode(conditionsNode, true);
                    exitElement.AppendChild(conditionsNode);
                }

                if (exit.getEffects() != null && !exit.getEffects().isEmpty())
                {
                    Debug.Log("SceneDOM Effects: " + exit.getEffects().getEffects().Count);
                    XmlNode effectsNode = EffectsDOMWriter.buildDOM(EffectsDOMWriter.EFFECTS, exit.getEffects());
                    doc.ImportNode(effectsNode, true);
                    exitElement.AppendChild(effectsNode);
                }

                if (exit.getPostEffects() != null && !exit.getPostEffects().isEmpty())
                {
                    Debug.Log("SceneDOM PostEffects: " + exit.getPostEffects().getEffects().Count);
                    XmlNode postEffectsNode = EffectsDOMWriter.buildDOM(EffectsDOMWriter.POST_EFFECTS,
                        exit.getPostEffects());
                    doc.ImportNode(postEffectsNode, true);
                    exitElement.AppendChild(postEffectsNode);
                }

                if (exit.getNotEffects() != null && !exit.getNotEffects().isEmpty())
                {
                    Debug.Log("SceneDOM NonEffects: " + exit.getNotEffects().getEffects().Count);
                    XmlNode notEffectsNode = EffectsDOMWriter.buildDOM(EffectsDOMWriter.NOT_EFFECTS,
                        exit.getNotEffects());
                    doc.ImportNode(notEffectsNode, true);
                    exitElement.AppendChild(notEffectsNode);
                }

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
                itemReferenceElement.SetAttribute("scale", itemReference.getScale().ToString());
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
                if (!itemReference.getConditions().isEmpty())
                {
                    XmlNode conditionsNode = ConditionsDOMWriter.buildDOM(itemReference.getConditions());
                    doc.ImportNode(conditionsNode, true);
                    itemReferenceElement.AppendChild(conditionsNode);
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
                npcReferenceElement.SetAttribute("scale", characterReference.getScale().ToString());
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
                if (!characterReference.getConditions().isEmpty())
                {
                    XmlNode conditionsNode = ConditionsDOMWriter.buildDOM(characterReference.getConditions());
                    doc.ImportNode(conditionsNode, true);
                    npcReferenceElement.AppendChild(conditionsNode);
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

                // Append the documentation (if avalaible)
                if (activeArea.getDocumentation() != null)
                {
                    XmlNode exitDocumentationNode = doc.CreateElement("documentation");
                    exitDocumentationNode.AppendChild(doc.CreateTextNode(activeArea.getDocumentation()));
                    aaElement.AppendChild(exitDocumentationNode);
                }

                // Append the conditions (if avalaible)
                if (!activeArea.getConditions().isEmpty())
                {
                    XmlNode conditionsNode = ConditionsDOMWriter.buildDOM(activeArea.getConditions());
                    doc.ImportNode(conditionsNode, true);
                    aaElement.AppendChild(conditionsNode);
                }


                foreach (Description description in activeArea.getDescriptions())
                {
                    // Create the description
                    XmlNode descriptionNode = doc.CreateElement("description");

                    // Append the conditions (if available)
                    if (description.getConditions() != null && !description.getConditions().isEmpty())
                    {
                        XmlNode conditionsNode = ConditionsDOMWriter.buildDOM(description.getConditions());
                        doc.ImportNode(conditionsNode, true);
                        descriptionNode.AppendChild(conditionsNode);
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
                    XmlNode actionsNode = ActionsDOMWriter.buildDOM(activeArea.getActions());
                    doc.ImportNode(actionsNode, true);

                    // Append the actions node
                    aaElement.AppendChild(actionsNode);
                }

                if (!activeArea.isRectangular())
                {
                    foreach (Vector2 point in activeArea.getPoints())
                    {
                        XmlElement pointNode = doc.CreateElement("point");
                        pointNode.SetAttribute("x", ((int) point.x).ToString());
                        pointNode.SetAttribute("y", ((int) point.y).ToString());
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
                if (!barrier.getConditions().isEmpty())
                {
                    XmlNode conditionsNode = ConditionsDOMWriter.buildDOM(barrier.getConditions());
                    doc.ImportNode(conditionsNode, true);
                    barrierElement.AppendChild(conditionsNode);
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
                atrezzoReferenceElement.SetAttribute("scale", atrezzoReference.getScale().ToString());
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
                if (!atrezzoReference.getConditions().isEmpty())
                {
                    XmlNode conditionsNode = ConditionsDOMWriter.buildDOM(atrezzoReference.getConditions());
                    doc.ImportNode(conditionsNode, true);
                    atrezzoReferenceElement.AppendChild(conditionsNode);
                }

                // Append the atrezzo reference
                atrezzoNode.AppendChild(atrezzoReferenceElement);
            }
            // Append the list of atrezzo references
            sceneElement.AppendChild(atrezzoNode);
        }

        if (scene.getTrajectory() != null)
        {
            XmlNode trajectoryNode = TrajectoryDOMWriter.buildDOM(scene.getTrajectory());
            doc.ImportNode(trajectoryNode, true);
            sceneElement.AppendChild(trajectoryNode);
        }

        return sceneElement;
    }
}