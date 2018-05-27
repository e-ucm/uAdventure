using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Runner
{
    public class BorderGenerator
    {
        private static Texture2D transparent;

        public static Texture2D generateFor(NPC npc)
        {
            Texture2D ret;
            if (npc.getShowsSpeechBubbles())
            {
                ret = new Texture2D(32, 32);
                //Color background = Color.white;
                // TODO use background
                //ColorUtility.TryParseHtmlString(npc.getBubbleBkgColor(), out background);

                BorderGenerator.Circle(ret, 16, 16, 15, npc.getBubbleBorderColor());
            }
            else
            {
                if (transparent == null)
                {
                    transparent = Resources.Load("1x1") as Texture2D;
                }
                ret = transparent;
            }

            return ret;
        }

        private static void Circle(Texture2D tex, int cx, int cy, int r, Color col)
        {
            int x, y, px, nx, py, ny, d;

            for (x = 0; x <= r; x++)
            {
                d = (int)Mathf.Ceil(Mathf.Sqrt(r * r - x * x));
                for (y = 0; y <= d; y++)
                {
                    px = cx + x;
                    nx = cx - x;
                    py = cy + y;
                    ny = cy - y;

                    tex.SetPixel(px, py, col);
                    tex.SetPixel(nx, py, col);

                    tex.SetPixel(px, ny, col);
                    tex.SetPixel(nx, ny, col);

                }
            }
        }
    }
}