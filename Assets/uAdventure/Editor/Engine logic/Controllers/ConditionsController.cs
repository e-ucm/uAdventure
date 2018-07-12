using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using uAdventure.Core;

namespace uAdventure.Editor
{
    /**
     * Controller for the conditions. This class operates on the conditions blocks
     * to add new single conditions, and either conditions blocks.
     */

    public class ConditionsController
    {
        public const int VAR_CONDITION = Condition.VAR_CONDITION;

        public const int FLAG_CONDITION = Condition.FLAG_CONDITION;

        public const int GLOBAL_STATE_CONDITION = Condition.GLOBAL_STATE_CONDITION;

        /*
         * Constants for KEYs (and some VALUES) of the Dictionary getCondition returns
         */
        // FLAG; VAR; GLOBAL_STATE
        public const string CONDITION_TYPE = "condition-type";

        public const string CONDITION_TYPE_VAR = "var";

        public const string CONDITION_TYPE_FLAG = "flag";

        public const string CONDITION_TYPE_GS = "global-state";

        public const string CONDITION_ID = "condition-id";

        public const string CONDITION_VALUE = "condition-value";

        //active|inactive|<,<=,>,>=,=
        public const string CONDITION_STATE = "condition-state";

        /*
         * Constants for setEvalFunction (param value)
         */
        public const int EVAL_FUNCTION_AND = 0;

        public const int EVAL_FUNCTION_OR = 1;

        // Constant for setEvalFunction (index)
        public const int INDEX_NOT_USED = -1;

        // KEYS FOR Context Dictionary
        public static string CONDITION_GROUP_TYPE = "condition-type";

        public static string CONDITION_RESTRICTIONS = "condition-restrictions";

        public static string CONDITION_OWNER = "condition-owner";

        public static string CONDITION_CUSTOM_MESSAGE = "condition-custom-message";

        /**
         * string values for the states of a condition
         */

        public static readonly string[] STATE_VALUES = new string[]
        {"Active", "Inactive", ">", ">=", "<", "<=", "=", "!=", "Met", "Not met"};

        /**
         * string values for the states of a flag condition
         */
        public static readonly string[] STATE_VALUES_FLAGS = { "Active", "Inactive" };

        /**
         * string values for the states of a varcondition
         */
        public static readonly string[] STATE_VALUES_VARS = { ">", ">=", "<", "<=", "=", "!=" };

        /**
         * string values for the states of a gs condition
         */
        public static readonly string[] STATE_VALUES_GLOBALSTATE = { "Met", "Not met" };

        /**
         * Returns the int value of the string state given.
         * 
         * @param stringState
         *            Condition state in string form
         * @return Int value of state
         */

        public static int getStateFromstring(string stringState)
        {

            int state = Condition.NO_STATE;

            if (stringState.Equals(STATE_VALUES[0]))
                state = FlagCondition.FLAG_ACTIVE;

            else if (stringState.Equals(STATE_VALUES[1]))
                state = FlagCondition.FLAG_INACTIVE;

            else if (stringState.Equals(STATE_VALUES[2]))
                state = VarCondition.VAR_GREATER_THAN;

            else if (stringState.Equals(STATE_VALUES[3]))
                state = VarCondition.VAR_GREATER_EQUALS_THAN;

            else if (stringState.Equals(STATE_VALUES[4]))
                state = VarCondition.VAR_LESS_THAN;

            else if (stringState.Equals(STATE_VALUES[5]))
                state = VarCondition.VAR_LESS_EQUALS_THAN;

            else if (stringState.Equals(STATE_VALUES[6]))
                state = VarCondition.VAR_EQUALS;

            else if (stringState.Equals(STATE_VALUES[7]))
                state = VarCondition.VAR_NOT_EQUALS;

            else if (stringState.Equals(STATE_VALUES[8]))
                state = GlobalStateCondition.GS_SATISFIED;

            else if (stringState.Equals(STATE_VALUES[9]))
                state = GlobalStateCondition.GS_NOT_SATISFIED;
            return state;
        }

        public static int getTypeFromstring(string newType)
        {

            int newTypeInt = -1;
            if (newType.Equals(ConditionsController.CONDITION_TYPE_FLAG))
                newTypeInt = Condition.FLAG_CONDITION;
            if (newType.Equals(ConditionsController.CONDITION_TYPE_VAR))
                newTypeInt = Condition.VAR_CONDITION;
            if (newType.Equals(ConditionsController.CONDITION_TYPE_GS))
                newTypeInt = Condition.GLOBAL_STATE_CONDITION;
            return newTypeInt;
        }

        /**
         * Updates the given flag summary, adding the flag references contained in
         * the given conditions.
         * 
         * @param varFlagSummary
         *            Flag summary to update
         * @param conditions
         *            Set of conditions to search in
         */

        public static void updateVarFlagSummary(VarFlagSummary varFlagSummary, Conditions conditions)
        {

            // First check the main block of conditions
            foreach (Condition condition in conditions.GetSimpleConditions())
            {
                if (condition.getType() == Condition.FLAG_CONDITION)
                    varFlagSummary.addFlagReference(condition.getId());
                else if (condition.getType() == Condition.VAR_CONDITION)
                    varFlagSummary.addVarReference(condition.getId());
            }

            // Then add the references from the either blocks
            for (int i = 0; i < conditions.GetEitherConditionsBlockCount(); i++)
            {
                foreach (Condition condition in conditions.GetEitherConditions(i))
                {
                    if (condition.getType() == Condition.FLAG_CONDITION)
                        varFlagSummary.addFlagReference(condition.getId());
                    else if (condition.getType() == Condition.VAR_CONDITION)
                        varFlagSummary.addVarReference(condition.getId());
                }
            }
        }

        public static Dictionary<string, ConditionContextProperty> createContextFromOwner(int ownerType, string ownerName)
        {

            Dictionary<string, ConditionContextProperty> context1 = new Dictionary<string, ConditionContextProperty>();
            ConditionOwner owner = new ConditionOwner(ownerType, ownerName);
            context1.Add(ConditionsController.CONDITION_OWNER, owner);

            if (TC.containsConditionsContextText(ownerType, TC.NORMAL_SENTENCE) && TC.containsConditionsContextText(ownerType, TC.NO_CONDITION_SENTENCE))
            {
                ConditionCustomMessage cMessage = new ConditionCustomMessage(TC.getConditionsContextText(ownerType, TC.NORMAL_SENTENCE), TC.getConditionsContextText(ownerType, TC.NO_CONDITION_SENTENCE));
                context1.Add(CONDITION_CUSTOM_MESSAGE, cMessage);
            }

            return context1;
        }

        /* private static Dictionary<string, ConditionContextProperty> createContextFromOwnerMessage( int ownerType, string ownerName, string message1, string message2 ) {

             Dictionary<string, ConditionContextProperty> context1 = new Dictionary<string, ConditionContextProperty>( );
             ConditionOwner owner = new ConditionOwner( ownerType, ownerName );

             ConditionCustomMessage cMessage = new ConditionCustomMessage( message1, message2 );
             context1.put( CONDITION_CUSTOM_MESSAGE, cMessage );
             context1.put( ConditionsController.CONDITION_OWNER, owner );
             return context1;
         }*/

        // Attributes
        private Dictionary<string, ConditionContextProperty> context;

        /**
         * Conditions data.
         */
        private Conditions conditions;
        public Conditions Conditions
        {
            get { return conditions; }
            set { this.conditions = value; }
        }

        // Constructors

        /**
         * Creates a new conditions controller with an empty context. Thus the
         * controller will not know anything about its context (where it is, the
         * parent it belongs to, special restrictions, etc.)
         */

        public ConditionsController(Conditions conditions) : this(conditions, new Dictionary<string, ConditionContextProperty>())
        {
        }

        /**
         * Creates a new conditions controller and sets the context with information
         * about the parent of the conditions (that is, the node of the data tree
         * which owns the conditions). The information provided is the type of the
         * owner (e.g. Controller.ACTIVE_AREA) and its name. This information will
         * only be used by the ConditionsPanel
         * 
         * @param conditions
         * @param ownerType
         * @param ownerName
         */

        public ConditionsController(Conditions conditions, int ownerType, string ownerName) : this(conditions, createContextFromOwner(ownerType, ownerName))
        {
        }

        /**
         * Creates a new conditions controller and sets the context with information
         * about the parent of the conditions (that is, the node of the data tree
         * which owns the conditions). In addition customized messages for
         * ConditionsPanel are provided. The information provided is the type of the
         * owner (e.g. Controller.ACTIVE_AREA) and its name. This information will
         * only be used by the ConditionsPanel, along with the messages used to
         * format the text to be displayed
         * 
         * @param conditions
         * @param ownerType
         * @param ownerName
         * @param message1
         * @param message2
         */
        /*  public ConditionsController( Conditions conditions, int ownerType, string ownerName, string message1, string message2 ) {

              this( conditions, createContextFromOwnerMessage( ownerType, ownerName, message1, message2 ) );
          }*/

        /**
         * Constructor.
         * 
         * @param conditions
         *            Conditions block of data
         */

        public ConditionsController(Conditions conditions, Dictionary<string, ConditionContextProperty> context)
        {

            this.conditions = conditions;
            this.context = context;
        }


        //private static string[] exceptions(string firstId)
        //{
        //    List<string> pending = new List<string>();
        //    List<string> exceptions = new List<string>();
        //    pending.Add(firstId);
        //    while (pending.Count>0)
        //    {
        //        string id = pending[0];
        //        pending.RemoveAt(0);
        //        exceptions.Add(id);
        //        string[] gsIds =
        //            Controller.getInstance().getIdentifierSummary().getGlobalStatesIds(exceptions.ToArray());
        //        foreach (string gsId in gsIds)
        //        {
        //            foreach (GlobalState gs in
        //            Controller.getInstance()
        //                .getSelectedChapterDataControl()
        //                .getGlobalStatesListDataControl()
        //                .getGlobalStatesList())
        //            {
        //                if (gs.getId().Equals(gsId))
        //                {
        //                    if (gs.getGloblaStateIds().Contains(id))
        //                    {
        //                        pending.Add(gsId);
        //                    }
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    return exceptions.ToArray();
        //}

        /**
         * Returns the number of blocks of the condition group. Simple conditions
         * and either groups are considered as blocks.
         * 
         * @return The number of blocks
         */

        public int getBlocksCount()
        {

            return conditions.Size();
        }

        /**
         * Returns true if the conditions group has no blocks. False otherwise
         * 
         * @return
         */

        public bool isEmpty()
        {

            return conditions.IsEmpty();
        }

        /**
         * Returns the number of simple conditions a block with the given index has.
         * If index1 is not in the range -1 is returned. This method can be used to
         * determine if a block is a simple condition (size==1) or an either block
         * (size>1). This method never returns 0
         * 
         * @param index1
         * @return
         */

        public int getConditionCount(int index1)
        {

            // Check index
            if (index1 < 0 || index1 >= conditions.Size())
                return -1;

            return conditions.Get(index1).Count;
        }

        /**
         * Deletes the selected condition, if indexes are in range
         * 
         * @param index1
         * @param index2
         * @return
         */

        //public bool deleteCondition(int index1, int index2)
        //{

        //    // Check index
        //    if (index1 < 0 || index1 >= conditions.size())
        //        return false;

        //    if (index2 < 0 || index2 >= conditions.get(index1).Count)
        //        return false;

        //    return Controller.getInstance().addTool(new DeleteConditionTool(conditions, index1, index2));
        //}

        /**
         * Duplicates the selected condition
         * 
         * @param index1
         * @param index2
         * @return
         */

        //public bool duplicateCondition(int index1, int index2)
        //{

        //    // Check index
        //    if (index1 < 0 || index1 >= conditions.size())
        //        return false;

        //    if (index2 < 0 || index2 >= conditions.get(index1).Count)
        //        return false;

        //    return Controller.getInstance().addTool(new DuplicateConditionTool(conditions, index1, index2));
        //}

        //public bool setCondition(int index1, int index2, Dictionary<string, string> properties)
        //{

        //    // Check index
        //    if (index1 < 0 || index1 >= conditions.size())
        //        return false;

        //    if (index2 < 0 || index2 >= conditions.get(index1).Count)
        //        return false;
        //    // here CONDITION_STATE has a default value in "Global State Condition", not is necessary
        //    if (properties[CONDITION_TYPE] == null || properties[CONDITION_ID] == null ||
        //        properties[CONDITION_STATE] == null)
        //        return false;

        //    if (properties[CONDITION_TYPE].Equals(ConditionsController.CONDITION_TYPE_VAR) &&
        //        properties[CONDITION_VALUE] == null)
        //        return false;

        //    return Controller.getInstance().addTool(new SetConditionTool(conditions, index1, index2, properties));
        //}

        /**
         * Delete all global states with "id"
         * 
         * @param id
         *            the global state identifier to delete
         */

        public void deleteIdentifierReferences(string id)
        {
            foreach (List<Condition> c in conditions.GetConditionsList())
            {
                foreach (Condition con in c)
                {
                    if (con.getId().Equals(id))
                    {
                        c.Remove(con);
                    }
                }
                if (c.Count == 0)
                    conditions.GetConditionsList().Remove(c);
            }
        }

        /**
         * Replace identifiers, if oldId is found.
         * 
         * @param oldId
         * @param newId
         */

        public void replaceIdentifierReferences(string oldId, string newId)
        {

            for (int i = 0; i < conditions.Size(); i++)
            {
                for (int j = 0; j < conditions.Get(i).Count; j++)
                {
                    if (conditions.Get(i)[j].getId().Equals(oldId))
                        conditions.Get(i)[j].setId(newId);
                }
            }

        }

        /**
         * Count the number of times that id appears in conditions
         * 
         * @param id
         * @return
         */

        public int countIdentifierReferences(string id)
        {

            int count = 0;

            for (int i = 0; i < conditions.Size(); i++)
            {
                for (int j = 0; j < conditions.Get(i).Count; j++)
                {
                    if (conditions.Get(i)[j].getId().Equals(id))
                        count++;
                }
            }

            return count;
        }

        public Dictionary<string, string> getCondition(int index1, int index2)
        {

            Dictionary<string, string> conditionProperties = new Dictionary<string, string>();

            // Check index
            if (index1 < 0 || index1 >= conditions.Size())
                return null;

            var conditionsList = conditions.Get(index1);
            // Check index2
            if (index2 < 0 || index2 >= conditionsList.Count)
                return null;

            Condition condition = conditionsList[index2];
            // Put ID
            conditionProperties.Add(CONDITION_ID, condition.getId());

            // Put State
            conditionProperties.Add(CONDITION_STATE, condition.getState().ToString());
            // Put Type
            if (condition.getType() == Condition.FLAG_CONDITION)
            {
                conditionProperties.Add(CONDITION_TYPE, CONDITION_TYPE_FLAG);
                //Put value
                conditionProperties.Add(CONDITION_VALUE, condition.getState().ToString());
            }
            else if (condition.getType() == Condition.VAR_CONDITION)
            {
                conditionProperties.Add(CONDITION_TYPE, CONDITION_TYPE_VAR);
                //Put value
                VarCondition varCondition = (VarCondition)condition;
                conditionProperties.Add(CONDITION_VALUE, varCondition.getValue().ToString());
            }
            else if (condition.getType() == Condition.GLOBAL_STATE_CONDITION)
            {
                conditionProperties.Add(CONDITION_TYPE, CONDITION_TYPE_GS);
                //Put value
                conditionProperties.Add(CONDITION_VALUE, condition.getState().ToString());
            }

            return conditionProperties;
        }

        /**
         * Adds a new condition to the given block.
         * 
         * @param blockIndex
         *            The index of the conditions block. Use MAIN_CONDITIONS_BLOCK
         *            (-1) to select the main block of conditions, and values from 0
         *            to getEitherConditionsBlockCount( ) to access the either
         *            blocks of conditions
         * @param conditionId
         *            Id of the condition
         * @param conditionState
         *            State of the condition
         */

        public bool addCondition(int index1, int index2, string conditionType, string conditionId, string conditionState,
            string value)
        {

            if (index1 < 0 || index1 > conditions.Size())
                return false;

            if (conditionType == null || conditionId == null || conditionState == null)
                return false;

            if (conditionType.Equals(ConditionsController.CONDITION_TYPE_VAR) && value == null)
                return false;

            return
                Controller.Instance                    .AddTool(new AddConditionTool(conditions, index1, index2, conditionType, conditionId, conditionState,
                        value));
        }

        /**
         * Sets the evaluation function at the given index
         * 
         * @param index1
         *            Int value in rage
         * @param index2
         *            Int value in rage or {@link #INDEX_NOT_USED} if not applicable
         * @param value
         *            {@link #EVAL_FUNCTION_AND} | {@link #EVAL_FUNCTION_OR}
         * @return
         */

        //public bool setEvalFunction(int index1, int index2, int value)
        //{

        //    // Check value
        //    if (value != EVAL_FUNCTION_AND && value != EVAL_FUNCTION_OR)
        //        return false;

        //    // Check index
        //    // Check if the algorithm must search deeper (index2==-1 means no search inside blocks must be carried out)
        //    if (index2 == -1)
        //    {
        //        if (index1 < 0 || index1 >= conditions.size() - 1)
        //            return false;
        //    }
        //    else if (index2 >= 0)
        //    {
        //        if (index1 < 0 || index1 >= conditions.size())
        //            return false;
        //    }

        //    return Controller.getInstance().addTool(new SetEvalFunctionTool(conditions, index1, index2, value));
        //}

        //Condition type. Values: GLOBAL_STATE | CONDITION

        /**
         * The Dictionary of the context
         */

        public Dictionary<string, ConditionContextProperty> getContext()
        {
            Dictionary<string, ConditionContextProperty> currentContext = new Dictionary<string, ConditionContextProperty>();
            foreach (string key in context.Keys)
            {
                currentContext.Add(key, context[key]);
            }

            if (currentContext.ContainsKey(CONDITION_OWNER))
            {
                ConditionOwner owner = (ConditionOwner)currentContext[CONDITION_OWNER];
                if (owner.getOwnerType() == Controller.GLOBAL_STATE)
                {
                    //string ownerId = owner.getOwnerName();
                    //ConditionRestrictions restrictions = new ConditionRestrictions( new string[] { ownerId } );
                    //ConditionRestrictions restrictions = new ConditionRestrictions(exceptions(ownerId));
                    //currentContext.Add(CONDITION_RESTRICTIONS, restrictions);
                }
            }

            return currentContext;
        }

        public abstract class ConditionContextProperty
        {

            public ConditionContextProperty(string type)
            {

                this.type = type;
            }

            private string type;

            public string getType()
            {

                return type;
            }
        }

        public class ConditionRestrictions : ConditionContextProperty
        {

            private string[] forbiddenIds;

            public ConditionRestrictions(string[] forbiddenIds) : base(CONDITION_RESTRICTIONS)
            {

                this.forbiddenIds = forbiddenIds;
            }

            public ConditionRestrictions(string forbiddenId) : this(new string[] { forbiddenId })
            {
            }

            public string[] getForbiddenIds()
            {

                return forbiddenIds;
            }
        }

        /**
         * @author Javier
         * 
         */

        public class ConditionCustomMessage : ConditionContextProperty
        {

            public const string ELEMENT_TYPE = "{#ELEMENT_TYPE$}";

            public const string ELEMENT_ID = "{#ELEMENT_ID$}";

            private string sentence;

            private string sentenceNoConditions;

            public ConditionCustomMessage(string[] sentencestrings, string[] noConditionstrings) :
                base(CONDITION_CUSTOM_MESSAGE)
            {


                sentence = "";
                foreach (string s in sentencestrings)
                {
                    sentence += s + " ";
                }
                if (sentencestrings.Length > 0)
                {
                    sentence = sentence.Substring(0, sentence.Length - 1);
                }

                sentenceNoConditions = "";
                foreach (string s in noConditionstrings)
                {
                    sentenceNoConditions += s + " ";
                }
                if (noConditionstrings.Length > 0)
                {
                    sentenceNoConditions = sentenceNoConditions.Substring(0, sentenceNoConditions.Length - 1);
                }
            }

            public ConditionCustomMessage(List<string> sentencestrings, List<string> noConditionstrings)
                : this(sentencestrings.ToArray(), noConditionstrings.ToArray())
            {
            }

            public ConditionCustomMessage(string sentence, string noConditionSentence) :
                base(CONDITION_CUSTOM_MESSAGE)
            {

                this.sentence = sentence;
                this.sentenceNoConditions = noConditionSentence;
            }

            private string formatSentence(ConditionOwner owner, string sentence)
            {

                string formattedSentence = sentence;
                if (sentence.Contains(ELEMENT_TYPE))
                {
                    //formattedSentence = formattedSentence.replace(ELEMENT_TYPE,
                    //    "<i>" + Language.GetTextElement(owner.getOwnerType()) + "</i>");
                }
                if (sentence.Contains(ELEMENT_ID))
                {
                    formattedSentence = formattedSentence.Replace(ELEMENT_ID, "<b>\"" + owner.getOwnerName() + "\"</b>");
                }
                return formattedSentence;
            }

            public string getFormattedSentence(ConditionOwner owner)
            {

                return formatSentence(owner, sentence);
            }

            public string getNoConditionFormattedSentence(ConditionOwner owner)
            {

                return formatSentence(owner, sentenceNoConditions);
            }

        }

        /**
         * Class associated with KEY #CONDITION_OWNER
         * 
         * @author Javier
         * 
         */

        public class ConditionOwner : ConditionContextProperty
        {

            private int ownerType;

            private string ownerName;

            private ConditionOwner parent;

            public ConditionOwner(int ownerType, string ownerName, ConditionOwner parent) :
                base(CONDITION_OWNER)
            {

                this.ownerType = ownerType;
                this.ownerName = ownerName;
                this.parent = parent;
            }

            public ConditionOwner(int ownerType, string ownerName) :
                this(ownerType, ownerName, null)
            {
            }

            /**
             * @return the ownerType
             */

            public int getOwnerType()
            {

                return ownerType;
            }

            /**
             * @return the owner name
             */

            public string getOwnerName()
            {

                return ownerName;
            }

            /**
             * @return the owner name
             */

            public ConditionOwner getParent()
            {

                return parent;
            }

        }

        public static string getVarOperatorFromstring(string s)
        {

            int op = int.Parse(s);

            switch (op)
            {
                case VarCondition.VAR_GREATER_THAN:
                    return STATE_VALUES[2];
                case VarCondition.VAR_GREATER_EQUALS_THAN:
                    return STATE_VALUES[3];
                case VarCondition.VAR_LESS_THAN:
                    return STATE_VALUES[4];
                case VarCondition.VAR_LESS_EQUALS_THAN:
                    return STATE_VALUES[5];
                case VarCondition.VAR_EQUALS:
                    return STATE_VALUES[6];
                case VarCondition.VAR_NOT_EQUALS:
                    return STATE_VALUES[7];
                default:
                    return STATE_VALUES[2];
            }

        }

        public static string getGSOperatorFromstring(string s)
        {

            int op = int.Parse(s);

            switch (op)
            {
                case GlobalStateCondition.GS_SATISFIED:
                    return STATE_VALUES[8];
                case GlobalStateCondition.GS_NOT_SATISFIED:
                    return STATE_VALUES[9];
                default:
                    return STATE_VALUES[8];
            }

        }

        public void clearConditions()
        {

            Controller.Instance.AddTool(new ClearConditionsTool(conditions));

        }
    }
}