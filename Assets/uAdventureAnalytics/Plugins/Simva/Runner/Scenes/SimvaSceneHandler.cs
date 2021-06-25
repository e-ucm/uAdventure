using uAdventure.Core;
using uAdventure.Runner;
using UnityEngine;
using UnityEngine.EventSystems;

namespace uAdventure.Simva
{
    [ChapterTargetFactory(typeof(SimvaScene), typeof(LoginScene), typeof(SurveyScene), typeof(FlushAllScene), typeof(BackupScene), typeof(EndScene))]
    public class SimvaSceneHandler : MonoBehaviour, IChapterTargetFactory
    {
        public IRunnerChapterTarget Instantiate(IChapterTarget modelObject)
        {
            GameObject form = null;
            switch (modelObject.getId())
            {
                case "Simva.Login":
                    form = GameObject.Instantiate(Resources.Load<GameObject>("SimvaLogin"));
                    break;
                case "Simva.Survey":
                    form = GameObject.Instantiate(Resources.Load<GameObject>("SimvaSurvey"));
                    break;
                case "Simva.FlushAll":
                    form = GameObject.Instantiate(Resources.Load<GameObject>("SimvaFlushAll"));
                    break;
                case "Simva.Backup":
                    form = GameObject.Instantiate(Resources.Load<GameObject>("SimvaBackup"));
                    break;
                case "Simva.End":
                    form = GameObject.Instantiate(Resources.Load<GameObject>("SimvaEnd"));
                    break;
            }

            if (form != null)
            {
                var runner = form.GetComponent<IRunnerChapterTarget>();
                runner.Data = modelObject;
                return runner;
            }

            return null;
        }
    }
}
