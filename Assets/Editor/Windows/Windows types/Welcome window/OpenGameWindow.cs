using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System;
using System.IO;
using System.Collections.Generic;

public class OpenGameWindow : LayoutWindow
{
    private System.Windows.Forms.OpenFileDialog ofd;
    private string selectedGameProjectPath = "";

    public OpenGameWindow(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
        : base(aStartPos, aContent, aStyle, aOptions)
    {
        ofd = new System.Windows.Forms.OpenFileDialog();
    }

    [DllImport("user32.dll")]
    private static extern void SaveFileDialog(); //in your case : OpenFileDialog

    public override void Draw(int aID)
    {
        GUILayout.Label(TC.get("GeneralText.Open"));
    }

    public void OpenFileDialog()
    {
        Stream myStream = null;
        ofd.InitialDirectory = "c:\\";
        ofd.Filter = "ead files (*.ead) | *.ead |eap files (*.eap) | *.eap | All files(*.*) | *.* ";
        ofd.FilterIndex = 2;
        ofd.RestoreDirectory = true;

        if (ofd.ShowDialog() == DialogResult.OK)
        {

            if ((myStream = ofd.OpenFile()) != null)
            {
                using (myStream)
                {
                    // Insert code to read the stream here.
                    selectedGameProjectPath = ofd.FileName;
                    GameRources.LoadOrCreateGameProject(selectedGameProjectPath);
                    EditorWindowBase.Init();
                    EditorWindowBase window = (EditorWindowBase)EditorWindow.GetWindow(typeof(EditorWindowBase));
                    window.Show();
                }
                myStream.Dispose();
            }

        }
    }
}