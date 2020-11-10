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
        private List<string> keys = new List<string>();
        [SerializeField]
        private List<string> stringValues = new List<string>();
        [SerializeField]
        private List<int> intValues = new List<int>();
        [SerializeField]
        private List<Vector2> vector2Values = new List<Vector2>();
        [SerializeField]
        private List<Vector2d> vector2dValues = new List<Vector2d>();
        [SerializeField]
        private List<Vector3> vector3Values = new List<Vector3>();
        [SerializeField]
        private List<Vector3d> vector3dValues = new List<Vector3d>();
        [SerializeField]
        private List<float> floatValues = new List<float>();
        [SerializeField]
        private List<bool> boolValues = new List<bool>();
        [SerializeField]
        private List<string> types = new List<string>();

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
            memories = new Dictionary<string, object>();
            for(int i = 0; i<keys.Count; i++)
            {
                if(types[i] == typeof(string).FullName)
                {
                    var s = stringValues[0];
                    stringValues.RemoveAt(0);
                    memories.Add(keys[i], s);
                }
                if (types[i] == typeof(Vector2).FullName)
                {
                    var v2 = vector2Values[0];
                    vector2Values.RemoveAt(0);
                    memories.Add(keys[i], v2);
                }
                if (types[i] == typeof(Vector2d).FullName)
                {
                    var v2d = vector2dValues[0];
                    vector2dValues.RemoveAt(0);
                    memories.Add(keys[i], v2d);
                }
                if (types[i] == typeof(Vector3).FullName)
                {
                    var v3 = vector3Values[0];
                    vector3Values.RemoveAt(0);
                    memories.Add(keys[i], v3);
                }
                if (types[i] == typeof(Vector3d).FullName)
                {
                    var v3d = vector3dValues[0];
                    vector3dValues.RemoveAt(0);
                    memories.Add(keys[i], v3d);
                }
                if (types[i] == typeof(int).FullName)
                {
                    var intval = intValues[0];
                    intValues.RemoveAt(0);
                    memories.Add(keys[i], intval);
                }
                if (types[i] == typeof(float).FullName)
                {
                    var f = floatValues[0];
                    floatValues.RemoveAt(0);
                    memories.Add(keys[i], f);
                }
                if (types[i] == typeof(bool).FullName)
                {
                    var b = boolValues[0];
                    boolValues.RemoveAt(0);
                    memories.Add(keys[i], b);
                }
            }
        }

        public void OnBeforeSerialize()
        {
            keys = memories.Keys.ToList();
            boolValues = new List<bool>();
            stringValues = new List<string>();
            vector2Values = new List<Vector2>();
            vector2dValues = new List<Vector2d>();
            vector3Values = new List<Vector3>();
            vector3dValues = new List<Vector3d>();
            floatValues = new List<float>();
            intValues = new List<int>();
            foreach(var elem in memories.Values)
            {
                if(elem is string)
                {
                    stringValues.Add(elem as string);
                }
                if (elem is Vector2)
                {
                    vector2Values.Add((Vector2)elem);
                }
                if (elem is Vector2d)
                {
                    vector2dValues.Add((Vector2d)elem);
                }
                if (elem is Vector3)
                {
                    vector2Values.Add((Vector2)elem);
                }
                if (elem is Vector3d)
                {
                    vector2dValues.Add((Vector3d)elem);
                }
                if (elem is float)
                {
                    floatValues.Add((float)elem);
                }
                if (elem is int)
                {
                    intValues.Add((int)elem);
                }
                if (elem is bool)
                {
                    boolValues.Add((bool)elem);
                }
            }

            types = memories.Values.Select(v => v.GetType().FullName).ToList();
        }
    }
}