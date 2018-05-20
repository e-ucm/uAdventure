using UnityEngine;

namespace uAdventure.Editor
{
    public class AdaptationProfileWindow : LayoutWindow
    {
        public AdaptationProfileWindow(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {

        }


        public override void Draw(int aID)
        {
            GUILayout.Label("AdaptationProfileWindow");
        }

    }
}