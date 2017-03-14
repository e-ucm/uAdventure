using System;

namespace uAdventure.Core
{
	public static class ExParsers {

		public static long ParseDefault(string toParse, long def){
			return string.IsNullOrEmpty (toParse) ? def : long.Parse (toParse);
		}

		public static int ParseDefault(string toParse, int def){
			return string.IsNullOrEmpty (toParse) ? def : int.Parse (toParse);
		}

		public static float ParseDefault(string toParse, System.IFormatProvider format, float def){
			return string.IsNullOrEmpty (toParse) ? def : float.Parse (toParse, format);
		}

		public static float ParseDefault(string toParse, float def){
			return string.IsNullOrEmpty (toParse) ? def : float.Parse (toParse);
		}

		public static double ParseDefault(string toParse, double def){
			return string.IsNullOrEmpty (toParse) ? def : double.Parse (toParse);
		}
	}
}