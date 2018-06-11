using System;
using UnityEngine;
using UnityEditor;

using uAdventure.Core;
using System.Collections.Generic;

namespace uAdventure.Editor
{
    public class TriggerSceneEffectEditor : EffectEditor
    {
        private bool collapsed = false;
        public bool Collapsed { get { return collapsed; } set { collapsed = value; } }
        private Rect window = new Rect(0, 0, 300, 0);
        private string[] scenes;
        private SceneEditor localSceneEditor;
        private Trajectory.Node playerDestination;
        private List<DataControl> elements;

        public Rect Window
        {
            get
            {
                if (collapsed) return new Rect(window.x, window.y, 50, 30);
                else return window;
            }
            set
            {
                if (collapsed) window = new Rect(value.x, value.y, window.width, window.height);
                else window = value;
            }
        }

        protected TriggerSceneEffect effect;

        public TriggerSceneEffectEditor()
        {
            scenes = Controller.Instance.IdentifierSummary.getIds<IChapterTarget>();
            this.effect = new TriggerSceneEffect(scenes[0], 400, 300);
            this.effect.DestinyScale = 1;

            localSceneEditor = new SceneEditor();
            playerDestination = new Trajectory.Node("", 0, 0, 1f);
            localSceneEditor.elements = new List<DataControl>() { new NodeDataControl(null, playerDestination, new Trajectory()) };
        }

        public virtual void draw()
        {
            scenes = Controller.Instance.IdentifierSummary.getIds<IChapterTarget>();
            var sceneIndex = EditorGUILayout.Popup(TC.get("Element.Name2"), Array.IndexOf(scenes, effect.getTargetId()), scenes);
            effect.setTargetId(scenes[sceneIndex]);
            if (sceneIndex == -1)
            {
                EditorGUILayout.HelpBox("Please select a valid destination!", MessageType.Error);
                return;
            }
            
            var scenesList = Controller.Instance.SelectedChapterDataControl.getScenesList();
            // If the selected scene IS a scene (not a cutscene or any other type)
            if(sceneIndex < scenesList.getScenes().Count)
            {
                var pos = EditorGUILayout.Vector2IntField(TC.get("Inventory.Position"), new Vector2Int(effect.getX(), effect.getY()));
                effect.setPosition(pos.x, pos.y);

                EditorGUI.BeginChangeCheck();
                bool useDestinyScale = EditorGUILayout.Toggle("Use destiny scale", effect.DestinyScale >= 0); // TODO LANG
                if (EditorGUI.EndChangeCheck())
                    effect.DestinyScale = useDestinyScale ? 1f : float.MinValue;

                if (useDestinyScale)
                {
                    EditorGUI.BeginChangeCheck();
                    var newScale = Mathf.Max(0.001f, EditorGUILayout.FloatField(TC.get("SceneLocation.Scale"), effect.DestinyScale));
                    if (EditorGUI.EndChangeCheck())
                        effect.DestinyScale = newScale;
                }
                else
                {
                    EditorGUILayout.HelpBox("The player size will stay as before entering the exit.", MessageType.Info); // TODO LANG
                }

                localSceneEditor.Components = EditorWindowBase.Components;
                localSceneEditor.Scene = scenesList.getScenes()[sceneIndex];
                playerDestination.setValues(effect.getX(), effect.getY(), effect.DestinyScale);

                localSceneEditor.Draw(GUILayoutUtility.GetRect(0, 200, GUILayout.ExpandWidth(true)));
                effect.setPosition(playerDestination.getX(), playerDestination.getY());
                effect.DestinyScale = playerDestination.getScale(); 
            }

            EditorGUILayout.HelpBox(TC.get("TriggerSceneEffect.Description"), MessageType.Info);
        }

        public IEffect Effect { get { return effect; } set { effect = value as TriggerSceneEffect; } }
        public virtual string EffectName { get { return TC.get("TriggerSceneEffect.Title"); } }
        public virtual EffectEditor clone() { return new TriggerSceneEffectEditor(); }

        public bool manages(IEffect c)
        {
            return c.GetType() == effect.GetType();
        }
        public virtual bool Usable { get { return true; } }
    }
}