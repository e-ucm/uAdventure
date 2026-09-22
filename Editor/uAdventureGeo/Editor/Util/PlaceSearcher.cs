using System;
using MapzenGo.Helpers.Search;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;
using uAdventure.Editor;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace uAdventure.Geo
{
    [DataContract]
    public class Place
    {
        [DataMember(Name = "place_id", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "place_id")]
        public long PlaceId { get; set; }

        [DataMember(Name = "licence", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "licence")]
        public string Licence { get; set; }

        [DataMember(Name = "osm_type", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "extraosm_type_data")]
        public string OsmType { get; set; }

        [DataMember(Name = "osm_id", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "osm_id")]
        public long OsmId { get; set; }

        [DataMember(Name = "boundingbox", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "boundingbox")]
        public double[] BoundingBox { get; set; }

        [DataMember(Name = "lat", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "lat")]
        public double Lat { get; set; }

        [DataMember(Name = "lon", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "lon")]
        public double Lon { get; set; }

        [DataMember(Name = "display_name", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "display_name")]
        public string DisplayName { get; set; }

        [DataMember(Name = "class", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "class")]
        public string Class { get; set; }

        [DataMember(Name = "type", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [DataMember(Name = "importance", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "importance")]
        public float Importance { get; set; }

        [DataMember(Name = "icon", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "icon")]
        public string Icon { get; set; }

        [JsonIgnore]
        public Vector2d LatLon 
        { 
            get
            {
                return new Vector2d(Lat, Lon);
            }
        }

        [JsonIgnore]
        public RectD RectBoundingBox 
        { 
            get
            {
                var cornerA = new Vector2d(BoundingBox[0], BoundingBox[2]);
                var cornerB = new Vector2d(BoundingBox[1], BoundingBox[3]);
                Debug.Log(cornerA);
                Debug.Log(cornerB);
                Debug.Log(cornerB - cornerA);
                return new RectD(cornerA, cornerB - cornerA);
            }
        }

    }

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

        public Place Place { get; set; }

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
        private Place[] addresses;

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
                Place = addresses.First(sd => sd.DisplayName == Value);
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
                    addressDropdown.Elements = addresses.Select(a => a.DisplayName).ToList();
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

        private Place[] DataProcessingOSM(string success)
        {
            return JsonConvert.DeserializeObject<Place[]>(success);
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