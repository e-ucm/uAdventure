using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace uAdventure.Runner
{
    public class uAdventureRaycaster : PhysicsRaycaster
    {
        private RaycastHit[] m_Hits;
        private RaycastHit2D[] m_Hits2D;

        public bool Disabled { get; set; }
        public GameObject Override { get; set; }
        public GameObject Base { get; set; }
        public static uAdventureRaycaster Instance { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Disabled = false;
            Instance = this;
        }

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            if (Disabled)
                return;

            if (Override != null)
            {
                var result = new RaycastResult
                {
                    gameObject = Override,
                    module = this,
                    screenPosition = eventData.position,
                    index = resultAppendList.Count,
                    sortingLayer = SortingLayer.NameToID("Overrides"),
                    sortingOrder = int.MaxValue,
                    distance = 0,
                    depth = int.MaxValue
                };
                resultAppendList.Add(result);
                return;
                
            }

            if (eventCamera == null)
                return;

            Ray ray = new Ray();
            float distanceToClipPlane = 0f;

#if UNITY_2019_3_OR_NEWER || (!UNITY_2017_4_1 && !UNITY_2017_4_2 && !UNITY_2017_4_3 && !UNITY_2017_4_4 &&!UNITY_2017_4_5 &&!UNITY_2017_4_6 &&!UNITY_2017_4_7 &&!UNITY_2017_4_8 &&!UNITY_2017_4_9 &&!UNITY_2017_4_10 &&!UNITY_2017_4_11 &&!UNITY_2017_4_12 &&!UNITY_2017_4_13 &&!UNITY_2017_4_14 &&!UNITY_2017_4_15 &&!UNITY_2017_4_16 &&!UNITY_2017_4_17 &&!UNITY_2017_4_18 &&!UNITY_2017_4_19 &&!UNITY_2017_4_20 &&!UNITY_2017_4_21 && !UNITY_2017_4_22 && !UNITY_2017_4_23 && !UNITY_2017_4_24 && !UNITY_2017_4_25 && !UNITY_2017_4_26 && !UNITY_2017_4_27 && !UNITY_2017_4_28 && !UNITY_2017_4_29 && !UNITY_2017_4_30 && !UNITY_2017_4_31 && !UNITY_2017_4_32 && !UNITY_2017_4_33 && !UNITY_2017_4_34)
            int eventDisplayIndex = 0;
            ComputeRayAndDistance(eventData, ref ray, ref eventDisplayIndex, ref distanceToClipPlane);
#else
            ComputeRayAndDistance(eventData, out ray, out distanceToClipPlane);
#endif




            int hitCount = 0, hitCount2D = 0;

            if (m_MaxRayIntersections == 0)
            {
                m_Hits = Physics.RaycastAll(ray, distanceToClipPlane, finalEventMask);
                hitCount = m_Hits.Length;
            }
            else
            {
                if (m_LastMaxRayIntersections != m_MaxRayIntersections)
                {
                    m_Hits = new RaycastHit[m_MaxRayIntersections];
                    m_LastMaxRayIntersections = m_MaxRayIntersections;
                }

                hitCount = Physics.RaycastNonAlloc(ray, m_Hits, distanceToClipPlane, finalEventMask);
            }

            m_Hits2D = Physics2D.GetRayIntersectionAll(ray, distanceToClipPlane, finalEventMask);
            hitCount2D = m_Hits2D.Length;

            if (hitCount > 1)
            {
                System.Array.Sort(m_Hits, (r1, r2) => r1.distance.CompareTo(r2.distance));
            }

            if (hitCount != 0)
            {
                for (int b = 0, bmax = hitCount; b < bmax; ++b)
                {
                    var hitTransparent = m_Hits[b].collider.gameObject.GetComponent<Transparent>();

                    if (hitTransparent && hitTransparent.CheckTransparency(m_Hits[b]))
                    {
                        continue;
                    }
    
                    var result = new RaycastResult
                    {
                        gameObject = m_Hits[b].collider.gameObject,
                        module = this,
                        distance = m_Hits[b].distance,
                        worldPosition = m_Hits[b].point,
                        worldNormal = m_Hits[b].normal,
                        screenPosition = eventData.position,
                        index = resultAppendList.Count,
                        sortingLayer = 0,
                        sortingOrder = 0
                    };
                    resultAppendList.Add(result);
                }
            }

            if (hitCount2D != 0)
            {
                for (int b = 0, bmax = hitCount2D; b < bmax; ++b)
                {
                    var sr = m_Hits2D[b].collider.gameObject.GetComponent<SpriteRenderer>();

                    var result = new RaycastResult
                    {
                        gameObject = m_Hits2D[b].collider.gameObject,
                        module = this,
                        distance = Vector3.Distance(eventCamera.transform.position, m_Hits2D[b].point),
                        worldPosition = m_Hits2D[b].point,
                        worldNormal = m_Hits2D[b].normal,
                        screenPosition = eventData.position,
                        index = resultAppendList.Count,
                        sortingLayer = sr != null ? sr.sortingLayerID : 0,
                        sortingOrder = sr != null ? sr.sortingOrder : 0
                    };
                    resultAppendList.Add(result);
                }
            }

            if (Base != null)
            {
                var result = new RaycastResult
                {
                    gameObject = Base,
                    module = this,
                    screenPosition = eventData.position,
                    index = -1,
                    sortingLayer = 0,
                    sortingOrder = 0,
                    distance = int.MinValue,
                    depth = int.MinValue
                };
                resultAppendList.Add(result);
            }
        }

    }
}