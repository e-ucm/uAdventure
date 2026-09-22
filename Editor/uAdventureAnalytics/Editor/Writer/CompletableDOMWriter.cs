using System.Xml;
using uAdventure.Editor;

namespace uAdventure.Analytics
{
    [DOMWriter(typeof(Completable))]
    public class CompletableDOMWriter : ParametrizedDOMWriter
    {

        public CompletableDOMWriter()
        {

        }
        
        protected override string GetElementNameFor(object target)
        {
            return "completable";
        }

        /**
         * Returns the DOM element for the chapter
         * 
         * @param chapter
         *            Chapter data to be written
         * @return DOM element with the chapter data
         */
        protected override void FillNode(XmlNode node, object target, params IDOMWriterParam[] options)
        {
            var completable = target as Completable;
            XmlElement completableNode = node as XmlElement;

            string type = "";
            switch (completable.getType())
            {
                case Completable.TYPE_COMBAT: type = "combat"; break;
                case Completable.TYPE_COMPLETABLE: type = "completable"; break;
                case Completable.TYPE_GAME: type = "game"; break;
                case Completable.TYPE_LEVEL: type = "level"; break;
                case Completable.TYPE_QUEST: type = "quest"; break;
                case Completable.TYPE_RACE: type = "race"; break;
                case Completable.TYPE_SESSION: type = "session"; break;
                case Completable.TYPE_STAGE: type = "stage"; break;
                case Completable.TYPE_STORYNODE: type = "storynode"; break;
                default: type = "completable"; break;
            }

            completableNode.SetAttribute("id", completable.getId());
            completableNode.SetAttribute("repeatable", completable.getRepeatable() ? "true" : "false");
            completableNode.SetAttribute("type", type);

            if (completable.getStart() != null)
            {
                completableNode.AppendChild(CompletableDOMWriter.buildMilestoneDOM(completable.getStart(), "start-milestone"));
            }

            if (completable.getEnd() != null)
            {
                completableNode.AppendChild(CompletableDOMWriter.buildMilestoneDOM(completable.getEnd(), "end-milestone"));
            }

            if (completable.getProgress() != null)
                completableNode.AppendChild(CompletableDOMWriter.buildProgressDOM(completable.getProgress()));

            if (completable.getScore() != null)
                completableNode.AppendChild(CompletableDOMWriter.buildScoreDOM(completable.getScore()));
            
        }

        public static XmlElement buildMilestoneDOM(Completable.Milestone milestone, string elementName = "milestone")
        {
            XmlElement milestoneNode = Writer.GetDoc().CreateElement(elementName);

            milestoneNode.SetAttribute("type", milestone.getType().ToString());

            if (milestone.getType() != Completable.Milestone.MilestoneType.CONDITION)
            {
                milestoneNode.SetAttribute("targetId", milestone.getId());
            }
            else
            {
                DOMWriterUtility.DOMWrite(milestoneNode, milestone.getConditions());
            }

            if (milestone.getProgress() >= 0)
                milestoneNode.SetAttribute("progress", milestone.getProgress().ToString());

            return milestoneNode;
        }

        public static XmlElement buildProgressDOM(Completable.Progress progress)
        {
            XmlElement progressNode = Writer.GetDoc().CreateElement("progress");

            progressNode.SetAttribute("type", progress.getType().ToString());

            foreach (Completable.Milestone milestone in progress.getMilestones())
            {
                progressNode.AppendChild(CompletableDOMWriter.buildMilestoneDOM(milestone));
            }

            return progressNode;
        }

        public static XmlElement buildScoreDOM(Completable.Score score)
        {
            XmlElement progressNode = Writer.GetDoc().CreateElement("score");

            progressNode.SetAttribute("type", score.getType().ToString());
            progressNode.SetAttribute("method", score.getMethod().ToString());

            if (score.getMethod() == Completable.Score.ScoreMethod.SINGLE)
            {
                progressNode.SetAttribute("id", score.getId());
            }
            else
            {
                XmlElement subScores = Writer.GetDoc().CreateElement("sub-scores");

                foreach (Completable.Score s in score.getSubScores())
                {
                    subScores.AppendChild(CompletableDOMWriter.buildScoreDOM(s));
                }

                progressNode.AppendChild(subScores);
            }

            return progressNode;
        }
    }
}