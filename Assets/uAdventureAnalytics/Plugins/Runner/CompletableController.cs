using UnityEngine;
using System.Collections.Generic;
using System;

using uAdventure.Core;
using System.Linq;
using uAdventure.Runner;

namespace uAdventure.Analytics
{
    [Serializable]
    public class MilestoneController
    {
        [NonSerialized]
        private Completable.Milestone milestone;
        [NonSerialized]
        private AnalyticsExtension analyticsExtension;

        public Completable.Milestone GetMilestone()
        {
            return milestone;
        }

        internal void SetMilestone(Completable.Milestone value)
        {
            milestone = value;
        }

        [SerializeField]
        private bool reached;

        public bool Reached { get { return reached; } private set { reached = value; } }

        public MilestoneController(Completable.Milestone milestone)
        {
            analyticsExtension = GameExtension.GetInstance<AnalyticsExtension>();
            SetMilestone(milestone);
        }

        public bool Update(IChapterTarget target)
        {
            bool hasBeenUpdated = false;

            if (!Reached)
            {
                switch (GetMilestone().getType())
                {
                    case Completable.Milestone.MilestoneType.SCENE:
                        var isTargetedScene = GetMilestone().getId() == target.getId();

                        if (isTargetedScene)
                        {
                            Reached = true;
                            hasBeenUpdated = true;
                        }
                        break;
                    case Completable.Milestone.MilestoneType.ENDING:
                        if(target is Cutscene || target.getId() == "EndScene")
                        {
                            if (target.getId() == "EndScene" || ((Cutscene)target).isEndScene())
                            {
                                Reached = true;
                                hasBeenUpdated = true;
                            }
                        }
                        break;
                    default:
                        break;
                }
                
            }
            
            return hasBeenUpdated;
        }

        public bool Update(Element element, string interaction = null, string targetId = null)
        {
            bool hasBeenUpdated = false;

            if (!Reached)
            {
                bool isTargetType = false;

                if (element is NPC)
                {
                    isTargetType = GetMilestone().getType() == Completable.Milestone.MilestoneType.CHARACTER;
                }
                else if (element is Item)
                {
                    isTargetType = GetMilestone().getType() == Completable.Milestone.MilestoneType.ITEM;
                }

                if (element != null)
                {
                    var isTargetedElement = GetMilestone().getId() == element.getId();

                    if (isTargetType && isTargetedElement)
                    {
                        Reached = true;
                        hasBeenUpdated = true;
                    }
                }
            }

            return hasBeenUpdated;
        }

        public bool Update()
        {
            bool hasBeenUpdated = false;

            if (!Reached)
            {
                switch (GetMilestone().getType())
                {
                    case Completable.Milestone.MilestoneType.COMPLETABLE:
                        var targetCompletable = analyticsExtension.CompletablesController.GetCompletable(GetMilestone().getId());
                        Reached = targetCompletable.End.Reached;
                        break;
                    case Completable.Milestone.MilestoneType.CONDITION:
                        Reached = ConditionChecker.check(GetMilestone().getConditions());
                        break;
                }

                if (Reached)
                {
                    hasBeenUpdated = true;
                }
            }

            return hasBeenUpdated;
        }

        public void Reset()
        {
            Reached = false;
        }
    }

    [Serializable]
    public class CompletableController
    {
        [SerializeField]
        private MilestoneController startController, endController;
        [SerializeField]
        private List<MilestoneController> progressControllers;
        [SerializeField]
        private DateTime startTime;
        [SerializeField]
        private bool completeOnExit;
        [SerializeField]
        private bool completed;
        [SerializeField]
        private float lastProgressedValue = float.MinValue;

        [NonSerialized]
        private Completable completable;
        [NonSerialized]
        private AnalyticsExtension analyticsExtension;

        public Completable GetCompletable()
        {
            return completable;
        }

        internal void SetCompletable(Completable value)
        {
            completable = value;
        }

        public MilestoneController Start { get { return startController; } }
        public MilestoneController End { get { return endController; } }
        internal List<MilestoneController> ProgressControllers { get { return progressControllers; } }

        public bool Started { get { return startController.Reached; } }
        public bool Completed { get { return completed; } }

        public float Progress
        {
            get
            {
                float progress = 0f;
                if (Completed)
                {
                    progress = 1f;
                }else
                {
                    var progressType = GetCompletable().getProgress().getType();
                    switch (progressType)
                    {
                        case Completable.Progress.ProgressType.SUM:
                            progress = progressControllers.Average(m => m.Reached ? 1f : 0f);
                            break;
                        case Completable.Progress.ProgressType.SPECIFIC:
                            progress = progressControllers
                                .Where(milestone => milestone.Reached)
                                .Max(milestone => milestone.GetMilestone().getProgress());
                            break;
                    }
                }

                return progress;
            }
        }

        public float Score
        {
            get
            {
                var score = CalculateScore(GetCompletable().getScore());
                return score; //TODO: enable this? Mathf.Clamp01(score / 10f);
            }
        }

        public CompletableController(Completable completable)
        {
            analyticsExtension = GameExtension.GetInstance<AnalyticsExtension>();
            this.SetCompletable(completable);

            this.startController = new MilestoneController(completable.getStart());
            this.endController = new MilestoneController(completable.getEnd());

            this.progressControllers = completable
                .getProgress()
                .getMilestones()
                .ConvertAll(c => new MilestoneController(c));
        }

        public bool UpdateMilestones()
        {
            return UpdateMilestones(milestone => milestone.Update());
        }

        private bool UpdateMilestones(Func<MilestoneController, bool> updateFunction)
        {
            if (completed)
            {
                return false;
            }

            if (!Start.Reached)
            {
                var started = updateFunction(Start);
                if (started)
                {
                    startTime = DateTime.Now;
                    analyticsExtension.CompletablesController.TrackStarted(this);
                }
            }

            completed = false;

            if (Start.Reached)
            {
                bool progressed = false;

                foreach (var milestoneController in progressControllers)
                {
                    progressed |= updateFunction(milestoneController);
                }

                if (progressed)
                {
                    lastProgressedValue = this.Progress;
                    analyticsExtension.CompletablesController.TrackProgressed(this);
                }

                if (End != null)
                {
                    completed = updateFunction(End);
                    if (completed)
                    {
                        if (lastProgressedValue != this.Progress)
                        {
                            analyticsExtension.CompletablesController.TrackProgressed(this);
                        }
                        analyticsExtension.CompletablesController.TrackCompleted(this, DateTime.Now - startTime);
                    }
                }
            }

            return completed;
        }

        public bool UpdateMilestones(IChapterTarget target)
        {
            if (completed)
            {
                return false;
            }

            if (completeOnExit && target.getId() != Start.GetMilestone().getId())
            {
                completed = true;
                analyticsExtension.CompletablesController.TrackCompleted(this, DateTime.Now - startTime);
            }
            else
            {
                var wasStarted = Started;
                completed = UpdateMilestones(milestone => milestone.Update(target));
                if (wasStarted != Started && GetCompletable().getEnd() == null)
                {
                    completeOnExit = true;
                }
            }

            return completed;
        }

        public bool UpdateMilestones(Element element, string interaction = null, string targetId = null)
        {
            return UpdateMilestones(milestone => milestone.Update(element, interaction, targetId));
        }

        private float CalculateScore(Completable.Score completableScore)
        {
            float score = 0;

            switch (completableScore.getMethod())
            {
                // Base case (calculated based on target)
                case Completable.Score.ScoreMethod.SINGLE:
                    var targetId = completableScore.getId();
                    switch (completableScore.getType())
                    {
                        case Completable.Score.ScoreType.VARIABLE:
                            // In case of variable type, the target id points to a variable
                            var variableValue = Game.Instance.GameState.GetVariable(targetId);
                            score = variableValue;
                            break;
                        case Completable.Score.ScoreType.COMPLETABLE:
                            // In case of completable type, the target id points to a completable
                            var referencedCompletable = analyticsExtension.CompletablesController.GetCompletable(targetId);
                            score = referencedCompletable.Score;
                            break;
                    }
                    break;

                // Recursive cases (calculated based on subscores)
                case Completable.Score.ScoreMethod.AVERAGE:
                    score = completableScore.getSubScores().Count > 0 ? completableScore.getSubScores().Average(s => CalculateScore(s)) : 1;
                    break;
                case Completable.Score.ScoreMethod.SUM:
                    score = completableScore.getSubScores().Sum(s => CalculateScore(s));
                    break;
            }

            return score;
        }

        public void Reset()
        {
            if (GetCompletable().getRepeatable()) {
                if (Start != null)
                {
                    Start.Reset();
                }

                if (End != null)
                {
                    End.Reset();
                }

                completed = false;

                foreach (var milestoneController in progressControllers)
                {
                    milestoneController.Reset();
                }
            }
        }
    }

}