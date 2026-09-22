using uAdventure.Core;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Editor
{
    public class ResourcesEditor
    {
        private readonly FileChooser file;
        private readonly AnimationField animation;
        private readonly ResourcesList resourcesList;
        private DataControlWithResources data;
        private int groupSelected;

        public DataControlWithResources Data 
        { 
            get { return data; }
            set
            {
                data = value;
                groupSelected = 0;
                resourcesList.Data = value;
            }
        }

        public bool ShowResourcesList { get; set; }

        public ResourcesEditor()
        {
            resourcesList = ScriptableObject.CreateInstance<ResourcesList>();
            file = new FileChooser()
            {
                FileType = FileType.BUTTON,
            };

            animation = new AnimationField()
            {
                FileType = FileType.CHARACTER_ANIM,
            };
        }

        public void DoLayout()
        {
            if (ShowResourcesList)
            {
                resourcesList.Height = 80;
                resourcesList.OnInspectorGUI();
            }
            var resources = Data.getResources()[Data.getSelectedResources()];

            if (resources.getAssetGroupCount() > 1)
            {
                var titles = new string[resources.getAssetGroupCount()];
                for (int i = 0, te = titles.Length; i < te; ++i)
                {
                    titles[i] = resources.getGroupInfo(i);
                }

                groupSelected = EditorGUILayout.Popup(TC.get("Resources.ResourcesGroup"), groupSelected, titles);
            }

            int assetIndex;
            int end = resources.getAssetCount();
            if(resources.getAssetGroupCount() > 1)
            {
                end = resources.getGroupAssetCount(groupSelected);
            }

            for (int i = 0; i < end; ++i)
            {
                assetIndex = resources.getAssetIndex(groupSelected, i);
                var assetCategory = resources.getAssetCategory(assetIndex);
                var label = resources.getAssetDescription(assetIndex);
                var value = resources.getAssetPath(assetIndex);
                EditorGUI.BeginChangeCheck();
                if (assetCategory == AssetsConstants.CATEGORY_BOOL)
                {
                    value = EditorGUILayout.Toggle(label, value == "yes") ? "yes" : "no";
                }
                else
                {
                    var field = resources.getAssetCategory(assetIndex) == AssetsConstants.CATEGORY_ANIMATION ? animation : file;
                    field.Label = label;
                    field.Path = value;
                    field.FileType = GetFileType(resources.getAssetCategory(assetIndex));
                    field.DoLayout();
                    value = field.Path;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    resources.addAsset(resources.getAssetName(assetIndex), value);
                }

            }
        }

        private static FileType GetFileType(int assetCategory)
        {
            FileType r = default(FileType);
            switch (assetCategory)
            {
                case AssetsConstants.CATEGORY_BACKGROUND:
                    r = FileType.SCENE_BACKGROUND;
                    break;
                case AssetsConstants.CATEGORY_ANIMATION:
                    r = FileType.CHARACTER_ANIM;
                    break;
                case AssetsConstants.CATEGORY_IMAGE:
                    r = FileType.ITEM_IMAGE;
                    break;
                case AssetsConstants.CATEGORY_ICON:
                    r = FileType.ITEM_ICON;
                    break;
                case AssetsConstants.CATEGORY_AUDIO:
                case AssetsConstants.CATEGORY_ANIMATION_AUDIO:
                    r = FileType.SCENE_MUSIC;
                    break;
                case AssetsConstants.CATEGORY_VIDEO:
                    r = FileType.CUTSCENE_VIDEO;
                    break;
                case AssetsConstants.CATEGORY_CURSOR:
                    r = FileType.ITEM_IMAGE;
                    break;
                case AssetsConstants.CATEGORY_STYLED_TEXT:
                    throw new System.NotImplementedException();
                case AssetsConstants.CATEGORY_ANIMATION_IMAGE:
                    r = FileType.FRAME_IMAGE;
                    break;
                case AssetsConstants.CATEGORY_BUTTON:
                    r = FileType.BUTTON;
                    break;
                case AssetsConstants.CATEGORY_ARROW_BOOK:
                    r = FileType.BOOK_ARROW_RIGHT_NORMAL;
                    break;
            }
            return r;
        }
    }
}
