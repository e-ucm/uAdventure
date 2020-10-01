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
            SimvaExtension.Instance.AddResponseManager(this);
        }

        protected void OnDestroy()
        {
            SimvaExtension.Instance.RemoveResponseManager(this);
        }

        public void Notify(string message)
        {
            Response.text = message;
        }
    }
}
