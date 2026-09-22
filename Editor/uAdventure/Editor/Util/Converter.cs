using System;

namespace uAdventure.Editor
{
    public static class Converters<TO>
    {
        public static Converter<TD> Create<TD>(Func<TO, TD> converterFunc)
        {
            return new Converter<TD>(typeof(TO), (o) => converterFunc((TO)o));
        }
    }

    public class Converter<T>
    {

        private readonly Func<object, T> converterFunc;
        private readonly Type oType;

        public Converter(Type oType, Func<object, T> converterFunc)
        {
            this.oType = oType;
            this.converterFunc = converterFunc;
        }

        public Type OriginType
        {
            get
            {
                return oType;
            }
        }

        public T Convert(object value)
        {
            return converterFunc(value);
        }
    }
}