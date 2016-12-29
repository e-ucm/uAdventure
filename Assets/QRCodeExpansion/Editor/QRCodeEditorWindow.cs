using UnityEngine;
using System.Collections;

using uAdventure.Editor;
using System;
using UnityEditorInternal;
using UnityEditor;
using QRCoder;
using System.IO;

namespace uAdventure.QR
{
    public class QRCodeEditorWindow : ReorderableListEditorWindowExtension
    {
        private QR selectedQR;
        private Texture2D qrCodeImage;

        private Rect mm_Rect;
        public QRCodeEditorWindow(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions) : base(aStartPos, aContent, aStyle, aOptions)
        {
            mm_Rect = aStartPos;
            
            var bc = new GUIContent();
            bc.image = (Texture2D)Resources.Load("qr", typeof(Texture2D));
            bc.text = "QR Codes";  //TC.get("Element.Name1");
            ButtonContent = bc;
        }


        public override void Draw(int aID)
        {
            base.Draw(aID);

            if(selectedQR == null)
            {
                GUILayout.Label("Please select or create a new QR.", GUILayout.Width(mm_Rect.width * .99f));
                return;
            }
            EditorGUILayout.BeginVertical(GUILayout.Width(mm_Rect.width * .99f));
            {
                EditorGUI.BeginChangeCheck();
                // Delayed in order to use content to render the QR
                selectedQR.Content = EditorGUILayout.DelayedTextField("Content", selectedQR.Content);
                if (EditorGUI.EndChangeCheck())
                {
                    RegenerateQR();
                }
                EditorGUILayout.LabelField("Documentation");
                selectedQR.Documentation = EditorGUILayout.TextArea(selectedQR.Documentation, GUILayout.Height(200));

                // Conditions and Effects
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Conditions"))
                    {

                        ConditionEditorWindow window =
                        (ConditionEditorWindow)ScriptableObject.CreateInstance(typeof(ConditionEditorWindow));
                        window.Init(selectedQR.Conditions);
                    }
                    if (GUILayout.Button("Effects"))
                    {
                        EffectEditorWindow window =
                            (EffectEditorWindow)ScriptableObject.CreateInstance(typeof(EffectEditorWindow));
                        window.Init(selectedQR.Effects);
                    }
                }
                EditorGUILayout.EndHorizontal();

                // QRCODE and buttons
                EditorGUILayout.BeginHorizontal();
                {
                    var rect = EditorGUILayout.BeginVertical("preBackground", GUILayout.Width(mm_Rect.width * .75f), GUILayout.ExpandHeight(true));
                    {
                        GUILayout.BeginHorizontal();
                        {
                            var hw = Mathf.Min(rect.height * .9f, rect.width * .7f);
                            GUI.DrawTexture(new Rect(rect.x + rect.width/2f - hw/2f,rect.y + rect.height/2f - hw/2f, hw, hw), qrCodeImage);
                        }
                        GUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    {
                        if (GUILayout.Button("Save"))
                        {
                            var fileName = EditorUtility.SaveFilePanel("Save the QR code", "", "qrcode", "png");
                            if(fileName != null)
                            {
                                File.WriteAllBytes(fileName, qrCodeImage.EncodeToPNG());
                            }
                        }
                        GUILayout.Button("Print");
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

        }

        private void RegenerateQR()
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(selectedQR.Content, QRCodeGenerator.ECCLevel.Q);
            UnityQRCode qrCode = new UnityQRCode(qrCodeData);
            qrCodeImage = qrCode.GetGraphic(20);
        }

        protected override void OnAdd(ReorderableList r)
        {
            Controller.getInstance().getSelectedChapterDataControl().getObjects<QR>().Add(new QR("newQRCode"));
        }

        protected override void OnAddOption(ReorderableList r, string option){}
        protected override void OnButton()
        {
            selectedQR = null;
            reorderableList.index = -1;
        }

        protected override void OnElementNameChanged(ReorderableList r, int index, string newName)
        {
            Controller.getInstance().getSelectedChapterDataControl().getObjects<QR>()[index].Id = newName;
        }

        protected override void OnRemove(ReorderableList r)
        {
            Controller.getInstance().getSelectedChapterDataControl().getObjects<QR>().RemoveAt(r.index);
        }

        protected override void OnReorder(ReorderableList r)
        {
            throw new NotImplementedException();
        }

        protected override void OnSelect(ReorderableList r)
        {
            if(r.index == -1)
            {
                selectedQR = null;
                return;
            }

            var newSelection = Controller.getInstance().getSelectedChapterDataControl().getObjects<QR>()[r.index];
            if (selectedQR == newSelection)
            {
                selectedQR = null;
                r.index = -1;
            }
            else if(newSelection != null)
            {
                selectedQR = newSelection;
                RegenerateQR();
            }
        }

        protected override void OnUpdateList(ReorderableList r)
        {
            r.list = Controller.getInstance().getSelectedChapterDataControl().getObjects<QR>().ConvertAll(qr => qr.Id);
        }
    }
}

