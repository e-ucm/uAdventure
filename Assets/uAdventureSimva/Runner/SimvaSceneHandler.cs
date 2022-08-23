using Simva;
using uAdventure.Core;
using uAdventure.Runner;
using UnityEngine;

namespace uAdventure.Simva
{
    [ChapterTargetFactory(typeof(SimvaScene), typeof(LoginScene), typeof(SurveyScene), typeof(FinalizeScene), typeof(EndScene))]
    public class SimvaSceneHandler : MonoBehaviour, IChapterTargetFactory
    {
        public IRunnerChapterTarget Instantiate(IChapterTarget modelObject)
        {
            GameObject form = SimvaSceneManager.LoadPrefabScene(modelObject.getId());

            if (form != null)
            {
                var runner = form.AddComponent<SimvaSceneWrapper>();
                runner.SimvaSceneController = form.GetComponent<SimvaSceneController>();
                runner.Data = modelObject;
                return runner;
            }

            return null;
        }
    }
}
