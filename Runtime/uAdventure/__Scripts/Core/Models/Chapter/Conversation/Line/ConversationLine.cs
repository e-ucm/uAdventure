using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    /**
     * This class stores a single conversation line, along with the name of the
     * speaker character.
     */

    public class ConversationLine : Named, ICloneable
    {

        public enum Type
        {
            NORMAL,
            WHISPER,
            THOUGHT,
            YELL
        };

        /*
                public override string Tostring()
            {

                switch (this)
                {
                    case WHISPER:
                        return "#:*";
                    case THOUGHT:
                        return "#O";
                    case YELL:
                        return "#!";
                    default:
                        return "";
                }
            public string getName()
            {

                string name;
                switch (this)
                {
                    case Type.WHISPER:
                        name = Language.GetText("ConversationLine.Type.Whisper");
                        break;
                    case Type.THOUGHT:
                        name = Language.GetText("ConversationLine.Type.Thought");
                        break;
                    case Type.YELL:
                        name = Language.GetText("ConversationLine.Type.Yell");
                        break;
                    default:
                        name = Language.GetText("ConversationLine.Type.Normal");
                        break;
                }
                return name + " " + this.ToString();
            }
        public static Type getType(string s)
        {

            if (s.Equals(Type.WHISPER.ToString()))
            {
                return Type.WHISPER;
            }
            else if (s.Equals(Type.THOUGHT.ToString()))
            {
                return Type.THOUGHT;
            }
            else if (s.Equals(Type.YELL.ToString()))
            {
                return Type.YELL;
            }
            return Type.NORMAL;
        }
    }

            */
        /**
         * Constant for the player identifier for the lines.
         */
        public const string PLAYER = Player.IDENTIFIER;

        /**
         * xapi is correct
         */
        public bool xapiCorrect = false;

        /**
         * string that holds the name of the character.
         */
        private string name;

        /**
         * Sentence said by the character. This can be replaced or combined with the image.
         */
        private string text;

        /**
         * Conditions associated to this line
         */
        private Conditions conditions;

        /**
         * Keep line showing until user interacts
         */
        private bool keepShowing;

        /**
         * The element's set of resources
         */
        private List<ResourcesUni> resources;

        /**
         * Constructor.
         * 
         * @param name
         *            Name of the character
         * @param text
         *            Sentence
         */

        public ConversationLine(string name, string text)
        {

            this.name = name;
            this.text = text;
            this.keepShowing = false;
            conditions = new Conditions();
            resources = new List<ResourcesUni> ();
        }


        /**
         * Returns the element's list of resources
         * 
         * @return the element's list of resources
         */
        public List<ResourcesUni> getResources()
        {

            return resources;
        }



        /**
         * Adds some resources to the list of resources
         * 
         * @param resources
         *            the resources to add
         */
        public void addResources(ResourcesUni resources)
        {

            this.resources.Add(resources);
        }

        /**
         * Returns true if the xapi question is a correct option
         * 
         * @return true if the xapi question is a correct option
         */

        public bool getXApiCorrect()
        {
            return xapiCorrect;
        }

        /**
         * Returns the name of the character.
         * 
         * @return The name of the character
         */

        public string getName()
        {

            return name;
        }

        /**
         * Returns the text of the converstational line.
         * 
         * @return The text of the conversational line
         */

        public string getText()
        {

            return text;
        }

        /**
         * Returns if the line belongs to the player.
         * 
         * @return True if the line belongs to the player, false otherwise
         */

        public bool isPlayerLine()
        {

            return name.Equals(PLAYER);
        }

        /**
         * Sets if the option is correct
         * 
         * @param if the option is correct
         */

        public void setXApiCorrect(bool xapiCorrect)
        {

            this.xapiCorrect = xapiCorrect;
        }

        /**
         * Sets the new name of the line.
         * 
         * @param name
         *            New name
         */

        public void setName(string name)
        {

            this.name = name;
        }

        /**
         * Sets the new text of the line.
         * 
         * @param text
         *            New text
         */

        public void setText(string text)
        {

            this.text = text;
        }

        /**
         * @return the conditions
         */

        public Conditions getConditions()
        {

            return conditions;
        }

        /**
         * @param conditions
         *            the conditions to set
         */

        public void setConditions(Conditions conditions)
        {

            this.conditions = conditions;
        }

        public bool isKeepShowing()
        {

            return keepShowing;
        }

        public void setKeepShowing(bool keepShowing)
        {

            this.keepShowing = keepShowing;
        }

        public object Clone()
        {
            ConversationLine cl = (ConversationLine)this.MemberwiseClone();
            cl.name = (name != null ? name : null);
            cl.text = (text != null ? text : null);
            cl.conditions = (conditions != null ? (Conditions)conditions.Clone() : null);
            cl.keepShowing = keepShowing;
            cl.resources = resources.ConvertAll(r => r.Clone() as ResourcesUni);
            return cl;
        }
    }
}