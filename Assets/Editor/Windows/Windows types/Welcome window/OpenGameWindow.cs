using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System;
using System.IO;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class OpenGameWindow : LayoutWindow
    {
        private string selectedGameProjectPath = "";

        public OpenGameWindow(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
        }

        public override void Draw(int aID)
        {
            GUILayout.Label(TC.get("GeneralText.Open"));
        }

        public void OpenFileDialog()
        {
            Stream myStream = null;

			var fileFilter = "eap";
			var result = EditorUtility.OpenFilePanel ("Select file", "C://", fileFilter);

			if (result != "")
			{
				FileInfo file = new FileInfo (result);

				if (file.Exists)
				{
                    // Insert code to read the stream here.
					Debug.Log("Opening project");
					selectedGameProjectPath = file.FullName;
                    GameRources.LoadGameProject(selectedGameProjectPath);
                    Controller.OpenEditorWindow();
                    EditorWindowBase.RefreshWindows();
                }

            }
        }
    }
}