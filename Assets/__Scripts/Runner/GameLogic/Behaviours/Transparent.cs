using UnityEngine;
using System.Collections;

namespace uAdventure.Runner
{
    public class Transparent : MonoBehaviour
    {

        bool checkingTransparency = false;
        Interactuable interactuable;
        private Renderer m_renderer;

        void Start()
        {
            interactuable = this.GetComponent<Interactuable>();
            m_renderer = GetComponent<Renderer>();
        }

        void Update()
        {
            if (checkingTransparency)
            {
                RaycastHit hit;
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);

                if (((Texture2D)m_renderer.material.mainTexture).GetPixelBilinear(hit.textureCoord.x, hit.textureCoord.y).a > 0f)
                {
                    interactuable.setInteractuable(true);
                    GUIManager.Instance.showHand(true);
                }
                else
                {
                    interactuable.setInteractuable(false);
                    GUIManager.Instance.showHand(false);
                }
            }
        }

        void OnMouseEnter()
        {
            Debug.Log("MouseEnter");
            checkingTransparency = true;
        }

        void OnMouseExit()
        {
            checkingTransparency = false;
            interactuable.setInteractuable(false);
            GUIManager.Instance.showHand(false);
        }
    }
}