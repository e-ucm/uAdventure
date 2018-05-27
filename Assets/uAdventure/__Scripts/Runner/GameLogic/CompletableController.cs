using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using uAdventure.Core;
using RAGE.Analytics;
using AssetPackage;

namespace uAdventure.Runner
{
    public class CompletableController
    {

        //#################################################################
        //########################### SINGLETON ###########################
        //#################################################################
        #region Singleton
        static CompletableController instance;

        public static CompletableController Instance
        {
            get
            {
                if (instance == null)
                    instance = new CompletableController();

                return instance;
            }
        }
        #endregion Singleton

        //##################################################################
        //########################### CONTROLLER ###########################
        //##################################################################
        #region Controller

        private List<Completable> completables = new List<Completable>();
        private List<Completable> trackingCompletables = new List<Completable>();
        private Dictionary<Completable, DateTime> times = new Dictionary<Completable, DateTime>();
        private Stack<Completable> toRemove = new Stack<Completable>();
        Completable completeOnExit;


        public List<Completable> getCompletables()
        {
            return completables;
        }

        public Completable getCompletable(string id)
        {
            foreach (Completable c in completables)
            {
                if (c.getId() == id)
                    return c;
            }

            return null;
        }

        public void setCompletables(List<Completable> completables)
        {
            this.completables = completables;
        }

        public void targetChanged(IChapterTarget target)
        {
            //Complete if any scene is completed on exit.
            if (completeOnExit != null)
            {
                TrackerAsset.Instance.Completable.Completed(completeOnExit.getId(), CompletableTracker.Completable.Stage, true, completeOnExit.getScore().getScore());
                completeOnExit = null;
            }

            //Buscamos en nuestra lista de completables si algun completable se completa o progresa al llegar aquí
            foreach (Completable toComplete in completables)
            {
                if (toComplete.getProgress().updateMilestones(target))
                    TrackerAsset.Instance.Completable.Progressed(toComplete.getId(), (CompletableTracker.Completable)toComplete.getType(), toComplete.currentProgress());

                if (toComplete.getEnd().Update(target))
                {
                    trackCompleted(toComplete);
                    toRemove.Push(toComplete);
                }
            }

            clearToRemove();

            //Buscamos en nuestros completables si alguno se inicia con esta escena
            foreach (Completable completable in completables)
            {
                // TODO:
                // prevent levels overlaping.

                if (!trackingCompletables.Contains(completable) && completable.getStart().Update(target))
                {
                    trackingCompletables.Add(completable);
                    times.Add(completable, DateTime.Now);
                    TrackerAsset.Instance.Completable.Initialized(completable.getId(), (CompletableTracker.Completable)completable.getType());
                    TrackerAsset.Instance.Completable.Progressed(completable.getId(), (CompletableTracker.Completable)completable.getType(), 0);

                    if (completable.getEnd() == null)
                    {
                        completeOnExit = completable;
                    }
                }
            }
        }

        public void elementInteracted(IRunnerChapterTarget interactuable)
        {
            // TODO
            // implement completable changes on interaction
        }

        public void conditionChanged()
        {
            foreach (Completable c in trackingCompletables)
            {
                if (c.getProgress().updateMilestones())
                {
                    TrackerAsset.Instance.Completable.Progressed(c.getId(), (CompletableTracker.Completable)c.getType(), c.currentProgress());
                }
                if (c.getEnd().getType() == Completable.Milestone.MilestoneType.CONDITION && c.getEnd().Update())
                {
                    trackCompleted(c);
                    toRemove.Push(c);
                }
            }

            clearToRemove();

            //Any completable starts when this condition changes?
            foreach (Completable completable in completables)
            {
                if (!trackingCompletables.Contains(completable) && completable.getStart().Update())
                {
                    trackingCompletables.Add(completable);
                    times.Add(completable, DateTime.Now);
                    TrackerAsset.Instance.Completable.Initialized(completable.getId(), (CompletableTracker.Completable)completable.getType());
                    TrackerAsset.Instance.Completable.Progressed(completable.getId(), (CompletableTracker.Completable)completable.getType(), 0);

                    if (completable.getEnd() == null)
                    {
                        completeOnExit = completable;
                    }
                }
            }
        }

        public void trackCompleted(Completable c)
        {
            float score = Mathf.Max(Mathf.Min(c.getScore().getScore() / 10f, 1f), 0f);
            
            TrackerAsset.Instance.setVar("time", (DateTime.Now - times[c]).TotalSeconds);
            TrackerAsset.Instance.Completable.Completed(c.getId(), (CompletableTracker.Completable)c.getType(), true, score);
        }

        public void clearToRemove()
        {
            Completable tmp;
            while (toRemove.Count > 0)
            {
                tmp = toRemove.Pop();
                times.Remove(tmp);
                trackingCompletables.Remove(tmp);
                tmp.Reset();
            }
        }

        #endregion Controller
    }
}