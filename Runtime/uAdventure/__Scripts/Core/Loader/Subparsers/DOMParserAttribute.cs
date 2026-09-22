using UnityEngine;
using System.Collections;
using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DOMParserAttribute : Attribute {

    public string[] Names { get; private set; }
    public Type[] Types { get; private set; }

    public DOMParserAttribute(params string[] names)
    {
        Names = names;
        Types = new Type[0];
    }

    public DOMParserAttribute(params Type[] types)
    {
        Types = types;
        Names = new string[0];
    }

}
