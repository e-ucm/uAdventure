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
            var titles = new string[resources.getAssetGroupCount()];
            for (int i = 0, end = titles.Length; i < end; ++i)
            {
                titles[i] = resources.getGroupInfo(i);
            }

            groupSelected = EditorGUILayout.Popup(TC.get("Resources.ResourcesGroup"), groupSelected, titles);
            int assetIndex;
            for (int i = 0, end = resources.getGroupAssetCount(groupSelected); i < end; ++i)
            {
                assetIndex = resources.getAssetIndex(groupSelected, i);
                var field = resources.getAssetCategory(assetIndex) == AssetsConstants.CATEGORY_ANIMATION ? animation : file;
                field.Label = resources.getAssetDescription(i);
                field.Path = resources.getAssetPath(assetIndex);
                EditorGUI.BeginChangeCheck();
                field.DoLayout();
                if (EditorGUI.EndChangeCheck())
                {
                    resources.addAsset(resources.getAssetName(i), field.Path);
                }
            }
        }
    }
}
