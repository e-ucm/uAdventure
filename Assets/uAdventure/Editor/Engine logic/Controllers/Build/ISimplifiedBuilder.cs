using UnityEditor;
using UnityEngine;

public delegate bool ValidateField(object value);

public struct ConfigField
{
    public int Identifier;
    public string Name;
    public System.Type ValueType;
    public object DefaultValue;
    public bool Optional;
    public ValidateField Validation;
}

public interface ISimplifiedBuilder {
    
    GUIContent ButtonContent { get; }
	ConfigField[] ConfigFields { get; }
    bool BuildsItself { get; }
    BuildPlayerOptions[] CreateOptionsForMainPipeline(object[] configValues);
    bool Build(object[] configValues);
    void PostProcessBuild(BuildTarget target, string pathToBuiltProject);

}
