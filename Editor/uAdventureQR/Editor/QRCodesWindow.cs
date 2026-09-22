using UnityEngine;

using uAdventure.Editor;
using UnityEditorInternal;

using System.Linq;
using uAdventure.Core;

namespace uAdventure.QR
{
    [EditorWindowExtension(130, typeof(QRCodeDataControl))]
    public class QRCodesWindow : PreviewDataControlExtension
    {
        public enum QRCodesWindows
        {
            QR
        }

        /* ----------------------------------
         * GUI ELEMENTS
         * -----------------------------------*/

        private readonly QRCodeEditorWindow qrCodeEditorWindow;

        public QRCodesWindow(Rect aStartPos, GUIStyle aStyle, params GUILayoutOption[] aOptions) : base(aStartPos, new GUIContent("QR.Title".Traslate()), aStyle, aOptions)
        {
            var _ = QRController.Instance;
            ButtonContent = new GUIContent()
            {
                image = Resources.Load<Texture2D>("qr"),
                text = "QR.Title".Traslate()
            };

            qrCodeEditorWindow = new QRCodeEditorWindow(aStartPos, aStyle, aOptions);

            AddTab(TC.get("QR.QRCodeEditorWindow.Title"), QRCodesWindows.QR, qrCodeEditorWindow);
            
        }

        /* ----------------------------------
          * ON GUI: Used for drawing the window every unity event
          * ----------------------------------*/


        protected override void OnSelect(ReorderableList r)
        {
            QRController.Instance.SelectedQRCode = r.index;
        }


        protected override void OnButton()
        {
            base.OnButton();
            dataControlList.SetData(QRController.Instance.QRCodes,
                qrCodesList => (qrCodesList as ListDataControl<ChapterDataControl, QRCodeDataControl>).DataControls.Cast<DataControl>().ToList());
            QRController.Instance.SelectedQRCode = -1;
        }

        protected override void OnDrawMainPreview(Rect rect, int index)
        {
            var qrDataControl = dataControlList.list[index] as QRCodeDataControl;
            qrCodeEditorWindow.Target = qrDataControl;
            qrCodeEditorWindow.DrawPreview(rect);
            qrCodeEditorWindow.Target = null;
        }
    }
}