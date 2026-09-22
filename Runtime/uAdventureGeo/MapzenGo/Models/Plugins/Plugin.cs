using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapzenGo.Models;
using UnityEngine;

namespace MapzenGo.Models.Plugins
{
    public class Plugin : MonoBehaviour
    {
        public virtual List<Plugin> Dependencies
        {
            get
            {
                // no dependencies by default
                return new List<Plugin>();
            }
        }

        public virtual void Create(Tile tile, Action<Plugin, bool> finished)
        {
            StartCoroutine(CreateRoutine(tile, success => finished(this, success)));
        }

        protected virtual IEnumerator CreateRoutine(Tile tile, Action<bool> finished)
        {
            finished(true);
            yield return null;
        }

        public virtual void UnLoad(Tile tile, Action<Plugin, bool> onFinish)
        {
            StartCoroutine(UnLoadRoutine(tile, success => onFinish(this, success)));
        }

        protected virtual IEnumerator UnLoadRoutine(Tile tile, Action<bool> finished)
        {
            finished(true);
            yield return null;
        }
    }
}
