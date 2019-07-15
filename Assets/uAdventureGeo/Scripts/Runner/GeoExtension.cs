using UnityEngine;
using System.Collections;
using AssetPackage;
using uAdventure.Runner;

namespace uAdventure.Geo
{
    public class GeoExtension : GameExtension {

        private static GeoExtension instance;
        public static GeoExtension Instance {
            get
            {
                return instance;
            }
        }

        public bool UsingDebugLocation
        {
            get { return memory.Get<bool>("using_debug_location"); }
            set
            {
                if (Application.isEditor && Application.isPlaying)
                {
                    memory.Set("using_debug_location", value);
                }
            }
        }


        public GeoPositionedCharacter geochar;
        public float timeToFlush = 5;
        public Texture2D connectedSimbol;
        public Texture2D connectingSimbol;
        public Texture2D disconnectedSimbol;
        public float blinkingTime;
        public float iconWidth, iconHeight;
        private Memory memory;
        private float timeSinceLastPositionUpdate = 0;

        private float time;

        public float update = .1f; // 5 meters
        public float accuracy = 1; // 10 meters

        void Awake()
        {
            instance = this;
            OnReset();
        }

        public void Start()
        {
            if (!IsStarted())
            {
                StartCoroutine(StartLocation());
            }
        }

        public override void OnReset()
        {
            memory = new Memory();
            memory.Set("using_debug_location", false);
            memory.Set("debug_location", Vector2d.zero);
            memory.Set("navigating", 0);
            memory.Set("zone_control", false);
            Game.Instance.GameState.SetMemory("geo_extension", memory);
            CreateNavigationAndZoneControl();
        }

        public override void OnAfterGameLoad()
        {
            this.memory = Game.Instance.GameState.GetMemory("geo_extension") ?? memory;
            CreateNavigationAndZoneControl();
        }

        public override void OnBeforeGameSave()
        {
        }

        private void CreateNavigationAndZoneControl()
        {
            var oldNavigation = FindObjectOfType<NavigationController>();
            if (oldNavigation)
            {
                DestroyImmediate(oldNavigation.gameObject);
            }

            // In case is necesary
            if (memory.Get<bool>("navigating"))
            {
                var newNavigation = Instantiate(Resources.Load<GameObject>("navigation"));
                newNavigation.GetComponent<NavigationController>().RestoreNavigation(memory);
            }

            var oldZoneControl = FindObjectOfType<ZoneControl>();
            if (oldZoneControl)
            {
                DestroyImmediate(oldZoneControl.gameObject);
            }

            if (memory.Get<bool>("zone_control") && !FindObjectOfType<ZoneControl>())
            {
                var newZoneControl = new GameObject("zone_control");
                newZoneControl.AddComponent<ZoneControl>().Restore(memory);
            }
        }


        IEnumerator StartLocation()
        {
            // First, check if user has location service enabled
            if (!Input.location.isEnabledByUser)
                yield break;

            // Start service before querying location
            Input.location.Start(accuracy, update);

            // Wait until service initializes
            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            // Service didn't initialize in 20 seconds
            if (maxWait < 1)
            {
                print("Timed out");
                yield break;
            }

            // Connection has failed
            if (Input.location.status == LocationServiceStatus.Failed)
            {
                print("Unable to determine device location");
                yield break;
            }

            // Access granted and location value could be retrieved
            print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);

        }

        public bool IsStarted()
        {
            return Input.location.status == LocationServiceStatus.Initializing || Input.location.status == LocationServiceStatus.Running;
        }

        void Update()
        {
            time += Time.deltaTime;
            timeSinceLastPositionUpdate += Time.deltaTime;

            if (time > blinkingTime)
            {
                time = time - Mathf.Floor(time / blinkingTime) * blinkingTime;
            }

            if (Game.Instance.CurrentTargetRunner.Data is MapScene && !geochar)
            {
                geochar = FindObjectOfType<GeoPositionedCharacter>();
            }

            if (timeSinceLastPositionUpdate > timeToFlush)
            {
                timeSinceLastPositionUpdate = 0;

                if (IsStarted() || UsingDebugLocation || geochar)
                {
                    var mapScene = Game.Instance.CurrentTargetRunner.Data as MapScene;
                    Debug.Log(Location);
                    if (mapScene != null)
                    {
                        TrackerExtension.Movement.Moved(mapScene.Id, Location);
                    }
                    else
                    {
                        TrackerExtension.Movement.Moved("World", Location);
                    }

                    TrackerAsset.Instance.Flush();
                }
            }

        }

        void OnGUI()
        {
            var paintSimbol = disconnectedSimbol;

            switch (Input.location.status) {
                default:
                case LocationServiceStatus.Failed:
                case LocationServiceStatus.Stopped:
                    paintSimbol = disconnectedSimbol;
                    break;
                case LocationServiceStatus.Initializing:
                    paintSimbol = connectingSimbol;
                    break;
                case LocationServiceStatus.Running:
                    var connecting = ((time > blinkingTime / 2f) ? connectedSimbol : connectingSimbol);
                    paintSimbol = IsLocationValid() ? connectedSimbol : connecting;
                    break;
            }

            if (Event.current.type == EventType.Repaint)
                GUI.DrawTexture(new Rect(Screen.width - iconWidth - 5, 5, iconWidth, iconHeight), paintSimbol);
        }

        public bool IsLocationValid()
        {
            return UsingDebugLocation || (Input.location.status == LocationServiceStatus.Running
                && Input.location.lastData.timestamp > 0 
                && Input.location.lastData.LatLon() != Vector2.zero
                && Mathf.Max(Input.location.lastData.horizontalAccuracy, Input.location.lastData.verticalAccuracy) < 50); // Max 50 metros
        }

        public Vector2d Location
        {
            get
            {
                if (UsingDebugLocation)
                {
                    return memory.Get<Vector2d>("debug_location");
                }

                if (IsLocationValid())
                {
                    return Input.location.lastData.LatLonD();
                }

                if (geochar)
                {
                    return geochar.LatLon;
                }

                return Vector2d.zero;
            }
            set
            {
                if (Application.isEditor && Application.isPlaying)
                {
                    UsingDebugLocation = true;
                    memory.Set("debug_location", value);
                }
            }
        }
    }
}