using UnityEngine;
using System.Collections;

namespace uAdventure.Runner
{
    public class Blur : MonoBehaviour
    {
        void Update()
        {
            var camera = Camera.main;

            var position = camera.transform.position;
            position.z = transform.position.z;
            transform.position = position;

            float cameraHeight = camera.orthographicSize * 2;
            float cameraWidth = Screen.width * cameraHeight / Screen.height;
            var scale = new Vector3(cameraWidth, cameraHeight, 1);

            transform.localScale = scale;
        }
    }
}