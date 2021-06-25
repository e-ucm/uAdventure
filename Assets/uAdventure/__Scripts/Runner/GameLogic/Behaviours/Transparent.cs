using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace uAdventure.Runner
{
    public class Transparent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Interactuable interactuable;
        private Renderer m_renderer;
        private Dictionary<int, bool> isReadable;
        private Dictionary<int, Texture2D> readableVersions;

        private void Awake()
        {
            isReadable = new Dictionary<int, bool>();
            readableVersions = new Dictionary<int, Texture2D>();
        }

        void Start()
        {
            interactuable = GetComponent<Interactuable>();
            m_renderer = GetComponent<Renderer>();
        }

        public bool CheckTransparency(RaycastHit raycastHit)
        {
            if (!m_renderer || !m_renderer.material || !m_renderer.material.mainTexture)
                return false;

            var texture = GetReadableTexture((Texture2D) m_renderer.material.mainTexture);

            var result = true;
            
            if (texture.GetPixelBilinear(raycastHit.textureCoord.x, raycastHit.textureCoord.y).a > 0f)
            {
                result = false;
            }

            return result;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            GUIManager.Instance.ShowHand(true);
            interactuable.setInteractuable(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            GUIManager.Instance.ShowHand(false);
            interactuable.setInteractuable(false);
        }

        private Texture2D GetReadableTexture(Texture2D texture)
        {
            var textureId = texture.GetInstanceID();

            if (isReadable.ContainsKey(textureId))
                return texture;
            else if (readableVersions.ContainsKey(textureId))
                return readableVersions[textureId];
            else
            {
                try
                {
                    texture.GetPixel(0, 0);
                    isReadable.Add(textureId, true);
                }
                catch
                {
                    isReadable.Add(textureId, false);
                    texture = CreateReadableTexture(texture);
                    readableVersions.Add(textureId, texture);
                }
            }

            return texture;
        }

        public static Texture2D CreateReadableTexture(Texture2D texture)
        {
            Debug.Log("Generated Readable: " + texture.name);
            // Create a temporary RenderTexture of the same size as the texture
            RenderTexture tmp = RenderTexture.GetTemporary(
                                texture.width,
                                texture.height,
                                0,
                                RenderTextureFormat.Default,
                                RenderTextureReadWrite.Linear);

            // Blit the pixels on texture to the RenderTexture
            Graphics.Blit(texture, tmp);
            // Backup the currently set RenderTexture
            RenderTexture previous = RenderTexture.active;
            // Set the current RenderTexture to the temporary one we created
            RenderTexture.active = tmp;
            // Create a new readable Texture2D to copy the pixels to it
            Texture2D readableVersion = new Texture2D(texture.width, texture.height);
            // Copy the pixels from the RenderTexture to the new Texture
            readableVersion.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            readableVersion.Apply();
            // Reset the active RenderTexture
            RenderTexture.active = previous;
            // Release the temporary RenderTexture
            tmp.Release();
            RenderTexture.ReleaseTemporary(tmp);

            return readableVersion;
        }
    }
}