using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using System;
using System.Linq;

namespace uAdventure.Editor
{
    public class ConversationLineDataControl : DataControlWithResources
    {
        private readonly ConversationLine conversationLine;
        private readonly ConditionsController conversationLineConditionsController;

        public ConversationLineDataControl(ConversationLine conversationLine) : base()
        {
            this.conversationLine = conversationLine;
            this.conversationLineConditionsController = new ConditionsController(conversationLine.getConditions());
            this.resourcesList = conversationLine.getResources();
            this.resourcesDataControlList = conversationLine.getResources().ConvertAll(r => new ResourcesDataControl(r, Controller.CONVERSATION_DIALOGUE_LINE));
        }

        public override bool addElement(int type, string id)
        {
            return false;
        }

        public override bool canAddElement(int type)
        {
            return false;
        }

        public override bool canBeDeleted()
        {
            return true;
        }

        public override bool canBeDuplicated()
        {
            return true;
        }

        public override bool canBeMoved()
        {
            return true;
        }

        public override bool canBeRenamed()
        {
            return false;
        }

        public override int countAssetReferences(string assetPath)
        {
            return resourcesDataControlList.Sum(r => r.countAssetReferences(assetPath));
        }

        public override int countIdentifierReferences(string id)
        {
            return (conversationLine.getName() == id ? 1 : 0) +
                conversationLineConditionsController.countIdentifierReferences(id);
        }

        public override void deleteAssetReferences(string assetPath)
        {
            resourcesDataControlList.ForEach(r => r.deleteAssetReferences(assetPath));
        }

        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {
            return false;
        }

        public override void deleteIdentifierReferences(string id)
        {
            if (conversationLine.getName() == id)
            {
                conversationLine.setName("Player");
            }

            conversationLineConditionsController.deleteIdentifierReferences(id);
        }

        public override int[] getAddableElements()
        {
            return null;
        }

        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {
            resourcesDataControlList.ForEach(r => r.getAssetReferences(assetPaths, assetTypes));
        }

        public override object getContent()
        {
            return conversationLine;
        }

        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {
            return null;
        }

        public override bool isValid(string currentPath, List<string> incidences)
        {
            bool valid = true;

            if (!conversationLine.isPlayerLine() && !controller.IdentifierSummary.existsId(conversationLine.getName()))
            {
                incidences.Add("Character identifier not found: \"" + conversationLine.getName() + "\"");
                valid = false;
            }

            valid &= resourcesDataControlList.All(r => r.isValid(currentPath, incidences));

            return valid;
        }

        public override bool moveElementDown(DataControl dataControl)
        {
            return false;
        }

        public override bool moveElementUp(DataControl dataControl)
        {
            return false;
        }

        public override void recursiveSearch()
        {
            check(conversationLine.getName(), TC.get("Search.LineName"));
            check(conversationLine.getText(), TC.get("Search.LineText"));
            
        }

        public override string renameElement(string newName)
        {
            return null;
        }

        public override void replaceIdentifierReferences(string oldId, string newId)
        {
            if (conversationLine.getName() == oldId)
            {
                conversationLine.setName(newId);
            }

            conversationLineConditionsController.replaceIdentifierReferences(oldId, newId);
        }

        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {
            ConditionsController.updateVarFlagSummary(varFlagSummary, conversationLineConditionsController.Conditions);
        }


        // Methods



        /**
         * Returns true if the xapi question is a correct option
         * 
         * @return true if the xapi question is a correct option
         */

        public bool getXApiCorrect()
        {
            return conversationLine.getXApiCorrect();
        }

        /**
         * Returns the name of the character.
         * 
         * @return The name of the character
         */

        public string getName()
        {
            return conversationLine.getName();
        }

        /**
         * Returns the text of the converstational line.
         * 
         * @return The text of the conversational line
         */

        public string getText()
        {
            return conversationLine.getText();
        }

        /**
         * Returns if the line belongs to the player.
         * 
         * @return True if the line belongs to the player, false otherwise
         */

        public bool isPlayerLine()
        {
            return conversationLine.isPlayerLine();
        }

        /**
         * Sets if the option is correct
         * 
         * @param if the option is correct
         */

        public void setXApiCorrect(bool xapiCorrect)
        {
            controller.AddTool(new ChangeBooleanValueTool(conversationLine, xapiCorrect, "getXApiCorrect", "setXApiCorrect"));
        }

        /**
         * Sets the new name of the line.
         * 
         * @param name
         *            New name
         */

        public void setName(string name)
        {
            controller.AddTool(new ChangeStringValueTool(conversationLine, name, "getName", "setName"));
        }

        public bool isValidImage()
        {
            return resourcesDataControlList.Any(r => !string.IsNullOrEmpty(r.getAssetPath("image")));
        }

        /**
         * Sets the new text of the line.
         * 
         * @param text
         *            New text
         */

        public void setText(string text)
        {
            controller.AddTool(new ChangeStringValueTool(conversationLine, text, "getText", "setText"));
        }

        /**
         * Returns true if the audio path is valid. That is when it is not null and
         * different to ""
         */

        public bool isValidAudio()
        {
            return resourcesDataControlList.Any(r => !string.IsNullOrEmpty(r.getAssetPath("audio")));
        }

        /**
         * Set if the line to be read by synthesizer
         * 
         * @param synthesizerVoice
         *            true for to be read by synthesizer
         */

        public void setSynthesizerVoice(bool synthesizerVoice)
        {
            controller.AddTool(new ChangeBooleanValueTool(conversationLine, synthesizerVoice, "getSynthesizerVoice", "setSynthesizerVoice"));
        }

        /**
         * @return the conditions
         */

        public ConditionsController getConditions()
        {
            return conversationLineConditionsController;
        }

        public bool isKeepShowing()
        {
            return conversationLine.isKeepShowing();
        }

        public void setKeepShowing(bool keepShowing)
        {
            controller.AddTool(new ChangeBooleanValueTool(conversationLine, keepShowing, "isKeepShowing", "setKeepShowing"));
        }
    }
}