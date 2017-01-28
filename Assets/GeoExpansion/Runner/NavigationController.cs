using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;
using MapzenGo.Helpers;

namespace uAdventure.Geo
{
    public class NavigationController : MonoBehaviour
    {
        public Transform arrow;

        // -----------------------
        // Singleton
        // -----------------------

        protected static NavigationController instance;
        public static NavigationController Instance
        {
            get
            {
                return (instance == null) ? instance = FindObjectOfType<NavigationController>() : instance;
            }
            private set
            {
                instance = value;
            }
        }


        // ----------------------
        //  Private variables
        // ---------------------

        private bool inited = false;
        private bool navigating = false;
        private NavigationStep currentStep;
        private List<NavigationStep> steps;
        private Dictionary<string, MonoBehaviour> referenceCache;
        private Dictionary<NavigationStep, bool> stepCompleted;
        private Dictionary<NavigationStep, int> completedElementsForStep;
        private GeoPositionedCharacter character;

        // ----------------------
        // Init
        // --------------------

        void Init()
        {
            if (!inited)
            {
                referenceCache = new Dictionary<string, MonoBehaviour>();
                stepCompleted = new Dictionary<NavigationStep, bool>();
                completedElementsForStep = new Dictionary<NavigationStep, int>();
                inited = true;
            }
        }

        // ---------------------
        // Properties
        // ---------------------

        public NavigationType NavigationStrategy { get; set; }
        public List<NavigationStep> Steps
        {
            get
            {
                return steps;
            }
            set
            {
                Init();
                steps = value;
                stepCompleted.Clear();
                // All the steps are to-do
                steps.ForEach(s => stepCompleted.Add(s, false));
            }
        }

        // -------------------------
        // Public methods
        // -------------------------

        public void Navigate()
        {
            Init();

            navigating = true;
            if (Steps.Count > 0)
                currentStep = Steps[0];
        }

        // -----------------------
        // MonoBehaviour methods
        // ------------------------

        void Awake()
        {
            Instance = this;
            Init();
        }

        public void Update()
        {
            if (navigating)
            {
                if (NavigationStrategy == NavigationType.Closeness)
                {
                    currentStep = FindClosestStep(Steps);
                }
                else if (currentStep == null)
                {
                    currentStep = FindNextStep(Steps);
                }

                if (currentStep != null)
                {
                    // Check if reached
                    bool reached = IsReached(currentStep);
                    UpdateArrow(reached);

                    // Go next
                    if (reached && !currentStep.LockNavigation)
                    {
                        // If the step is completed successfully we set the current step to null
                        if (CompleteStep(currentStep)) 
                            currentStep = null;
                    }
                }
            }
        }



        // -------------------- 
        // Private methods
        // -------------------

        private NavigationStep FindNextStep(List<NavigationStep> steps)
        {
            return steps.Find(s => !stepCompleted[s]); // The first not completed
        }


        private bool IsReached(NavigationStep currentStep)
        {
            if (!character)
            {
                character = FindObjectOfType<GeoPositionedCharacter>();
            }

            var mb = GetReference(currentStep.Reference);
            if (mb == null)
                return true; // If the element is not there, just try to skip it
            else if (mb is GeoWrapper)
            {
                var wrap = mb as GeoWrapper;
                var position = (Vector2d)wrap.Reference.TransformManagerParameters["Position"];
                var interactionRange = (float) wrap.Reference.TransformManagerParameters["InteractionRange"];
                
                var distance = GM.SeparationInMeters(position, character.LatLon);
                var realDistance = GM.SeparationInMeters(position, Input.location.lastData.LatLonD());

                // Is inside if the character is in range but also the real coords are saying so
                // Otherwise, if there is no character or gps, only one of the checks is valid

                return (!character || distance < interactionRange) 
                    && (!GPSController.Instance.IsStarted() || realDistance < interactionRange);
            }
            else if (mb is GeoElementMB)
            {
                var geomb = mb as GeoElementMB;

                // Is inside if the character is inside the influence but also the real coords are saying so
                // Otherwise, if there is no character or gps, only one of the checks is valid

                // TODO check if the line element is reached
                return (!character || geomb.Element.Geometry.InsideInfluence(character.LatLon)) 
                    && (!GPSController.Instance.IsStarted() || geomb.Element.Geometry.InsideInfluence(Input.location.lastData.LatLonD()));
            }
            else return true;
        }

        /// <summary>
        /// To complete the step, all the elements inside the step must be completed.
        /// For all the elements except for paths (LineString geometries) the elements are one, 
        /// so the step is completed in one element completition.
        /// </summary>
        /// <param name="currentStep"></param>
        private bool CompleteStep(NavigationStep currentStep)
        {
            bool completed = false;

            var elems = GetElementsFor(currentStep);
            if (elems == 1)
            {
                completed = true;
            }
            else
            {
                // If this is the first time we deal with this step, lets say we are in the 0
                if (!completedElementsForStep.ContainsKey(currentStep))
                    completedElementsForStep.Add(currentStep, 0);

                // Add one
                completedElementsForStep[currentStep]++;
                // Is completed if it matches in number
                completed = completedElementsForStep[currentStep] == elems;
            }

            // Only complete
            completed &= !currentStep.LockNavigation;

            stepCompleted[currentStep] = completed;
            return completed;
        }

        private int GetElementsFor(NavigationStep currentStep)
        {
            var mb = GetReference(currentStep.Reference);
            if(mb is GeoElementMB && (mb as GeoElementMB).Element.Geometry.Type == GMLGeometry.GeometryType.LineString)
            {
                // If it is a path, all the points are elements
                return (mb as GeoElementMB).Element.Geometry.Points.Count;
            }
            else
            {
                // Just itself is the element
                return 1;
            }
        }


        private void UpdateArrow(bool reached)
        {
            arrow.gameObject.SetActive(!reached && character);

            if(currentStep != null && character)
            {
                var pos = GetElementPosition(currentStep.Reference);
                var direction = GM.LatLonToMeters(pos) - GM.LatLonToMeters(character.LatLon);
                arrow.position = character.transform.position;
                arrow.rotation = Quaternion.Euler(0, Mathf.Atan2((float)direction.normalized.x, (float)direction.normalized.y)*Mathf.Rad2Deg, 0);
            }
        }
        

        /// <summary>
        /// Finds the closest step based on the element position. Only returns not completed steps.
        /// </summary>
        /// <param name="steps">All the steps</param>
        /// <returns>Closest not completed step</returns>
        private NavigationStep FindClosestStep(List<NavigationStep> steps)
        {
            if (!character)
            {
                character = FindObjectOfType<GeoPositionedCharacter>();
            }

            var latLon = character // If there is a character
                ? character.LatLon // use the position of the character to calculate distance
                : GPSController.Instance.IsLocationValid() // If not, but we have a location source
                    ? Input.location.lastData.LatLonD() // Use the location
                    : new Vector2d(double.NegativeInfinity, double.NegativeInfinity); // Otherwise, minus infinite, as the elements will be positioned at inifine

            return steps
                .FindAll(e => !stepCompleted[e]) // Filter the completed
                .FindMin(s => GM.SeparationInMeters(GetElementPosition(s.Reference), latLon)); // Order the rest to distance
        }

        /// <summary>
        /// Returns any element position (LatLon) by reference, looking for existing instances.
        /// </summary>
        /// <param name="reference">Reference id of the element</param>
        /// <returns>Position in Latitude Longitude Vector2d format</returns>
        private Vector2d GetElementPosition(string reference)
        {
            var mb = GetReference(reference);
            if (mb == null) return new Vector2d(double.NaN, double.NaN);

            if(mb is GeoElementMB)
            {
                return GetGeoElementPosition((mb as GeoElementMB));
            }
            else if(mb is GeoWrapper)
            {
                // Only works with geopositioned elements TODO make it compatible with the rest of types
                return (Vector2d) (mb as GeoWrapper).Reference.TransformManagerParameters["Position"];
            }

            return new Vector2d(double.PositiveInfinity, double.PositiveInfinity);
        }

        /// <summary>
        /// Returns a MonoBehaviour of type GeoElementMB or GeoWrapper that holds the referenced element.
        /// </summary>
        /// <param name="reference">Element reference id</param>
        /// <returns>The monobehaviour in case it exists</returns>
        private MonoBehaviour GetReference(string reference)
        {
            if (!referenceCache.ContainsKey(reference) || referenceCache[reference] == null)
            {
                referenceCache.Clear();
                FindObjectsOfType<GeoElementMB>().ToList().ForEach(geoElem => referenceCache.Add(geoElem.Reference.getTargetId(), geoElem));
                FindObjectsOfType<GeoWrapper>().ToList().ForEach(geoWrap => referenceCache.Add(geoWrap.Reference.getTargetId(), geoWrap));
            }

            var mb = referenceCache.ContainsKey(reference) ? referenceCache[reference] : null;
            return mb;
        }

        /// <summary>
        /// Returns the GeoElement position in the map. 
        /// The point is calculated based on the type of the geometry:
        ///  - Point: Just the point's position
        ///  - LineString: The first point's position
        ///  - Geometry: The closest point to the geometry
        /// </summary>
        /// <param name="geoElementMB">Element to return positioning of</param>
        /// <returns>The position in latitude and longitude Vector2d format</returns>
        private Vector2d GetGeoElementPosition(GeoElementMB geoElementMB)
        {
            switch (geoElementMB.Element.Geometry.Type)
            {
                case GMLGeometry.GeometryType.LineString:
                    return geoElementMB.Element.Geometry.Points[0];
                default:
                case GMLGeometry.GeometryType.Polygon:
                case GMLGeometry.GeometryType.Point:
                    return geoElementMB.Element.Geometry.Center;
            }
        }
    }

    public static class ExtensionLocationInfo
    {
        public static Vector2 LatLon(this LocationInfo l)
        {
            return new Vector2(l.latitude, l.longitude);
        }

        public static Vector2d LatLonD(this LocationInfo l)
        {
            return new Vector2d(l.latitude, l.longitude);
        }
    }

    public static class ExtensionList
    {
        public static T FindMin<T,TResult>(this List<T> l, Func<T,TResult> selector)
        {
            var min = l.Min(selector);
            return l.Find(e => selector(e).Equals(min));
        }
    }
}