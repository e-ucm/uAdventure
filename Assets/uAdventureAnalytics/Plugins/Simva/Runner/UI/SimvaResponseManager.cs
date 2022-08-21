using uAdventure.Runner;
using uAdventure.Simva;
using UnityEngine;
using UnityEngine.UI;

namespace Simva
{
    public class SimvaResponseManager : MonoBehaviour
    {
        public Text Response;

        protected void Start()
        {
            GameExtension.GetInstance<SimvaExtension>().AddResponseManager(this);
        }

        protected void OnDestroy()
        {
            GameExtension.GetInstance<SimvaExtension>().RemoveResponseManager(this);
        }

        public void Notify(string message)
        {
            Response.text = message;
        }
    }
}
