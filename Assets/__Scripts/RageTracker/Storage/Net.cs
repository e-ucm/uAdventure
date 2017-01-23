/*
 * Copyright 2016 e-UCM (http://www.e-ucm.es/)
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * This project has received funding from the European Union’s Horizon
 * 2020 research and innovation programme under grant agreement No 644187.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0 (link is external)
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace uAdventure.RageTracker
{
    public class Net
    {
        private MonoBehaviour behaviour;

        public Net(MonoBehaviour behaviour)
        {
            this.behaviour = behaviour;
        }

        public WWW GET(string url, IRequestListener requestListener)
        {

            WWW www = new WWW(url);
            behaviour.StartCoroutine(WaitForRequest(www, requestListener));
            return www;
        }

        public WWW POST(string url, byte[] data, Dictionary<string, string> headers, IRequestListener requestListener)
        {
            // Force post
            if (data == null)
            {
                data = Encoding.UTF8.GetBytes(" ");
            }
            WWW www = new WWW(url, data, headers);

            behaviour.StartCoroutine(WaitForRequest(www, requestListener));
            return www;
        }

        private IEnumerator WaitForRequest(WWW www, IRequestListener requestListener)
        {
            yield return www;
            // check for errors
            if (www.error == null)
            {
                requestListener.Result(www.text);
            }
            else
            {
                Debug.LogError(www.error);
                requestListener.Error(www.error);
            }
        }

        public interface IRequestListener
        {

            void Result(string data);

            void Error(string error);
        }
    }
}