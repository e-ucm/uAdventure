using UnityEngine;
using System.Collections;

namespace RAGE.Analytics.Formats
{
    public class AlternativeTracker : Tracker.IGameObjectTracker
    {

        private Tracker tracker;

        public void setTracker(Tracker tracker)
        {
            this.tracker = tracker;
        }

        /* ALTERNATIVES */

        public enum Alternative
        {
            Question,
            Menu,
            Dialog,
            Path,
            Arena,
            Alternative
        }

        /// <summary>
        /// Player selected an option in a presented alternative
        /// Type = Alternative
        /// </summary>
        /// <param name="alternativeId">Alternative identifier.</param>
        /// <param name="optionId">Option identifier.</param>
        public void Selected(string alternativeId, string optionId)
        {
            tracker.setResponse(optionId);
            tracker.ActionTrace(Tracker.Verb.Selected.ToString().ToLower(), Alternative.Alternative.ToString().ToLower(), alternativeId);
        }

        /// <summary>
        /// Player selected an option in a presented alternative
        /// Type = Alternative
        /// </summary>
        /// <param name="alternativeId">Alternative identifier.</param>
        /// <param name="optionId">Option identifier.</param>
        public void Selected(string alternativeId, string optionId, bool correct)
        {
            tracker.setResponse(optionId);
            tracker.setSuccess(correct);
            tracker.ActionTrace(Tracker.Verb.Selected.ToString().ToLower(), Alternative.Alternative.ToString().ToLower(), alternativeId);
        }

        /// <summary>
        /// Player selected an option in a presented alternative
        /// </summary>
        /// <param name="alternativeId">Alternative identifier.</param>
        /// <param name="optionId">Option identifier.</param>
        /// <param name="type">Alternative type.</param>
        public void Selected(string alternativeId, string optionId, Alternative type)
        {
            tracker.setResponse(optionId);
            tracker.ActionTrace(Tracker.Verb.Selected.ToString().ToLower(), type.ToString().ToLower(), alternativeId);
        }

        /// <summary>
        /// Player selected an option in a presented alternative
        /// </summary>
        /// <param name="alternativeId">Alternative identifier.</param>
        /// <param name="optionId">Option identifier.</param>
        /// <param name="type">Alternative type.</param>
        public void Selected(string alternativeId, string optionId, bool correct, Alternative type)
        {
            tracker.setResponse(optionId);
            tracker.setSuccess(correct);
            tracker.ActionTrace(Tracker.Verb.Selected.ToString().ToLower(), type.ToString().ToLower(), alternativeId);
        }

        /// <summary>
        /// Player unlocked an option
        /// Type = Alternative
        /// </summary>
        /// <param name="alternativeId">Alternative identifier.</param>
        /// <param name="optionId">Option identifier.</param>
        public void Unlocked(string alternativeId, string optionId)
        {
            tracker.setResponse(optionId);
            tracker.ActionTrace(Tracker.Verb.Unlocked.ToString().ToLower(), Alternative.Alternative.ToString().ToLower(), alternativeId);
        }

        /// <summary>
        /// Player unlocked an option
        /// </summary>
        /// <param name="alternativeId">Alternative identifier.</param>
        /// <param name="optionId">Option identifier.</param>
        /// <param name="type">Alternative type.</param>
        public void Unlocked(string alternativeId, string optionId, Alternative type)
        {
            tracker.setResponse(optionId);
            tracker.ActionTrace(Tracker.Verb.Unlocked.ToString().ToLower(), type.ToString().ToLower(), alternativeId);
        }
    }
}