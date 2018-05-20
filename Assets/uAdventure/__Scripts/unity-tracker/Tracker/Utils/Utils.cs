using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAGE.Analytics.Exceptions;

namespace RAGE.Analytics{

	public static class Utils{
		
		public static string[] parseCSV(string trace){
			List<string> p = new List<string> ();

			bool escape = false;
			int start = 0;
			for (int i = 0; i < trace.Length; i++) {
				switch (trace [i]) {
				case '\\':
					escape = true;
					break;
				case ',':
					if (!escape) {
						p.Add (trace.Substring (start, i-start).Replace("\\,",","));
						start = i + 1;
					} else
						escape = false;
					break;
				default: break;
				}
			}
			p.Add(trace.Substring(start).Replace("\\,",","));

			return p.ToArray ();
		}

		public static bool checkExtension(string key, System.Object value){
			return 
				check<KeyExtensionException>(key, "Tracker: Extension key is null or empty. Ignored extension.", "Tracker: Extension key is null or empty.")
				&&
				check<ValueExtensionException>(value, "Tracker: Extension value is null or empty. Ignored extension.", "Tracker: Extension value is null or empty.");
		}

		public static bool check<T>(System.Object value, string message, string strict_message) where T : TrackerException{
			bool r = true;


			if (value == null 
				|| (value.GetType() == typeof(string) && ((string) value) == "") 
				|| (value.GetType() == typeof(float) && float.IsNaN((float)value))) {

				r = false;
				notify<T> (message, strict_message);
			}

			return r;
		}

		public static bool isFloat<T>(string value, string message, string strict_message, out float result) where T : TrackerException{
			if (!float.TryParse (value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out result)) {
				notify<T> (message, strict_message);
				return false;
			} else {
				return true;
			}
		}

		public static bool isBool<T>(string value, string message, string strict_message, out bool result) where T : TrackerException{
			if (!bool.TryParse (value, out result)) {
				notify<T> (message, strict_message);
				return false;
			} else {
				return true;
			}
		}

		public static void notify<T>(string message, string strict_message) where T : TrackerException{
			if (Tracker.strictMode) {
				throw (T)Activator.CreateInstance (typeof(T), strict_message);
			} else {
				Debug.LogWarning (message);
			}
		}
	}
}