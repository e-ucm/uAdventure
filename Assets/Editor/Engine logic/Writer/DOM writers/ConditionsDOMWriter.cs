using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class ConditionsDOMWriter 
{

    /**
         * Constant for "condition" tag (general case)
         */
    public const int CONDITIONS = 0;

    /**
     * Constant for "init-condition" tag (timers)
     */
    public const int INIT_CONDITIONS = 1;

    /**
     * Constant for "end-condition" tag (timers)
     */
    public const int END_CONDITIONS = 2;

    /**
     * Constant for "global-state" tag
     */
    public const int GLOBAL_STATE = 3;

    /**
     * Private constructor.
     */

    private ConditionsDOMWriter()
    {

    }

    public static XmlNode buildDOM(Conditions conditions)
    {

        return buildDOM(CONDITIONS, conditions);
    }

    /**
     * Builds the DOM element for a global state
     * 
     * @param globalState
     * @return
     */

    public static XmlElement buildDOM(GlobalState globalState)
    {

        XmlElement conditionsNode = null;


        // Create the necessary elements to create the DOM
        XmlDocument doc = Writer.GetDoc();
        conditionsNode = doc.CreateElement("global-state");
        conditionsNode.SetAttribute("id", globalState.getId());
        XmlNode documentationNode = doc.CreateElement("documentation");
        documentationNode.AppendChild(doc.CreateTextNode(globalState.getDocumentation()));
        conditionsNode.AppendChild(documentationNode);

        // Iterate all the condition'blocks
        for (int i = 0; i < globalState.size(); i++)
        {
            List<Condition> block = globalState.get(i);
            // Single condition
            if (block.Count == 1)
            {
                XmlElement conditionElement = createConditionElement(doc, block[0]);
                doc.ImportNode(conditionElement, true);
                conditionsNode.AppendChild(conditionElement);

            }
            else if (block.Count > 1)
            {
                XmlNode eitherNode = createElementWithList("either", block);
                doc.ImportNode(eitherNode, true);
                conditionsNode.AppendChild(eitherNode);
            }
        }

        /*createElementWithList(doc, conditionsNode, globalState.getMainConditions() );
            // Create and write the either condition blocks
            for( int i = 0; i < globalState.getEitherConditionsBlockCount( ); i++ ) {
            	List<Condition> eitherBlock = globalState.getEitherConditions( i );
            	// Write it if the block is not empty.
            	if ( eitherBlock.size( )>0 ){
            		Node eitherNode = createElementWithList( "either", eitherBlock );
            		doc.adoptNode( eitherNode );
            		conditionsNode.AppendChild( eitherNode );
            	}
            }*/


        return conditionsNode;
    }

    public static XmlNode buildDOM(int type, Conditions conditions)
    {

        XmlNode conditionsNode = null;


        // Create the necessary elements to create the DOM
        XmlDocument doc = Writer.GetDoc();

        // Create the root node (with its children)
        if (type == CONDITIONS)
            conditionsNode = doc.CreateElement("condition");
        else if (type == INIT_CONDITIONS)
            conditionsNode = doc.CreateElement("init-condition");
        else if (type == END_CONDITIONS)
            conditionsNode = doc.CreateElement("end-condition");
        doc.ImportNode(conditionsNode, true);

        // Iterate all the condition'blocks
        for (int i = 0; i < conditions.size(); i++)
        {
            List<Condition> block = conditions.get(i);
            // Single condition
            if (block.Count == 1)
            {
                XmlElement conditionElement = createConditionElement(doc, block[0]);
                doc.ImportNode(conditionElement, true);
                conditionsNode.AppendChild(conditionElement);

            }
            else if (block.Count > 1)
            {
                XmlNode eitherNode = createElementWithList("either", block);
                doc.ImportNode(eitherNode, true);
                conditionsNode.AppendChild(eitherNode);
            }
        }
        // Create the root node (with its children)
        /*if (type == CONDITIONS)
            	conditionsNode = createElementWithList( "condition", conditions.getMainConditions( ) );
            else if (type == INIT_CONDITIONS)
            	conditionsNode = createElementWithList( "init-condition", conditions.getMainConditions( ) );
            else if (type == END_CONDITIONS)
            	conditionsNode = createElementWithList( "end-condition", conditions.getMainConditions( ) );
            doc.adoptNode( conditionsNode );

            // Create and write the either condition blocks
            for( int i = 0; i < conditions.getEitherConditionsBlockCount( ); i++ ) {
            	List<Condition> eitherBlock = conditions.getEitherConditions( i );
            	// Write it if the block is not empty.
            	if ( eitherBlock.size( )>0 ){
            		Node eitherNode = createElementWithList( "either", eitherBlock );
            		doc.adoptNode( eitherNode );
            		conditionsNode.AppendChild( eitherNode );
            	}
            }*/



        return conditionsNode;
    }

    private static XmlNode createElementWithList(String tagname, List<Condition> conditions)
    {

        XmlElement conditionsListNode = null;

        // Create the necessary elements to create the DOM
        XmlDocument doc = Writer.GetDoc();

        // Create the root node
        conditionsListNode = doc.CreateElement(tagname);

        createElementWithList(doc, conditionsListNode, conditions);
        return conditionsListNode;
    }

    private static XmlElement createConditionElement(XmlDocument doc, Condition condition)
    {

        XmlElement conditionElement = null;

        if (condition.getType() == Condition.FLAG_CONDITION)
        {
            // Create the tag
            if (condition.getState() == FlagCondition.FLAG_ACTIVE)
                conditionElement = doc.CreateElement("active");
            else if (condition.getState() == FlagCondition.FLAG_INACTIVE)
                conditionElement = doc.CreateElement("inactive");

            // Set the target flag and append it
            conditionElement.SetAttribute("flag", condition.getId());
        }
        else if (condition.getType() == Condition.VAR_CONDITION)
        {
            VarCondition varCondition = (VarCondition) condition;
            // Create the tag
            if (varCondition.getState() == VarCondition.VAR_EQUALS)
                conditionElement = doc.CreateElement("equals");
            else if (varCondition.getState() == VarCondition.VAR_NOT_EQUALS)
                conditionElement = doc.CreateElement("not-equals");
            else if (condition.getState() == VarCondition.VAR_GREATER_EQUALS_THAN)
                conditionElement = doc.CreateElement("greater-equals-than");
            else if (condition.getState() == VarCondition.VAR_GREATER_THAN)
                conditionElement = doc.CreateElement("greater-than");
            else if (condition.getState() == VarCondition.VAR_LESS_EQUALS_THAN)
                conditionElement = doc.CreateElement("less-equals-than");
            else if (condition.getState() == VarCondition.VAR_LESS_THAN)
                conditionElement = doc.CreateElement("less-than");

            // Set the target flag and append it
            conditionElement.SetAttribute("var", varCondition.getId());
            conditionElement.SetAttribute("value", varCondition.getValue().ToString());
        }
        else if (condition.getType() == Condition.GLOBAL_STATE_CONDITION)
        {
            GlobalStateCondition globalStateCondition = (GlobalStateCondition) condition;
            // Create the tag
            conditionElement = doc.CreateElement("global-state-ref");

            // Set the target flag and append it
            conditionElement.SetAttribute("id", globalStateCondition.getId());

            conditionElement.SetAttribute("value",
                globalStateCondition.getState() == GlobalStateCondition.GS_SATISFIED ? "true" : "false");
        }

        return conditionElement;
    }

    private static void createElementWithList(XmlDocument doc, XmlElement conditionsListNode, List<Condition> conditions)
    {

        // Write every condition
        foreach (Condition condition in conditions)
        {

            conditionsListNode.AppendChild(createConditionElement(doc, condition));
        }
    }
}