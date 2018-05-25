using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    public class ColorConverter : MonoBehaviour
    {

        public static string ColorToHex(Color32 color)
        {
            string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return "#" + hex;
        }

        public static Color HexToColor(string hex)
        {
            hex = hex.Replace("#", "");

            while (hex.Length < 6)
            {
                hex += "0";
            }

            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }
    }
}
