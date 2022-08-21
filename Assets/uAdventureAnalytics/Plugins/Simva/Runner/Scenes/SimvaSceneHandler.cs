using uAdventure.Core;
using uAdventure.Runner;
using UnityEngine;
using UnityEngine.EventSystems;

namespace uAdventure.Simva
{
    [ChapterTargetFactory(typeof(SimvaScene), typeof(LoginScene), typeof(SurveyScene), typeof(FinalizeScene), typeof(EndScene))]
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
                case "Simva.Finalize":
                    form = GameObject.Instantiate(Resources.Load<GameObject>("SimvaFinalize"));
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
