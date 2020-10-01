using UnityEngine;

namespace uAdventure.Runner
{
    public class PlayerFollower : MonoBehaviour
    {
        private static readonly Vector2 BackgroundObjectPivot = SceneMB.DefaultPivot;
        private static readonly Vector2 CameraPivot = SceneMB.DefaultPivot;

        private bool follow = false;
        private bool init = false;

        private GameObject background;

        public GameObject Background {
            get { return background; }
            set
            {
                if (background != value)
                {
                    background = value;
                }
            }
        }

        // Use this for initialization
        protected void Start()
        {
        }

        public void Init()
        {
            init = false;
        }

        public void SettleInstant()
        {
            if (!Background)
            {
                return;
            }

            var z = transform.position.z;

            if (PlayerMB.Instance)
            {
                transform.position = PlayerMB.Instance.transform.position;
            }

            FixInside(transform, Background, z);
        }

        // Update is called once per frame
        protected void Update()
        {
            if (!Game.Instance || Game.Instance.GameState == null)
            {
                return;
            }

            follow = !Game.Instance.GameState.IsFirstPerson;

            if (!Background)
            {
                return;
            }

            var z = transform.position.z;

            if (!init)
            {
                init = true;
                if (follow && PlayerMB.Instance)
                {
                    transform.position = PlayerMB.Instance.transform.position;
                }
                else
                {
                    transform.position = Background.transform.position; // Point it to the center
                }
            }

            if (follow && PlayerMB.Instance)
            {
                var playerPos = PlayerMB.Instance.transform.position;
                transform.position = Vector3.Lerp(transform.position, playerPos, 0.05f);
            }

            FixInside(transform, Background, z);
        }

        public static void FixInside(Transform transform, GameObject gameObject, float z)
        {
            var screenRatio = Screen.width / (float)Screen.height;
            var cameraSize = new Vector2(Camera.main.orthographicSize * 2 * screenRatio, Camera.main.orthographicSize * 2);
            var cameraPosition = ReversePivot(transform.position, cameraSize, CameraPivot);
            var cameraRect = new Rect(cameraPosition, cameraSize);

            var sceneSize = gameObject.transform.lossyScale;
            var scenePosition = ReversePivot(gameObject.transform.position, sceneSize, BackgroundObjectPivot);
            var sceneRect = new Rect(scenePosition, sceneSize);

            transform.position = cameraRect.KeepInside(sceneRect).center;

            var aux = transform.position;
            aux.z = z;
            transform.position = aux;
        }

        private static Vector3 ReversePivot(Vector3 position, Vector3 size, Vector3 pivot)
        {
            var traslation = size;
            traslation.Scale(pivot);
            return position - traslation;
        }
    }
}