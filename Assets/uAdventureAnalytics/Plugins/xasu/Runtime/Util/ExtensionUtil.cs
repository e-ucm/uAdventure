using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Xasu.Util
{
    public class ExtensionUtil
    {
        public static void AddExtensionToJObject(KeyValuePair<string, object> extension, JObject jObject)
        {
            var key = new Uri(extension.Key).ToString();
            if (jObject.ContainsKey(key))
            {
                var valueType = jObject.GetValue(key).Type;
                // In case it stores the same string value
                if (valueType == JTokenType.String
                    && jObject.Value<string>(key).Equals(extension.Value.ToString()))
                {
                    return;
                }

                // In case it stores the same int value
                if (valueType == JTokenType.Integer
                    && jObject.Value<int>(key).ToString().Equals(extension.Value.ToString()))
                {
                    return;
                }
            }

            MoveToOldIfPresent(jObject, key);

            if (extension.Value is int)
            {
                jObject.Add(key, (int)extension.Value);
            }
            else
            {
                jObject.Add(key, extension.Value.ToString());
            }
        }
        private static void MoveToOldIfPresent(JObject jObject, string key)
        {
            if (jObject.ContainsKey(key))
            {
                var value = jObject.GetValue(key);
                var newKey = key + "_old/";
                jObject.Remove(key);
                MoveToOldIfPresent(jObject, newKey);
                jObject[newKey] = value;
            }
        }
    }
}
