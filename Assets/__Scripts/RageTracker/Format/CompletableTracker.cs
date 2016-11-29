using UnityEngine;
using System.Collections;

namespace uAdventure.RageTracker
{
    public class CompletableTracker : Tracker.IGameObjectTracker
    {

        private Tracker tracker;

        public void setTracker(Tracker tracker)
        {
            this.tracker = tracker;
        }

        /* COMPLETABLES */

        public enum Completable
        {
            Game,
            Session,
            Level,
            Quest,
            Stage,
            Combat,
            StoryNode,
            Race,
            Completable
        }

        /// <summary>
        /// Player initialized a completable.
        /// </summary>
        /// <param name="completableId">Completable identifier.</param>
        public void Initialized(string completableId)
        {
            tracker.Trace(Tracker.Verb.Initialized.ToString().ToLower(), Completable.Completable.ToString().ToLower(), completableId);
        }

        /// <summary>
        /// Player initialized a completable.
        /// </summary>
        /// <param name="completableId">Completable identifier.</param>
        /// <param name="type">Completable type.</param>
        public void Initialized(string completableId, Completable type)
        {
            tracker.Trace(Tracker.Verb.Initialized.ToString().ToLower(), type.ToString().ToLower(), completableId);
        }

        /// <summary>
        /// Player progressed a completable.
        /// Type = Completable
        /// </summary>
        /// <param name="completableId">Completable identifier.</param>
        /// <param name="value">New value for the completable's progress.</param>
        public void Progressed(string completableId, float value)
        {
            tracker.setProgress(value);
            tracker.Trace(Tracker.Verb.Progressed.ToString().ToLower(), Completable.Completable.ToString().ToLower(), completableId);
        }

        /// <summary>
        /// Player progressed a completable.
        /// </summary>
        /// <param name="completableId">Completable identifier.</param>
        /// <param name="value">New value for the completable's progress.</param>
        /// <param name="type">Completable type.</param>
        public void Progressed(string completableId, Completable type, float value)
        {
            tracker.setProgress(value);
            tracker.Trace(Tracker.Verb.Progressed.ToString().ToLower(), type.ToString().ToLower(), completableId);

            Debug.Log(completableId + " - " + value);
        }

        /// <summary>
        /// Player completed a completable.
        /// Type = Completable
        /// Success = true
        /// Score = 1
        /// </summary>
        /// <param name="completableId">Completable identifier.</param>
        public void Completed(string completableId)
        {
            tracker.setSuccess(true);
            tracker.setScore(1f);
            tracker.Trace(Tracker.Verb.Completed.ToString().ToLower(), Completable.Completable.ToString().ToLower(), completableId);
        }

        /// <summary>
        /// Player completed a completable.
        /// Success = true
        /// Score = 1
        /// </summary>
        /// <param name="completableId">Completable identifier.</param>
        /// <param name="type">Completable type.</param>
        public void Completed(string completableId, Completable type)
        {
            tracker.setSuccess(true);
            tracker.setScore(1f);
            tracker.Trace(Tracker.Verb.Completed.ToString().ToLower(), type.ToString().ToLower(), completableId);
        }

        /// <summary>
        /// Player completed a completable.
        /// Score = 1
        /// </summary>
        /// <param name="completableId">Completable identifier.</param>
        /// <param name="type">Completable type.</param>
        /// <param name="success">Completable success.</param>
        public void Completed(string completableId, Completable type, bool success)
        {
            tracker.setSuccess(success);
            tracker.setScore(1f);
            tracker.Trace(Tracker.Verb.Completed.ToString().ToLower(), type.ToString().ToLower(), completableId);
        }

        /// <summary>
        /// Player completed a completable.
        /// </summary>
        /// <param name="completableId">Completable identifier.</param>
        /// <param name="type">Completable type.</param>
        /// <param name="success">Completable success.</param>
        /// <param name="score">Completable score.</param>
        public void Completed(string completableId, Completable type, bool success, float score)
        {
            tracker.setSuccess(success);
            tracker.setScore(score);
            tracker.Trace(Tracker.Verb.Completed.ToString().ToLower(), type.ToString().ToLower(), completableId);
        }

    }
}