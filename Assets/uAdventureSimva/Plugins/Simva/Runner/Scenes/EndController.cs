using UnityEngine;

namespace Simva
{
    // Manager for "Simva.End"
    public class EndController : SimvaSceneController
    {
        public void Quit()
        {
            Application.Quit();
        }

        public override void Render()
        {
            Ready = true;
        }

        public override void Destroy()
        {
            GameObject.DestroyImmediate(this.gameObject);
        }
    }
}

