using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using uAdventure.Core;
using System;

namespace uAdventure.Editor
{
    public class EffectsEditor : CollapsibleGraphEditor<Effects, IEffect>
    {
        private static readonly Vector2 initialSize = new Vector2(200, 50);
        private static GUIContent buttonContent = new GUIContent();
        private Dictionary<IEffect, EffectEditor> editors = new Dictionary<IEffect, EffectEditor>();
        private GUIStyle conditionStyle, eitherConditionStyle;
        private Dictionary<string, Texture2D> icons = new Dictionary<string, Texture2D>();

        public override void Init(Effects content)
        {
            if (conditionStyle == null)
            {
                conditionStyle = new GUIStyle(GUI.skin.box);
                conditionStyle.normal.background = MakeTex(1, 1, new Color(0.627f, 0.627f, 0.627f));
            }

            if (eitherConditionStyle == null)
            {
                eitherConditionStyle = new GUIStyle(GUI.skin.box);
                eitherConditionStyle.normal.background = MakeTex(1, 1, new Color(0.568f, 0.568f, 0.568f));
                eitherConditionStyle.padding.left = 15;
            }

            base.Init(content);
        }

        protected override IEffect[] ChildsFor(Effects effects, IEffect parent)
        {
            var index = effects.FindIndex(parent.Equals);
            return index == effects.Count - 1 ? new IEffect[0] : new IEffect[1] { effects[index + 1] };
        }

        protected override void DeleteNode(Effects effects, IEffect effect)
        {
            effects.Remove(effect);
            if(Selection.Contains(effect))
                Selection.Remove(effect);
        }

        protected override void DrawOpenNodeContent(Effects effects, IEffect effect)
        {
            var abstractEffect = effect as AbstractEffect;

            if (abstractEffect != null)
            {
                GUILayout.BeginVertical(conditionStyle);
                GUILayout.Label("CONDITIONS");
                if (GUILayout.Button("Add Block"))
                {
                    abstractEffect.getConditions().Add(new FlagCondition(""));
                }

                //##################################################################################
                //############################### CONDITION HANDLING ###############################
                //##################################################################################

                var toRemove = new List<Condition>();
                var listsToRemove = new List<List<Condition>>();
                var conditions = abstractEffect.getConditions();
                ConditionEditorWindow.LayoutConditionEditor(conditions);

                //##################################################################################

                GUILayout.EndVertical();
            }
            var prevLabelSize = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 75;
            if (editors.ContainsKey(effect))
                editors[effect].draw(); 
            EditorGUIUtility.labelWidth = prevLabelSize;
        }

        protected override IEffect[] GetNodes(Effects effects)
        {
            return effects.ToArray();
        }

        private bool CreateAndInitEffectEditor(Effects effects, IEffect effect)
        {
            var editor = EffectEditorFactory.Intance.createEffectEditorFor(effect);
            if (editor.manages(effect))
                editor.Effect = effect;
            editor.Window = new Rect(new Vector2(50, 50), initialSize);
            if (editor == null)
            {
                Debug.LogWarning("No effect editor available for: " + effect.getType().ToString());
                return false;
            }

            var index = effects.IndexOf(effect);
            if (index > 0)
            {
                var pos = editor.Window;
                var parentPos = editors[effects[index - 1]].Window;
                pos.position = parentPos.position + new Vector2(parentPos.size.x + 50, 0);
                editor.Window = pos;
            }

            editors[effect] = editor;
            return true;
        }

        protected override Rect GetOpenedNodeRect(Effects effects, IEffect effect)
        {
            if (!editors.ContainsKey(effect))
            {
                if(!CreateAndInitEffectEditor(effects, effect))
                    return Rect.zero;
            }

            return editors[effect].Window;
        }

        protected override string GetTitle(Effects effects, IEffect effect)
        {
            return effect.getType().ToString();
        }

        protected override GUIContent OpenButtonText(Effects content, IEffect node)
        {
            var nodeType = node.getType().ToString();
            if (!icons.ContainsKey(nodeType))
            {
                var n = nodeType.Replace("_", "-").ToLower();
                icons[nodeType] = Resources.Load<Texture2D>("EAdventureData/img/icons/effects/16x16/" + n);
            }
            buttonContent.image = icons[nodeType];
            buttonContent.text = node.ToString();
            return buttonContent;
        }

        protected override void DrawNodeControls(Effects effects, IEffect effect)
        {
            EffectEditor editor = null;
            editors.TryGetValue(effect, out editor);

            string[] editorNames = EffectEditorFactory.Intance.CurrentEffectEditors;
            int preEditorSelected = EffectEditorFactory.Intance.EffectEditorIndex(effect);
            int editorSelected = EditorGUILayout.Popup(preEditorSelected, editorNames);


            if (preEditorSelected != editorSelected)
            {
                editor = EffectEditorFactory.Intance.createEffectEditorFor(editorNames[editorSelected]);
                editor.Window = editors[effect].Window;
                editors[editor.Effect] = editor;
                collapsedState[editor.Effect] = collapsedState[effect];
                collapsedState.Remove(effect);

                if (editor.manages(effect))
                    editor.Effect = effect;
                else
                {
                    // Re insert the conditions
                    var abstractEffect = effect as AbstractEffect;
                    if (abstractEffect != null && editor.Effect is AbstractEffect)
                    {
                        (editor.Effect as AbstractEffect).setConditions(abstractEffect.getConditions());
                    }
                    // Replace the effect
                    effects[effects.IndexOf(effect)] = editor.Effect;
                }
            }

            base.DrawNodeControls(effects, effect);
        }

        protected override void SetNodeChild(Effects effects, IEffect effect, int slot, IEffect child)
        {
            var index = effects.IndexOf(effect) + 1;
            var childIndex = effects.IndexOf(child);
            while (index < childIndex)
            {
                effects.RemoveAt(index);
                ++index;
            }
        }

        protected override void SetNodePosition(Effects effects, IEffect effect, Vector2 position)
        {
            if (!editors.ContainsKey(effect))
                return;

            var rect = editors[effect].Window;
            rect.position = position;
            editors[effect].Window = rect;
        }

        protected override void SetNodeSize(Effects effects, IEffect effect, Vector2 size)
        {
            if (!editors.ContainsKey(effect))
                return;

            var rect = editors[effect].Window;
            rect.size = size;
            editors[effect].Window = rect;
        }

        // AUX METHODS
        private static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }
    }

    public class EffectEditorWindow : EditorWindow
    {
        private static EffectEditorWindow editor;
        private EffectsEditor effectsEditor;
        private Effects effects;

        public Vector2 scrollPosition = Vector2.zero;

        public void Init(Effects e)
        {
            editor = EditorWindow.GetWindow<EffectEditorWindow>();
            effects = e;
            effectsEditor = CreateInstance<EffectsEditor>();
            effectsEditor.Repaint = Repaint;
            effectsEditor.BeginWindows = BeginWindows;
            effectsEditor.EndWindows = EndWindows;
            effectsEditor.Init(e);

            EffectEditorFactory.Intance.ResetInstance();

        }

        public void Init(EffectsController e)
        {
            Init(e.getEffectsDirectly());
        }

        void OnGUI()
        {
            if (effects == null) {
                Close(); 
				DestroyImmediate (this);
				return;
			}
            this.wantsMouseMove = true;

            GUILayout.BeginVertical(GUILayout.Height(20));
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("New Effect"))
            {
                var names = EffectEditorFactory.Intance.CurrentEffectEditors;
                if(names.Length > 0)
                {
                    Controller.Instance.ShowInputDialog(TC.get("Effects.SelectEffectType"), TC.get("Effects.SelectEffectType"),
                       EffectEditorFactory.Intance.CurrentEffectEditors, (sender, selected) =>
                       {
                           effects.Add(EffectEditorFactory.Intance.createEffectEditorFor(selected).Effect);
                           Repaint();
                       });
                }
                else
                {
                    EditorUtility.DisplayDialog("Cant create", "No effects available!", "Ok");
                }
                    
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            
            effectsEditor.OnInspectorGUI();
        }
    }
}