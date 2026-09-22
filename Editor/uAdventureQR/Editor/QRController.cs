using uAdventure.Editor;

namespace uAdventure.QR
{
    public class QRController
    {
        private static QRController instance;
        public static QRController Instance
        {
            get { return instance ?? (instance = new QRController()); }
        }
        private ChapterDataControl lastSelectedChapterDataControl;
        private ListDataControl<ChapterDataControl, QRCodeDataControl> qrCodes;


        public ListDataControl<ChapterDataControl, QRCodeDataControl> QRCodes
        {
            get
            {
                UpdateChapter();
                return qrCodes;
            }
        }

        public int SelectedQRCode { get; set; }

        private QRController()
        {
            UpdateChapter();
        }

        private void UpdateChapter()
        {
            if (Controller.Instance.SelectedChapterDataControl != null && lastSelectedChapterDataControl != Controller.Instance.SelectedChapterDataControl)
            {
                // QRCodeslist list manages only QRCodes
                qrCodes = new ListDataControl<ChapterDataControl, QRCodeDataControl>(
                    Controller.Instance.SelectedChapterDataControl,
                    Controller.Instance.SelectedChapterDataControl.getObjects<QR>(),
                    new ListDataControl<ChapterDataControl, QRCodeDataControl>.ElementFactoryView
                    {
                        Titles = { { 3428323, "QR.Create.Title.QRCode" } },
                        DefaultIds = { { 3428323, "QRCode" } },
                        Errors = { { 3428323, "QR.Create.Error.QRCode" } },
                        Messages = { { 3428323, "QR.Create.Message.QRCode" } },
                        ElementFactory = new DefaultElementFactory<QRCodeDataControl>(
                            new DefaultElementFactory<QRCodeDataControl>.ElementCreator()
                            {
                                CreateDataControl = qr => new QRCodeDataControl(qr as QR),
                                CreateElement = (type, id, _) => new QRCodeDataControl(new QR(id)),
                                TypeDescriptors = new[]
                                {
                                    new DefaultElementFactory<QRCodeDataControl>.ElementCreator.TypeDescriptor
                                    {
                                        Type = 3428323,
                                        ContentType = typeof(QR),
                                        RequiresId = true
                                    }
                                }
                            })
                    });
                Controller.Instance.SelectedChapterDataControl.RegisterExtraDataControl(qrCodes);
                SelectedQRCode = -1;
                

                lastSelectedChapterDataControl = Controller.Instance.SelectedChapterDataControl;
                Controller.Instance.updateVarFlagSummary();
            }
        }
    }
}
