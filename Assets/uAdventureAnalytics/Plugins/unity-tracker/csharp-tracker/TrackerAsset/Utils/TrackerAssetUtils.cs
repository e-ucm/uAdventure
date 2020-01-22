using System;
using System.Collections.Generic;


namespace AssetPackage.Utils{
    using AssetPackage;
    using AssetPackage.Exceptions;

    public class TrackerAssetUtils{

        TrackerAsset Tracker { get; set; }

        public TrackerAssetUtils(TrackerAsset tracker)
        {
            this.Tracker = tracker;
        }
		
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

        public static bool quickCheckExtension(string key, System.Object value)
        {
            return quickCheck(key) && quickCheck(value);
        }


        public bool checkExtension(string key, System.Object value){
			return 
				check<KeyExtensionException>(key, "Tracker: Extension key is null or empty. Ignored extension.", "Tracker: Extension key is null or empty.")
				&&
				check<ValueExtensionException>(value, "Tracker: Extension value is null or empty. Ignored extension.", "Tracker: Extension value is null or empty.");
		}

        public static bool quickCheck(System.Object value)
        {
            return !(value == null
                || (value.GetType() == typeof(string) && ((string)value) == "")
                || (value.GetType() == typeof(float) && float.IsNaN((float)value)));
        }

        public bool check<T>(System.Object value, string message, string strict_message) where T : TrackerException{
            bool r = quickCheck(value);

            if(!r)
				notify<T> (message, strict_message);

			return r;
		}

		public bool isFloat<T>(string value, string message, string strict_message, out float result) where T : TrackerException{
			if (!float.TryParse (value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out result)) {
				notify<T> (message, strict_message);
				return false;
			} else {
				return true;
			}
		}

		public bool isBool<T>(string value, string message, string strict_message, out bool result) where T : TrackerException{
			if (!bool.TryParse (value, out result)) {
				notify<T> (message, strict_message);
				return false;
			} else {
				return true;
			}
		}

		public void notify<T>(string message, string strict_message) where T : TrackerException{
			if (Tracker.StrictMode) {
				throw (T)Activator.CreateInstance (typeof(T), strict_message);
			} else {
				Tracker.Log(Severity.Warning, message);
			}
		}

        public static bool TryParseEnum<T>(string text, out T value)
        {
            bool ret = true;
            value = (T) Enum.GetValues(typeof(T)).GetValue(0);

            try {
                value = (T) Enum.Parse(typeof(T), text, true);
            }catch(Exception e) {
                ret = false;
            }

            return ret;
        }
    }
}