using UnityEngine;
using UnityEngine.UI;

namespace uAdventure.Analytics
{
    public class SimvaResponseManager : MonoBehaviour
    {
        public Text Response;

        protected void Start()
        {
            SimvaController.Instance.AddResponseManager(this);
        }

        protected void OnDestroy()
        {
            SimvaController.Instance.RemoveResponseManager(this);
        }

        public void Notify(string message)
        {
            Response.text = message;
        }
    }
}
