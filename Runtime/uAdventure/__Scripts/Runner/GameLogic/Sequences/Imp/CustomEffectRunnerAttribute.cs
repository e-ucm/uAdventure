using System;

[AttributeUsage(AttributeTargets.Class)]
public class CustomEffectRunnerAttribute : Attribute {

	public CustomEffectRunnerAttribute(params Type[] types)
    {
        Types = types;
    }

    public Type[] Types { get; private set; }
}
