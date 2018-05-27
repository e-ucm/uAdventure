using UnityEngine;

namespace uAdventure.Runner
{
    public class PlayerFollower : MonoBehaviour
    {

        private static readonly Vector2 BackgroundObjectPivot = SceneMB.DefaultPivot;
        private static readonly Vector2 CameraPivot = SceneMB.DefaultPivot;

        bool follow = false;
        bool init = false;

        Vector3 minpos;
        Vector3 maxpos;
        float z;

        private GameObject background, lastBackground;

        // Use this for initialization
        void Start()
        {
            follow = !Game.Instance.GameState.IsFirstPerson;
        }

        private Vector3 ReversePivot(Vector3 position, Vector3 size, Vector3 pivot)
        {
            var traslation = size;
            traslation.Scale(pivot);
            return position - traslation;
        }

        public void SettleInstant()
        {
            z = transform.position.z;

            background = GameObject.Find("Background");

            if (PlayerMB.Instance)
                transform.position = PlayerMB.Instance.transform.position;
            else
                transform.position = background.transform.position; // Point it to the center

            FixInside();
        }

        // Update is called once per frame
        void Update()
        {

            if (!background)
                background = GameObject.Find("Background");

            if (lastBackground != background)
            {
                init = false;
                lastBackground = background;
            }

            if (!background)
                return;

            z = transform.position.z;

            if (!init)
            {
                init = true;
                if (follow && PlayerMB.Instance)
                    transform.position = PlayerMB.Instance.transform.position;
                else
                    transform.position = background.transform.position; // Point it to the center
            }

            if (follow && PlayerMB.Instance)
            {
                var playerPos = PlayerMB.Instance.transform.position;
                transform.position = Vector3.Lerp(transform.position, playerPos, 0.05f);
            }

            FixInside();
        }

        private void FixInside()
        {

            var screenRatio = Screen.width / (float)Screen.height;
            var cameraSize = new Vector2(Camera.main.orthographicSize * 2 * screenRatio, Camera.main.orthographicSize * 2);
            var cameraPosition = ReversePivot(transform.position, cameraSize, CameraPivot);
            var cameraRect = new Rect(cameraPosition, cameraSize);

            var sceneSize = background.transform.lossyScale;
            var scenePosition = ReversePivot(background.transform.position, sceneSize, BackgroundObjectPivot);
            var sceneRect = new Rect(scenePosition, sceneSize);

            Camera.main.transform.position = cameraRect.KeepInside(sceneRect).center;

            var aux = transform.position;
            aux.z = z;
            transform.position = aux;
        }
    }
}