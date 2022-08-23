using UnityEngine;

namespace Simva
{
    public static class SimvaSceneManager
    {
        public static GameObject LoadPrefabScene(string name)
        {
            GameObject form = null;
            switch (name)
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
            return form;
        }
    }
}

