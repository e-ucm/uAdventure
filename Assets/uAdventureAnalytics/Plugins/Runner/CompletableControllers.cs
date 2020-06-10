using UnityEngine;
using System.Collections.Generic;
using System;

using uAdventure.Core;
using System.Linq;
using uAdventure.Runner;
using System.Collections;

namespace uAdventure.Analytics
{
    [Serializable]
    public class CompletableControllers : IList<CompletableController>
    {
        [SerializeField]
        private List<CompletableController> controllers = new List<CompletableController>();

        public CompletableControllers()
        {
        }

        public CompletableController this[int index]
        {
            get
            {
                return controllers[index];
            }
            set
            {
                controllers[index] = value;
            }
        }

        public int Count { get { return controllers.Count; } }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(CompletableController item)
        {
            controllers.Add(item);
        }

        public void Clear()
        {
            controllers.Clear();
        }

        public bool Contains(CompletableController item)
        {
            return controllers.Contains(item);
        }

        public void CopyTo(CompletableController[] array, int arrayIndex)
        {
            controllers.CopyTo(array, arrayIndex);
        }

        public IEnumerator<CompletableController> GetEnumerator()
        {
            return controllers.GetEnumerator();
        }

        public int IndexOf(CompletableController item)
        {
            return controllers.IndexOf(item);
        }

        public void Insert(int index, CompletableController item)
        {
            controllers.Insert(index, item);
        }

        public bool Remove(CompletableController item)
        {
            return controllers.Remove(item);
        }

        public void RemoveAt(int index)
        {
            controllers.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return controllers.GetEnumerator();
        }

        public void AddRange(IEnumerable<CompletableController> completableControllers)
        {
            controllers.AddRange(completableControllers);
        }
    }
}
