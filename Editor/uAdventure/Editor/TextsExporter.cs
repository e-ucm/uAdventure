using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using uAdventure.Core;
using uAdventure.Editor;
using System.Linq;
using uAdventure.Geo;
using UnityEngine.Assertions;

public class TextsExporter {

    [MenuItem("uAdventure/Experimental/Export conversations")]
    public static void ExportConversations ()
    {
        if (!Controller.Instance.Initialized)
        {
            return;
        }

        var conversations = Controller.Instance.SelectedChapterDataControl.getConversationsList();
        var items = Controller.Instance.SelectedChapterDataControl.getItemsList();

        using (var outFile = System.IO.File.Open("conversations.txt", System.IO.FileMode.OpenOrCreate))
        using (var outWriter = new System.IO.StreamWriter(outFile))
        {
            // Conversations
            foreach (var conversation in conversations.getConversations())
            {
                outWriter.WriteLine("Conversation: " + conversation.getId());
                var nodeNames = new Dictionary<ConversationNodeDataControl, string>();
                var nodeContinuations = new Dictionary<ConversationNodeDataControl, List<string>>();
                var number = 0;

                foreach(var node in conversation.getAllNodes())
                {
                    nodeNames.Add(node, "Node: " + (++number));
                    nodeContinuations.Add(node, new List<string>());
                }
                foreach (var node in conversation.getAllNodes())
                {
                    var childNumber = 0;
                    foreach (var child in node.getChilds())
                    {
                        var afterText = " -- Empty node -- ";
                        if(node.getChildCount() > 1)
                        {
                            afterText = node.getLine(childNumber).getText();
                        }
                        else if(node.getLineCount() > 0)
                        { 
                            afterText = node.getLine(node.getLineCount()-1).getText();
                        }

                        nodeContinuations[child].Add(" * After " + nodeNames[node] + ": " + afterText);
                        childNumber++;
                    }
                }

                foreach (var node in conversation.getAllNodes())
                {
                    outWriter.WriteLine(nodeNames[node]);
                    foreach(var continuation in nodeContinuations[node])
                    {
                        outWriter.WriteLine(continuation);
                    }

                    foreach(var line in node.getLines())
                    {
                        outWriter.WriteLine(" - " + line.getName() + ": " + line.getText()); 
                    }

                    WriteEffects(outWriter, node.getEffects());
                }
                outWriter.WriteLine("");

            }
            
            // Items effects (Grab, use or examine)
            foreach (var item in items.getItems())
            {
                if(!item.getActionsList().getActions().Any(a => a.getType() == Controller.ACTION_GRAB || a.getType() == Controller.ACTION_USE))
                {
                    continue;
                }

                outWriter.WriteLine("Objeto: " + item.getId());

                foreach(var action in item.getActionsList().getActions().Where(a => a.getType() == Controller.ACTION_GRAB || a.getType() == Controller.ACTION_USE))
                {
                    switch (action.getType())
                    {
                        case Controller.ACTION_EXAMINE:
                            outWriter.WriteLine(" - Al examinarlo:");
                            break;
                        case Controller.ACTION_GRAB:
                            outWriter.WriteLine(" - Al cogerlo:");
                            break;
                        case Controller.ACTION_USE:
                            outWriter.WriteLine(" - Al tirarlo:");
                            break;
                    }
                    var effects = action.getEffects();
                    WriteEffects(outWriter, effects);
                }
            }

            // GeoElements effects (Enter, Exit, LookAt, Inspect)
            foreach (var geoElement in GeoController.Instance.GeoElements.DataControls)
            {
                if (!geoElement.GeoActions.DataControls.Any(a => a.Effects.getEffects().Any(e => e is SpeakPlayerEffect || e is SpeakCharEffect)))
                {
                    continue;
                }

                outWriter.WriteLine("Lugar: " + (geoElement.getContent() as GeoElement).Id);

                foreach (var action in geoElement.GeoActions.DataControls)
                {
                    outWriter.WriteLine(" - " + action.getType() + ":");

                    foreach (var effect in action.Effects.getEffects().Where(e => e is SpeakCharEffect || e is SpeakPlayerEffect))
                    {
                        var abstractEffect = effect as AbstractEffect;
                        if (abstractEffect != null && abstractEffect.getConditions().Size() > 0)
                        {
                            outWriter.Write("  * Cuando " + abstractEffect.getConditions().ToString() + ": ");
                        }
                        else
                        {
                            outWriter.Write("  * ");
                        }

                        var speakCharEffect = effect as SpeakCharEffect;
                        var speakerName = "Player";
                        var text = "";

                        if (speakCharEffect != null)
                        {
                            speakerName = speakCharEffect.getTargetId();
                            text = speakCharEffect.getLine();
                        }
                        else
                        {
                            text = (effect as SpeakPlayerEffect).getLine();
                        }

                        outWriter.WriteLine(speakerName + ": " + text);
                    }
                }
            }
        }
    }

    [MenuItem("uAdventure/Experimental/Import conversations")]
    public static void ImportConversations()
    {
        if (!Controller.Instance.Initialized)
        {
            return;
        }

        var conversations = Controller.Instance.SelectedChapterDataControl.getConversationsList().getConversations();
        var items = Controller.Instance.SelectedChapterDataControl.getItemsList().getItems();
        var geoElements = (List<GeoElement>)GeoController.Instance.GeoElements.getContent();
        Conversation currentConv = null;
        Item currentItem = null;
        GeoElement currentGeoElem = null;
        string currentElementId;
        int currentNode = -1;
        int currentLine = -1;
        Effects currentEffects = null;
        int currentEffect = -1;
        int currentAction = -1;

        using (var inFile = System.IO.File.Open("import.txt", System.IO.FileMode.Open))
        using (var inReader = new System.IO.StreamReader(inFile))
        {
            var line = "";
            while ((line = inReader.ReadLine()) != null)
            {
                if (currentConv == null && currentItem == null && currentGeoElem == null && currentEffects == null)
                {
                    if (line.StartsWith("Conversation: "))
                    {
                        currentElementId = line.Split(':')[1].Trim();
                        Assert.IsTrue(conversations.Any(c => c.getId() == currentElementId),
                            "Conversation \"" + currentElementId + "\" does not exist!");
                        currentNode = 0;
                        currentConv = conversations.FirstOrDefault(c => c.getId() == currentElementId).getConversation();
                    }
                    else if (line.StartsWith("Objeto: "))
                    {
                        currentElementId = line.Split(':')[1].Trim();
                        Assert.IsTrue(items.Any(i => i.getId() == currentElementId),
                            "Item \"" + currentElementId + "\" does not exist!");
                        currentAction = 0;
                        currentItem = (Item)items.FirstOrDefault(i => i.getId() == currentElementId).getContent();
                    }
                    else if (line.StartsWith("Lugar: "))
                    {
                        currentElementId = line.Split(':')[1].Trim();
                        Assert.IsTrue(geoElements.Any(g => g.Id == currentElementId),
                            "GeoElement \"" + currentElementId + "\" does not exist!");
                        currentAction = 0;
                        currentGeoElem = geoElements.FirstOrDefault(g => g.getId() == currentElementId);
                    }
                } 
                // Effects Parser
                else if (currentEffects != null && line.StartsWith("  * "))
                {
                    var validEffects = currentEffects.getEffects().Where(e => e is SpeakCharEffect || e is SpeakPlayerEffect).ToList();
                    if (validEffects[currentEffect] is SpeakPlayerEffect)
                    {
                        var spe = validEffects[currentEffect] as SpeakPlayerEffect; 
                        var lineText = line.Substring(line.IndexOf(':') + 2); // 1 to skip the semicolon and 1 to skip the space

                        Assert.IsTrue(line.Split(':')[0].Contains("Player"),
                            "Current effect is a SpeakPlayerEffect but player is not speaking!: " + line);
                        spe.setLine(lineText);
                    }
                    else if (validEffects[currentEffect] is SpeakCharEffect)
                    {
                        var sce = validEffects[currentEffect] as SpeakCharEffect;
                        var lineText = line.Substring(line.IndexOf(':') + 2); // 1 to skip the semicolon and 1 to skip the space
                        Assert.IsTrue(line.Split(':')[0].Contains(sce.getTargetId()),
                            "Current effect is a SpeakCharEffect but \""+ sce.getTargetId() + "\" is not speaking!: " + line);
                        sce.setLine(lineText);
                    }
                    currentEffect++;
                }
                // Conversations parser
                else if (currentConv != null)
                {
                    if (line.StartsWith("Node: "))
                    {
                        currentNode = int.Parse(line.Split(':')[1].Trim()) - 1;
                        currentLine = 0;
                        currentEffects = currentConv.getAllNodes()[currentNode].getEffects();
                        currentEffect = 0;
                        Assert.IsTrue(currentNode < currentConv.getAllNodes().Count,
                            "Conversation \"" + currentConv.getId() + "\" does not contain node \"" + currentNode + "\"!");

                    } 
                    else if(line.StartsWith(" * After"))
                    {
                        continue;
                    }
                    else if(line.StartsWith(" - "))
                    {
                        Assert.IsTrue(currentNode != -1, "Current node is not initialized!");
                        Assert.IsTrue(currentLine != -1, "Current line is not initialized!");

                        Assert.IsTrue(currentLine < currentConv.getAllNodes()[currentNode].getLineCount(), 
                            "Line \"" + currentLine + "\" does not exist in node \""+currentNode+"\" of conversation \""+currentConv.getId()+ "\": " + line);

                        var lineText = line.Substring(line.IndexOf(':') + 2); // 1 to skip the semicolon and 1 to skip the space
                        currentConv.getAllNodes()[currentNode].getLine(currentLine).setText(lineText);
                        currentLine++;
                    } 
                    else if (line.StartsWith("  * "))
                    {
                        throw new System.NotImplementedException("Parsing Effects in conversations is not yet implemented!");
                    }
                    else if (line.Length == 0)
                    {
                        currentConv = null;
                        currentLine = -1;
                        currentNode = -1;
                        currentEffects = null;
                        currentEffect = -1;
                    }
                    else
                    {
                        throw new System.FormatException("Unexpected line found while reading a conversation: " + line);
                    }
                } 
                // Items parser
                else if(currentItem != null)
                {
                    var actions = currentItem.getActions().Where(a => a.getType() == Action.GRAB || a.getType() == Action.USE).ToList();

                    if (line == " - Al examinarlo:")
                    {
                        Assert.IsTrue(currentAction < actions.Count, "Current Examine action is out of bounds!");
                        Assert.IsTrue(actions[currentAction].getType() == Action.EXAMINE, "Next found action is not examine!");
                        currentEffect = 0;
                        currentEffects = actions[currentAction].Effects;
                        currentAction++;
                    }
                    else if (line == " - Al cogerlo:")
                    {
                        Assert.IsTrue(currentAction < actions.Count, "Current Grab action is out of bounds!");
                        Assert.IsTrue(actions[currentAction].getType() == Action.GRAB, "Next found action is not grab!");
                        currentEffect = 0;
                        currentEffects = actions[currentAction].Effects;
                        currentAction++;
                    }
                    else if (line == " - Al tirarlo:")
                    {
                        Assert.IsTrue(currentAction < actions.Count, "Current Use action is out of bounds!");
                        Assert.IsTrue(actions[currentAction].getType() == Action.USE, "Next found action is not use!");
                        currentEffect = 0;
                        currentEffects = actions[currentAction].Effects;
                        currentAction++;
                    }
                    else if (line.Length == 0)
                    {
                        currentItem = null;
                        currentEffects = null;
                        currentEffect = -1;
                        currentAction = -1;
                    }
                    else
                    {
                        throw new System.FormatException("Unexpected line found while reading an item: " + line);
                    }
                } 
                // Geo elements parser
                else if (currentGeoElem != null)
                {
                    var geoActions = currentGeoElem.Actions;
                    if (line == " - Enter:")
                    {
                        Assert.IsTrue(geoActions[currentAction] is EnterAction, "Next found action is not examine!");
                        currentEffect = 0;
                        currentEffects = geoActions[currentAction].Effects;
                        currentAction++;

                    }
                    else if (line == " - Exit:")
                    {
                        Assert.IsTrue(geoActions[currentAction] is ExitAction, "Next found action is not grab!");
                        currentEffect = 0;
                        currentEffects = geoActions[currentAction].Effects;
                        currentAction++;

                    }
                    else if (line == " - LookTo:")
                    {
                        Assert.IsTrue(geoActions[currentAction] is LookToAction, "Next found action is not use!");
                        currentEffect = 0;
                        currentEffects = geoActions[currentAction].Effects;
                        currentAction++;

                    }
                    else if (line == " - Inspect:")
                    {
                        Assert.IsTrue(geoActions[currentAction] is InspectAction, "Next found action is not use!");
                        currentEffect = 0;
                        currentEffects = geoActions[currentAction].Effects;
                        currentAction++;

                    }
                    else if (line.Length == 0)
                    {
                        currentGeoElem = null;
                        currentEffects = null;
                        currentEffect = -1;
                        currentAction = -1;
                    }
                    else
                    {
                        throw new System.FormatException("Unexpected line found while reading a geo element: " + line);
                    }
                }
            }
        }
    }

    private static void WriteEffects(System.IO.StreamWriter outWriter, EffectsController effects)
    {
        foreach (var effect in effects.getEffects().Where(e => e is SpeakCharEffect || e is SpeakPlayerEffect))
        {
            var abstractEffect = effect as AbstractEffect;
            if (abstractEffect != null && abstractEffect.getConditions().Size() > 0)
            {
                outWriter.Write("  * Cuando " + abstractEffect.getConditions().ToString() + ": ");
            }
            else
            {
                outWriter.Write("  * "); 
            }

            var speakCharEffect = effect as SpeakCharEffect;
            var speakerName = "Player";
            var text = "";

            if (speakCharEffect != null)
            {
                speakerName = speakCharEffect.getTargetId();
                text = speakCharEffect.getLine();
            }
            else
            {
                text = (effect as SpeakPlayerEffect).getLine();
            }

            outWriter.WriteLine(speakerName + ": " + text);
        }
    }
}
