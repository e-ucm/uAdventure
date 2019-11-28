using uAdventure.Editor;

namespace uAdventure.Unity
{
    public class UnityScenePluginController
    {
        private static UnityScenePluginController instance;
        public static UnityScenePluginController Instance
        {
            get { return instance ?? (instance = new UnityScenePluginController()); }
        }
        private ChapterDataControl lastSelectedChapterDataControl;
        private ListDataControl<ChapterDataControl, UnitySceneDataControl> unityScenes;


        public ListDataControl<ChapterDataControl, UnitySceneDataControl> UnityScenes
        {
            get
            {
                UpdateChapter();
                return unityScenes;
            }
        }

        public int SelectedUnityScene { get; set; }

        private UnityScenePluginController()
        {
            UpdateChapter();
        }

        private void UpdateChapter()
        {
            if (Controller.Instance.SelectedChapterDataControl != null && lastSelectedChapterDataControl != Controller.Instance.SelectedChapterDataControl)
            {
                // QRCodeslist list manages only QRCodes
                unityScenes = new ListDataControl<ChapterDataControl, UnitySceneDataControl>(
                    Controller.Instance.SelectedChapterDataControl,
                    Controller.Instance.SelectedChapterDataControl.getObjects<UnityScene>(),
                    new ListDataControl<ChapterDataControl, UnitySceneDataControl>.ElementFactoryView
                    {
                        Titles = { { 3428323, "UnityPlugin.Create.Title.UnityScene" } },
                        DefaultIds = { { 3428323, "UnityScene" } },
                        Errors = { { 3428323, "UnityPlugin.Create.Error.UnityScene" } },
                        Messages = { { 3428323, "UnityPlugin.Create.Message.UnityScene" } },
                        ElementFactory = new DefaultElementFactory<UnitySceneDataControl>(
                            new DefaultElementFactory<UnitySceneDataControl>.ElementCreator()
                            {
                                CreateDataControl = unityScene => new UnitySceneDataControl(unityScene as UnityScene),
                                CreateElement = (type, id, _) => new UnitySceneDataControl(new UnityScene { Id = id }),
                                TypeDescriptors = new[]
                                {
                                    new DefaultElementFactory<UnitySceneDataControl>.ElementCreator.TypeDescriptor
                                    {
                                        Type = 3428323,
                                        ContentType = typeof(UnityScene),
                                        RequiresId = true
                                    }
                                }
                            })
                    });

                Controller.Instance.SelectedChapterDataControl.RegisterExtraDataControl(unityScenes);
                SelectedUnityScene = -1;


                lastSelectedChapterDataControl = Controller.Instance.SelectedChapterDataControl;
                Controller.Instance.updateVarFlagSummary();
            }
        }
    }
}
