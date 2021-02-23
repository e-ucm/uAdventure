using System;

namespace uAdventure.Core.Metadata
{
    public class LomTypeEnumAttribute : Attribute
    {
        public string Source { get; private set; }
        public Type EnumType { get; private set; }

        public LomTypeEnumAttribute(string source, Type enumType)
        {
            Source = source;
            EnumType = enumType;
        }
    }
}