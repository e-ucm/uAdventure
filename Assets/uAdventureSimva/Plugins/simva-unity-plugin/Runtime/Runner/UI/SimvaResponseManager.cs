using UnityEngine;
using UnityEngine.UI;

namespace Simva
{
    public class SimvaResponseManager : MonoBehaviour
    {
        public Text Response;

        protected void Start()
        {
            SimvaManager.Instance.AddResponseManager(this);
        }

        protected void OnDestroy()
        {
            SimvaManager.Instance.RemoveResponseManager(this);
        }

        public void Notify(string message)
        {
            Response.text = message;
        }
    }
}
