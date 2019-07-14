using System;
using System.Collections.Generic;
using uAdventure.Core;

namespace uAdventure.Analytics
{
    /**
     * Suits Completable in xApi model
     */

    public class Completable : HasId, ICloneable
    {
        public class Milestone : HasId, HasTargetId
        {
            public enum MilestoneType { SCENE, ITEM, CHARACTER, COMPLETABLE, CONDITION, ENDING };

            private MilestoneType type;
            private string id = "";
            private string targetId = "";
            private Conditions conditions = new Conditions();
            private float progress = -1;

            public MilestoneType getType()
            {
                return this.type;
            }

            public string getId()
            {
                return id;
            }

            public void setType(MilestoneType type)
            {
                this.type = type;
            }

            public string getTargetId()
            {
                return targetId;
            }

            public void setTargetId(string id)
            {
                this.targetId = id;
            }

            public Conditions getConditions()
            {
                return conditions;
            }

            public void setId(string id)
            {
                this.id = id;
            }

            public float getProgress()
            {
                return progress;
            }

            public void setProgress(float progress)
            {
                this.progress = progress;
            }

            public void setConditions(Conditions conditions)
            {
                this.conditions = conditions;
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
                    case MilestoneType.ENDING:
                        //ENDING is special case, not showing
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

        private bool repeatable = false;

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

        public bool getRepeatable()
        {
            return repeatable;
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

        public void setRepeatable(bool repeatable)
        {
            this.repeatable = repeatable;
        }
        #endregion setters
        //#################################################
        //#################################################
        //#################################################

        public object Clone()
        {
            Completable nc = new Completable();
            return nc;
        }
    }
}