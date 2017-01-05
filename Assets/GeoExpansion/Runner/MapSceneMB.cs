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
                tileManager.Latitude = (float) mapScene.LatLon.x;
                tileManager.Latitude = (float) mapScene.LatLon.y;
            }
        }

        public bool canBeInteracted()
        {
            return false;
        }

        public void Destroy(float time = 0)
        {
            throw new NotImplementedException();
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

        
        void Start()
        {
            StartCoroutine(StartLocation());
            Physics.gravity = this.transform.rotation * Physics.gravity;
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

            //geoCharacter.MoveTo(geoCharacter.LatLon + GM.MetersToLatLon(new Vector2d(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"))));
            geoCharacter.MoveTo(mapScene.LatLon + GM.MetersToLatLon(new Vector2d(100, 100)));
            //character.Move(new Vector3(Input.GetAxis("Horizontal"),0, Input.GetAxis("Vertical")), false, false);
        }
    }
}