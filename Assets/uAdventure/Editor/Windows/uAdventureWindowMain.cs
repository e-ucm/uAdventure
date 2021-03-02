
using System;
using System.Collections.Generic;
using System.Linq;
using uAdventure.Analytics;
using uAdventure.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace uAdventure.Editor
{
    public class uAdventureWindowMain : EditorWindowBase
    {
        public static uAdventureWindowMain Instance { get { return thisWindowReference; } }
        public enum EditorMenuItem
        {
            File,
            Edit,
            Adventure,
            Chapters,
            Run,
            Configuration,
            About
        };

        public enum EditorWindowType
        {
            Chapter,
            AdaptationProfiles,
            Extension
        };

        // The position of the window
        private static uAdventureWindowMain thisWindowReference;

        private static Texture2D redoTexture = null;
        private static Texture2D undoTexture = null;

        private static Texture2D adaptationTexture = null;
        private GameObject debugGUIHolder = null;

        private void Return(PlayModeStateChange playModeStateChange)
        {
            if (playModeStateChange == PlayModeStateChange.EnteredEditMode)
            {

                if (debugGUIHolder)
                {
                    DestroyImmediate(debugGUIHolder);
                }

                FocusWindowIfItsOpen(GetType());
            }
        }
        public void OnEnable()
        {
            Debug.Log("InitWindows"); 
            if (!thisWindowReference)
            {
                thisWindowReference = this;
                EditorApplication.playModeStateChanged += Return;
            }
            else
            {
                DestroyImmediate(thisWindowReference);
                return;
            }

            if (!Language.Initialized)
                Language.Initialize();

            if (!Controller.Instance.Initialized)
            {
                Controller.ResetInstance();
                Controller.Instance.Init();
            }

            var initialScene = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/uAdventure/Scenes/_Scene1.unity");

            if (initialScene)
            {
                EditorSceneManager.playModeStartScene = initialScene;
            }
            else
            {
                var title = "EditorWindow.MainSceneNotFound.Title".Traslate();
                var body = "EditorWindow.MainSceneNotFound.Body".Traslate();
                var ok = "GeneralText.OK".Traslate();
                EditorUtility.DisplayDialog(title, body, ok);
            }


            if (!redoTexture)
            {
                redoTexture = Resources.Load<Texture2D>("EAdventureData/img/icons/redo");
            }
            if (!undoTexture)
            {
                undoTexture = Resources.Load<Texture2D>("EAdventureData/img/icons/undo");
            }
            if (!adaptationTexture)
            {
                adaptationTexture = Resources.Load<Texture2D>("EAdventureData/img/icons/adaptationProfiles");
            }

            //var g = GeoController.Instance;
            var a = AnalyticsController.Instance;
        }

        protected override void InitWindows()
        {
            knownComponents = new Dictionary<Type, List<EditorComponent>>();
            Extensions = new List<EditorWindowExtension>();
            Menus = new Dictionary<GenericMenu, string>();

            AddMenu(new FileMenu().menu, TC.get("MenuFile.Title"));
            AddMenu(new EditMenu().menu, TC.get("MenuEdit.Title"));
            AddMenu(new AdventureMenu().menu, TC.get("MenuAdventure.Title"));
            AddMenu(new ChaptersMenu().menu, TC.get("MenuChapters.Title"));
            AddMenu(new ConfigurationMenu().menu, TC.get("MenuConfiguration.Title"));
            AddMenu(new AboutMenu().menu, TC.get("Menu.About"));


            openedWindow = EditorWindowType.Chapter;
            var chapterWindow = new ChapterWindow(Rect.zero, new GUIContent(TC.get("Element.Name0")), "Window");
            AddExtension(new ChapterWindow(Rect.zero, new GUIContent(TC.get("Element.Name0")), "Window"));

            // Extensions of the editor
            foreach (var extension in EditorWindowBaseExtensionFactory.Instance.CreateAllExistingExtensions(Rect.zero, "Window"))
            {
                AddExtension(extension);
            }
            RequestMainView(chapterWindow);
        }

        public void RefreshChapter()
        {
            // TODO
        }

        public void SelectElement(List<Searchable> path)
        {
            var extension = EditorWindowBaseExtensionFactory.Instance.GetExistingExtensionFor(path.First().GetType(),
                thisWindowReference.Extensions);


            if (extension != null)
            {
                thisWindowReference.RequestMainView(extension);
                extension.SelectElement(path);
            }
        }

        protected void OnDestroy()
        {
            if (thisWindowReference == this)
            {
                EditorApplication.playModeStateChanged -= Return;
            }

            if (debugGUIHolder)
            {
                DestroyImmediate(debugGUIHolder);
            }
        }

        // Components



        private static Dictionary<Type, List<EditorComponent>> knownComponents;
        public static Dictionary<Type, List<EditorComponent>> Components
        {
            get { return knownComponents; }
        }

        public void RegisterComponent<T>(EditorComponent component) { RegisterComponent(typeof(T), component); }

        public void RegisterComponent(Type t, EditorComponent component)
        {
            if (knownComponents == null)
            {
                knownComponents = new Dictionary<Type, List<EditorComponent>>();
            }

            if (!knownComponents.ContainsKey(t))
            {
                knownComponents.Add(t, new List<EditorComponent>());
            }

            if (component is BaseWindow)
            {
                (component as BaseWindow).OnRequestRepaint = () => this.Repaint();
            }

            // if there's a component of the same tipe already registered we ignore it
            if (knownComponents[t].Any(c => c.GetType() == component.GetType()))
            {
                return;
            }

            knownComponents[t].Add(component);
            knownComponents[t].Sort((c1, c2) => CompareComponents(c1, c2));
        }

        private static int CompareComponents(EditorComponent c1, EditorComponent c2)
        {
            var c1Attr = c1.GetType().GetCustomAttributes(typeof(EditorComponentAttribute), true)[0] as EditorComponentAttribute;
            var c2Attr = c2.GetType().GetCustomAttributes(typeof(EditorComponentAttribute), true)[0] as EditorComponentAttribute;

            return c1Attr.Order.CompareTo(c2Attr.Order);
        }
    }
}
