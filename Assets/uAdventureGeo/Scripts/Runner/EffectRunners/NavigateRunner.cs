using UnityEngine;
using System.Collections;

using uAdventure.Runner;
using System;
using uAdventure.Core;

namespace uAdventure.Geo
{
    [CustomEffectRunner(typeof(NavigateEffect))]
    public class NavigateRunner : CustomEffectRunner
    {
        private NavigateEffect navEffect;
        public IEffect Effect { get { return navEffect; } set { navEffect = value as NavigateEffect; } }
        
        public NavigateRunner()
        {
            if(NavigationController.Instance == null)
            {
                var navigationPrefab = Resources.Load<GameObject>("navigation");
                if (navigationPrefab != null)
                {
                    GameObject.Instantiate(navigationPrefab);
                }
            }
        }

        public bool execute()
        {
            NavigationController.Instance.NavigationStrategy = navEffect.NavigationType;
            NavigationController.Instance.Steps = navEffect.Steps;
            NavigationController.Instance.Navigate();

            return false;
        }
    }
}