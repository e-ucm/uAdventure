using UnityEngine;
using UnityEditor;
using System.IO;

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
                    uAdventureWindowMain.Instance.RefreshWindows();
                }
            }
        }
    }
}