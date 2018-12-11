using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class EffectsEditor : CollapsibleGraphEditor<Effects, IEffect>
    {
        private static readonly Vector2 initialSize = new Vector2(200, 50);
        private static GUIContent buttonContent = new GUIContent();
        private readonly Dictionary<IEffect, EffectEditor> editors = new Dictionary<IEffect, EffectEditor>();
        private GUIStyle conditionStyle, eitherConditionStyle;
        private readonly Dictionary<string, Texture2D> icons = new Dictionary<string, Texture2D>();

        public override void Init(Effects content)
        {
            if (conditionStyle == null)
            {
                conditionStyle = new GUIStyle(GUI.skin.box);
                conditionStyle.normal.background = TextureUtil.MakeTex(1, 1, new Color(0.627f, 0.627f, 0.627f));
            }

            if (eitherConditionStyle == null)
            {
                eitherConditionStyle = new GUIStyle(GUI.skin.box);
                eitherConditionStyle.normal.background = TextureUtil.MakeTex(1, 1, new Color(0.568f, 0.568f, 0.568f));
                eitherConditionStyle.padding.left = 15;
            }

            base.Init(content);
        }

        protected override IEffect[] ChildsFor(Effects Content, IEffect parent)
        {
            var index = Content.FindIndex(parent.Equals);
            return index == Content.Count - 1 ? new IEffect[0] : new IEffect[1] { Content[index + 1] };
        }

        protected override void DeleteNode(Effects content, IEffect node)
        {
            content.Remove(node);
            if (Selection.Contains(node))
            {
                Selection.Remove(node);
            }
        }

        protected override void DrawOpenNodeContent(Effects content, IEffect node)
        {
            var abstractEffect = node as AbstractEffect;

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
                
                var conditions = abstractEffect.getConditions();
                ConditionEditorWindow.LayoutConditionEditor(conditions);

                //##################################################################################

                GUILayout.EndVertical();
            }
            var prevLabelSize = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 75;
            if (editors.ContainsKey(node))
            {
                editors[node].draw();
            }
            EditorGUIUtility.labelWidth = prevLabelSize;
        }

        protected override IEffect[] GetNodes(Effects Content)
        {
            return Content.ToArray();
        }

        private bool CreateAndInitEffectEditor(Effects content, IEffect node)
        {
            var editor = EffectEditorFactory.Intance.createEffectEditorFor(node);
            if (editor == null)
            {
                Debug.LogWarning("No effect editor available for: " + node.getType().ToString());
                return false;
            }

            if (editor.manages(node))
            {
                editor.Effect = node;
            }
            editor.Window = new Rect(new Vector2(50, 50), initialSize);

            var index = content.IndexOf(node);
            if (index > 0)
            {
                var pos = editor.Window;
                var parentPos = editors[content[index - 1]].Window;
                pos.position = parentPos.position + new Vector2(parentPos.size.x + 50, 0);
                editor.Window = pos;
            }

            editors[node] = editor;
            return true;
        }

        protected override Rect GetOpenedNodeRect(Effects content, IEffect node)
        {
            if (!editors.ContainsKey(node) && !CreateAndInitEffectEditor(content, node))
            {
                return Rect.zero;
            }

            return editors[node].Window;
        }

        protected override string GetTitle(Effects Content, IEffect node)
        {
            return node.getType().ToString();
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

        protected override void DrawNodeControls(Effects content, IEffect node)
        {
            EffectEditor editor = null;
            editors.TryGetValue(node, out editor);

            string[] editorNames = EffectEditorFactory.Intance.CurrentEffectEditors;
            int preEditorSelected = EffectEditorFactory.Intance.EffectEditorIndex(node);
            int editorSelected = EditorGUILayout.Popup(preEditorSelected, editorNames);


            if (preEditorSelected != editorSelected)
            {
                editor = EffectEditorFactory.Intance.createEffectEditorFor(editorNames[editorSelected]);
                editor.Window = editors[node].Window;
                editors[editor.Effect] = editor;
                collapsedState[editor.Effect] = collapsedState[node];
                collapsedState.Remove(node);

                if (editor.manages(node))
                {
                    editor.Effect = node;
                }
                else
                {
                    // Re insert the conditions
                    var abstractEffect = node as AbstractEffect;
                    if (abstractEffect != null && editor.Effect is AbstractEffect)
                    {
                        (editor.Effect as AbstractEffect).setConditions(abstractEffect.getConditions());
                    }
                    // Replace the effect
                    content[content.IndexOf(node)] = editor.Effect;
                }
            }

            base.DrawNodeControls(content, node);
        }

        protected override void SetNodeChild(Effects content, IEffect node, int slot, IEffect child)
        {
            var index = content.IndexOf(node) + 1;
            var childIndex = content.IndexOf(child);
            while (index < childIndex)
            {
                content.RemoveAt(index);
                ++index;
            }
        }

        protected override void SetNodePosition(Effects content, IEffect node, Vector2 position)
        {
            if (!editors.ContainsKey(node))
            {
                return;
            }

            var rect = editors[node].Window;
            rect.position = position;
            editors[node].Window = rect;
        }

        protected override void SetNodeSize(Effects content, IEffect node, Vector2 size)
        {
            if (!editors.ContainsKey(node))
            {
                return;
            }

            var rect = editors[node].Window;
            rect.size = size;
            editors[node].Window = rect;
        }
    }

    public class EffectEditorWindow : EditorWindow
    {
        private EffectsEditor effectsEditor;
        private Effects effects;

        public Vector2 scrollPosition = Vector2.zero;
        private GUIContent addButton;

        public void Init(Effects e)
        {
            var addTex = Resources.Load<Texture2D>("EAdventureData/img/icons/addNode");
            addButton = new GUIContent(addTex);

            EditorWindow.GetWindow<EffectEditorWindow>();
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

        protected void OnGUI()
        {
            if (effects == null) {
                Close(); 
				DestroyImmediate (this);
				return;
			}
            this.wantsMouseMove = true;

            var buttonRect = new Rect(position.width - 100, position.height - 100, 80, 80);
            var lastEvent = Event.current.type;
            var eventUsed = false;
            if (buttonRect.Contains(Event.current.mousePosition)
                && Event.current.type != EventType.Repaint
                && Event.current.type != EventType.Layout)
            {
                eventUsed = true;
                Event.current.Use();
            }

            effectsEditor.OnInspectorGUI();

            if (eventUsed)
            {
                Event.current.type = lastEvent; 
            }

            
            if (GUI.Button(buttonRect, addButton))
            {
                var names = EffectEditorFactory.Intance.CurrentEffectEditors;
                if (names.Length > 0)
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
        }
    }
}