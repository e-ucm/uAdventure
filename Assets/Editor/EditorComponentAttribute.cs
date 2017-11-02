using System;


namespace uAdventure.Editor
{
    public class EditorComponentAttribute : Attribute
    {
        public Type[] Types { get; private set; }
        public EditorComponentAttribute(params Type[] t)
        {
            Types = t;
        }
        public String Name { get; set; }
        public int Order { get; set; }
    }
}