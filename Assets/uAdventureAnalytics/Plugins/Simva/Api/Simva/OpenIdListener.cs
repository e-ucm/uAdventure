using UnityEngine;

namespace Simva
{
    public class OpenIdListener : MonoBehaviour
    {

        public delegate void OnAuthReceived(string query);
        public OnAuthReceived onAuthReceived;

        public void OnAuthReply(string query)
        {
            onAuthReceived?.Invoke(query);
        }

    }
}
