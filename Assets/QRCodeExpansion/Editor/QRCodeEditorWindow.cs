using UnityEngine;
using System.Collections.Generic;

using uAdventure.Editor;
using System;
using UnityEditorInternal;
using UnityEditor;
using QRCoder;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace uAdventure.QR
{
    [EditorWindowExtension(120, typeof(QR))]
    public class QRCodeEditorWindow : ReorderableListEditorWindowExtension
    {
        private QR selectedQR;
        private Texture2D qrCodeImage;
        
        public QRCodeEditorWindow(Rect rect, GUIStyle style, params GUILayoutOption[] options) : base(rect, new GUIContent("QR Codes"), style, options)
        {            
            var bc = new GUIContent();
            bc.image = (Texture2D)Resources.Load("qr", typeof(Texture2D));
            bc.text = "QR Codes";  //TC.get("Element.Name1");
            ButtonContent = bc;
        }


        public override void Draw(int aID)
        {
            if(selectedQR == null)
            {
                GUILayout.Label("Please select or create a new QR.");
                return;
            }
            EditorGUILayout.BeginVertical();
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
                    var rect = EditorGUILayout.BeginVertical("preBackground", GUILayout.Width(Rect.width * .75f), GUILayout.ExpandHeight(true));
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
                        if (GUILayout.Button("Print"))
                        {
                            var printDialog = new PrintDialog();
                            var printDocument = new PrintDocument();
                            printDialog.Document = printDocument;
                            
                            printDocument.PrintPage += new PrintPageEventHandler((object sender, PrintPageEventArgs ev) =>
                            {
                                float leftMargin = ev.PageBounds.Left;
                                float topMargin = ev.PageBounds.Top;
                                
                                // Calculate the number of lines per page.
                                var center = new Vector2(ev.PageBounds.Width, ev.PageBounds.Height)/2;
                                var qrwh = Mathf.Min(ev.PageBounds.Width, ev.PageBounds.Height) * (3f / 4f); // Only 3/4 of the space
                                var rectangleF = new System.Drawing.RectangleF(center.x - (qrwh / 2f), center.y - (qrwh / 2f), qrwh, qrwh);

                                var qrBitmap = new System.Drawing.Bitmap(qrCodeImage.width, qrCodeImage.height);
                                Color pixelColor;
                                for (int i = 0; i < qrCodeImage.width; i++)
                                    for (int j = qrCodeImage.height -1 ; j >= 0; j--)
                                    {
                                        pixelColor = qrCodeImage.GetPixel(i, qrCodeImage.height - 1 - j);
                                        qrBitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(
                                            Mathf.RoundToInt(255 * pixelColor.a),
                                            Mathf.RoundToInt(255 * pixelColor.r),
                                            Mathf.RoundToInt(255 * pixelColor.g),
                                            Mathf.RoundToInt(255 * pixelColor.b)));
                                    }

                                ev.Graphics.DrawImage(qrBitmap, rectangleF);
                                ev.HasMorePages = false;
                            });

                            var result = printDialog.ShowDialog();
                            // If the result is OK then print the document.
                            if (result == DialogResult.OK)
                            {
                                printDocument.Print();
                            }
                        }
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
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(selectedQR.Id, QRCodeGenerator.ECCLevel.Q);
            UnityQRCode qrCode = new UnityQRCode(qrCodeData);
            qrCodeImage = qrCode.GetGraphic(20);
           // qrCodeImage = FlipTexture(qrCodeImage, false, true);
        }

        Texture2D FlipTexture(Texture2D original, bool horizontalflip, bool verticalflip)
        {
            Texture2D flipped = new Texture2D(original.width, original.height);

            int xN = original.width;
            int yN = original.height;

            int originalX = horizontalflip ? xN - 1 : 0,
                originalY = verticalflip ? yN - 1 : 0,
                Xo = horizontalflip ? -1 : 1,
                Yo = verticalflip ? -1 : 1;

            for (int i = 0; i < xN; i++)
            {
                for (int j = 0; j < yN; j++)
                {
                    flipped.SetPixel(originalX, originalY, original.GetPixel(i, j));
                    originalX += Xo; originalY += Yo;
                }
            }
            flipped.Apply();

            return flipped;
        }

        protected override void OnAdd(ReorderableList r)
        {
            Controller.Instance.SelectedChapterDataControl.getObjects<QR>().Add(new QR("newQRCode"));
        }

        protected override void OnAddOption(ReorderableList r, string option){}
        protected override void OnButton()
        {
            selectedQR = null;
            reorderableList.index = -1;
        }

        protected override void OnElementNameChanged(ReorderableList r, int index, string newName)
        {
            Controller.Instance.SelectedChapterDataControl.getObjects<QR>()[index].Id = newName;
        }

        protected override void OnRemove(ReorderableList r)
        {
            Controller.Instance.SelectedChapterDataControl.getObjects<QR>().RemoveAt(r.index);
        }

        protected override void OnReorder(ReorderableList r)
		{
			string idToMove = r.list [r.index] as string;
			var temp = Controller.Instance .SelectedChapterDataControl .getObjects<QR> ();
			QR toMove = temp.Find (qr => qr.getId () == idToMove);
			temp.Remove (toMove);
			temp.Insert (r.index, toMove);
        }

        protected override void OnSelect(ReorderableList r)
        {
            if(r.index == -1)
            {
                selectedQR = null;
                return;
            }

            var newSelection = Controller.Instance.SelectedChapterDataControl.getObjects<QR>()[r.index];
            if(newSelection != null && newSelection != selectedQR)
            {
                selectedQR = newSelection;
                RegenerateQR();
            }
        }

        protected override void OnUpdateList(ReorderableList r)
        {
            r.list = Controller.Instance.SelectedChapterDataControl.getObjects<QR>().ConvertAll(qr => qr.Id);
        }
    }
}

