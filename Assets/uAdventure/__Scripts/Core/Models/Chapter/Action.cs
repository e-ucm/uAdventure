using System;
using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * An action that can be done during the game.
     */

    public class Action : Documented, HasTargetId, ICloneable
    {
        public Effects Effects { get { return effects; } set { effects = value; } }

        /**
         * An action of type examine.
         */
        public const int EXAMINE = 0;

        /**
         * An action of type grab.
         */
        public const int GRAB = 1;

        /**
         * An action of type give-to.
         */
        public const int GIVE_TO = 2;

        /**
         * An action of type use-with.
         */
        public const int USE_WITH = 3;

        /**
         * An action of type use
         */
        public const int USE = 4;

        /**
         * A custom action
         */
        public const int CUSTOM = 5;

        /**
         * A custom interaction action
         */
        public const int CUSTOM_INTERACT = 6;

        /**
         * An action of the type talk-to
         */
        public const int TALK_TO = 7;

        /**
         * An action of type drag to
         */
        public const int DRAG_TO = 8;

        /**
         * Stores the action type
         */
        private int type;

        /**
         * Documentation of the action.
         */
        private string documentation;

        /**
         * Id of the target of the action (in give to and use with)
         */
        private string idTarget;

        /**
         * Conditions of the action
         */
        private Conditions conditions;

        /**
         * Effects of performing the action
         */
        private Effects effects;

        /**
         * Alternative effects, when the conditions aren't OK
         */
        private Effects notEffects;

        /**
         * Activate not effects
         */
        private bool activatedNotEffects;

        /**
         * Effects to be launched when an "interaction" action (give to, drag, use with, custom interaction) is pressed 
         */
        private Effects clickEffects;

        /**
         * Activate click effects
         */
        private bool activatedClickEffects;

        /**
         * Indicates whether the character needs to go up to the object
         */
        private bool needsGoTo;

        /**
         * Indicates the minimum distance the character should leave between the
         * object and himself
         */
        private int keepDistance;

        /**
         * The conditions of the action are met
         */
        private bool conditionsAreMeet;

        /**
         * Constructor.
         * 
         * @param type
         *            The type of the action
         */

        public Action(int type) : this(type, null, new Conditions(), new Effects(), new Effects(), new Effects())
        {
            
        }

        /**
         * Constructor.
         * 
         * @param type
         *            The type of the action
         * @param idTarget
         *            The target of the action
         */

        public Action(int type, string idTarget)
            : this(type, idTarget, new Conditions(), new Effects(), new Effects(), new Effects())
        {
        }

        /**
         * Constructor  for actions that only need one object. This constructor cant't be called to 
         * create interactions actions (drag to, use with, give to, custom interaction) due to don't init clickEffects
         * 
         * @param type
         *            The type of the action
         * @param conditions
         *            The conditions of the action (must not be null)
         * @param effects
         *            The effects of the action (must not be null)
         * @param notEffects
         *            The effects of the action when the conditions aren't OK (must
         *            not be null)
         */

        public Action(int type, Conditions conditions, Effects effects, Effects notEffects)
            : this(type, null, conditions, effects, notEffects, new Effects())
        {

            // added attribute to the constructor: this co 

        }

        /**
         * Constructor
         * 
         * @param type
         *            The type of the action
         * @param idTarget
         *            The target of the action
         * @param conditions
         *            The conditions of the action (must not be null)
         * @param effects
         *            The effects of the action (must not be null)
         * @param notEffects
         *            The effects of the action when the conditions aren't OK (must
         *            not be null)
         * @param clickEffects
         *            The effects of the "interaction" action when user makes first click  
         */

        public Action(int type, string idTarget, Conditions conditions, Effects effects, Effects notEffects,
            Effects clickEffects)
        {

            this.type = type;
            this.idTarget = idTarget;
            this.conditions = conditions;
            this.effects = effects;
            this.notEffects = notEffects;
            this.clickEffects = clickEffects;
            documentation = null;

            switch (type)
            {
                case EXAMINE:
                    needsGoTo = false;
                    keepDistance = 0;
                    break;
                case GRAB:
                    needsGoTo = true;
                    keepDistance = 35;
                    break;
                case GIVE_TO:
                    needsGoTo = true;
                    keepDistance = 35;
                    break;
                case USE_WITH:
                    needsGoTo = true;
                    keepDistance = 35;
                    break;
                case USE:
                    needsGoTo = true;
                    keepDistance = 35;
                    break;
                case TALK_TO:
                    needsGoTo = true;
                    keepDistance = 35;
                    break;
                case DRAG_TO:
                    needsGoTo = false;
                    keepDistance = 0;
                    break;
                default:
                    needsGoTo = false;
                    keepDistance = 0;
                    break;
            }
        }

        /**
         * Returns the type of the action.
         * 
         * @return the type of the action
         */

        public int getType()
        {

            return type;
        }

        /**
         * Returns the documentation of the action.
         * 
         * @return the documentation of the action
         */

        public string getDocumentation()
        {

            return documentation;
        }

        /**
         * Returns the target of the action.
         * 
         * @return the target of the action
         */

        public string getTargetId()
        {

            return idTarget;
        }

        /**
         * Returns the conditions of the action.
         * 
         * @return the conditions of the action
         */

        public Conditions getConditions()
        {

            return conditions;
        }

        /**
         * Returns the effects of the action.
         * 
         * @return the effects of the action
         */

        public Effects getEffects()
        {

            return effects;
        }

        /**
         * Changes the documentation of this action.
         * 
         * @param documentation
         *            The new documentation
         */

        public void setDocumentation(string documentation)
        {

            this.documentation = documentation;
        }

        /**
         * Changes the id target of this action.
         * 
         * @param idTarget
         *            The new id target
         */

        public void setTargetId(string idTarget)
        {

            this.idTarget = idTarget;
        }

        /**
         * Changes the conditions for this next scene
         * 
         * @param conditions
         *            the new conditions
         */

        public void setConditions(Conditions conditions)
        {

            this.conditions = conditions;
        }

        /**
         * Changes the effects for this next scene
         * 
         * @param effects
         *            the new effects
         */

        public void setEffects(Effects effects)
        {

            this.effects = effects;
        }

        /**
         * @return the needsGoTo
         */

        public bool isNeedsGoTo()
        {

            return needsGoTo;
        }

        /**
         * @param needsGoTo
         *            the needsGoTo to set
         */

        public void setNeedsGoTo(bool needsGoTo)
        {

            this.needsGoTo = needsGoTo;
        }

        /**
         * @return the keepDistance
         */

        public int getKeepDistance()
        {

            return keepDistance;
        }

        /**
         * @param keepDistance
         *            the keepDistance to set
         */

        public void setKeepDistance(int keepDistance)
        {

            this.keepDistance = keepDistance;
        }

        /**
         * @return the notEffects
         */

        public Effects getNotEffects()
        {

            return notEffects;
        }

        /**
         * @param notEffects
         *            the notEffects to set
         */

        public void setNotEffects(Effects notEffects)
        {

            this.notEffects = notEffects;
        }

        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {
            Action a = (Action) super.clone( );
            a.conditions = ( conditions != null ? (Conditions) conditions.clone( ) : null );
            a.documentation = ( documentation != null ? new string(documentation ) : null );
            a.effects = ( effects != null ? (Effects) effects.clone( ) : null );
            a.idTarget = ( idTarget != null ? new string(idTarget ) : null );
            a.keepDistance = keepDistance;
            a.needsGoTo = needsGoTo;
            a.type = type;
            a.notEffects = ( notEffects != null ? (Effects) notEffects.clone( ) : null );
            a.activatedNotEffects = activatedNotEffects;
            a.activatedClickEffects = activatedClickEffects;
            a.clickEffects = (clickEffects != null ? (Effects) clickEffects.clone( ) :  null);
            a.conditionsAreMeet = conditionsAreMeet;
            return a;
        }*/

        /**
         * @return the activateNotEffects
         */

        public bool isActivatedNotEffects()
        {

            return activatedNotEffects;
        }

        /**
         * @param activateNotEffects
         *            the activateNotEffects to set
         */

        public void setActivatedNotEffects(bool activateNotEffects)
        {

            this.activatedNotEffects = activateNotEffects;
        }


        public bool isConditionsAreMeet()
        {

            return conditionsAreMeet;
        }


        public void setConditionsAreMeet(bool conditionsAreMeet)
        {

            this.conditionsAreMeet = conditionsAreMeet;
        }


        public Effects getClickEffects()
        {

            return clickEffects;
        }


        public void setClickEffects(Effects clickEffects)
        {

            this.clickEffects = clickEffects;
        }


        public bool isActivatedClickEffects()
        {

            return activatedClickEffects;
        }


        public void setActivatedClickEffects(bool activatedClickEffects)
        {

            this.activatedClickEffects = activatedClickEffects;
        }

        public virtual object Clone()
        {
            Action a = (Action)this.MemberwiseClone();
            a.conditions = (conditions != null ? (Conditions)conditions.Clone() : null);
            a.documentation = (documentation != null ? documentation : null);
            a.effects = (effects != null ? (Effects)effects.Clone() : null);
            a.idTarget = (idTarget != null ? idTarget : null);
            a.keepDistance = keepDistance;
            a.needsGoTo = needsGoTo;
            a.type = type;
            a.notEffects = (notEffects != null ? (Effects)notEffects.Clone() : null);
            a.activatedNotEffects = activatedNotEffects;
            a.activatedClickEffects = activatedClickEffects;
            a.clickEffects = (clickEffects != null ? (Effects)clickEffects.Clone() : null);
            a.conditionsAreMeet = conditionsAreMeet;
            return a;
        }
    }
}