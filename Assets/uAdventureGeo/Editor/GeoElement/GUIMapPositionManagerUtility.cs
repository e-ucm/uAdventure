using System.Collections.Generic;
using System.Linq;
using uAdventure.Editor;

namespace uAdventure.Geo
{

    public static class GuiMapPositionManagerUtility
    {
        public static void InsertDefaults(Dictionary<string, ParameterDescription> parameterDescriptions, TransformManagerDataControl into, bool overriding)
        {
            foreach (var param in parameterDescriptions)
            {
                if (!into.ContainsParameter(param.Key) || overriding)
                {
                    into[param.Key] = param.Value.DefaultValue;
                }
            }
        }

        public static T GetValue<T>(this TransformManagerDataControl transformManagerDataControl, T @default, params string[] names)
        {
            return transformManagerDataControl.GetValue(@default, null, names);
        }

        public static T GetValue<T>(this TransformManagerDataControl transformManagerDataControl, T @default, Converter<T>[] converters, params string[] names)
        {
            var value = names.Where(n => transformManagerDataControl.ContainsParameter(n))
                .Select(n => transformManagerDataControl[n]).DefaultIfEmpty(@default).First();

            if (value is T)
            {
                return (T)value;
            }

            if (converters != null)
            {
                var validConverter = converters.Where(c => c.OriginType == value.GetType());
                if (validConverter.Any())
                {
                    return validConverter.First().Convert(value);
                }
            }

            return @default;
        }
    }
}
