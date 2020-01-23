using System;
using MapzenGo.Helpers.Search;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;
using uAdventure.Editor;

namespace uAdventure.Geo
{
    public class PlaceSearcher : ScriptableObject
    {
        const string PATH_SAVE_SCRIPTABLE_OBJECT = "Assets/GeoExpansion/MapzenGo/Resources/Settings/";
        public delegate void RequestRepaint();
        public RequestRepaint OnRequestRepaint;

        /* ---------------------------------
         * Public variables
         * -------------------------------- */

        public string Value
        {
            get { return addressDropdown.Value; }
            set { addressDropdown.Value = value; }
        }
        public string Label
        {
            get { return addressDropdown.Label; }
            set { addressDropdown.Label = value; }
        }
        public Vector2d LatLon { get; set; }
        public RectD BoundingBox { get; set; }

        /* ---------------------------------
         * Attributes
         * -------------------------------- */

        private DropDown addressDropdown;
        const string spainSearchUrl = "http://rage.e-ucm.es/search/{0}?format=json";
        const string seachUrl = "http://nominatim.openstreetmap.org/search/{0}?format=json";
        public string namePlace = "";
        public string namePlaceСache = "";
        public StructSearchData DataStructure;
        private string lastSearch = "";
        private float timeSinceLastWrite;
        private bool searched;
        private EditorApplication.CallbackFunction update;
        private UnityWebRequestAsyncOperation request;
        private SearchData[] addresses;

        public int Source { get; set; }

        public string[] Sources {
            get
            {
                return new string[] { "Spain", "Worldwide (Has Limit)" };
            }
        }

        /* --------------------------------
         * Constructor
         * ------------------------------*/
        public void Awake()
        {
            Source = 0;
            addressDropdown = new DropDown("");
            
            update = new EditorApplication.CallbackFunction(Update);
            // Register the update
            EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, update);
        }


        /* --------------------------------
         * Destructor
         * ------------------------------*/
        public void OnDestroy()
        {
            EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, update);
        }


        /* --------------------------------
         * Draw methods
         * ------------------------------*/
        public bool DoLayout()
        {
            return DoLayout(null);
        }


        public bool DoLayout(GUIStyle style)
        {
            var prevAddress = Value;
            var selected = addressDropdown.DoLayout(style);
            if (selected)
            {
                lastSearch = Value;
                var searchData = addresses.First(sd => sd.label == Value);
                this.LatLon = searchData.coordinates.ToVector2d();
                this.BoundingBox = searchData.boundingBox;
            }

            if (!selected && Value != prevAddress)
            {
                timeSinceLastWrite = 0;
                searched = false;
            }

            return selected;
        }

        /* ---------------------------------------
         * PerformSearch: Used to control the start of searches
         * --------------------------------------- */
        private void PerformSearch()
        {
            if (lastSearch != Value && !searched)
            {
                if (Value != null && Value.Trim() != "")
                {
                    lastSearch = Value;
                    searched = true;
                    SearchInOSM(Value, Source == 0);
                }
                else
                {
                    addressDropdown.Elements = null;
                }
            }
        }

        /* ------------------------------------------
         * Update: used for taking care of the http requests
         * ------------------------------------------ */
        void Update()
        {
            timeSinceLastWrite += Time.fixedDeltaTime;
            if (timeSinceLastWrite > 3f)
            {
                PerformSearch();
            }

            if (request != null && request.isDone)
            {
                addresses = DataProcessingOSM(request.webRequest.downloadHandler.text);
                if(addresses == null || request.webRequest.isHttpError || request.webRequest.isNetworkError)
                {
                    Controller.Instance.ShowErrorDialog("Geo.PlaceSearcher.ErrorNoResults.Title", "Geo.PlaceSearcher.ErrorNoResults.Message");
                    Debug.LogError("Cannot connect with address search server (err: " + request.webRequest.responseCode + "): " + request.webRequest.error);
                } 
                else
                {
                    addressDropdown.Elements = addresses.Select(a => a.label).ToList();
                    // Request the repaint of the element 
                    if (OnRequestRepaint != null)
                    {
                        OnRequestRepaint();
                    }
                }
                request = null;
            }
        }


        private void SearchInOSM(string namePlace, bool spainOnly)
        {
            var searchURL = spainOnly ? spainSearchUrl : seachUrl;
            UnityWebRequest www = UnityWebRequest.Get(String.Format(searchURL, System.Uri.EscapeDataString(namePlace)));
            request = www.SendWebRequest();
        }

        private SearchData[] DataProcessingOSM(string success)
        {
            JSONObject obj = new JSONObject(success);
            var dataCache = new List<SearchData>();

            if(obj == null || obj.list == null)
            {
                return null;
            }

            foreach (JSONObject jsonObject in obj.list)
            {
                var minLat = double.Parse(jsonObject["boundingbox"][0].str);
                var maxLat = double.Parse(jsonObject["boundingbox"][1].str);
                var minLon = double.Parse(jsonObject["boundingbox"][2].str);
                var maxLon = double.Parse(jsonObject["boundingbox"][3].str);

                dataCache.Add(new SearchData()
                {
                    coordinates = new Vector2(float.Parse(jsonObject["lat"].str), float.Parse(jsonObject["lon"].str)),
                    label = jsonObject["display_name"].str,
                    boundingBox = new RectD(new Vector2d(minLon, minLat), new Vector2d(maxLon - minLon, maxLat - minLat))
                });
            }

            return dataCache.ToArray();
        }
    }

    public static class ExtensionV2d
    {
        public static Vector2d Swap(this Vector2d v)
        {
            return new Vector2d(v.y, v.x);
        }
    }
}