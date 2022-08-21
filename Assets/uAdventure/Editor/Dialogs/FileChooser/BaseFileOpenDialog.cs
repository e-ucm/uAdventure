using UnityEditor;
using System.IO;
using uAdventure.Core;

namespace uAdventure.Editor
{
    public enum FileType
    {
        PATH,
        SCENE_BACKGROUND,
        SCENE_FOREGROUND,
        SCENE_MUSIC,
        EXIT_MUSIC,
        EXIT_ICON,
        CUTSCENE_MUSIC,
        CUTSCENE_VIDEO,
        CUTSCENE_SLIDES,
        FRAME_IMAGE,
        FRAME_MUSIC,
        BOOK_IMAGE_PARAGRAPH,
        BOOK_ARROW_LEFT_NORMAL,
        BOOK_ARROW_RIGHT_NORMAL,
        BOOK_ARROW_LEFT_OVER,
        BOOK_ARROW_RIGHT_OVER,
        ITEM_IMAGE,
        ITEM_ICON,
        ITEM_IMAGE_OVER,
        ITEM_DESCRIPTION_NAME_SOUND,
        ITEM_DESCRIPTION_BRIEF_SOUND,
        ITEM_DESCRIPTION_DETAILED_SOUND,
        NPC_DESCRIPTION_NAME_SOUND,
        NPC_DESCRIPTION_BRIEF_SOUND,
        NPC_DESCRIPTION_DETAILED_SOUND,
        SET_ITEM_IMAGE,
        CHARACTER_ANIM,
        PLAY_SOUND_EFFECT,
        PLAY_ANIMATION_EFFECT,
        BUTTON,
        BUTTON_OVER,
        BUTTON_SOUND,
        CURSOR
    }

    public static class FileTypeExtension
    {
        public static int GetAssetCategory(this FileType fileType)
        {
            switch (fileType)
            {
                case FileType.PATH:
                    break;
                case FileType.SCENE_BACKGROUND:
                case FileType.SCENE_FOREGROUND:
                    return AssetsConstants.CATEGORY_BACKGROUND;
                case FileType.BUTTON_SOUND:
                case FileType.CUTSCENE_MUSIC:
                case FileType.SCENE_MUSIC:
                case FileType.EXIT_MUSIC:
                case FileType.PLAY_SOUND_EFFECT:
                case FileType.ITEM_DESCRIPTION_NAME_SOUND:
                case FileType.ITEM_DESCRIPTION_BRIEF_SOUND:
                case FileType.ITEM_DESCRIPTION_DETAILED_SOUND:
                case FileType.NPC_DESCRIPTION_NAME_SOUND:
                case FileType.NPC_DESCRIPTION_BRIEF_SOUND:
                case FileType.NPC_DESCRIPTION_DETAILED_SOUND:
                    return AssetsConstants.CATEGORY_AUDIO;
                case FileType.EXIT_ICON:
                case FileType.ITEM_ICON:
                    return AssetsConstants.CATEGORY_ICON;
                case FileType.CUTSCENE_VIDEO:
                    return AssetsConstants.CATEGORY_VIDEO;
                case FileType.BOOK_IMAGE_PARAGRAPH:
                case FileType.ITEM_IMAGE:
                case FileType.ITEM_IMAGE_OVER:
                case FileType.SET_ITEM_IMAGE:
                    return AssetsConstants.CATEGORY_IMAGE;
                case FileType.BOOK_ARROW_LEFT_NORMAL:
                case FileType.BOOK_ARROW_RIGHT_NORMAL:
                case FileType.BOOK_ARROW_LEFT_OVER:
                case FileType.BOOK_ARROW_RIGHT_OVER:
                    return AssetsConstants.CATEGORY_ARROW_BOOK;
                case FileType.CURSOR:
                    return AssetsConstants.CATEGORY_CURSOR;
                case FileType.BUTTON:
                case FileType.BUTTON_OVER:
                    return AssetsConstants.CATEGORY_BUTTON;
                // Animations
                case FileType.CUTSCENE_SLIDES:
                case FileType.CHARACTER_ANIM:
                case FileType.PLAY_ANIMATION_EFFECT:
                    return AssetsConstants.CATEGORY_ANIMATION;
                case FileType.FRAME_IMAGE:
                    return AssetsConstants.CATEGORY_ANIMATION_IMAGE;
                case FileType.FRAME_MUSIC:
                    return AssetsConstants.CATEGORY_ANIMATION_AUDIO;
            }
            return AssetsConstants.CATEGORY_IMAGE;
        }
    }


public abstract class BaseFileOpenDialog : EditorWindow
    {


        protected DialogReceiverInterface reference;
        protected FileType fileType;

        protected string selectedAssetPath = "";

        protected string fileFilter;
        protected string basePath = "Assets/uAdventure/Resources/CurrentGame/assets";

        // Return string (for engine purpose)
        protected string returnPath;

        public virtual void Init(DialogReceiverInterface e, FileType fType)
        {
            reference = e;
            fileType = fType;

            OpenFileDialog();
        }

        public void OpenFileDialog()
        {
            string result;
            if(fileType == FileType.PATH)
            {
                result = EditorUtility.SaveFolderPanel("Select folder", "/", "Builds");
            }
            else
            {
                result = EditorUtility.OpenFilePanel("Select file", basePath, fileFilter);
            }

			if (result != "")
            {
                bool isValid = false;
                if (fileType == FileType.PATH)
                {
                    DirectoryInfo directory = new DirectoryInfo(result);
                    isValid = directory.Exists;
                    selectedAssetPath = directory.FullName;
                }
                else
                {
                    FileInfo file = new FileInfo(result);
                    isValid = file.Exists;
                    selectedAssetPath = file.FullName;
                }

				if (isValid)
                {
                    var validFormat = true;
                    if (!string.IsNullOrEmpty(fileFilter))
                    {
                        var formatToCheck = selectedAssetPath.ToLowerInvariant();
                        var formats = this.fileFilter.ToLowerInvariant().Split(',');
                        validFormat = false;
                        foreach (var format in formats)
                        {
                            if (formatToCheck.EndsWith(format))
                            {
                                validFormat = true;
                                break;
                            }
                        }
                    }

                    if (validFormat)
                    {
                        // Insert code to read the stream here.
                        ChoosedCorrectFile();
                    }
                    else
                    {
                        Controller.Instance.ShowErrorDialog("Wrong file format!", "The selected file format is not valid!");
                    }
                }
                else
                {
                    Controller.Instance.ShowErrorDialog("Wrong file!", "The selected file is not valid!");
                }
            }
            else
            {
                FileSelectionNotPerfromed();
            }
        }

        protected void CopySelectedAssset()
        {
            returnPath = AssetsController.AddSingleAsset(fileType.GetAssetCategory(), selectedAssetPath);
        }

        protected abstract void ChoosedCorrectFile();

        protected abstract void FileSelectionNotPerfromed();
    }
}