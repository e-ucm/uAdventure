using UnityEngine;
using System.Collections.Generic;

using uAdventure.Core;
using System.Linq;
using uAdventure.Runner;
using System.Collections;
using AssetPackage;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace uAdventure.Analytics
{
    [System.Serializable]
    public class CompletablesController : IList<CompletableController>
    {
        [SerializeField]
        private List<CompletableController> controllers = new List<CompletableController>();
        [System.NonSerialized]
        private List<Completable> completables;

        public CompletablesController()
        {
            InitCompletables();

            Game.Instance.GameState.OnConditionChanged += (_, __) => ConditionChanged();
            Game.Instance.OnTargetChanged += TargetChanged;
            Game.Instance.OnElementInteracted += ElementInteracted;
        }

        #region Completables

        // #########################################
        // ############### COMPLETABLES ############
        // #########################################



        private void InitCompletables()
        {
            //Create Main game completabl
            Completable mainGame = new Completable();

            Completable.Milestone gameStart = new Completable.Milestone();
            gameStart.setType(Completable.Milestone.MilestoneType.SCENE);
            gameStart.setId(Game.Instance.GameState.InitialChapterTarget.getId());
            mainGame.setStart(gameStart);
            mainGame.setId(Game.Instance.GameState.Data.getTitle());
            mainGame.setType(Completable.TYPE_GAME);

            Completable.Milestone gameEnd = new Completable.Milestone();
            gameEnd.setType(Completable.Milestone.MilestoneType.ENDING);
            mainGame.setEnd(gameEnd);

            Completable.Progress gameProgress = new Completable.Progress();
            gameProgress.setType(Completable.Progress.ProgressType.SUM);

            Completable.Score mainScore = new Completable.Score();
            mainScore.setMethod(Completable.Score.ScoreMethod.AVERAGE);

            completables = new List<Completable>(Game.Instance.GameState.GetObjects<Completable>());

            foreach (Completable part in completables)
            {
                Completable.Milestone tmpMilestone = new Completable.Milestone();
                tmpMilestone.setType(Completable.Milestone.MilestoneType.COMPLETABLE);
                tmpMilestone.setId(part.getId());
                gameProgress.addMilestone(tmpMilestone);

                Completable.Score tmpScore = new Completable.Score();
                tmpScore.setMethod(Completable.Score.ScoreMethod.SINGLE);
                tmpScore.setType(Completable.Score.ScoreType.COMPLETABLE);
                tmpScore.setId(part.getId());
                mainScore.addSubScore(tmpScore);
            }
            mainGame.setProgress(gameProgress);
            mainGame.setScore(mainScore);

            completables.Insert(0, mainGame);

            SetCompletables(completables);
        }

        public void RestoreCompletables(Memory analyticsMemory)
        {
            try
            {
                var serializedCompletables = analyticsMemory.Get<string>("completables");
                if (!string.IsNullOrEmpty(serializedCompletables))
                {
                    CompletablesController restoredCompletables;
                    if (serializedCompletables[0] == '{')
                    {
                        restoredCompletables = (CompletablesController)JsonUtility.FromJson(serializedCompletables, typeof(CompletablesController));
                    }
                    else
                    {
                        restoredCompletables = new CompletablesController();
                        restoredCompletables.AddRange(DeserializeFromString<List<CompletableController>>(serializedCompletables));
                    }

                    if (!VerifyControllers(restoredCompletables))
                    {
                        throw new System.Exception("The saved completable controllers didn't match the current completables. The save is ignored.");
                    }

                    this.Clear();
                    this.AddRange(restoredCompletables);

                    for (int i = 0; i < this.Count; i++)
                    {
                        this[i].SetCompletable(this.completables[i]);
                        this[i].Start.SetMilestone(this.completables[i].getStart());
                        this[i].End.SetMilestone(this.completables[i].getEnd());
                        for (int j = 0; j < this.completables[i].getProgress().getMilestones().Count; j++)
                        {
                            this[i].ProgressControllers[j].SetMilestone(this.completables[i].getProgress().getMilestones()[j]);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error parsing the completables: " + ex.Message + " " + ex.StackTrace);
            }
        }

        private bool VerifyControllers(CompletablesController restoredCompletables)
        {
            return restoredCompletables.Count == completables.Count &&
                    restoredCompletables
                        .Select((c, i) => new { c, i })
                        .All(r => r.c.ProgressControllers.Count == completables[r.i].getProgress().getMilestones().Count);
        }

        public CompletableController GetCompletable(string id)
        {
            return this.FirstOrDefault(c => c.GetCompletable().getId().Equals(id));
        }

        public void SetCompletables(List<Completable> value)
        {
            this.Clear();
            this.AddRange(value.ConvertAll(c => new CompletableController(c)));
        }

        private void UpdateCompletables(System.Func<CompletableController, bool> updatefunction)
        {

            bool somethingCompleted;
            do
            {
                somethingCompleted = false;
                foreach (var completableController in this)
                {
                    somethingCompleted |= updatefunction(completableController);

                }

                if (somethingCompleted)
                {
                    CompletableCompleted();
                }
            }
            while (somethingCompleted);

            ResetFinishedCompletables();
        }

        public void ConditionChanged()
        {
            UpdateCompletables(completableController => completableController.UpdateMilestones());
        }

        public void TargetChanged(IChapterTarget target)
        {
            if (TrackerAsset.Instance.Started && !string.IsNullOrEmpty(target.getXApiClass()) && target.getXApiClass() == "accesible")
            {
                TrackerAsset.Instance.Accessible.Accessed(target.getId(), ExParsers.ParseEnum<AccessibleTracker.Accessible>(target.getXApiType()));
                TrackerAsset.Instance.Flush();
            }

            UpdateCompletables(completableController => completableController.UpdateMilestones(target));
        }

        public void ElementInteracted(bool finished, Element element, Action action)
        {
            if (element == null || !TrackerAsset.Instance.Started)
            {
                return;
            }

            if (!finished)
            {
                UpdateElementsInteracted(element, action.getType().ToString(), element.getId());

                Game.Instance.GameState.BeginChangeAmbit();
            }
            else
            {
                string actionType = string.Empty;
                switch (action.getType())
                {
                    case Action.CUSTOM: actionType = (action as CustomAction).getName(); break;
                    case Action.CUSTOM_INTERACT: actionType = (action as CustomAction).getName(); break;
                    case Action.DRAG_TO: actionType = "drag_to"; break;
                    case Action.EXAMINE: actionType = "examine"; break;
                    case Action.GIVE_TO: actionType = "give_to"; break;
                    case Action.GRAB: actionType = "grab"; break;
                    case Action.TALK_TO: actionType = "talk_to"; break;
                    case Action.USE: actionType = "use"; break;
                    case Action.USE_WITH: actionType = "use_with"; break;
                }

                if (!string.IsNullOrEmpty(action.getTargetId()))
                {
                    TrackerAsset.Instance.setVar("action_target", action.getTargetId());
                }
                if (!string.IsNullOrEmpty(actionType))
                {
                    TrackerAsset.Instance.setVar("action_type", actionType);
                }

                Game.Instance.GameState.EndChangeAmbitAsExtensions();

                if (element is NPC)
                {
                    TrackerAsset.Instance.GameObject.Interacted(element.getId(), GameObjectTracker.TrackedGameObject.Npc);
                }
                else if (element is Item)
                {
                    TrackerAsset.Instance.GameObject.Interacted(element.getId(), GameObjectTracker.TrackedGameObject.Item);
                }
                else if (element is ActiveArea)
                {
                    TrackerAsset.Instance.GameObject.Interacted(element.getId(), GameObjectTracker.TrackedGameObject.Item);
                }
                else
                {
                    TrackerAsset.Instance.GameObject.Interacted(element.getId(), GameObjectTracker.TrackedGameObject.GameObject);
                }
                TrackerAsset.Instance.Flush();
            }
        }

        public void UpdateElementsInteracted(Element element, string interaction, string targetId)
        {
            UpdateCompletables(completableController => completableController.UpdateMilestones(element, interaction, targetId));
        }

        public void CompletableCompleted()
        {
            UpdateCompletables(completableController => completableController.UpdateMilestones());
        }

        public void TrackStarted(CompletableController completableController)
        {
            if (!TrackerAsset.Instance.Started)
            {
                return;
            }

            var completableId = completableController.GetCompletable().getId();
            var completableType = (CompletableTracker.Completable)completableController.GetCompletable().getType() - 1;

            TrackerAsset.Instance.Completable.Initialized(completableId, completableType);
            TrackerAsset.Instance.Completable.Progressed(completableId, completableType, 0);
        }

        public void TrackProgressed(CompletableController completableController)
        {
            if (!TrackerAsset.Instance.Started)
            {
                return;
            }

            var completableId = completableController.GetCompletable().getId();
            var completableType = (CompletableTracker.Completable)completableController.GetCompletable().getType() - 1;
            var completableProgress = completableController.Progress;

            TrackerAsset.Instance.Completable.Progressed(completableId, completableType, completableProgress);
        }


        public void TrackCompleted(CompletableController completableController, System.TimeSpan timeElapsed)
        {
            if (!TrackerAsset.Instance.Started)
            {
                return;
            }

            var completableId = completableController.GetCompletable().getId();
            var completableType = (CompletableTracker.Completable)completableController.GetCompletable().getType() - 1;
            var completableScore = completableController.Score;

            TrackerAsset.Instance.setVar("time", timeElapsed.TotalSeconds);
            TrackerAsset.Instance.Completable.Completed(completableId, completableType, true, completableScore);
        }



        public void ResetFinishedCompletables()
        {
            foreach (var completableController in this)
            {
                if (completableController.Completed)
                {
                    completableController.Reset();
                }
            }
        }

        #endregion Controller
        // ################################

        #region List

        public CompletableController this[int index]
        {
            get
            {
                return controllers[index];
            }
            set
            {
                controllers[index] = value;
            }
        }

        public int Count { get { return controllers.Count; } }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(CompletableController item)
        {
            controllers.Add(item);
        }

        public void Clear()
        {
            controllers.Clear();
        }

        public bool Contains(CompletableController item)
        {
            return controllers.Contains(item);
        }

        public void CopyTo(CompletableController[] array, int arrayIndex)
        {
            controllers.CopyTo(array, arrayIndex);
        }

        public IEnumerator<CompletableController> GetEnumerator()
        {
            return controllers.GetEnumerator();
        }

        public int IndexOf(CompletableController item)
        {
            return controllers.IndexOf(item);
        }

        public void Insert(int index, CompletableController item)
        {
            controllers.Insert(index, item);
        }

        public bool Remove(CompletableController item)
        {
            return controllers.Remove(item);
        }

        public void RemoveAt(int index)
        {
            controllers.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return controllers.GetEnumerator();
        }

        public void AddRange(IEnumerable<CompletableController> completableControllers)
        {
            controllers.AddRange(completableControllers);
        }
        #endregion



        #region Serialization

        private static TData DeserializeFromString<TData>(string settings)
        {
            byte[] b = System.Convert.FromBase64String(settings);
            using (var stream = new MemoryStream(b))
            {
                var formatter = new BinaryFormatter();
                stream.Seek(0, SeekOrigin.Begin);
                return (TData)formatter.Deserialize(stream);
            }
        }

        private static string SerializeToString<TData>(TData settings)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, settings);
                stream.Flush();
                stream.Position = 0;
                return System.Convert.ToBase64String(stream.ToArray());
            }
        }

        #endregion
    }
}
