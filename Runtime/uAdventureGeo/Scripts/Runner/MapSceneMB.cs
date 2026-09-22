using UnityEngine;

using uAdventure.Runner;
using System;
using System.Collections.Generic;
using MapzenGo.Models;
using UnityStandardAssets.Characters.ThirdPerson;
using MapzenGo.Helpers;
using UnityEngine.EventSystems;
using UnityEngine.Android;

namespace uAdventure.Geo
{
    public class MapSceneMB : MonoBehaviour, IRunnerChapterTarget, IPointerClickHandler
    {
        public TileManager tileManager;
        public uAdventurePlugin uAdventurePlugin;
        public GeoPositionedCharacter geoCharacter;
        public ThirdPersonCharacter character;
        public List<GeoPositioner> geoPositioners = new List<GeoPositioner>();
        public List<GeoElementMB> geoElements = new List<GeoElementMB>();

        private bool ready = false;
        private MapScene mapScene;
        private bool isPinching = false;
        private float startDist;
        private float startOrtho;
        private static float OriginalOrthoSize, LastOrthoSize;
        private const float MinOrthoSize = 15, MaxOrthoSize = 40;

        public List<MapElement> MapElements {
            get
            {
                return mapScene.Elements;
            }
        }

        public object Data
        {
            get
            {
                return mapScene;
            }

            set
            {
                mapScene = (MapScene) value;
                tileManager.Latitude = (float) mapScene.LatLon.x;
                tileManager.Longitude = (float) mapScene.LatLon.y;
            }
        }

        public bool IsReady { get { return ready; } }

        public bool canBeInteracted()
        {
            return false;
        }

        public void Destroy(float time, System.Action onDestroy)
        {
            Camera.main.transform.Restore(bkCameraTransform);
            DestroyImmediate(this.gameObject);
            onDestroy();
        }

        public InteractuableResult Interacted(PointerEventData pointerData = null)
        {
            return InteractuableResult.IGNORES;
        }

        public void RenderScene()
        {
            ready = false;
            /*
            tileManager.ReloadPlugins<uAdventurePlugin>();
            tileManager.ReloadPlugins<MapElementFactory>();*/
            foreach (var geopos in geoPositioners.FindAll(gp => gp != null))
            {
                geopos.UpdateConditions();
            }
            foreach (var geoelement in geoElements.FindAll(gp => gp != null))
            {
                geoelement.UpdateConditions();
            }

        }

        public void setInteractuable(bool state)
        {
            throw new NotImplementedException();
        }

        // ------------------------------
        // MONO Behaviour
        // ---------------------------
        private TransformData bkCameraTransform;
        
        protected void Awake()
        {
            uAdventurePlugin.MapSceneMB = this;
        }

        protected void Start()
        {
            bkCameraTransform = Camera.main.transform.Backup();

#if PLATFORM_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation))
                {
                    Permission.RequestUserPermission(Permission.CoarseLocation);
                }
                Permission.RequestUserPermission(Permission.FineLocation);
            }
#endif

            var geoExtension = GameExtension.GetInstance<GeoExtension>();
            // Start the gps just in case is not
            if (!geoExtension.IsStarted())
            {
                geoExtension.Start();
            }

            // If the location is valid
            if(geoExtension.IsLocationValid())
            {
                geoCharacter.InstantMoveTo(geoExtension.Location);
            }
            else
            {
                // if not, just put the character in the center of the map
                geoCharacter.InstantMoveTo(new Vector2d(tileManager.Latitude, tileManager.Longitude));
            }

            if(OriginalOrthoSize == 0)
            {
                LastOrthoSize = OriginalOrthoSize = Camera.main.orthographicSize;
            }

            Camera.main.orthographicSize = LastOrthoSize;
            InventoryManager.Instance.Show = true;
        }

        protected void OnDestroy()
        {
            if (Camera.main)
            {
                Camera.main.orthographic = true;
                Camera.main.orthographicSize = OriginalOrthoSize;
            }
        }


        private Vector2d lastUpdatedPosition;

        protected void Update()
        {
            ready = uAdventurePlugin.ready;
            var geoExtension = GameExtension.GetInstance<GeoExtension>();
            if (geoExtension.IsLocationValid())
            {
                var inputLatLon = geoExtension.Location;
                if (geoExtension.IsLocationValid() 
                    && (GM.LatLonToMeters(lastUpdatedPosition) - GM.LatLonToMeters(inputLatLon)).sqrMagnitude >= 1f)
                {
                    lastUpdatedPosition = inputLatLon;
                    if (GM.SeparationInMeters(geoCharacter.LatLon, inputLatLon) > 150) geoCharacter.InstantMoveTo(inputLatLon);
                    else geoCharacter.MoveTo(inputLatLon);
                }
                
            }

            if ((uAdventureRaycaster.Instance.Override == null || uAdventureRaycaster.Instance.Override == gameObject) && Input.touchCount >= 2)
            {
                var touch0 = Input.GetTouch(0);
                var touch1 = Input.GetTouch(1);
                if (!isPinching)
                {
                    isPinching = true;
                    uAdventureRaycaster.Instance.Override = this.gameObject;
                    startDist = (touch1.position - touch0.position).sqrMagnitude;
                    startOrtho = LastOrthoSize;
                }

                if ((touch0.phase == TouchPhase.Moved || touch0.phase == TouchPhase.Stationary) &&
                    (touch1.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Stationary))
                {
                    var currentDist = (touch1.position - touch0.position).sqrMagnitude;
                    var distGrowth = startDist / currentDist;
                    var ortho = startOrtho * distGrowth;
                    LastOrthoSize = Mathf.Clamp(ortho, MinOrthoSize, MaxOrthoSize);
                }
            }
            else if (isPinching)
            {
                uAdventureRaycaster.Instance.Override = null;
                isPinching = false;
            }


            if ((uAdventureRaycaster.Instance.Override == null || uAdventureRaycaster.Instance.Override == gameObject) && Input.mouseScrollDelta.y != 0)
            {
                LastOrthoSize = Mathf.Clamp(LastOrthoSize - Input.mouseScrollDelta.y, MinOrthoSize, MaxOrthoSize);
            }

            if (InventoryManager.Instance.Opened)
            {
                Camera.main.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                mapScene.CameraType = CameraType.Ortographic3D;
                var distancePercentage = (LastOrthoSize - MinOrthoSize) / (MaxOrthoSize - MinOrthoSize);
                var distancePercentageWithMinimum = distancePercentage * 0.65f + 0.35f; // Adjusted to a minimum distance of 30%

                switch (mapScene.CameraType)
                {
                    case CameraType.Aerial2D:
                        Camera.main.orthographic = true;
                        Camera.main.orthographicSize = LastOrthoSize;
                        Camera.main.transform.position = character.transform.position + Vector3.up * 50;
                        Camera.main.transform.LookAt(character.transform.position);
                        break;
                    case CameraType.Ortographic3D:
                        Camera.main.orthographic = true;
                        Camera.main.orthographicSize = LastOrthoSize;
                        Camera.main.transform.position = character.transform.position + Vector3.up * 50 + Vector3.back * 50 * (1 - distancePercentage);
                        Camera.main.transform.LookAt(character.transform.position);
                        break;
                    case CameraType.Perspective3D:
                        Camera.main.orthographic = false;
                        Camera.main.transform.position = character.transform.position + Vector3.up * 70 * distancePercentageWithMinimum + Vector3.back * 35 * (1 - distancePercentageWithMinimum);
                        Camera.main.transform.LookAt(character.transform.position);
                        break;
                    default:
                        break;

                }
            }

            //            Debug.Log("LatLon: " + geoCharacter.LatLon + " LT->Meters->LT: " + GM.MetersToLatLon(GM.LatLonToMeters(geoCharacter.LatLon)));

            //geoCharacter.MoveTo(geoCharacter.LatLon + GM.MetersToLatLon(new Vector2d(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"))));
            //geoCharacter.MoveTo(GM.MetersToLatLon(GM.LatLonToMeters(mapScene.LatLon.y, mapScene.LatLon.x) + new Vector2d(100, 100)));
            //geoCharacter.MoveTo(new Vector2d(-3.707398, 40.415363));
            //character.Move(new Vector3(Input.GetAxis("Horizontal"),0, Input.GetAxis("Vertical")), false, false);
            
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var geoExtension = GameExtension.GetInstance<GeoExtension>();
            if (!Application.isMobilePlatform || PreviewManager.Instance.InPreviewMode || geoExtension.UsingDebugLocation)
            {
                geoExtension.UsingDebugLocation = true;
                eventData.Use();
                var tileManagerRelative = GM.LatLonToMeters(tileManager.Latitude, tileManager.Longitude);
                var localPosition = tileManager.transform.worldToLocalMatrix.MultiplyPoint(eventData.pointerCurrentRaycast.worldPosition);
                var meters = localPosition.ToVector2xz().ToVector2d();
                var latLon = GM.MetersToLatLon(tileManagerRelative + meters);
                geoExtension.Location = latLon;
            }
        }
    }



    // Transform management
    internal class TransformData
    {
        public Vector3 position, scale;
        public Quaternion rotation;

    }

    internal static class TransformDataManagement
    {
        public static TransformData Backup(this Transform t)
        {
            return new TransformData
            {
                position = t.position,
                scale = t.localScale,
                rotation = t.rotation
            };
        }

        public static void Restore(this Transform t, TransformData backup)
        {
            t.position = backup.position;
            t.localScale = backup.scale;
            t.rotation = backup.rotation;
        }
    }
}