using UnityEngine;

using uAdventure.Runner;
using System;
using System.Collections.Generic;
using MapzenGo.Models;
using UnityStandardAssets.Characters.ThirdPerson;
using MapzenGo.Helpers;
using UnityEngine.EventSystems;

namespace uAdventure.Geo
{
    public class MapSceneMB : MonoBehaviour, IRunnerChapterTarget
    {
        public TileManager tileManager;
        public uAdventurePlugin uAdventurePlugin;
        public GeoPositionedCharacter geoCharacter;
        public ThirdPersonCharacter character;

        private bool ready = false;
        private MapScene mapScene;

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

        public void Destroy(float time = 0)
        {
            Camera.main.transform.Restore(bkCameraTransform);
            DestroyImmediate(this.gameObject);
        }

        public InteractuableResult Interacted(PointerEventData pointerData = null)
        {
            return InteractuableResult.IGNORES;
        }

        public void RenderScene()
        {
            tileManager.ReloadPlugins<uAdventurePlugin>();
            tileManager.ReloadPlugins<MapElementFactory>();
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

            // Start the gps just in case is not
            if (GPSController.Instance.IsStarted())
            {
                GPSController.Instance.Start();
            }

            // If the location is valid
            if(GPSController.Instance.IsLocationValid())
            {
                geoCharacter.LatLon = new Vector2d(Input.location.lastData.latitude, Input.location.lastData.longitude);
            }
            else
            {
                // if not, just put the character in the center of the map
                geoCharacter.LatLon = new Vector2d(tileManager.Latitude, tileManager.Longitude);
            }
        }


        private Vector2d lastUpdatedPosition;

        protected void Update()
        {
            if(Input.location.status == LocationServiceStatus.Running && Input.location.lastData.timestamp != 0 && Input.location.lastData.latitude != 0)
            {
                var inputLatLon = new Vector2d(Input.location.lastData.latitude, Input.location.lastData.longitude);
                if (GPSController.Instance.IsLocationValid() 
                    && (GM.LatLonToMeters(lastUpdatedPosition) - GM.LatLonToMeters(inputLatLon)).sqrMagnitude >= 1f)
                {
                    ready = true;
                    lastUpdatedPosition = inputLatLon;
                    if (GM.SeparationInMeters(geoCharacter.LatLon, inputLatLon) > 150) geoCharacter.LatLon = inputLatLon;
                    else geoCharacter.MoveTo(inputLatLon);
                }
                
            }

            switch (mapScene.CameraType)
            {
                case CameraType.Aerial2D:
                    Camera.main.transform.position = character.transform.position + Vector3.up * 50;
                    Camera.main.transform.rotation = Quaternion.Euler(90, 0, 0);
                    break;
                case CameraType.Ortographic3D:
                    throw new System.NotImplementedException();
                case CameraType.Perspective3D:
                    throw new System.NotImplementedException();
                default:
                    break;

            }
            //            Debug.Log("LatLon: " + geoCharacter.LatLon + " LT->Meters->LT: " + GM.MetersToLatLon(GM.LatLonToMeters(geoCharacter.LatLon)));

            //geoCharacter.MoveTo(geoCharacter.LatLon + GM.MetersToLatLon(new Vector2d(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"))));
            //geoCharacter.MoveTo(GM.MetersToLatLon(GM.LatLonToMeters(mapScene.LatLon.y, mapScene.LatLon.x) + new Vector2d(100, 100)));
            //geoCharacter.MoveTo(new Vector2d(-3.707398, 40.415363));
            //character.Move(new Vector3(Input.GetAxis("Horizontal"),0, Input.GetAxis("Vertical")), false, false);
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
            var td = new TransformData();
            td.position = t.position;
            td.scale = t.localScale;
            td.rotation = t.rotation;
            return td;
        }

        public static void Restore(this Transform t, TransformData backup)
        {
            t.position = backup.position;
            t.localScale = backup.scale;
            t.rotation = backup.rotation;
        }
    }
}