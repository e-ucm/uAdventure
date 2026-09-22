using UnityEngine;
using UnityEditor;

using uAdventure.Core;
using uAdventure.Editor;

namespace uAdventure.Analytics
{

    public class TrackerConfigWindow : LayoutWindow
    {
        public TrackerConfigWindow(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            var _ = AnalyticsController.Instance;
        }

        public override void Draw(int aID)
        {
            var trackerConfig = AnalyticsController.Instance.TrackerConfig;

            //XApi class
            EditorGUI.BeginChangeCheck();
            var newRawCopy = EditorGUILayout.Toggle(new GUIContent("Local Backup"), trackerConfig.getRawCopy());
            if (EditorGUI.EndChangeCheck())
            {
                trackerConfig.setRawCopy(newRawCopy);
            }

            // Storage Type
            EditorGUI.BeginChangeCheck();
            var newStorageType = (TrackerConfig.StorageType)EditorGUILayout.EnumPopup("Storage Type", trackerConfig.getStorageType());
            if (EditorGUI.EndChangeCheck())
            {
                trackerConfig.setStorageType(newStorageType);
            }

            // Trace Format
            EditorGUI.BeginChangeCheck();
            var newTraceFormat = (TrackerConfig.TraceFormat)EditorGUILayout.EnumPopup("Trace Format", trackerConfig.getTraceFormat());
            if (EditorGUI.EndChangeCheck())
            {
                trackerConfig.setTraceFormat(newTraceFormat);
            }

            // Name
            EditorGUI.BeginChangeCheck();
            var newHost = EditorGUILayout.TextField(TC.get("Hostname"), trackerConfig.getHost());
            if (EditorGUI.EndChangeCheck())
            {
                trackerConfig.setHost(newHost);
            }

            // Name
            EditorGUI.BeginChangeCheck();
            var newTrackingCode = EditorGUILayout.TextField(TC.get("Tracking Code"), trackerConfig.getTrackingCode());
            if (EditorGUI.EndChangeCheck())
            {
                trackerConfig.setTrackingCode(newTrackingCode);
            }

            // Name
            EditorGUI.BeginChangeCheck();
            var newFlushInterval = EditorGUILayout.IntField(TC.get("Flush Interval"), trackerConfig.getFlushInterval());
            if (EditorGUI.EndChangeCheck())
            {
                trackerConfig.setFlushInterval(newFlushInterval);
            }

            EditorGUI.BeginChangeCheck();
            var newDebug = EditorGUILayout.Toggle(TC.get("Debug mode"), trackerConfig.getDebug());
            if (EditorGUI.EndChangeCheck())
            {
                trackerConfig.setDebug(newDebug);
            }
        }
    }
}