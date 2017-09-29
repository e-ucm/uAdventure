using System;


namespace uAdventure.Editor
{
    public class EditorComponentAttribute : Attribute
    {
        public Type Type { get; private set; }
        public EditorComponentAttribute(Type t)
        {
            Type = t;
        }
        public String Name { get; set; }
        public int Order { get; set; }
    }
}