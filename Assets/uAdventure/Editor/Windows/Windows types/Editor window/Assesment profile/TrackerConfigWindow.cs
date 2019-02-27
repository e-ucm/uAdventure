using UnityEngine;
using UnityEditor;

using uAdventure.Core;

namespace uAdventure.Editor
{

    public class TrackerConfigWindow : LayoutWindow
    {
        public TrackerConfigWindow(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
        }

        public override void Draw(int aID)
        {
            var trackerConfig = Controller.Instance.AdventureData.getTrackerConfig();

            //XApi class
            EditorGUI.BeginChangeCheck();
            var newRawCopy = EditorGUILayout.Toggle(new GUIContent("Raw Copy"), trackerConfig.getRawCopy());
            if (EditorGUI.EndChangeCheck())
                trackerConfig.setRawCopy(newRawCopy);

            // Xapi Type
            trackerConfig.setStorageType((TrackerConfig.StorageType)EditorGUILayout.EnumPopup(new GUIContent("Storage Type"), trackerConfig.getStorageType()));

            // Xapi Type
            trackerConfig.setTraceFormat((TrackerConfig.TraceFormat)EditorGUILayout.EnumPopup(new GUIContent("Trace Format"), trackerConfig.getTraceFormat()));


            // Name
            EditorGUI.BeginChangeCheck();
            var newHost = EditorGUILayout.TextField(TC.get("Tracker.Host"), trackerConfig.getHost());
            if (EditorGUI.EndChangeCheck())
                trackerConfig.setHost(newHost);

            // Name
            EditorGUI.BeginChangeCheck();
            var newTrackingCode = EditorGUILayout.TextField(TC.get("Tracker.TrackingCode"), trackerConfig.getTrackingCode());
            if (EditorGUI.EndChangeCheck())
                trackerConfig.setTrackingCode(newTrackingCode);
        }
    }
}