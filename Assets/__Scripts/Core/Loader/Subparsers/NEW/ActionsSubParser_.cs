using UnityEngine;
using System.Collections;
using System.Xml;

public class ActionsSubParser_ : Subparser_
{
    /**
     * Stores the current conditions being read
     */
    private Conditions currentConditions;

    /**
     * Stores the current effects being read
     */
    private Effects currentEffects;

    /**
     * Stores the current not-effects being read
     */
    private Effects currentNotEffects;

    /**
     * Stores the current click-effects being read
     * 
     */ /**
     * Stores the current IdTarget being read
     */
    private string currentIdTarget;

    /**
     * Stores the current Name being read
     */
    private string currentName;

    /**
     * Stores the current needsGoTo being read
     */
    private bool currentNeedsGoTo;

    /**
     * Stores the current keepDinstance being read
     */
    private int currentKeepDistance;

    /**
     * Stores the current customAction being read
     */
    private CustomAction currentCustomAction;

    /**
     * Stores the current Resources being read
     */
    private ResourcesUni currentResources;

    /**
     * Activate not effects
     */
    bool activateNotEffects;

    /**
     * Activate click effects
     */
    bool activateClickEffects;

    private Effects currentClickEffects;
    private Element element;

    public ActionsSubParser_(Chapter chapter, Element element) : base(chapter)
    {
        this.element = element;
    }

    public override void ParseElement(XmlElement element)
    {
        string tmpArgVal;
        XmlElement tmpXmlEl;

        if (element.SelectSingleNode ("documentation") != null) {
            this.element.setDocumentation (element.SelectSingleNode ("documentation").InnerText);
            element.RemoveChild (element.SelectSingleNode ("documentation"));
        }

        foreach (XmlElement action in element.ChildNodes) {
            //First we parse the elements every action haves:
            tmpArgVal = action.GetAttribute("needsGoTo");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentNeedsGoTo = tmpArgVal.Equals("yes");
            }
            tmpArgVal = action.GetAttribute("keepDistance");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentKeepDistance = int.Parse(tmpArgVal);
            }
            tmpArgVal = action.GetAttribute("not-effects");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                activateNotEffects = tmpArgVal.Equals("yes");
            }
            tmpArgVal = action.GetAttribute("click-effects");

            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                activateClickEffects = tmpArgVal.Equals("yes");
            }
            tmpArgVal = action.GetAttribute("idTarget");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentIdTarget = tmpArgVal;
            }

            currentConditions = new Conditions();
            currentEffects = new Effects();
            currentNotEffects = new Effects();
            currentClickEffects = new Effects();
            tmpXmlEl = (XmlElement) action.SelectSingleNode ("condition");
            if (tmpXmlEl != null)
                new ConditionSubParser_ (currentConditions, chapter).ParseElement (tmpXmlEl);
            tmpXmlEl = (XmlElement) action.SelectSingleNode ("effect");
            if (tmpXmlEl != null)
                new EffectSubParser_ (currentEffects, chapter).ParseElement (tmpXmlEl);

            tmpXmlEl = (XmlElement) action.SelectSingleNode ("click-effect");
            if (tmpXmlEl != null)
                new EffectSubParser_ (currentClickEffects, chapter).ParseElement (tmpXmlEl);

            tmpXmlEl = (XmlElement) action.SelectSingleNode ("not-effect");
            if (tmpXmlEl != null)
                new EffectSubParser_ (currentNotEffects, chapter).ParseElement (tmpXmlEl);

            //Then we instantiate the correct action by name.
            //We also parse the elements that are unique of that action.
            Action currentAction = new Action(0);
            switch (action.Name) {
            case "examines":        currentAction = new Action(Action.EXAMINE, currentConditions, currentEffects, currentNotEffects); break;
            case "grabs":           currentAction = new Action(Action.GRAB, currentConditions, currentEffects, currentNotEffects); break;
            case "use":             currentAction = new Action(Action.USE, currentConditions, currentEffects, currentNotEffects); break;
            case "talk-to":         currentAction = new Action(Action.TALK_TO, currentConditions, currentEffects, currentNotEffects); break;
            case "use-with":        currentAction = new Action(Action.USE_WITH, currentIdTarget, currentConditions, currentEffects, currentNotEffects, currentClickEffects); break;
            case "give-to":         currentAction = new Action(Action.GIVE_TO, currentIdTarget, currentConditions, currentEffects, currentNotEffects, currentClickEffects); break;
            case "drag-to":         currentAction = new Action (Action.DRAG_TO, currentIdTarget, currentConditions, currentEffects, currentNotEffects, currentClickEffects); break;
            case "custom":
            case "custom-interact":
                CustomAction customAction = new CustomAction ((action.Name == "custom") ? Action.CUSTOM : Action.CUSTOM_INTERACT); 

                tmpArgVal = action.GetAttribute ("name");
                if (!string.IsNullOrEmpty (tmpArgVal)) {
                    currentName = tmpArgVal;
                }
                customAction.setName (currentName);

                tmpXmlEl = (XmlElement) action.SelectSingleNode ("resources");
                if (tmpXmlEl != null)
                    customAction.addResources (parseResources (tmpXmlEl));
                
                currentAction = customAction;
                break;
            }

            //Finally we set al the attributes to the action;
            currentAction.setConditions(currentConditions);
            currentAction.setEffects(currentEffects);
            currentAction.setNotEffects(currentNotEffects);
            currentAction.setKeepDistance(currentKeepDistance);
            currentAction.setNeedsGoTo(currentNeedsGoTo);
            currentAction.setActivatedNotEffects(activateNotEffects);
            currentAction.setClickEffects(currentClickEffects);
            currentAction.setActivatedClickEffects(activateClickEffects);

            this.element.addAction(currentAction);
        }
       



        /*foreach (XmlElement el in examines)
        {
            tmpArgVal = el.GetAttribute("needsGoTo");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentNeedsGoTo = tmpArgVal.Equals("yes");
            }
            tmpArgVal = el.GetAttribute("keepDistance");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentKeepDistance = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("not-effects");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                activateNotEffects = tmpArgVal.Equals("yes");
            }
            currentConditions = new Conditions();
            currentEffects = new Effects();
            currentNotEffects = new Effects();
            currentClickEffects = new Effects();

            Action examineAction = new Action(Action.EXAMINE, currentConditions, currentEffects, currentNotEffects);
            examineAction.setKeepDistance(currentKeepDistance);
            examineAction.setNeedsGoTo(currentNeedsGoTo);
            examineAction.setActivatedNotEffects(activateNotEffects);
            examineAction.setActivatedClickEffects(activateClickEffects);
            this.element.addAction(examineAction);
        }
        foreach (XmlElement el in grabs)
        {
            tmpArgVal = el.GetAttribute("needsGoTo");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentNeedsGoTo = tmpArgVal.Equals("yes");
            }
            tmpArgVal = el.GetAttribute("keepDistance");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentKeepDistance = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("not-effects");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                activateNotEffects = tmpArgVal.Equals("yes");
            }
            currentConditions = new Conditions();
            currentEffects = new Effects();
            currentNotEffects = new Effects();
            currentClickEffects = new Effects();

            Action grabAction = new Action(Action.GRAB, currentConditions, currentEffects, currentNotEffects);
            grabAction.setKeepDistance(currentKeepDistance);
            grabAction.setNeedsGoTo(currentNeedsGoTo);
            grabAction.setActivatedNotEffects(activateNotEffects);
            grabAction.setActivatedClickEffects(activateClickEffects);
            this.element.addAction(grabAction);
        }
        foreach (XmlElement el in uses)
        {
            tmpArgVal = el.GetAttribute("needsGoTo");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentNeedsGoTo = tmpArgVal.Equals("yes");
            }
            tmpArgVal = el.GetAttribute("keepDistance");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentKeepDistance = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("not-effects");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                activateNotEffects = tmpArgVal.Equals("yes");
            }
            currentConditions = new Conditions();
            currentEffects = new Effects();
            currentNotEffects = new Effects();
            currentClickEffects = new Effects();

            Action useAction = new Action(Action.USE, currentConditions, currentEffects, currentNotEffects);
            useAction.setNeedsGoTo(currentNeedsGoTo);
            useAction.setKeepDistance(currentKeepDistance);
            useAction.setActivatedNotEffects(activateNotEffects);
            useAction.setActivatedClickEffects(activateClickEffects);
            this.element.addAction(useAction);
        }
        foreach (XmlElement el in talksto)
        {
            tmpArgVal = el.GetAttribute("needsGoTo");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentNeedsGoTo = tmpArgVal.Equals("yes");
            }
            tmpArgVal = el.GetAttribute("keepDistance");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentKeepDistance = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("not-effects");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                activateNotEffects = tmpArgVal.Equals("yes");
            }
            currentConditions = new Conditions();
            currentEffects = new Effects();
            currentNotEffects = new Effects();
            currentClickEffects = new Effects();

            Action talkToAction = new Action(Action.TALK_TO, currentConditions, currentEffects, currentNotEffects);
            talkToAction.setNeedsGoTo(currentNeedsGoTo);
            talkToAction.setKeepDistance(currentKeepDistance);
            talkToAction.setActivatedNotEffects(activateNotEffects);
            talkToAction.setActivatedClickEffects(activateClickEffects);
            this.element.addAction(talkToAction);
        }


        foreach (XmlElement el in useswith)
        {
            tmpArgVal = el.GetAttribute("needsGoTo");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentNeedsGoTo = tmpArgVal.Equals("yes");
            }
            tmpArgVal = el.GetAttribute("keepDistance");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentKeepDistance = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("not-effects");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                activateNotEffects = tmpArgVal.Equals("yes");
            }
            tmpArgVal = el.GetAttribute("click-effects");

            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                activateClickEffects = tmpArgVal.Equals("yes");
            }
            tmpArgVal = el.GetAttribute("idTarget");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentIdTarget = tmpArgVal;
            }

            currentConditions = new Conditions();
            currentEffects = new Effects();
            currentNotEffects = new Effects();
            currentClickEffects = new Effects();

            Action useWithAction = new Action(Action.USE_WITH, currentIdTarget, currentConditions, currentEffects,
                currentNotEffects, currentClickEffects);
            useWithAction.setKeepDistance(currentKeepDistance);
            useWithAction.setNeedsGoTo(currentNeedsGoTo);
            useWithAction.setActivatedNotEffects(activateNotEffects);
            useWithAction.setActivatedClickEffects(activateClickEffects);
            this.element.addAction(useWithAction);
        }


        foreach (XmlElement el in dragsto)
        {
            tmpArgVal = el.GetAttribute("idTarget");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentIdTarget = tmpArgVal;
            }
            tmpArgVal = el.GetAttribute("needsGoTo");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentNeedsGoTo = tmpArgVal.Equals("yes");
            }
            tmpArgVal = el.GetAttribute("keepDistance");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentKeepDistance = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("not-effects");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                activateNotEffects = tmpArgVal.Equals("yes");
            }
            tmpArgVal = el.GetAttribute("click-effects");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                activateClickEffects = tmpArgVal.Equals("yes");
            }
            currentConditions = new Conditions();
            currentEffects = new Effects();
            currentNotEffects = new Effects();
            currentClickEffects = new Effects();

            Action useWithAction = new Action(Action.DRAG_TO, currentIdTarget, currentConditions, currentEffects,
                currentNotEffects, currentClickEffects);
            useWithAction.setKeepDistance(currentKeepDistance);
            useWithAction.setNeedsGoTo(currentNeedsGoTo);
            useWithAction.setActivatedNotEffects(activateNotEffects);
            useWithAction.setActivatedClickEffects(activateClickEffects);
            this.element.addAction(useWithAction);
        }


        foreach (XmlElement el in givesto)
        {
            tmpArgVal = el.GetAttribute("idTarget");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentIdTarget = tmpArgVal;
            }
            tmpArgVal = el.GetAttribute("needsGoTo");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentNeedsGoTo = tmpArgVal.Equals("yes");
            }
            tmpArgVal = el.GetAttribute("keepDistance");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentKeepDistance = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("not-effects");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                activateNotEffects = tmpArgVal.Equals("yes");
            }
            tmpArgVal = el.GetAttribute("click-effects");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                activateClickEffects = tmpArgVal.Equals("yes");
            }
            currentConditions = new Conditions();
            currentEffects = new Effects();
            currentNotEffects = new Effects();
            currentClickEffects = new Effects();

            Action giveToAction = new Action(Action.GIVE_TO, currentIdTarget, currentConditions, currentEffects,
                currentNotEffects, currentClickEffects);
            giveToAction.setKeepDistance(currentKeepDistance);
            giveToAction.setNeedsGoTo(currentNeedsGoTo);
            giveToAction.setActivatedNotEffects(activateNotEffects);
            giveToAction.setActivatedClickEffects(activateClickEffects);
            this.element.addAction(giveToAction);
        }


        foreach (XmlElement el in customs)
        {
            tmpArgVal = el.GetAttribute("idTarget");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentIdTarget = tmpArgVal;
            }
            tmpArgVal = el.GetAttribute("name");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentName = tmpArgVal;
            }
            tmpArgVal = el.GetAttribute("needsGoTo");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentNeedsGoTo = tmpArgVal.Equals("yes");
            }
            tmpArgVal = el.GetAttribute("keepDistance");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentKeepDistance = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("not-effects");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                activateNotEffects = tmpArgVal.Equals("yes");
            }
            tmpArgVal = el.GetAttribute("click-effects");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                activateClickEffects = tmpArgVal.Equals("yes");
            }


            currentConditions = new Conditions();
            currentEffects = new Effects();
            currentNotEffects = new Effects();
            currentClickEffects = new Effects();
            tmpXmlEl = el.SelectSingleNode ("condition");
            if (tmpXmlEl != null)
                new ConditionSubParser_ (currentConditions, chapter).ParseElement (tmpXmlEl);

            tmpXmlEl = el.SelectSingleNode ("effect");
            if (tmpXmlEl != null)
                new EffectSubParser_ (currentEffects, chapter).ParseElement (tmpXmlEl);

            tmpXmlEl = el.SelectSingleNode ("click-effect");
            if (tmpXmlEl != null)
                new EffectSubParser_ (currentClickEffects, chapter).ParseElement (tmpXmlEl);

            tmpXmlEl = el.SelectSingleNode ("not-effect");
            if (tmpXmlEl != null)
                new EffectSubParser_ (currentNotEffects, chapter).ParseElement (tmpXmlEl);

            currentCustomAction = new CustomAction(Action.CUSTOM);

            currentCustomAction.setName(currentName);
            currentCustomAction.setConditions(currentConditions);
            currentCustomAction.setEffects(currentEffects);
            currentCustomAction.setNotEffects(currentNotEffects);
            currentCustomAction.setKeepDistance(currentKeepDistance);
            currentCustomAction.setNeedsGoTo(currentNeedsGoTo);
            currentCustomAction.setActivatedNotEffects(activateNotEffects);
            currentCustomAction.setClickEffects(currentClickEffects);
            currentCustomAction.setActivatedClickEffects(activateClickEffects);
            XmlElement res = (XmlElement) el.SelectSingleNode ("resources");
            if (res != null)
                currentCustomAction.addResources(parseResources(res));
            this.element.addAction(currentCustomAction);
            currentCustomAction = null;
        }

        foreach (XmlElement el in customsinteract)
        {
            tmpArgVal = el.GetAttribute("idTarget");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentIdTarget = tmpArgVal;
            }
            tmpArgVal = el.GetAttribute("name");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentName = tmpArgVal;
            }
            tmpArgVal = el.GetAttribute("needsGoTo");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentNeedsGoTo = tmpArgVal.Equals("yes");
            }
            tmpArgVal = el.GetAttribute("keepDistance");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentKeepDistance = int.Parse(tmpArgVal);
            }
            tmpArgVal = el.GetAttribute("not-effects");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                activateNotEffects = tmpArgVal.Equals("yes");
            }
            tmpArgVal = el.GetAttribute("click-effects");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                activateClickEffects = tmpArgVal.Equals("yes");
            }
            currentConditions = new Conditions();
            currentEffects = new Effects();
            currentNotEffects = new Effects();
            currentClickEffects = new Effects();
            currentCustomAction = new CustomAction(Action.CUSTOM_INTERACT);

            currentCustomAction.setConditions(currentConditions);
            currentCustomAction.setEffects(currentEffects);
            currentCustomAction.setNotEffects(currentNotEffects);
            currentCustomAction.setName(currentName);
            currentCustomAction.setTargetId(currentIdTarget);
            currentCustomAction.setKeepDistance(currentKeepDistance);
            currentCustomAction.setNeedsGoTo(currentNeedsGoTo);
            currentCustomAction.setActivatedNotEffects(activateNotEffects);
            currentCustomAction.setClickEffects(currentClickEffects);
            currentCustomAction.setActivatedClickEffects(activateClickEffects);

            XmlElement res = (XmlElement) el.SelectSingleNode ("resources");
            if (res != null)
                currentCustomAction.addResources(parseResources(res));
            
            this.element.addAction(currentCustomAction);
            currentCustomAction = null;
        }

        foreach (XmlElement el in effects)
        {
            currentEffects = new Effects();
            new EffectSubParser_(currentEffects, chapter).ParseElement(el);
        }
        foreach (XmlElement el in notseffect)
        {
            currentNotEffects = new Effects();
            new EffectSubParser_(currentNotEffects, chapter).ParseElement(el);
        }
        foreach (XmlElement el in effects)
        {
            currentClickEffects = new Effects();
            new EffectSubParser_(currentClickEffects, chapter).ParseElement(el);
        }*/

    }


    private ResourcesUni parseResources(XmlElement resources){
        XmlNodeList assets, conditions;
        string tmpArgVal = "";

        currentResources = new ResourcesUni();

        tmpArgVal = resources.GetAttribute("name");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            currentResources.setName(tmpArgVal);
        }

        assets = resources.SelectNodes("asset");
        foreach (XmlElement asset in assets)
        {
            string type = "";
            string path = "";

            tmpArgVal = asset.GetAttribute("type");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                type = tmpArgVal;
            }
            tmpArgVal = asset.GetAttribute("uri");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                path = tmpArgVal;
            }
            currentResources.addAsset(type, path);
        }

        conditions = resources.SelectNodes("condition");
        foreach (XmlElement condition in conditions)
        {
            currentConditions = new Conditions();
            new ConditionSubParser_(currentConditions, chapter).ParseElement(condition);
            currentResources.setConditions(currentConditions);
        }

           
        return currentResources;
    }
}