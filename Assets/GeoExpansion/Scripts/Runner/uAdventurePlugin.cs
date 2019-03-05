﻿using UnityEngine;
using System.Collections;
using MapzenGo.Models.Plugins;
using MapzenGo.Models;
using System;
using System.Collections.Generic;

using uAdventure.Runner;
using System.Runtime.CompilerServices;

namespace uAdventure.Geo
{
    public class uAdventurePlugin : Plugin
    {
        public MapSceneMB MapSceneMB { get; set; }

        public List<MapElement> OrphanElements
        {
            get; private set;
        }

        public List<MapElement> AdoptedElements
        {
            get; private set;
        }

        void Awake()
        {
            OrphanElements = new List<MapElement>();
            AdoptedElements = new List<MapElement>();
        }

        protected override IEnumerator CreateRoutine(Tile tile, Action<bool> finished)
        {
            
            var allElements = MapSceneMB.MapElements.FindAll(elem => elem.Conditions == null || ConditionChecker.check(elem.Conditions));
            foreach(var elem in allElements)
            {
                if (!AdoptedElements.Contains(elem) && !OrphanElements.Contains(elem))
                {
                    OrphanElements.Add(elem);
                }
            }

            finished(true);

            yield return null;
        }

        protected override IEnumerator UnLoadRoutine(Tile tile, Action<bool> finished)
        {

            var notExisting = MapSceneMB.MapElements.FindAll(elem => elem.Conditions != null && !ConditionChecker.check(elem.Conditions));
            foreach (var elem in notExisting)
            {
                if (OrphanElements.Contains(elem))
                {
                    OrphanElements.Remove(elem);
                }
            }

            finished(true);

            yield return null;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool AdoptElement(MapElement mapElement)
        {
            if(OrphanElements.Contains(mapElement))
            {
                AdoptedElements.Add(mapElement);
                OrphanElements.Remove(mapElement);
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ReleaseElement(MapElement mapElement)
        {
            if (AdoptedElements.Contains(mapElement))
            {
                AdoptedElements.Remove(mapElement);
                if(mapElement.Conditions == null || ConditionChecker.check(mapElement.Conditions))
                {
                    OrphanElements.Add(mapElement);
                }
            }
        }
    }
}
