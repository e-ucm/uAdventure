

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraMatrixTests))]
public class CameraMatrixTestsEditor : Editor
{

    private Vector3 pos;
    private Vector3 euler;
    private Vector3 scale;

    public override void OnInspectorGUI()
    {
        /*this.DrawDefaultInspector();

        var cameraMatrixTests = target as CameraMatrixTests;
        if (cameraMatrixTests.targetGameObject == null)
        {
            return;
        }

        EditorGUILayout.HelpBox("ObjectLocalPosition: " + cameraMatrixTests.targetGameObject.localPosition.ToString(), MessageType.None);
        EditorGUILayout.HelpBox("ObjectMatrix: \n" + cameraMatrixTests.targetGameObject.localToWorldMatrix.ToString(), MessageType.None);
        EditorGUILayout.HelpBox("ObjectPosition: " + cameraMatrixTests.targetGameObject.position.ToString(), MessageType.None);
        cameraMatrixTests.camera.ResetWorldToCameraMatrix();
        var cm = cameraMatrixTests.camera.worldToCameraMatrix;
        EditorGUILayout.HelpBox("CameraMatrix: \n" + cameraMatrixTests.camera.worldToCameraMatrix.ToString(), MessageType.None);
        
        cameraMatrixTests.camera.worldToCameraMatrix = Matrix4x4.Inverse(Matrix4x4.TRS(
            cameraMatrixTests.camera.transform.position,
            cameraMatrixTests.camera.transform.rotation,
            new Vector3(1, 1, -1)
        ));

        EditorGUILayout.HelpBox("CalculatedCameraMatrix: \n" + cameraMatrixTests.camera.worldToCameraMatrix.ToString(), MessageType.None);
        //EditorGUILayout.HelpBox("InverseLookat: \n" + Matrix4x4.Inverse(Matrix4x4.LookAt(cameraMatrixTests.camera.transform.position)), MessageType.None);
        EditorGUILayout.HelpBox("CameraPosition: " + cameraMatrixTests.camera.WorldToViewportPoint(cameraMatrixTests.targetGameObject.position), MessageType.None);*/


    }
}