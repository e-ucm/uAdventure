using System;
using UnityEngine;

namespace uAdventure.Core
{
	public static class ExParsers {

		public static long ParseDefault(string toParse, long def){
            long toReturn;
            if (!string.IsNullOrEmpty(toParse) && long.TryParse(toParse, out toReturn))
            {
                return toReturn;
            }
            else
            {
                return def;
            }
        }

		public static int ParseDefault(string toParse, int def)
        {
            int toReturn;
            if (!string.IsNullOrEmpty(toParse) && int.TryParse(toParse, out toReturn))
            {
                return toReturn;
            }
            else
            {
                return def;
            }
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
            {
                return toReturn;
            }
            else
            {
                return def;
            }
        }

		public static double ParseDefault(string toParse, double def)
        {
            double toReturn;
            if (!string.IsNullOrEmpty(toParse) && double.TryParse(toParse, out toReturn))
            {
                return toReturn;
            }
            else
            {
                return def;
            }
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
        public static T ParseDefault<T>(string value, T def)
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