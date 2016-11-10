using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CompletableController {

    //#################################################################
    //########################### SINGLETON ###########################
    //#################################################################
    #region Singleton
    static CompletableController instance;

    public static CompletableController Instance
    {
        get {
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
        foreach(Completable c in completables)
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

    public void sceneChanged(GeneralScene scene)
    {
        //Complete if any scene is completed on exit.
        if (completeOnExit != null)
        {
            Tracker.T.completable.Completed(completeOnExit.getId(), CompletableTracker.Completable.Stage, true, completeOnExit.getScore().getScore());
            completeOnExit = null;
        }

        //Buscamos en nuestra lista de completables si algun completable se completa o progresa al llegar aquí
        foreach (Completable toComplete in completables)
        {
            if (toComplete.getProgress().updateMilestones(scene))
                Tracker.T.completable.Progressed(toComplete.getId(), (CompletableTracker.Completable)toComplete.getType(), toComplete.currentProgress());

            if (toComplete.getEnd().Update(scene))
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

            if(completable.getStart().Update(scene))
            {
                trackingCompletables.Add(completable);
                times.Add(completable, DateTime.Now);
                Tracker.T.completable.Initialized(completable.getId(), (CompletableTracker.Completable) completable.getType());
                Tracker.T.completable.Progressed(completable.getId(), (CompletableTracker.Completable)completable.getType(), 0);

                if(completable.getEnd() == null)
                {
                    completeOnExit = completable;
                }
            }
        }
    }

    public void elementInteracted(Interactuable interactuable)
    {
        // TODO
        // implement completable changes on interaction
    }

    public void conditionChanged()
    {
        foreach (Completable c in trackingCompletables)
        {
            if (c.getProgress().updateMilestones()) { 
                Tracker.T.completable.Progressed(c.getId(), (CompletableTracker.Completable) c.getType(), c.currentProgress());
            }

            if (c.getEnd().getType() == Completable.Milestone.MilestoneType.CONDITION && c.getEnd().getReached())
            {
                trackCompleted(c);
                toRemove.Push(c);
            }
        }

        clearToRemove();

        //Any completable starts when this condition changes?
        foreach (Completable completable in completables)
        {
            if (completable.getStart().Update())
            {
                trackingCompletables.Add(completable);
                times.Add(completable, DateTime.Now);
                Tracker.T.completable.Initialized(completable.getId(), (CompletableTracker.Completable)completable.getType());
                Tracker.T.completable.Progressed(completable.getId(), (CompletableTracker.Completable)completable.getType(), 0);

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

        Tracker.T.completable.Completed(c.getId(), (CompletableTracker.Completable)c.getType(), true, score);
        Tracker.T.setExtension("time", (DateTime.Now - times[c]).TotalSeconds);
        Tracker.T.completable.Completed(c.getId(), (CompletableTracker.Completable)c.getType(), true, score);
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
