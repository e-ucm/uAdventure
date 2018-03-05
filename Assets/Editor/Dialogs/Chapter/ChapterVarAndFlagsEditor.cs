using UnityEngine;
using UnityEditor;
using System.Collections;

using uAdventure.Core;
using System;

namespace uAdventure.Editor
{
	public class ChapterVarAndFlagsEditor : EditorWindow, DialogReceiverInterface
    {
        private enum WindowType
        {
            FLAGS,
            VARS
        }

        private Texture2D flagsTex = null;
        private Texture2D varTex = null;

        private Rect contentRect, addDeleteButtonRect;

        private static GUISkin defaultSkin;
        private static GUISkin noBackgroundSkin;
        private static GUISkin selectedButtonSkin;
        private static GUISkin selectedAreaSkin;

        private int selectedObject = -1;

        private Vector2 scrollPosition;

        private WindowType openedWindow;

        public static ChapterVarAndFlagsEditor s_DrawerParametersMenu;
        private static long s_LastClosedTime;

        internal static bool ShowAtPosition(Rect buttonRect)
        {
            long num = DateTime.Now.Ticks / 10000L;
            if (num >= ChapterVarAndFlagsEditor.s_LastClosedTime + 50L)
            {
                if (Event.current != null)
                {
                    Event.current.Use();
                }
                if (ChapterVarAndFlagsEditor.s_DrawerParametersMenu == null)
                {
                    ChapterVarAndFlagsEditor.s_DrawerParametersMenu = ScriptableObject.CreateInstance<ChapterVarAndFlagsEditor>();
                }
                ChapterVarAndFlagsEditor.s_DrawerParametersMenu.Init(buttonRect);

                return true;
            }
            return false;
        }

        private void Init(Rect buttonRect)
        {
            buttonRect.position = GUIUtility.GUIToScreenPoint(buttonRect.position);
            float y = 305f;
            Vector2 windowSize = new Vector2(300f, y);
            base.ShowAsDropDown(buttonRect, windowSize);
        }


        [MenuItem("eAdventure/Flags and variables")]
		public static void Init()
		{
			var window = GetWindow<ChapterVarAndFlagsEditor> ();
			window.Show();
		}

        public void OnEnable()
        {
			if(!flagsTex)
				flagsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/flag16", typeof(Texture2D));
			if(!varTex)
            	varTex = (Texture2D)Resources.Load("EAdventureData/img/icons/vars", typeof(Texture2D));

			if(!selectedButtonSkin)
				selectedButtonSkin = (GUISkin)Resources.Load("Editor/ButtonSelected", typeof(GUISkin));
			if(!noBackgroundSkin)
				noBackgroundSkin = (GUISkin)Resources.Load("Editor/EditorNoBackgroundSkin", typeof(GUISkin));
			if(!selectedAreaSkin)
            	selectedAreaSkin = (GUISkin)Resources.Load("Editor/EditorLeftMenuItemSkinConcreteOptions", typeof(GUISkin));

        }

        void OnGUI()
        {
			var windowWidth = position.width;
			var windowHeight = position.height;

			contentRect = new Rect(0f, 0.1f * windowHeight, windowWidth, 0.7f * windowHeight);
			addDeleteButtonRect = new Rect(0f, 0.8f * windowHeight, windowWidth, 0.15f * windowHeight);
            /*
            * Upper buttons
            */
            GUILayout.BeginHorizontal();
            if (openedWindow == WindowType.FLAGS)
            {
                GUI.skin = selectedButtonSkin;
            }
            if (GUILayout.Button(new GUIContent(TC.get("Flags.Title"), flagsTex), GUILayout.MaxHeight(0.08f * windowHeight)))
            {
                if (openedWindow == WindowType.VARS)
                    OnWindowTypeChanged();
            }
            GUI.skin = defaultSkin;

            if (openedWindow == WindowType.VARS)
            {
                GUI.skin = selectedButtonSkin;
            }
            if (GUILayout.Button(new GUIContent(TC.get("Vars.Title"), varTex), GUILayout.MaxHeight(0.08f * windowHeight)))
            {
                if (openedWindow == WindowType.FLAGS)
                    OnWindowTypeChanged();
            }
            GUI.skin = defaultSkin;
            GUILayout.EndHorizontal();

            /*
            * Content part
            */
            GUILayout.BeginArea(contentRect);
            GUILayout.Space(10);
            if (openedWindow == WindowType.FLAGS)
            {
                GUILayout.Label(TC.get("Flags.Title"));
                GUILayout.BeginHorizontal();
                GUILayout.Box(TC.get("Flags.FlagName"), GUILayout.Width(0.7f * windowWidth));
                GUILayout.Box(TC.get("Flags.FlagReferences"), GUILayout.Width(0.25f * windowWidth));
                GUILayout.EndHorizontal();

                scrollPosition = GUILayout.BeginScrollView(scrollPosition);
                for (int i = 0; i < Controller.Instance.VarFlagSummary.getFlagCount(); i++)
                {
                    GUILayout.BeginHorizontal();
                    if (selectedObject == i)
                        GUI.skin = selectedAreaSkin;
                    else
                        GUI.skin = noBackgroundSkin;

                    if (GUILayout.Button(Controller.Instance.VarFlagSummary.getFlag(i),
                        GUILayout.Width(0.7f * windowWidth)))
                    {
                        OnSelectedObjectChange(i);
                    }

                    if (GUILayout.Button(Controller.Instance.VarFlagSummary.getFlagReferences(i).ToString(),
                        GUILayout.Width(0.25f * windowWidth)))
                    {
                        OnSelectedObjectChange(i);
                    }
                    GUI.skin = defaultSkin;
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label(TC.get("Vars.Title"));
                GUILayout.BeginHorizontal();
                GUILayout.Box(TC.get("Vars.VarName"), GUILayout.Width(0.7f * windowWidth));
                GUILayout.Box(TC.get("Vars.VarReferences"), GUILayout.Width(0.25f * windowWidth));
                GUILayout.EndHorizontal();

                scrollPosition = GUILayout.BeginScrollView(scrollPosition);
                for (int i = 0; i < Controller.Instance.VarFlagSummary.getVarCount(); i++)
                {
                    GUILayout.BeginHorizontal();
                    if (selectedObject == i)
                        GUI.skin = selectedAreaSkin;
                    else
                        GUI.skin = noBackgroundSkin;

                    if (GUILayout.Button(Controller.Instance.VarFlagSummary.getVar(i),
                        GUILayout.Width(0.7f * windowWidth)))
                    {
                        OnSelectedObjectChange(i);
                    }

                    if (GUILayout.Button(Controller.Instance.VarFlagSummary.getVarReferences(i).ToString(),
                        GUILayout.Width(0.25f * windowWidth)))
                    {
                        OnSelectedObjectChange(i);
                    }
                    GUI.skin = defaultSkin;
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndArea();


            /*
            * Add/delete part
            */
            GUILayout.BeginArea(addDeleteButtonRect);
            if (openedWindow == WindowType.FLAGS)
            {
                if (GUILayout.Button(TC.get("Flags.AddFlag")))
                {
                    OnAddCliked();
                }

                if (GUILayout.Button(TC.get("Flags.DeleteFlag")))
                {
                    OnDeleteClicked();
                }
            }
            else
            {
                if (GUILayout.Button(TC.get("Vars.AddVar")))
                {
                    OnAddCliked();
                }

                if (GUILayout.Button(TC.get("Vars.DeleteVar")))
                {
                    OnDeleteClicked();
                }
            }
            GUILayout.EndArea();
        }

        void OnSelectedObjectChange(int i)
        {
            selectedObject = i;
        }

        void OnWindowTypeChanged()
        {
            if (openedWindow == WindowType.FLAGS)
                openedWindow = WindowType.VARS;
            else
                openedWindow = WindowType.FLAGS;

            selectedObject = -1;
            scrollPosition = Vector2.zero;
        }

        void OnAddCliked()
        {
            if (openedWindow == WindowType.FLAGS)
            {
                ChapterFlagNameInputPopup window =
                         (ChapterFlagNameInputPopup)ScriptableObject.CreateInstance(typeof(ChapterFlagNameInputPopup));
                window.Init(this, "IdFlag");
            }
            else
            {
                ChapterVarNameInputPopup window =
                         (ChapterVarNameInputPopup)ScriptableObject.CreateInstance(typeof(ChapterVarNameInputPopup));
                window.Init(this, "IdVar");
            }
        }

        void OnDeleteClicked()
        {
            if (selectedObject >= 0)
            {
                if (openedWindow == WindowType.FLAGS)
                {
                    Controller.Instance                        .VarFlagSummary                        .deleteFlag(Controller.Instance.VarFlagSummary.getFlag(selectedObject));
                }
                else
                {
                    Controller.Instance                        .VarFlagSummary                        .deleteVar(Controller.Instance.VarFlagSummary.getVar(selectedObject));
                }
                selectedObject = -1;
            }
        }

        public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
        {
            if (workingObject is ChapterFlagNameInputPopup)
                Controller.Instance                    .VarFlagSummary.addFlag(message);

            else if (workingObject is ChapterVarNameInputPopup)
                Controller.Instance                    .VarFlagSummary.addVar(message);
        }

        public void OnDialogCanceled(object workingObject = null)
        {
        }
    }
}