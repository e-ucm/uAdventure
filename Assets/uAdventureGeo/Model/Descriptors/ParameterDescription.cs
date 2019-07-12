using System;
using System.Collections.Generic;

namespace uAdventure.Geo
{

    /// <summary>
    /// Parameter description for GeoReferenceTransformManager Parameters
    /// </summary>
    public class ParameterDescription
    {
        public ParameterDescription(Type type, object defaultValue) : this(type, defaultValue, null, null, null) { }
        public ParameterDescription(Type type, object defaultValue, object minValue, object maxValue) : this(type, defaultValue, minValue, maxValue, null) { }
        public ParameterDescription(Type type, object defaultValue, List<object> allowedValues) : this(type, defaultValue, null, null, allowedValues) { }
        public ParameterDescription(Type type, object defaultValue, object minValue, object maxValue, List<object> allowedValues)
        {
            this.Type = type;
            this.MinValue = minValue;
            this.MaxValue = maxValue;
            this.AllowedValues = allowedValues;
            this.DefaultValue = defaultValue;
        }

        public Type Type { get; set; }
        public object MinValue { get; set; }
        public object MaxValue { get; set; }
        public List<object> AllowedValues { get; set; }
        public object DefaultValue { get; set; }
    }
}
