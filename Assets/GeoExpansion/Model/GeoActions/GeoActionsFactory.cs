using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace uAdventure.Geo
{
    public class GeoActionsFactory
    {

        protected static GeoActionsFactory instance;
        public static GeoActionsFactory Instance
        {
            get
            {
                if (instance == null)
                    instance = new GeoActionsFactory();

                return instance;
            }
        }

        protected GeoActionsFactory()
        {
            actionsList = new List<GeoAction>();

            // Add actions here
            actionsList.Add(new EnterAction());
            actionsList.Add(new ExitAction());
            actionsList.Add(new InspectAction());
            actionsList.Add(new LookToAction());
            // End add

            avaliableActions = new Dictionary<Type, string>();
            actionsList.ForEach(a => avaliableActions.Add(a.GetType(), a.Name));
        }

        protected List<GeoAction> actionsList;
        protected Dictionary<Type, string> avaliableActions;
        public Dictionary<Type, string> AvaliableActions { get { return avaliableActions; } }

        public GeoAction CreateActionFor(Type type)
        {
            return avaliableActions.ContainsKey(type) ? actionsList.Find(a => a.GetType() == type).Clone() as GeoAction : null;
        }

        public GeoAction CreateActionFor(string name)
        {
            return avaliableActions.ContainsValue(name) ? actionsList.Find(a => a.Name == name).Clone() as GeoAction : null;
        }
    }   

}