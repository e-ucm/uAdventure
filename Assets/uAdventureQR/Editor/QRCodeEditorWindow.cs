using System.Collections.Generic;
using UnityEngine;

using uAdventure.Editor;
using UnityEditor;
using QRCoder;
using System.IO;
//using System.Windows.Forms;
//using System.Drawing.Printing;
using uAdventure.Core;

namespace uAdventure.QR
{
    [EditorComponent(typeof(QRCodeDataControl), Name = "QR.QRCodeEditorWindow.Title")]
    public class QRCodeEditorWindow : AbstractEditorComponentWithPreview
    {
        private const string QRCodeDefaultName = "qrcode";
        private const string QRCodeFileFormat = "png";

        private static Dictionary<QRCodeDataControl, Texture2D> qrCodeTextures = new Dictionary<QRCodeDataControl, Texture2D>();
        private static Dictionary<QRCodeDataControl, string> qrCodeIds = new Dictionary<QRCodeDataControl, string>();

        private QRCodeDataControl selectedQR;

        public QRCodeEditorWindow(Rect rect, GUIStyle style, params GUILayoutOption[] options) : base(rect,
            new GUIContent("QR.QRCodeEditorWindow.Title".Traslate()), style, options)
        {
        }

        protected override void DrawInspector()
        {
            selectedQR = QRController.Instance.QRCodes[QRController.Instance.SelectedQRCode];

            EditorGUI.BeginChangeCheck();
            // Delayed in order to use content to render the QR
            var newContent = EditorGUILayout.DelayedTextField("QR.QRCodeEditorWindow.Content".Traslate(), selectedQR.Content);
            if (EditorGUI.EndChangeCheck())
            {
                selectedQR.Content = newContent;
            }
            EditorGUILayout.LabelField("Search.Documentation".Traslate());
            selectedQR.Documentation = EditorGUILayout.TextArea(selectedQR.Documentation, GUILayout.Height(200));

            // Conditions and Effects

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Conditions.Title".Traslate()))
                {

                    ConditionEditorWindow window =
                    (ConditionEditorWindow)ScriptableObject.CreateInstance(typeof(ConditionEditorWindow));
                    window.Init(selectedQR.Conditions);
                }
                if (GUILayout.Button("Element.Effects".Traslate()))
                {
                    EffectEditorWindow window =
                        (EffectEditorWindow)ScriptableObject.CreateInstance(typeof(EffectEditorWindow));
                    window.Init(selectedQR.Effects);
                }
            }

            // QRCODE and buttons
            if (GUILayout.Button("QR.QRCodeEditorWindow.Save".Traslate()))
            {
                SaveQR(selectedQR);
            }
            if (GUILayout.Button("QR.QRCodeEditorWindow.Print".Traslate()))
            {
                PrintQR(selectedQR);
            }
        }

        public override void DrawPreview(Rect rect)
        {
            var qr = Target as QRCodeDataControl ?? selectedQR;
            if (qr == null)
            {
                return;
            }

            var hw = Mathf.Min(rect.height * .9f, rect.width * .7f);
            GUI.DrawTexture(new Rect(rect.x + rect.width / 2f - hw / 2f, rect.y + rect.height / 2f - hw / 2f, hw, hw), GetQRTexture(qr), ScaleMode.ScaleToFit);
        }

        private static void SaveQR(QRCodeDataControl qr)
        {
            var fileName = EditorUtility.SaveFilePanel("QR.QRCodeEditorWindow.Save.Title".Traslate(), "",
                QRCodeDefaultName, QRCodeFileFormat);
            if (fileName != null)
            {
                File.WriteAllBytes(fileName, GetQRTexture(qr).EncodeToPNG());
            }
        }

        private static void PrintQR(QRCodeDataControl qr)
        {
            /*var printDialog = new PrintDialog();
            var printDocument = new PrintDocument();
            printDialog.Document = printDocument;

            printDocument.PrintPage += new PrintPageEventHandler((object sender, PrintPageEventArgs ev) =>
            {
                // Calculate the number of lines per page.
                var center = new Vector2(ev.PageBounds.Width, ev.PageBounds.Height) / 2;
                var qrwh = Mathf.Min(ev.PageBounds.Width, ev.PageBounds.Height) * (3f / 4f); // Only 3/4 of the space
                var rectangleF = new System.Drawing.RectangleF(center.x - (qrwh / 2f), center.y - (qrwh / 2f), qrwh, qrwh);

                var qrCodeImage = GetQRTexture(qr);
                var qrBitmap = new System.Drawing.Bitmap(qrCodeImage.width, qrCodeImage.height);
                Color pixelColor;
                for (int i = 0; i < qrCodeImage.width; i++)
                    for (int j = qrCodeImage.height - 1; j >= 0; j--)
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
            }*/
        }

        private static Texture2D GetQRTexture(QRCodeDataControl qr)
        {
            if (!qrCodeIds.ContainsKey(qr) || qrCodeIds[qr] != qr.Id)
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qr.Id, QRCodeGenerator.ECCLevel.Q);
                UnityQRCode qrCode = new UnityQRCode(qrCodeData);
                qrCodeIds[qr] = qr.Id;
                qrCodeTextures[qr] = qrCode.GetGraphic(20);
            }

            return qrCodeTextures[qr];
        }
    }
}

