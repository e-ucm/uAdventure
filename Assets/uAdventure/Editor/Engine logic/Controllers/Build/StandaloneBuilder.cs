using System;
using System.Collections;
using System.Collections.Generic;
using uAdventure.Core;
using UnityEditor;
using UnityEngine;

public class StandaloneBuilder : ISimplifiedBuilder
{
    private static readonly GUIContent buttonContent = new GUIContent("Windows", EditorGUIUtility.IconContent("BuildSettings.Metro").image);

    public GUIContent ButtonContent { get { return buttonContent; } }

    public ConfigField[] ConfigFields { get { return null; } }

    public bool BuildsItself { get { return false; } }

    public bool Build(object[] configValues)
    {
        throw new NotImplementedException();
    }

    public BuildPlayerOptions[] CreateOptionsForMainPipeline(object[] configValues)
    {
        throw new NotImplementedException();
    }

    public void PostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        throw new NotImplementedException();
    }
}
