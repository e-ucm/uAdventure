using UnityEngine;
using UnityEngine.UI;

namespace uAdventure.Runner
{
    public class ItemParticle : MonoBehaviour
    {

        [SerializeField]
        private Image image;

        public Sprite Image { get { return image.sprite; } set { image.sprite = value; } }

        public void OnAnimationFinished()
        {
            Destroy(this.gameObject);
        }
    }
}