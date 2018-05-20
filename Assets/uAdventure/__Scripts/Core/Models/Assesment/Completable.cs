using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// TODO possible unnecesary coupling
using uAdventure.Runner;

namespace uAdventure.Core
{
    /**
     * Suits Completable in xApi model
     */

    public class Completable : HasId, ICloneable
    {
        public class Milestone : HasId
        {
            public enum MilestoneType { SCENE, ITEM, CHARACTER, COMPLETABLE, CONDITION };

            MilestoneType type;
            string id = "";
            Conditions conditions = new Conditions();
            float progress = -1;
            bool reached = false;

            public MilestoneType getType()
            {
                return this.type;
            }

            public string getId()
            {
                return id;
            }

            public Conditions getConditions()
            {
                return conditions;
            }

            public float getProgress()
            {
                return progress;
            }

            public bool getReached()
            {
                return reached;
            }

            public void setType(MilestoneType type)
            {
                this.type = type;
            }

            public void setId(string id)
            {
                this.id = id;
            }

            public void setConditions(Conditions conditions)
            {
                this.conditions = conditions;
            }

            public void setProgress(float progress)
            {
                this.progress = progress;
            }

            public void setReached(bool reached)
            {
                this.reached = reached;
            }

            public bool Update(IChapterTarget target)
            {
                if (!reached && type == Completable.Milestone.MilestoneType.SCENE && id == target.getId())
                    reached = true;

                return reached;
            }

            public bool Update(Interactuable interactuable)
            {
                Milestone.MilestoneType type;

                switch (interactuable.GetType().ToString())
                {
                    case "CharacterMB": type = Milestone.MilestoneType.CHARACTER; break;
                    case "ObjectMB": type = Milestone.MilestoneType.ITEM; break;
                    default: return false;
                }

                if (!reached && type == this.type && id == ((Representable)interactuable).Element.getId())
                    reached = true;

                return reached;
            }

            public bool Update()
            {
                if (!reached)
                    if (type == MilestoneType.CONDITION)
                        reached = ConditionChecker.check(conditions);
                    else if (type == MilestoneType.COMPLETABLE)
                        reached = CompletableController.Instance.getCompletable(id).getEnd().reached;

                return reached;
            }

            override public string ToString()
            {
                if (string.IsNullOrEmpty(id) && conditions == null)
                    return "Not specified yet";

                string s = "When ";

                switch (type)
                {
                    case MilestoneType.CHARACTER:
                        s += "player interacts with character " + id;
                        break;
                    case MilestoneType.ITEM:
                        s += "player interacts with item " + id;
                        break;
                    case MilestoneType.SCENE:
                        s += "player reaches scene " + id;
                        break;
                    case MilestoneType.COMPLETABLE:
                        s += "player completes " + id;
                        break;
                    case MilestoneType.CONDITION:
                        s += "a condition is satisfied";
                        break;
                    default:
                        s = "Not specified yet";
                        break;
                }

                return s;
            }
        }

        public class Progress
        {
            public enum ProgressType { SUM, SPECIFIC }

            private ProgressType type;
            private List<Milestone> milestones = new List<Milestone>();

            public ProgressType getType()
            {
                return type;
            }

            public List<Milestone> getMilestones()
            {
                return milestones;
            }

            public void setType(ProgressType type)
            {
                this.type = type;
            }

            public void setMilestones(List<Milestone> milestones)
            {
                this.milestones = milestones;
            }

            public void addMilestone(Milestone milestone)
            {
                this.milestones.Add(milestone);
            }

            public bool updateMilestones(IChapterTarget target)
            {
                bool reached = false;

                foreach (Milestone m in milestones)
                    reached = m.Update(target) || reached;

                return reached;
            }

            public bool updateMilestones(Interactuable interactuable)
            {
                bool reached = false;



                return reached;
            }

            public bool updateMilestones()
            {
                bool reached = false;

                foreach (Milestone m in milestones)
                    reached = m.Update() || reached;

                return reached;
            }

            public float getProgress()
            {
                float p = 0f;

                if (type == ProgressType.SUM)
                {
                    int r = 0;
                    foreach (Milestone m in milestones)
                    {
                        if (m.getReached()) r++;
                    }
                    p = r / milestones.Count;
                }
                else if (type == ProgressType.SPECIFIC)
                {
                    foreach (Milestone m in milestones)
                        if (m.getReached() && m.getProgress() > p)
                            p = m.getProgress();
                }

                return p;
            }

            public void Reset()
            {
                foreach (Milestone milestone in milestones)
                {
                    milestone.setReached(false);
                }
            }
        }

        public class Score
        {
            public enum ScoreType { VARIABLE, COMPLETABLE };
            public enum ScoreMethod { AVERAGE, SUM, SINGLE };

            ScoreType type;
            ScoreMethod method;

            string id;
            List<Score> scores = new List<Score>();

            public ScoreType getType()
            {
                return this.type;
            }

            public ScoreMethod getMethod()
            {
                return this.method;
            }

            public string getId()
            {
                return this.id;
            }

            public List<Score> getSubScores()
            {
                return this.scores;
            }

            public void setType(ScoreType type)
            {
                this.type = type;
            }

            public void setMethod(ScoreMethod method)
            {
                this.method = method;
            }

            public void setId(string id)
            {
                this.id = id;
            }

            public void setSubScores(List<Score> scores)
            {
                this.scores = scores;
            }

            public void addSubScore(Score score)
            {
                if (this.scores == null)
                    this.scores = new List<Score>();

                this.scores.Add(score);
            }

            public float getScore()
            {
                float score = 0;

                switch (method)
                {
                    case ScoreMethod.SINGLE:
                        switch (type)
                        {
                            case ScoreType.VARIABLE: score = Game.Instance.GameState.getVariable(id); break;
                            case ScoreType.COMPLETABLE: score = CompletableController.Instance.getCompletable(id).getScore().getScore(); break;
                        }
                        break;
                    case ScoreMethod.AVERAGE:
                        score = sumScores() / scores.Count;
                        break;
                    case ScoreMethod.SUM:
                        score = sumScores();
                        break;
                }

                return score;
            }

            private float sumScores()
            {
                float sum = 0;

                foreach (Score score in scores)
                {
                    sum += score.getScore();
                }

                return sum;
            }
        }

        public const int TYPE_GAME = 1;
        public const int TYPE_SESSION = 2;
        public const int TYPE_LEVEL = 3;
        public const int TYPE_QUEST = 4;
        public const int TYPE_STAGE = 5;
        public const int TYPE_COMBAT = 6;
        public const int TYPE_STORYNODE = 7;
        public const int TYPE_RACE = 8;
        public const int TYPE_COMPLETABLE = 9;

        /**
         * identificator for the Completable
         */
        private string id = "";

        /**
         * xApi Class type
         */
        protected int type = TYPE_COMPLETABLE;

        private Score score;

        private Milestone start = new Milestone();

        private Milestone end = new Milestone();

        private Progress progress = new Progress();

        //#################################################
        //#################### GETTERS ####################
        //#################################################
        #region getters
        public string getId()
        {
            return this.id;
        }

        public Progress getProgress()
        {
            return this.progress;
        }

        public float currentProgress()
        {
            return this.progress.getProgress();
        }

        public int getType()
        {
            return this.type;
        }

        public Score getScore()
        {
            return score;
        }

        public Milestone getStart()
        {
            return start;
        }

        public Milestone getEnd()
        {
            return end;
        }
        #endregion getters
        //#################################################
        //#################### SETTERS ####################
        //#################################################
        #region setters
        public void setId(string id)
        {
            this.id = id;
        }

        public void setType(int type)
        {
            this.type = type;
        }

        public void setScore(Score score)
        {
            this.score = score;
        }

        public void setProgress(Progress progress)
        {
            this.progress = progress;
        }

        public void setStart(Milestone start)
        {
            this.start = start;
        }

        public void setEnd(Milestone end)
        {
            this.end = end;
        }
        #endregion setters
        //#################################################
        //#################################################
        //#################################################

        public void Reset()
        {
            this.progress.Reset();

            if (this.start != null)
                this.start.setReached(false);

            if (this.end != null)
                this.end.setReached(false);
        }

        public object Clone()
        {
            Completable nc = new Completable();
            return nc;
        }
    }
}