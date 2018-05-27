using UnityEngine;
using UnityEngine.SceneManagement;

namespace uAdventure.GameSelector
{
    public class SceneChangerMB : MonoBehaviour
    {
        public void ChangeScene(string name)
        {
            SceneManager.LoadScene(name);
        }
    }
}
