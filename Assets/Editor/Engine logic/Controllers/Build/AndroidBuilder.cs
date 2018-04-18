using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Editor
{
    public class AndroidBuilder : ISimplifiedBuilder
    {
        private static readonly GUIContent button = new GUIContent("Android", EditorGUIUtility.IconContent("BuildSettings.Android").image);
        private static readonly ConfigField[] configFields = new ConfigField[]
        {
            new ConfigField()
            {
                Identifier = 0,
                Name = "Package Name",
                Validation = (o) =>
                {
                    var s = o as string ?? "";
                    var split = s.Split('.');
                    return !string.IsNullOrEmpty(s) && s.Split('.').Length >= 2 && split.All(t => !string.IsNullOrEmpty(t));
                },
                Optional = false,
                ValueType = typeof(string)
            }
        };

        public GUIContent ButtonContent { get { return button; } }

        public ConfigField[] ConfigFields { get { return configFields; } }

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

}

