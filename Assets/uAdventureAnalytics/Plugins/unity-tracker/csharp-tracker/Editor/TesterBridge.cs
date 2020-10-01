/*
 * Copyright 2016 Open University of the Netherlands
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * This project has received funding from the European Union’s Horizon
 * 2020 research and innovation programme under grant agreement No 644187.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace TrackerAssetUnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using AssetPackage;
    using System.Net;

    public class TesterBridge : IBridge, ILog, IDataStorage, IAppend, IWebServiceRequest
    {
        readonly String StorageDir = String.Format(@".{0}TestStorage", Path.DirectorySeparatorChar);
        /// <summary>
        /// Initializes a new instance of the asset_proof_of_concept_demo_CSharp.Bridge class.
        /// </summary>
        public TesterBridge()
        {
            /*if (!Directory.Exists(StorageDir))
            {
                Directory.CreateDirectory(StorageDir);
            }*/
        }

        #region IDataStorage Members

        Dictionary<string, string> files = new Dictionary<string, string>();

        public bool Exists(string fileId)
        {
            return files.ContainsKey(fileId);
        }
        /// <summary>
        /// Gets the files.
        /// </summary>
        ///
        /// <returns>
        /// A List&lt;String&gt;
        /// </returns>
        public String[] Files()
        {
            /*return Directory.GetFiles(StorageDir).ToList().ConvertAll(
                new Converter<String, String>(p => p.Replace(StorageDir + Path.DirectorySeparatorChar, ""))).ToArray();*/
            return null;
            //! EnumerateFiles not supported in Unity3D.
            // 
            //return Directory.EnumerateFiles(StorageDir).ToList().ConvertAll(
            //    new Converter<String, String>(p => p.Replace(StorageDir +  Path.DirectorySeparatorChar, ""))).ToList();
        }

        public void Save(string fileId, string fileData)
        {
            if (Exists(fileId))
                files[fileId] = fileData;
            else
                files.Add(fileId, fileData);
        }

        public string Load(string fileId)
        {
            string content = "";

            if (Exists(fileId))
                content = files[fileId];

            return content;
        }

        public bool Delete(string fileId)
        {
            if (Exists(fileId))
                files.Remove(fileId);

            return true;
        }
        #endregion

        #region IAppend Members

        public void Append(string fileId, string fileData)
        {
            if (Exists(fileId))
                files[fileId] = files[fileId] + fileData;
            else
                files.Add(fileId, fileData);
        }

        #endregion IAppend Members

        #region ILog Members

        /// <summary>
        /// Executes the log operation.
        /// 
        /// Implement this in Game Engine Code.
        /// </summary>
        ///
        /// <param name="severity"> The severity. </param>
        /// <param name="msg">      The message. </param>
        public void Log(Severity severity, string msg)
        {
            // if (((int)LogLevel.Info & (int)severity) == (int)severity)
            {
                if (String.IsNullOrEmpty(msg))
                {
                    Console.WriteLine("");
                }
                else
                {
                    Console.WriteLine(String.Format("{0}: {1}", severity, msg));
                }
            }
        }

        #endregion ILog Members

        #region IWebServiceRequest Members
        bool connected = true;
        public bool Connnected
        {
            get { return this.connected; }
            set { this.connected = value; }
        }

        public void WebServiceRequestAsync(RequestSetttings requestSettings, Action<RequestResponse> callback)
        {
            var response = this.WebServiceRequest(requestSettings);
            callback(response);
        }
        public void WebServiceRequest(RequestSetttings requestSettings, out RequestResponse requestResponse)
        {
            requestResponse = this.WebServiceRequest(requestSettings);
        }



        private RequestResponse WebServiceRequest(RequestSetttings requestSettings)
        {
            RequestResponse result = new RequestResponse(requestSettings);

            if (connected)
            {
                result.responseCode = 200;
                result.body = SimpleJSON.JSON.Parse("{"
                    + "\"authToken\": \"5a26cb5ac8b102008b41472b5a30078bc8b102008b4147589108928341\", "
                    + "\"actor\": { \"account\": { \"homePage\": \"http://a2:3000/\", \"name\": \"Anonymous\"}, \"name\": \"test-animal-name\"}, "
                    + "\"playerAnimalName\": \"test-animal-name\", "
                    + "\"playerId\": \"5a30078bc8b102008b41475769103\", "
                    + "\"objectId\": \"http://a2:3000/api/proxy/gleaner/games/5a26cb5ac8b102008b41472a/5a26cb5ac8b102008b41472b\", "
                    + "\"session\": 1, "
                    + "\"firstSessionStarted\": \"2017-12-12T16:44:59.273Z\", "
                    + "\"currentSessionStarted\": \"2017-12-12T16:44:59.273Z\" "
                    + "}").ToString();

                this.Append("netstorage", requestSettings.body);
            }
            else
            {
                result.responseCode = 0;
            }

            return result;
        }

#endregion IWebServiceRequest Members
    }
}