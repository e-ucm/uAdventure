using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simva
{
    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner instance;

#if UNITY_EDITOR
        public delegate void ExecuteCoroutineInEditorDelegate(IEnumerator routine);
        public ExecuteCoroutineInEditorDelegate ExecuteCoroutineInEditor; 
#endif

        public static CoroutineRunner Instance 
        { 
            get 
            { 
                if(instance == null)
                {
                    var requestRunnerGo = new GameObject("CoroutineRunner", typeof(CoroutineRunner))
                    {
                        hideFlags = HideFlags.HideAndDontSave
                    };
                    instance = requestRunnerGo.GetComponent<CoroutineRunner>();
                    if (Application.isPlaying)
                    {
                        GameObject.DontDestroyOnLoad(requestRunnerGo);
                    }
                }
                return instance;
            } 
        }

        public void RunRoutine(IEnumerator routine)
        {
            if (Application.isPlaying)
            {
                StartCoroutine(routine);
            }
            else
            {
#if UNITY_EDITOR
                if(ExecuteCoroutineInEditor != null)
                {
                    ExecuteCoroutineInEditor(routine);
                }
#endif
            }
        }

    }
}

