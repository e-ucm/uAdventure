using System;
using UnityEngine;
using UnityEditor;

using uAdventure.Core;
using System.Collections.Generic;
using System.Linq;

namespace uAdventure.Editor
{
    public class TriggerSceneEffectEditor : IEffectEditor
    {
        public bool Collapsed { get; set; }
        private Rect window = new Rect(0, 0, 300, 0);
        private string[] scenes;
        private readonly SceneEditor localSceneEditor;
        private readonly Trajectory.Node playerDestination;
        private readonly string[] transitionTypes;

        public Rect Window
        {
            get
            {
                return Collapsed ? new Rect(window.x, window.y, 50, 30) : window;
            }
            set
            {
                window = Collapsed ? new Rect(value.x, value.y, window.width, window.height) : value;
            }
        }

        protected TriggerSceneEffect effect;

        public TriggerSceneEffectEditor()
        {
            scenes = Controller.Instance.IdentifierSummary.getIds<IChapterTarget>();
            this.effect = new TriggerSceneEffect(scenes[0], 400, 300)
            {
                DestinyScale = 1
            };

            SetDestinyScene(0);

            localSceneEditor = new SceneEditor();
            playerDestination = new Trajectory.Node("", 0, 0, 1f);
            localSceneEditor.Elements = new List<DataControl>() { new NodeDataControl(null, playerDestination, new Trajectory()) };

            transitionTypes = new string[]
            {
                TC.get("NextScene.NoTransition"),
                TC.get("NextScene.TopToBottom"),
                TC.get("NextScene.BottomToTop"),
                TC.get("NextScene.LeftToRight"),
                TC.get("NextScene.RightToLeft"),
                TC.get("NextScene.FadeIn")
            };
        }

        public virtual void draw()
        {
            scenes = Controller.Instance.IdentifierSummary.getIds<IChapterTarget>();
            scenes = scenes.Except(Controller.Instance.IdentifierSummary.getIds<Cutscene>()).ToArray();

            EditorGUI.BeginChangeCheck();
            var sceneIndex = EditorGUILayout.Popup(TC.get("Element.Name2"), Array.IndexOf(scenes, effect.getTargetId()), scenes);
            if (EditorGUI.EndChangeCheck())
            {
                SetDestinyScene(sceneIndex);
            }

            if (sceneIndex == -1)
            {
                EditorGUILayout.HelpBox("TriggerSceneEffectEditor.ValidDestination".Traslate(), MessageType.Error);
                return;
            }

            // Transition Type
            EditorGUI.BeginChangeCheck();
            var newType = EditorGUILayout.Popup(TC.get("NextScene.Transition"), (int)effect.getTransitionType(), transitionTypes);
            if (EditorGUI.EndChangeCheck())
            {
                effect.setTransitionType((TransitionType)newType);
            }

            // Transition Time
            EditorGUI.BeginChangeCheck();
            var time = Mathf.Clamp(EditorGUILayout.IntField(TC.get("NextScene.TransitionTime"), effect.getTransitionTime()), 0, 5000);
            if (EditorGUI.EndChangeCheck())
            {
                effect.setTransitionTime(time);
            }

            var scenesList = Controller.Instance.SelectedChapterDataControl.getScenesList();
            // If the selected scene IS a scene (not a cutscene or any other type)
            if(Controller.Instance.PlayerMode == Controller.FILE_ADVENTURE_3RDPERSON_PLAYER && sceneIndex < scenesList.getScenes().Count)
            {
                var pos = EditorGUILayout.Vector2IntField(TC.get("Inventory.Position"), new Vector2Int(effect.getX(), effect.getY()));
                effect.setPosition(pos.x, pos.y);

                EditorGUI.BeginChangeCheck();
                bool useDestinyScale = EditorGUILayout.Toggle("TriggerSceneEffectEditor.UseScale".Traslate(), effect.DestinyScale >= 0);
                if (EditorGUI.EndChangeCheck())
                {
                    effect.DestinyScale = useDestinyScale ? 1f : float.MinValue;
                }

                if (useDestinyScale)
                {
                    EditorGUI.BeginChangeCheck();
                    var newScale = Mathf.Max(0.001f, EditorGUILayout.FloatField(TC.get("SceneLocation.Scale"), effect.DestinyScale));
                    if (EditorGUI.EndChangeCheck())
                    {
                        effect.DestinyScale = newScale;
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("TriggerSceneEffectEditor.SizeMaintained".Traslate(), MessageType.Info);
                }

                localSceneEditor.Components = uAdventureWindowMain.Components;
                localSceneEditor.Scene = scenesList.getScenes()[sceneIndex];
                playerDestination.setValues(effect.getX(), effect.getY(), useDestinyScale ? effect.DestinyScale : 1f);

                var previousScale = playerDestination.getScale();

                localSceneEditor.Draw(GUILayoutUtility.GetRect(0, 200, GUILayout.ExpandWidth(true)));
                effect.setPosition(playerDestination.getX(), playerDestination.getY());
                if(previousScale != playerDestination.getScale())
                {
                    effect.DestinyScale = playerDestination.getScale();
                }
            }

            EditorGUILayout.HelpBox(TC.get("TriggerSceneEffect.Description"), MessageType.Info);
        }

        private void SetDestinyScene(int sceneIndex)
        {
            effect.setTargetId(scenes[sceneIndex]);

            var scenesListDataControl = Controller.Instance.SelectedChapterDataControl.getScenesList();
            var sceneDataControlIndex = scenesListDataControl.getSceneIndexByID(scenes[sceneIndex]);
            if (sceneDataControlIndex != -1)
            {
                var destinationScene = scenesListDataControl.getScenes()[sceneDataControlIndex];
                var defPlayerPos = destinationScene.getDefaultInitialPosition();
                effect.setPosition((int)defPlayerPos.x, (int)defPlayerPos.y);
                var defScale = destinationScene.getPlayerAppropiateScale();
                effect.DestinyScale = defScale != 1 ? defScale : float.MinValue;
            }
        }

        public IEffect Effect { get { return effect; } set { effect = value as TriggerSceneEffect; } }
        public virtual string EffectName { get { return TC.get("TriggerSceneEffect.Title"); } }
        public virtual IEffectEditor clone() { return new TriggerSceneEffectEditor(); }

        public bool manages(IEffect c)
        {
            return c.GetType() == effect.GetType();
        }
        public virtual bool Usable { get { return true; } }
    }
}