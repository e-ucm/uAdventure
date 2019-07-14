using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace uAdventure.Runner
{
    [Serializable]
    public class Memory : ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<string> keys;
        [SerializeField]
        private List<string> values;
        [SerializeField]
        private List<string> types;
        
        private Dictionary<string, object> memories;

        public Memory()
        {
            memories = new Dictionary<string, object>();
        }

        public void Set<T>(string remember, T value)
        {
            memories[remember] = value;
        }


        public T Get<T>(string remember)
        {
            if (memories.ContainsKey(remember)&& memories[remember] is T)
            {
                return (T) memories[remember];
            }

            return default(T);
        }

        public void OnAfterDeserialize()
        {
            
        }

        public void OnBeforeSerialize()
        {
            keys = memories.Keys.ToList();
            values = memories.Values.Select(v => JsonUtility.ToJson(v)).ToList();
            types = memories.Values.Select(v => v.GetType().ToString()).ToList();
        }
    }
}