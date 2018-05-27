using UnityEngine;
using System.Collections;

namespace uAdventure.Runner
{
    public class AutoGlower : MonoBehaviour
    {

        public enum GlowingPhase { GETTING_BIGGER, MOVING, GETTING_SMALLER, WAITING };
        public enum GlowingStyle { FLASH, FADE_AND_FLASH, FADE };

        public GlowingStyle style = GlowingStyle.FLASH;
        private GlowingPhase phase = GlowingPhase.WAITING;

        Material shader;
        void Start()
        {
            shader = this.GetComponent<Renderer>().material;
            if (style == GlowingStyle.FLASH)
                shader.SetColor("_Color", new Color(1, 1, 1, 1));
        }


        float size = 0f;
        float pos = 0f;
        float fade = 0f;

        public float speed = 5f;
        private float time_since_last_glow = 0f;

        private float flashduration = 2f;
        void Update()
        {

            switch (phase)
            {
                case GlowingPhase.GETTING_BIGGER:
                    size += Time.deltaTime * flashduration;

                    if (style == GlowingStyle.FADE_AND_FLASH || style == GlowingStyle.FADE)
                    {
                        fade += Time.deltaTime / 2;
                    }

                    if (size >= 0.5f)
                    {
                        size = 0.5f;
                        phase = GlowingPhase.MOVING;
                    }
                    break;
                case GlowingPhase.MOVING:
                    pos += Time.deltaTime * flashduration;
                    if (pos >= 1f)
                    {
                        pos = 1f;
                        phase = GlowingPhase.GETTING_SMALLER;
                    }
                    break;
                case GlowingPhase.GETTING_SMALLER:
                    size -= Time.deltaTime * flashduration;

                    if (style == GlowingStyle.FADE_AND_FLASH || style == GlowingStyle.FADE)
                    {
                        fade -= Time.deltaTime / 2;
                    }

                    if (size <= 0f)
                    {
                        size = 0f;
                        phase = GlowingPhase.WAITING;
                    }
                    break;
                case GlowingPhase.WAITING:
                    pos = 0f;
                    size = 0f;
                    if (style == GlowingStyle.FADE_AND_FLASH || style == GlowingStyle.FADE)
                    {
                        fade = 0f;
                    }

                    time_since_last_glow += Time.deltaTime;
                    if (time_since_last_glow > speed)
                    {
                        time_since_last_glow = 0f;
                        phase = GlowingPhase.GETTING_BIGGER;
                    }

                    break;
            }

            shader.SetFloat("_ShineLocation", pos);
            shader.SetFloat("_ShineWidth", size);

            if (style == GlowingStyle.FADE_AND_FLASH || style == GlowingStyle.FADE)
            {
                shader.SetColor("_Color", new Color(1, 1, 1, fade));
            }
        }
    }
}