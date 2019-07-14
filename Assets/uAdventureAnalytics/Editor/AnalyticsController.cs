

using uAdventure.Core;
using uAdventure.Editor;

namespace uAdventure.Analytics
{
    public class AnalyticsController
    {
        /**
         * Id for Completables
         */

        public const int COMPLETABLE = 67;

        public const int MILESTONE = 68;

        public const int PROGRESS = 69;

        public const int SCORE = 70;

        private static AnalyticsController instance;
        public static AnalyticsController Instance
        {
            get { return instance ?? (instance = new AnalyticsController()); }
        }

        private ChapterDataControl lastSelectedChapterDataControl;
        private CompletableListDataControl completableListDataControl;
        private readonly TrackerConfigDataControl trackerConfigDataControl;

        public CompletableListDataControl Completables
        {
            get
            {
                UpdateChapter();
                return completableListDataControl;
            }
        }

        /**
         * @return the trackerConfigDataControl
         */
        public TrackerConfigDataControl TrackerConfig
        {
            get { return trackerConfigDataControl; }
        }

        public int SelectedGeoElement { get; set; }
        public int SelectedMapScene { get; set; }

        private AnalyticsController()
        {
            var trackerConfigs = Controller.Instance.AdventureData.getAdventureData().getObjects<TrackerConfig>();
            if (trackerConfigs.Count == 0)
            {
                trackerConfigs.Add(new TrackerConfig());
            }
            trackerConfigDataControl = new TrackerConfigDataControl(trackerConfigs[0]);
            UpdateChapter();
        }

        private void UpdateChapter()
        {
            if (Controller.Instance.SelectedChapterDataControl != null && lastSelectedChapterDataControl != Controller.Instance.SelectedChapterDataControl)
            {
                completableListDataControl = new CompletableListDataControl(Controller.Instance.SelectedChapterDataControl.getObjects<Completable>());
                Controller.Instance.SelectedChapterDataControl.RegisterExtraDataControl(completableListDataControl);

                lastSelectedChapterDataControl = Controller.Instance.SelectedChapterDataControl;
                Controller.Instance.updateVarFlagSummary();
            }
        }
    }
}
