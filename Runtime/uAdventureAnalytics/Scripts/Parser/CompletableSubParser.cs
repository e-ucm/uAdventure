using System.Xml;
using uAdventure.Core;

namespace uAdventure.Analytics
{
	[DOMParser("completable")]
	[DOMParser(typeof(Completable))]
	public class CompletableSubParser : IDOMParser
    {
		public object DOMParse(XmlElement element, params object[] parameters)
        {
            XmlElement tmpXmlElement;

            var completable = new Completable();

			completable.setId(element.GetAttribute("id"));

            completable.setRepeatable(element.GetAttribute("repeatable") == "true");

            switch (element.GetAttribute("type"))
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

            tmpXmlElement = (XmlElement)element.SelectSingleNode("start-milestone");
            if (tmpXmlElement != null)
            {
				completable.setStart(parseMilestone(tmpXmlElement, parameters));
            }

            tmpXmlElement = (XmlElement)element.SelectSingleNode("end-milestone");
            if (tmpXmlElement != null)
            {
				completable.setEnd(parseMilestone(tmpXmlElement, parameters));
            }

            tmpXmlElement = (XmlElement)element.SelectSingleNode("progress");
            if (tmpXmlElement != null)
            {
				completable.setProgress(parseProgress(tmpXmlElement, parameters));
            }

            tmpXmlElement = (XmlElement)element.SelectSingleNode("score");
            if (tmpXmlElement != null)
            {
				completable.setScore(parseScore(tmpXmlElement, parameters));
            }

			return completable;
        }


		private Completable.Milestone parseMilestone(XmlElement element, params object[] parameters)
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
					milestone.setConditions(DOMParserUtility.DOMParse (c, parameters) as Conditions ?? new Conditions());
                }
            }

            tmpString = element.GetAttribute("progress");
            if (!string.IsNullOrEmpty(tmpString))
            {
                milestone.setProgress(float.Parse(tmpString));
            }

            return milestone;
        }

		private Completable.Progress parseProgress(XmlElement element, params object[] parameters)
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
				progress.addMilestone(parseMilestone(milestone, parameters));
            }

            return progress;
        }

		private Completable.Score parseScore(XmlElement element, params object[] parameters)
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
						score.addSubScore(parseScore(subscore, parameters));
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