using UnityEngine;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ScenesWindowPlayerMovement : LayoutWindow, DialogReceiverInterface
    {

        private string backgroundPath = "";
        private Texture2D backgroundPreviewTex = null;
        private static Rect previewRect;

        public ScenesWindowPlayerMovement(Rect aStartPos, GUIContent aContent, GUIStyle aStyle,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            if (GameRources.GetInstance().selectedSceneIndex >= 0)
                backgroundPath =
                    Controller.Instance.SelectedChapterDataControl.getScenesList().getScenes()[
                        GameRources.GetInstance().selectedSceneIndex].getPreviewBackground();
            if (backgroundPath != null && !backgroundPath.Equals(""))
                backgroundPreviewTex = AssetsController.getImage(backgroundPath).texture;
        }

        public override void Draw(int aID)
        {
            var windowWidth = m_Rect.width;
            var windowHeight = m_Rect.height;
            previewRect = new Rect(0f, 0.3f * windowHeight, windowWidth, windowHeight * 0.65f);

            // Show preview dialog
            if (GUILayout.Button(TC.get("GeneralText.Edit")))
            {
                PlayerMovementEditor window =
                    (PlayerMovementEditor)ScriptableObject.CreateInstance(typeof(PlayerMovementEditor));
                window.Init(this, Controller.Instance.SelectedChapterDataControl.getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex]);
            }
            GUI.DrawTexture(previewRect, backgroundPreviewTex, ScaleMode.ScaleToFit);
        }

        public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
        {
        }

        public void OnDialogCanceled(object workingObject = null)
        {
        }
    }
}