using System;
using System.Reflection;
using UnityEngine;

namespace uAdventure.Core
{
	public static class ExParsers {

		public static long ParseDefault(string toParse, long def){
            long toReturn;
            if (!string.IsNullOrEmpty(toParse) && long.TryParse(toParse, out toReturn))
                return toReturn;
            else
                return def;
		}

		public static int ParseDefault(string toParse, int def)
        {
            int toReturn;
            if (!string.IsNullOrEmpty(toParse) && int.TryParse(toParse, out toReturn))
                return toReturn;
            else
                return def;
		}

		public static float ParseDefault(string toParse, System.IFormatProvider format, float def){
            try
            {
                return string.IsNullOrEmpty(toParse) ? def : float.Parse(toParse, format);
            }
            catch
            {
                return def;
            }
		}

		public static float ParseDefault(string toParse, float def)
        {
            float toReturn;
            if (!string.IsNullOrEmpty(toParse) && float.TryParse(toParse, out toReturn))
                return toReturn;
            else
                return def;
        }

		public static double ParseDefault(string toParse, double def)
        {
            double toReturn;
            if (!string.IsNullOrEmpty(toParse) && double.TryParse(toParse, out toReturn))
                return toReturn;
            else
                return def;
        }

        public static RectD ParseDefault(string toParse, System.IFormatProvider format, RectD def)
        {
            RectD toReturn;
            if (!string.IsNullOrEmpty(toParse) && RectD.TryParse(toParse, out toReturn, format))
                return toReturn;
            else
                return def;
        }

        public static Vector2d ParseDefault(string toParse, Vector2d def)
        {
            Vector2d toReturn;
            if (!string.IsNullOrEmpty(toParse) && Vector2d.TryParse(toParse, out toReturn))
                return toReturn;
            else
                return def;
        }

        public static Color ParseDefault(string toParse, Color def)
        {
            try
            {
                return string.IsNullOrEmpty(toParse) ? def : ColorConverter.HexToColor(toParse);
            }
            catch
            {
                return def;
            }
        }


        public static T ParseEnum<T>(string value)
        {
            return (T)System.Enum.Parse(typeof(T), value, true);
        }

        public static T ParseDefaultEnum<T>(string value, T def)
        {
            try
            {
                return (T)System.Enum.Parse(typeof(T), value, true);
            }
            catch
            {
                return def;
            }
        }
        public static object Parse(string value, Type type)
        {
            try
            {
                var parse = type.GetMethod("Parse", BindingFlags.Static);
                if (parse != null)
                {
                    value = (string)parse.Invoke(null, new object[] { value });
                }

                return value;
            }
            catch
            {
                return null;
            }
        }

        public static T ParseDefault<T>(string value, T def)
        {
            try
            {
                var parse = typeof(T).GetMethod("Parse", BindingFlags.Static);
                if (parse != null)
                {
                    value = (string) parse.Invoke(null, new object[] { value });
                }

                return (T)System.Enum.Parse(typeof(T), value, true);
            }
            catch
            {
                return def;
            }
        }
    }

    public static class ExString
    {
        public static string Default(string original, string def)
        {
            return string.IsNullOrEmpty(original) ? def : original;
        }

        public static bool EqualsDefault(string toCompare, string to, bool def)
        {
            return string.IsNullOrEmpty(toCompare) ? def : to.Equals(toCompare);
        }
    }
}