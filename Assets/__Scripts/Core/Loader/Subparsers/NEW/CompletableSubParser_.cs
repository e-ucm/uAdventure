using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace uAdventure.Core
{
    public class CompletableSubParser_ : Subparser_
    {
        /// <summary>
        /// Completable object being parsed.
        /// </summary>
        private Completable completable;

        private string currentstring;

        public CompletableSubParser_(Chapter chapter) : base(chapter)
        {
            this.chapter = chapter;
        }

        public override void ParseElement(XmlElement element)
        {
            string tmpArgVal;
            XmlElement tmpXmlElement;

            completable = new Completable();

            tmpArgVal = element.GetAttribute("id");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                completable.setId(tmpArgVal);
            }

            tmpArgVal = element.GetAttribute("type");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                switch (tmpArgVal)
                {
                    case "combat": completable.setType(Completable.TYPE_COMBAT); break;
                    case "completable": completable.setType(Completable.TYPE_COMPLETABLE); break;
                    case "game": completable.setType(Completable.TYPE_GAME); break;
                    case "level": completable.setType(Completable.TYPE_LEVEL); break;
                    case "quest": completable.setType(Completable.TYPE_QUEST); break;
                    case "race": completable.setType(Completable.TYPE_RACE); break;
                    case "session": completable.setType(Completable.TYPE_SESSION); break;
                    case "stage": completable.setType(Completable.TYPE_STAGE); break;
                    case "storynode": completable.setType(Completable.TYPE_STORYNODE); break;
                    default: completable.setType(Completable.TYPE_COMPLETABLE); break;
                }
            }

            tmpXmlElement = (XmlElement)element.SelectSingleNode("start-milestone");
            if (tmpXmlElement != null)
            {
                completable.setStart(parseMilestone(tmpXmlElement));
            }

            tmpXmlElement = (XmlElement)element.SelectSingleNode("end-milestone");
            if (tmpXmlElement != null)
            {
                completable.setEnd(parseMilestone(tmpXmlElement));
            }

            tmpXmlElement = (XmlElement)element.SelectSingleNode("progress");
            if (tmpXmlElement != null)
            {
                completable.setProgress(parseProgress(tmpXmlElement));
            }

            tmpXmlElement = (XmlElement)element.SelectSingleNode("score");
            if (tmpXmlElement != null)
            {
                completable.setScore(parseScore(tmpXmlElement));
            }

            chapter.addCompletable(completable);
        }


        private Completable.Milestone parseMilestone(XmlElement element)
        {
            Completable.Milestone milestone = new Completable.Milestone();

            string tmpString = "";



            tmpString = element.GetAttribute("type");
            if (!string.IsNullOrEmpty(tmpString))
            {
                milestone.setType(ParseEnum<Completable.Milestone.MilestoneType>(tmpString));
            }

            if (milestone.getType() != Completable.Milestone.MilestoneType.CONDITION)
            {
                tmpString = element.GetAttribute("targetId");
                if (!string.IsNullOrEmpty(tmpString))
                {
                    milestone.setId(tmpString);
                }
            }
            else
            {
                XmlElement c = (XmlElement)element.SelectSingleNode("condition");

                if (c != null)
                {
                    Conditions conditions = new Conditions();
                    new ConditionSubParser_(conditions, chapter).ParseElement(c);
                    milestone.setConditions(conditions);
                }
            }

            tmpString = element.GetAttribute("progress");
            if (!string.IsNullOrEmpty(tmpString))
            {
                milestone.setProgress(float.Parse(tmpString));
            }

            return milestone;
        }

        private Completable.Progress parseProgress(XmlElement element)
        {
            Completable.Progress progress = new Completable.Progress();

            string tmpString = "";

            tmpString = element.GetAttribute("type");
            if (!string.IsNullOrEmpty(tmpString))
            {
                progress.setType(ParseEnum<Completable.Progress.ProgressType>(tmpString));
            }

            foreach (XmlElement milestone in element.ChildNodes)
            {
                progress.addMilestone(parseMilestone(milestone));
            }

            return progress;
        }

        private Completable.Score parseScore(XmlElement element)
        {
            Completable.Score score = new Completable.Score();

            string tmpString = "";

            tmpString = element.GetAttribute("type");
            if (!string.IsNullOrEmpty(tmpString))
            {
                score.setType(ParseEnum<Completable.Score.ScoreType>(tmpString));
            }

            tmpString = element.GetAttribute("method");
            if (!string.IsNullOrEmpty(tmpString))
            {
                score.setMethod(ParseEnum<Completable.Score.ScoreMethod>(tmpString));
            }

            if (score.getMethod() == Completable.Score.ScoreMethod.SINGLE)
            {
                tmpString = element.GetAttribute("id");
                if (!string.IsNullOrEmpty(tmpString))
                {
                    score.setId(tmpString);
                }
            }
            else
            {
                XmlNode subscores = element.SelectSingleNode("sub-scores");

                if (subscores != null)
                    foreach (XmlElement subscore in subscores.ChildNodes)
                    {
                        score.addSubScore(parseScore(subscore));
                    }
            }

            return score;
        }

        public static T ParseEnum<T>(string value)
        {
            return (T)System.Enum.Parse(typeof(T), value, true);
        }
    }
}