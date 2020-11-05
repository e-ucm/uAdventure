/*
 * Copyright 2017 Universidad Complutense de Madrid and Open University of the Netherlands
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
namespace AssetPackage
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Net.Security;
	using System.Security.Cryptography.X509Certificates;
	using UnityEngine;
	using UniRx;
	using UnityEngine.Networking;
	using System.Collections;
    using System.Net;
    using System.Collections.Generic;
    using System.Text;

    public class UnityBridge : IBridge, IDataStorage, IAppend, IWebServiceRequest, ILog
	{
		readonly String StorageDir = Application.temporaryCachePath;
		/// <summary>
		/// Initializes a new instance of the asset_proof_of_concept_demo_CSharp.Bridge class.
		/// </summary>
		public UnityBridge()
		{
			if (!Directory.Exists(StorageDir))
			{
				Directory.CreateDirectory(StorageDir);
			}
		}

		#region IDataStorage Members
		/// <summary>
		/// Exists the given file.
		/// </summary>
		///
		/// <param name="fileId"> The file identifier to delete. </param>
		///
		/// <returns>
		/// true if it succeeds, false if it fails.
		/// </returns>
		public bool Exists(string fileId)
		{
			return File.Exists(Path.Combine(StorageDir, fileId));
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
			return Directory.GetFiles(StorageDir).ToList().ConvertAll(new Converter<String, String>(p => p.Replace(StorageDir + Path.DirectorySeparatorChar, ""))).ToArray();
		}
		/// <summary>
		/// Saves the given file.
		/// </summary>
		///
		/// <param name="fileId">   The file identifier to delete. </param>
		/// <param name="fileData"> Information describing the file. </param>
		public void Save(string fileId, string fileData)
		{
			File.WriteAllText(Path.Combine(StorageDir, fileId), fileData);
		}
		/// <summary>
		/// Loads the given file.
		/// </summary>
		///
		/// <param name="fileId"> The file identifier to delete. </param>
		///
		/// <returns>
		/// A String.
		/// </returns>
		public string Load(string fileId)
		{
			return File.ReadAllText(Path.Combine(StorageDir, fileId));
		}
		/// <summary>
		/// Deletes the given fileId.
		/// </summary>
		///
		/// <param name="fileId"> The file identifier to delete. </param>
		///
		/// <returns>
		/// true if it succeeds, false if it fails.
		/// </returns>
		public bool Delete(string fileId)
		{
			if (Exists(fileId))
			{
				File.Delete(Path.Combine(StorageDir, fileId));
				return true;
			}
			return false;
		}
		#endregion

		#region IDataStorage Members

		/// <summary>
		/// Appends fileData to the given fileId
		/// </summary>
		///
		/// <param name="fileId">   The file identifier to delete. </param>
		/// <param name="fileData"> Information describing the file. </param>
		public void Append(string fileId, string fileData)
		{
			File.AppendAllText(Path.Combine(StorageDir, fileId), fileData);
		}

		#endregion

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
					UnityEngine.Debug.Log("");
				}
				else
				{
					UnityEngine.Debug.Log(String.Format("{0}: {1}", severity, msg));
				}
			}
		}

		#endregion ILog Members

		#region IWebServiceRequest Members

		// See http://stackoverflow.com/questions/12224602/a-method-for-making-http-requests-on-unity-ios
		// for persistence.
		// See http://18and5.blogspot.com.es/2014/05/mono-unity3d-c-https-httpwebrequest.html

		/// <summary>
		/// Web service request.
		/// </summary>
		///
		/// <param name="requestSettings">  Options for controlling the operation. </param>
		/// <param name="requestResponse"> The request response. </param>
		public void WebServiceRequest(RequestSetttings requestSettings, out RequestResponse requestResponse)
		{
			RequestResponse requestResponseAux = null;
			// This method is called synchronous, so the anonymous method is always called before the next line
			WebServiceRequestAux(requestSettings, false, response => requestResponseAux = response);
			requestResponse = requestResponseAux;
		}

		public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			bool isOk = true;
			// If there are errors in the certificate chain, look at each error to determine the cause.
			if (sslPolicyErrors != SslPolicyErrors.None)
			{
				for (int i = 0; i < chain.ChainStatus.Length; i++)
				{
					if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
					{
						chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
						chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
						chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
						chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
						bool chainIsValid = chain.Build((X509Certificate2)certificate);
						if (!chainIsValid)
						{
							isOk = false;
						}
					}
				}
			}
			return isOk;
		}

		/// <summary>
		/// Web service request.
		/// </summary>
		///
		/// <param name="requestSettings">Options for controlling the operation. </param>
		///
		/// <returns>
		/// A RequestResponse.
		/// </returns>
		public void WebServiceRequestAsync(RequestSetttings requestSettings, Action<RequestResponse> callback)
		{
			WebServiceRequestAux(requestSettings, true, callback);
		}

		public void WebServiceRequestAux(RequestSetttings requestSettings, bool async, Action<RequestResponse> callback)
		{
			RequestResponse result = new RequestResponse(requestSettings);

			try
			{
				UnityWebRequest request = null;

				switch (requestSettings.method.ToUpper())
				{
					case "GET":
						request = UnityWebRequest.Get(requestSettings.uri);
						break;
					case "POST":
						request = UnityWebRequest.Post(requestSettings.uri, "");
						break;
					case "PUT":
						request = UnityWebRequest.Put(requestSettings.uri, "");
						break;
					case "DELETE":
						request = UnityWebRequest.Delete(requestSettings.uri);
						break;
				}

                if (!string.IsNullOrEmpty(requestSettings.body))
                {
					byte[] bytes = Encoding.UTF8.GetBytes(requestSettings.body);
					UploadHandlerRaw uH = new UploadHandlerRaw(bytes);
                    if (requestSettings.requestHeaders.ContainsKey("Content-Type"))
					{
						uH.contentType = requestSettings.requestHeaders["Content-Type"];
					}
					request.uploadHandler = uH;
				}

				foreach (var header in requestSettings.requestHeaders)
				{
					request.SetRequestHeader(header.Key, header.Value);
				}

                if (async || Application.platform == RuntimePlatform.WebGLPlayer)
				{
					Observable.FromCoroutine(() => DoRequest(request, operation =>
					{
						var uwr = operation.webRequest;
						CopyResponse(result, uwr);
						callback(result);
					})).Subscribe();
                }
                else
				{
					request.SendWebRequest();
					while (!request.isDone) { }
					CopyResponse(result, request);
					callback(result);
				}
			}
			catch (Exception e)
			{
				result.responsMessage = e.Message;
				Log(Severity.Error, String.Format("{0} - {1}", e.GetType().Name, e.Message));
			}
		}

		private static void CopyResponse(RequestResponse result, UnityWebRequest uwr)
		{
			result.body = uwr.downloadHandler.text;
			result.binaryResponse = uwr.downloadHandler.data;
			result.responseCode = (int)uwr.responseCode;
			result.responseHeaders = uwr.GetResponseHeaders();
		}

		private static IEnumerator DoRequest(UnityWebRequest webRequest, Action<UnityWebRequestAsyncOperation> done)
		{
			var asyncOp = webRequest.SendWebRequest();
			asyncOp.completed += a => done((UnityWebRequestAsyncOperation)a);
			yield return asyncOp;
		}

		#endregion IWebServiceRequest Members

	}

}