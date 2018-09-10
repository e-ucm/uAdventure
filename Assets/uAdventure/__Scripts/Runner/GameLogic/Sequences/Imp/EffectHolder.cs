using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

using uAdventure.Core;
using UnityEngine.EventSystems;

namespace uAdventure.Runner
{
    public class EffectHolderNode
    {
        IEffect effect;
        Conditions conditions;

        private bool runs_once = true;
        private int times_runed = 0;
        private bool waitForLoadPulse = false;

        private bool validated = false;
        private bool is_valid = false;

        Dictionary<string, object> aditional_info = new Dictionary<string, object>();
        public void AddAditionalInfo(string key, object value)
        {
            aditional_info[key] = value;
        }

        public EffectHolderNode(IEffect effect)
        {
            this.effect = effect;
            if (effect is AbstractEffect)
                conditions = (effect as AbstractEffect).getConditions();
        }

        public bool execute()
        {
            bool forcewait = false;
            if (!(runs_once && times_runed > 0) || waitForLoadPulse)
            {
                if (effect != null)
                {
                    if (!validated)
                    {
                        is_valid = conditions == null || ConditionChecker.check(conditions);
                        validated = true;
                    }

                    if (is_valid)
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
                                var showTextEffect = (ShowTextEffect)effect;
                                Game.Instance.Talk(showTextEffect.getText(), showTextEffect.getX(), showTextEffect.getY(),
                                    showTextEffect.getRgbFrontColor(), showTextEffect.getRgbBorderColor());
                                forcewait = true;
                                break;
                            case EffectType.SPEAK_PLAYER:
                                Game.Instance.Talk(((SpeakPlayerEffect)effect).getLine(), Player.IDENTIFIER);
                                forcewait = true;
                                break;
                            case EffectType.SPEAK_CHAR:
                                Game.Instance.Talk(((SpeakCharEffect)effect).getLine(), ((SpeakCharEffect)effect).getTargetId());
                                forcewait = true;
                                break;
                            case EffectType.TRIGGER_SCENE:
                                if (!waitForLoadPulse)
                                {
                                    runs_once = false;
                                    TriggerSceneEffect tse = ((TriggerSceneEffect)effect);
                                    if (!Game.Instance.GameState.IsFirstPerson)
                                    {
                                        var playerContext = Game.Instance.GameState.PlayerContext;
                                        if (tse.getX() != int.MinValue || tse.getY() != int.MinValue)
                                        {
                                            playerContext.setPosition(tse.getX(), tse.getY());
                                            if (tse.DestinyScale > 0)
                                                playerContext.setScale(tse.DestinyScale);
                                        }
                                        else
                                        {
                                            var targetScene = Game.Instance.GameState.GetChapterTarget(tse.getTargetId()) as Scene;
                                            if (targetScene != null)
                                            {
                                                if(targetScene.getTrajectory() != null)
                                                {
                                                    var initial = targetScene.getTrajectory().getInitial();
                                                    playerContext.setPosition(initial.getX(), initial.getY());
                                                    playerContext.setScale(initial.getScale());
                                                }
                                                else
                                                {
                                                    playerContext.setPosition(targetScene.getPositionX(), targetScene.getPositionY());
                                                    playerContext.setScale(targetScene.getPlayerScale());
                                                }
                                            }
                                        }
                                    }
                                    bool trace = true;
                                    if (aditional_info.ContainsKey("disable_trace") && (bool)aditional_info["disable_trace"] == true)
                                        trace = false;

                                    Game.Instance.RunTarget(tse.getTargetId(), tse.getTransitionTime(), tse.getTransitionType(), null, trace);
                                    waitForLoadPulse = true;
                                    forcewait = true;
                                }
                                else
                                {
                                    waitForLoadPulse = false;
                                }
                                // DODO make something to wait until the target is ready to prevent undesired effect advance
                                break;
                            case EffectType.TRIGGER_CUTSCENE:
                                runs_once = false;
                                TriggerCutsceneEffect tce = (TriggerCutsceneEffect)effect;
                                if (times_runed > 1) // The first interaction is the run target callback
                                {
                                    if (aditional_info.ContainsKey("sub_effects_wait"))
                                        forcewait = false;
                                    else
                                    {
                                        InteractuableResult res = ((Interactuable)aditional_info["scene"]).Interacted();
                                        if (res == InteractuableResult.REQUIRES_MORE_INTERACTION)
                                            forcewait = true;
                                        else if (res == InteractuableResult.DOES_SOMETHING)
                                        {
                                            aditional_info["sub_effects_wait"] = true;
                                            forcewait = true;
                                        }
                                    }
                                }
                                else if(times_runed == 1)
                                {
                                    forcewait = true;
                                }
                                else if (times_runed == 0)
                                {
                                    bool trace = true;
                                    if (aditional_info.ContainsKey("disable_trace") && (bool)aditional_info["disable_trace"] == true)
                                        trace = false;
                                    
                                    aditional_info.Add("lastscene", Game.Instance.GameState.CurrentTarget);
                                    aditional_info.Add("scene", Game.Instance.RunTarget(tce.getTargetId(), null, trace));
                                    forcewait = true;
                                }

                                if (!forcewait && ((Cutscene)((IRunnerChapterTarget)aditional_info["scene"]).Data).getNext() == Cutscene.GOBACK)
                                {
                                    string last = (string)aditional_info["lastscene"];
                                    Game.Instance.RunTarget(last);
                                }

                                break;
                            case EffectType.TRIGGER_LAST_SCENE:
                                runs_once = false;
                                Game.Instance.SwitchToLastTarget();
                                break;
                            case EffectType.TRIGGER_CONVERSATION:
                                runs_once = false;
                                runs_once = false;
                                if (times_runed == 0)
                                {
                                    TriggerConversationEffect tcoe = (TriggerConversationEffect)effect;
                                    this.aditional_info.Add("conversation", new GraphConversationHolder(Game.Instance.GameState.GetConversation(tcoe.getTargetId())));
                                }
                                forcewait = ((GraphConversationHolder)this.aditional_info["conversation"]).execute();
                                break;
                            case EffectType.RANDOM_EFFECT:
                                runs_once = false;
                                RandomEffect re = (RandomEffect)effect;

                                if (!aditional_info.ContainsKey("first"))
                                {
                                    aditional_info.Add("first", new EffectHolderNode(re.getPositiveEffect()));
                                    aditional_info.Add("second", new EffectHolderNode(re.getNegativeEffect()));
                                }

                                if (times_runed == 0)
                                {
                                    int pro = re.getProbability(), now = Random.Range(0, 100);
                                    if (aditional_info.ContainsKey("current"))
                                        aditional_info.Remove("current");

                                    if (pro <= now)
                                        aditional_info.Add("current", "first");
                                    else
                                        aditional_info.Add("current", "second");

                                    forcewait = ((EffectHolderNode)aditional_info[((string)aditional_info["current"])]).execute();
                                }
                                else
                                    forcewait = ((EffectHolderNode)aditional_info[((string)aditional_info["current"])]).execute();

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
                                runs_once = false;
                                if (times_runed == 0)
                                {
                                    MacroReferenceEffect mre = (MacroReferenceEffect)effect;
                                    this.aditional_info.Add("macro", new EffectHolder(Game.Instance.GameState.GetMacro(mre.getTargetId())));
                                }
                                forcewait = ((EffectHolder)this.aditional_info["macro"]).execute();
                                break;
                            case EffectType.MOVE_OBJECT:
                                MoveObjectEffect moe = (MoveObjectEffect)effect;
                                Game.Instance.GameState.Move(moe.getTargetId(), new Vector2(moe.getX(), 600 - moe.getY()) / 10f);
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
                                if (times_runed == 0)
                                {
                                    if (InventoryManager.Instance.Opened)
                                    {
                                        InventoryManager.Instance.Close();
                                    }
                                    TriggerBookEffect triggerBookEffect = (TriggerBookEffect)effect;
                                    Game.Instance.ShowBook(triggerBookEffect.getTargetId());
                                }
                                runs_once = false;
                                forcewait = Game.Instance.ShowingBook;
                                break;
                            case EffectType.CUSTOM_EFFECT:
                                runs_once = false;
                                if(times_runed == 0)
                                {
                                    this.aditional_info["custom_effect_runner"] = CustomEffectRunnerFactory.Instance.CreateRunnerFor(effect);
                                }
                                forcewait = ((CustomEffectRunner)this.aditional_info["custom_effect_runner"]).execute();
                                break;
                        }
                    }
                }
            }

            if (!forcewait)
                times_runed = 0;
            else
                times_runed++;

            return forcewait;
        }

        public bool check()
        {
            return conditions == null || ConditionChecker.check(conditions);
        }
    }

    public class EffectHolder : Secuence, Interactuable
    {
        public List<EffectHolderNode> effects;

        private string documentation;

        public EffectHolder(Effects effects)
        {
            this.effects = new List<EffectHolderNode>();

            if (effects != null && effects.getEffects().Count > 0)
            {
                
                //List<Condition> conditions = new List<Condition>();
                foreach (IEffect effect in effects.getEffects())
                {
                    if (effect != null) // TODO check if this (if) is correct
                        this.effects.Add(new EffectHolderNode(effect));
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