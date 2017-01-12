using UnityEngine;

using uAdventure.Runner;
using System;
using System.Collections.Generic;
using System.Collections;
using MapzenGo.Models;
using UnityStandardAssets.Characters.ThirdPerson;
using MapzenGo.Helpers;

namespace uAdventure.Geo
{
    public class MapSceneMB : MonoBehaviour, IRunnerChapterTarget
    {
        public TileManager tileManager;
        public GeoPositionedCharacter geoCharacter;
        public ThirdPersonCharacter character;

        private MapScene mapScene;
        private LocationService locationService;

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
                tileManager.Latitude = (float) mapScene.LatLon.y;
                tileManager.Longitude = (float) mapScene.LatLon.x;
            }
        }

        public bool canBeInteracted()
        {
            return false;
        }

        public void Destroy(float time = 0)
        {
            DestroyImmediate(this.gameObject);
            Camera.main.transform.Restore(bkCameraTransform);
        }

        public InteractuableResult Interacted(RaycastHit hit = default(RaycastHit))
        {
            return InteractuableResult.IGNORES;
        }

        public void RenderScene()
        {
        }

        public void setInteractuable(bool state)
        {
            throw new NotImplementedException();
        }

        // ------------------------------
        // MONO Behaviour
        // ---------------------------
        private TransformData bkCameraTransform;
        
        void Start()
        {
            StartCoroutine(StartLocation());
            //Physics.gravity = this.transform.rotation * Physics.gravity;
            bkCameraTransform = Camera.main.transform.Backup();
        }

        IEnumerator StartLocation()
        {
            // First, check if user has location service enabled
            if (!Input.location.isEnabledByUser)
                yield break;

            // Start service before querying location
            Input.location.Start();

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
            else
            {
                // Access granted and location value could be retrieved
                print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
            }
        }

        void Update()
        {
            if(Input.location.status == LocationServiceStatus.Running)
            {
                geoCharacter.MoveTo(new Vector2d(Input.location.lastData.latitude, Input.location.lastData.longitude));
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
            geoCharacter.MoveTo(GM.MetersToLatLon(GM.LatLonToMeters(mapScene.LatLon.y, mapScene.LatLon.x) + new Vector2d(100, 100)));
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