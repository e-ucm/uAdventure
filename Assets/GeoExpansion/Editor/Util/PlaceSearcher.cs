using System;
using MapzenGo.Helpers.Search;
using MapzenGo.Models.Settings.Editor;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using MapzenGo.Helpers;

namespace uAdventure.Geo
{
    public class PlaceSearcher
    {
        const string PATH_SAVE_SCRIPTABLE_OBJECT = "Assets/MapzenGo/Resources/Settings/";
        public delegate void RequestRepaint();
        public RequestRepaint OnRequestRepaint;

        /* ---------------------------------
         * Public variables
         * -------------------------------- */
        
        public string Value { get; set; }
        public Vector2d LatLon { get; protected set; }

        /* ---------------------------------
         * Attributes
         * -------------------------------- */

        private DropDown addressDropdown;
        private SearchPlace place;
        private string lastSearch = "";
        private float timeSinceLastWrite;

        /* --------------------------------
         * Constructor
         * ------------------------------*/
        public PlaceSearcher(string label)
        {
            addressDropdown = new DropDown(label);

            // Get existing open window or if none, make a new one:
            place = UnityEngine.Object.FindObjectOfType<SearchPlace>();
            if (place == null)
            {
                SearchPlace search = new GameObject("Searcher").AddComponent<SearchPlace>();
                place = search;
            }

            place.DataStructure = HelperExtention.GetOrCreateSObjectReturn<StructSearchData>(ref place.DataStructure, PATH_SAVE_SCRIPTABLE_OBJECT);
            place.namePlaceСache = "";
            place.DataStructure.dataChache.Clear();

            EditorApplication.update += this.Update;
        }


        /* --------------------------------
         * Destructor
         * ------------------------------*/
        ~PlaceSearcher()
        {
            EditorApplication.update -= this.Update;
        }


        /* --------------------------------
         * Draw methods
         * ------------------------------*/
        public string LayoutBegin()
        {
            var prevAddress = Value;
            Value = addressDropdown.LayoutBegin();
            if (Value != prevAddress)
            {
                timeSinceLastWrite = 0;
            }

            return Value;
        }

        public bool LayoutEnd()
        {
            bool r = addressDropdown.LayoutEnd();
            if (r)
            {
                // If new Location is selected from the dropdown
                lastSearch = Value = addressDropdown.Value;
                LatLon = place.DataStructure.dataChache.Find(l => l.label == Value).coordinates.ToVector2d().Swap();
                place.DataStructure.dataChache.Clear();
            }

            return r;
        }

        /* ---------------------------------------
         * PerformSearch: Used to control the start of searches
         * --------------------------------------- */
        private void PerformSearch()
        {
            if (Value != null && Value.Trim() != "" && lastSearch != Value)
            {
                place.namePlace = Value;
                place.SearchInMapzen();
                lastSearch = Value;
            }
        }

        /* ------------------------------------------
         * Update: used for taking care of the http requests
         * ------------------------------------------ */
        void Update()
        {
            //Debug.Log(Time.fixedDeltaTime);
            timeSinceLastWrite += Time.fixedDeltaTime;
            if (timeSinceLastWrite > 3f)
            {
                PerformSearch();
            }

            if (place.DataStructure.dataChache.Count > 0)
            {
                var addresses = new List<string>();
                foreach (var r in place.DataStructure.dataChache)
                    addresses.Add(r.label);
                addressDropdown.Elements = addresses;
                // Request the repaint of the element
                OnRequestRepaint();
            }
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