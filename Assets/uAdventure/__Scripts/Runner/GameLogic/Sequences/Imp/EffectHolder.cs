using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

using uAdventure.Core;
using UnityEngine.EventSystems;
using Xasu;
using Xasu.HighLevel;

namespace uAdventure.Runner
{
    public class EffectHolderNode
    {
        private readonly IEffect effect;
        private readonly Conditions conditions;

        private bool runsOnce = true;
        private ulong timesRun = 0;
        private bool waitForLoadPulse = false;
        private bool pulsed = false;

        public void doPulse()
        {
            this.pulsed = true;
        }

        private bool validated = false;
        private bool isValid = false;

        readonly Dictionary<string, object> additionalInfo = new Dictionary<string, object>();
        public void AddAdditionalInfo(string key, object value)
        {
            additionalInfo[key] = value;
        }

        public EffectHolderNode(IEffect effect)
        {
            this.effect = effect;
            var abstractEffect = effect as AbstractEffect;
            if (abstractEffect != null)
            {
                conditions = abstractEffect.getConditions();
            }
        }

        public bool execute()
        {
            var forceWait = false;
            if (effect != null && (!runsOnce || timesRun == 0 || waitForLoadPulse))
            {
                if (!validated)
                {
                    isValid = conditions == null || ConditionChecker.check(conditions);
                    validated = true;
                }

                if (isValid)
                {
                    switch (effect.getType())
                    {
                        case EffectType.ACTIVATE:
                            Game.Instance.GameState.SetFlag(((ActivateEffect)effect).getTargetId(), FlagCondition.FLAG_ACTIVE);
                            break;
                        case EffectType.DEACTIVATE:
                            Game.Instance.GameState.SetFlag(((DeactivateEffect)effect).getTargetId(), FlagCondition.FLAG_INACTIVE);
                            break;
                        case EffectType.SHOW_TEXT:
                        case EffectType.SPEAK_PLAYER:
                        case EffectType.SPEAK_CHAR:
                            runsOnce = false;
                            if (timesRun == 0)
                            {
                                if(effect.getType() == EffectType.SHOW_TEXT)
                                {
                                    var showTextEffect = (ShowTextEffect)effect;
                                    Game.Instance.Talk(showTextEffect.getText(), showTextEffect.getX(), showTextEffect.getY(),
                                        showTextEffect.getRgbFrontColor(), showTextEffect.getRgbBorderColor());
                                }
                                else if (effect.getType() == EffectType.SPEAK_PLAYER)
                                {
                                    Game.Instance.Talk(((SpeakPlayerEffect)effect).getLine(), Player.IDENTIFIER);
                                }
                                else if(effect.getType() == EffectType.SPEAK_CHAR)
                                {
                                    Game.Instance.Talk(((SpeakCharEffect)effect).getLine(), ((SpeakCharEffect)effect).getTargetId());
                                }
                                if (XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
                                {
                                    CompletableTracker.Instance.Initialized(effect.GUID, CompletableTracker.CompletableType.DialogFragment);
                                }
                                forceWait = true;
                            }
                            else 
                            {
                                if (GUIManager.Instance.InteractWithDialogue() == InteractuableResult.REQUIRES_MORE_INTERACTION)
                                {
                                    forceWait = true;
                                    if (XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
                                    {
                                        CompletableTracker.Instance.Progressed(effect.GUID, CompletableTracker.CompletableType.DialogFragment, 1f);
                                    }
                                }
                                else if(XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
                                {
                                    CompletableTracker.Instance.Completed(effect.GUID, CompletableTracker.CompletableType.DialogFragment);
                                }
                            }
                            break;
                        case EffectType.TRIGGER_SCENE:
                            if (!waitForLoadPulse)
                            {
                                runsOnce = false;
                                TriggerSceneEffect tse = ((TriggerSceneEffect)effect);
                                if (!Game.Instance.GameState.IsFirstPerson)
                                {
                                    var playerContext = Game.Instance.GameState.PlayerContext;
                                    if (tse.getX() != int.MinValue || tse.getY() != int.MinValue)
                                    {
                                        playerContext.setPosition(tse.getX(), tse.getY());
                                        if (tse.DestinyScale > 0)
                                        {
                                            playerContext.Scale = tse.DestinyScale;
                                        }
                                    }
                                    else
                                    {
                                        var targetScene = Game.Instance.GameState.GetChapterTarget(tse.getTargetId()) as Scene;
                                        if (targetScene != null)
                                        {
                                            if (targetScene.getTrajectory() != null)
                                            {
                                                var initial = targetScene.getTrajectory().getInitial();
                                                playerContext.setPosition(initial.getX(), initial.getY());
                                                playerContext.Scale = initial.getScale();
                                            }
                                            else
                                            {
                                                playerContext.setPosition(targetScene.getPositionX(), targetScene.getPositionY());
                                                playerContext.Scale = targetScene.getPlayerScale();
                                            }
                                        }
                                    }
                                }

                                var trace = !(additionalInfo.ContainsKey("disable_trace") && (bool)additionalInfo["disable_trace"]);
                                Game.Instance.RunTarget(tse.getTargetId(), tse.getTransitionTime(), tse.getTransitionType(), null, trace);
                                waitForLoadPulse = true;
                                forceWait = true;
                            }
                            else
                            {
                                waitForLoadPulse = false;
                            }
                            // DODO make something to wait until the target is ready to prevent undesired effect advance
                            break;
                        case EffectType.TRIGGER_CUTSCENE:
                            runsOnce = false;
                            TriggerCutsceneEffect tce = (TriggerCutsceneEffect)effect;
                            if (timesRun > 1) // The first interaction is the run target callback
                            {
                                if (additionalInfo.ContainsKey("sub_effects_wait"))
                                    forceWait = false;
                                else
                                {
                                    InteractuableResult res = ((Interactuable)additionalInfo["scene"]).Interacted();
                                    if (res == InteractuableResult.REQUIRES_MORE_INTERACTION)
                                        forceWait = true;
                                    else if (res == InteractuableResult.DOES_SOMETHING)
                                    {
                                        additionalInfo["sub_effects_wait"] = true;
                                        forceWait = true;
                                    }
                                }
                            }
                            else if (timesRun == 1)
                            {
                                forceWait = true;
                            }
                            else if (timesRun == 0)
                            {
                                var trace = !(additionalInfo.ContainsKey("disable_trace") && (bool)additionalInfo["disable_trace"]);

                                additionalInfo.Add("lastscene", Game.Instance.GameState.CurrentTarget);
                                additionalInfo.Add("scene", Game.Instance.RunTarget(tce.getTargetId(), null, trace));
                                forceWait = true;
                            }

                            if (!forceWait && ((Cutscene)((IRunnerChapterTarget)additionalInfo["scene"]).Data).getNext() == Cutscene.GOBACK)
                            {
                                string last = (string)additionalInfo["lastscene"];
                                Game.Instance.RunTarget(last);
                            }

                            break;
                        case EffectType.TRIGGER_LAST_SCENE:
                            runsOnce = false;
                            Game.Instance.SwitchToLastTarget();
                            break;
                        case EffectType.TRIGGER_CONVERSATION:
                            runsOnce = false;
                            runsOnce = false;
                            if (timesRun == 0)
                            {
                                var tcoe = (TriggerConversationEffect)effect;
                                this.additionalInfo.Add("conversation", new GraphConversationHolder(Game.Instance.GameState.GetConversation(tcoe.getTargetId())));
                            }
                            forceWait = ((GraphConversationHolder)this.additionalInfo["conversation"]).execute();
                            break;
                        case EffectType.RANDOM_EFFECT:
                            RandomEffect re = (RandomEffect)effect;

                            if (timesRun == 0)
                            {
                                int pro = re.getProbability(), now = Random.Range(0, 100);

                                if (pro <= now)
                                {
                                    if (re.getPositiveEffect() != null)
                                    {
                                        additionalInfo.Add("current", new EffectHolderNode(re.getPositiveEffect()));
                                        runsOnce = false;
                                    }
                                }
                                else if (re.getNegativeEffect() != null)
                                {
                                    additionalInfo.Add("current", new EffectHolderNode(re.getNegativeEffect()));
                                    runsOnce = false;
                                }
                            }

                            if (additionalInfo.ContainsKey("current"))
                            {
                                var subEffectHolder = (EffectHolderNode)additionalInfo["current"];
                                forceWait = subEffectHolder.execute();
                                runsOnce = subEffectHolder.runsOnce;
                            }

                            break;
                        case EffectType.SET_VALUE:
                            SetValueEffect sve = (SetValueEffect)effect;
                            Game.Instance.GameState.SetVariable(sve.getTargetId(), sve.getValue());
                            break;
                        case EffectType.INCREMENT_VAR:
                            IncrementVarEffect ive = (IncrementVarEffect)effect;
                            Game.Instance.GameState.SetVariable(ive.getTargetId(), Game.Instance.GameState.GetVariable(ive.getTargetId()) + ive.getIncrement());
                            break;
                        case EffectType.DECREMENT_VAR:
                            DecrementVarEffect dve = (DecrementVarEffect)effect;
                            Game.Instance.GameState.SetVariable(dve.getTargetId(), Game.Instance.GameState.GetVariable(dve.getTargetId()) - dve.getDecrement());
                            break;
                        case EffectType.MACRO_REF:
                            runsOnce = false;
                            if (timesRun == 0)
                            {
                                MacroReferenceEffect mre = (MacroReferenceEffect)effect;
                                this.additionalInfo.Add("macro", new EffectHolder(Game.Instance.GameState.GetMacro(mre.getTargetId())));
                            }
                            forceWait = ((EffectHolder)this.additionalInfo["macro"]).execute();
                            break;
                        case EffectType.MOVE_OBJECT:
                            MoveObjectEffect moe = (MoveObjectEffect)effect;
                            if (timesRun == 0)
                            {
                                if (moe.isAnimated())
                                {
                                    Game.Instance.GameState.Move(moe.getTargetId(), new Vector2(moe.getX(), moe.getY()), moe.getTranslateSpeed(), this);
                                    runsOnce = false;
                                }
                                else
                                {
                                    Game.Instance.GameState.Move(moe.getTargetId(), new Vector2(moe.getX(), moe.getY()));
                                }
                            }
                            if (!runsOnce && !pulsed)
                            {
                                forceWait = true;
                            }
                            break;
                        case EffectType.MOVE_NPC:
                            MoveNPCEffect mne = (MoveNPCEffect)effect;
                            if (timesRun == 0)
                            {
                                runsOnce = false;
                                timesRun++;
                                Game.Instance.GameState.Move(mne.getTargetId(), new Vector2(mne.getX(), mne.getY()), 1, this);
                                Game.Instance.RunInBackground(this);
                                return false;
                            }
                            if (!pulsed)
                            {
                                forceWait = true;
                            }
                            break;
                        case EffectType.GENERATE_OBJECT:
                            GenerateObjectEffect gen = (GenerateObjectEffect)effect;
                            var toAdd = Game.Instance.GameState.FindElement<Item>(gen.getTargetId());
                            InventoryManager.Instance.AddElement(toAdd);
                            break;
                        case EffectType.CONSUME_OBJECT:
                            ConsumeObjectEffect con = (ConsumeObjectEffect)effect;
                            var toRemove = Game.Instance.GameState.FindElement<Item>(con.getTargetId());
                            InventoryManager.Instance.RemoveElement(toRemove);
                            break;
                        case EffectType.TRIGGER_BOOK:
                            if (timesRun == 0)
                            {
                                if (InventoryManager.Instance.Opened)
                                {
                                    InventoryManager.Instance.Close();
                                }
                                TriggerBookEffect triggerBookEffect = (TriggerBookEffect)effect;
                                Game.Instance.ShowBook(triggerBookEffect.getTargetId());
                            }
                            runsOnce = false;
                            forceWait = Game.Instance.ShowingBook;
                            break;
                        case EffectType.PLAY_SOUND:
                            PlaySoundEffect pse = (PlaySoundEffect)effect;
                            AudioClip audioClip = Game.Instance.ResourceManager.getAudio(pse.getPath());
                            PlayMusicOn(audioClip, Game.Instance);
                            break;
                        case EffectType.WAIT_TIME:
                            WaitTimeEffect wte = (WaitTimeEffect)effect;
                            runsOnce = false;
                            if (timesRun == 0)
                            {
                                Game.Instance.PulseOnTime(this, wte.getTime());
                            }
                            if (!pulsed)
                            {
                                forceWait = true;
                            }
                            break;
                        case EffectType.CANCEL_ACTION:
                            Game.Instance.ActionCanceled();
                            forceWait = true;
                            break;
                        case EffectType.CUSTOM_EFFECT:
                            runsOnce = false;
                            if (timesRun == 0)
                            {
                                this.additionalInfo["custom_effect_runner"] = CustomEffectRunnerFactory.Instance.CreateRunnerFor(effect);
                            }
                            forceWait = ((CustomEffectRunner)this.additionalInfo["custom_effect_runner"]).execute();
                            break;
                    }
                }
            }

            if (!forceWait)
                timesRun = 0;
            else
                timesRun++;

            return forceWait;
        }

        public bool check()
        {
            return conditions == null || ConditionChecker.check(conditions);
        }

        private void PlayMusicOn(AudioClip clip, MonoBehaviour player)
        {
            if (!clip)
            {
                return;
            }

            player.StartCoroutine(PlayMusicCoroutineOn(clip, player));
        }

        private IEnumerator PlayMusicCoroutineOn(AudioClip clip, MonoBehaviour player)
        {
            if (!clip)
            {
                yield return null;
            }

            GameObject tmp = new GameObject("SoundHolder");
            tmp.transform.SetParent(player.transform);
            tmp.transform.localPosition = Vector3.zero;
            AudioSource tmpaudio = tmp.AddComponent<AudioSource>();
            tmp.transform.SetParent(Camera.main.transform);
            tmpaudio.PlayOneShot(clip);

            yield return new WaitWhile(() => tmpaudio.isPlaying);

            Object.DestroyImmediate(tmp);
        }
    }

    public class EffectHolder : Secuence, Interactuable
    {
        public List<EffectHolderNode> effects;
        internal readonly Effects originalEffects;

        private string documentation;

        public EffectHolder(Effects effects)
        {
            this.originalEffects = effects;
            this.effects = new List<EffectHolderNode>();
            EffectHolderNode previousEffect = null;

            if (effects != null && effects.getEffects().Count > 0)
            {

                //List<Condition> conditions = new List<Condition>();
                foreach (IEffect effect in effects.getEffects())
                {
                    if (effect != null) // TODO check if this (if) is correct
                    {
                        var newHolder = new EffectHolderNode(effect);
                        if(previousEffect != null)
                        {
                            previousEffect.AddAdditionalInfo("next_effect", newHolder);
                        }
                        this.effects.Add(newHolder);
                        previousEffect = newHolder;
                    }
                }
            }
        }

        private int lastexecuted = 0;
        public bool execute()
        {
            bool forcewait = false;
            for (int i = lastexecuted; i < effects.Count; i++)
            {
                if (effects[i].execute())
                {
                    lastexecuted = i;
                    forcewait = true;
                    break;
                }
            }

            if (!forcewait)
                lastexecuted = 0;

            return forcewait;
        }

        public InteractuableResult Interacted(PointerEventData pointerData = null)
        {
            if (this.execute())
            {
                return InteractuableResult.REQUIRES_MORE_INTERACTION;
            }
            else
                return InteractuableResult.DOES_SOMETHING;
        }

        public bool canBeInteracted()
        {
            return true;
        }

        public void setInteractuable(bool state)
        {
        }
    }
}
