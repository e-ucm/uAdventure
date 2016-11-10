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
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using System.Collections;

public class NetStorage : Storage
{
	public const string start = "start/";
	public const string track = "track/";
	private Net net;
	private string host;
	private string trackingCode;
	private string authorization;
	private NetStartListener netStartListener;
	private Dictionary<string,string> trackHeaders = new Dictionary<string, string> ();

	/// <summary>
	/// </summary>
	/// <param name="net">An object to interact with the network.</param>
	/// <param name="host">Host of the collector server.</param>
	/// <param name="trackingCode">Tracking code for the game.</param>
	/// <param name="authorization">Authorization to start the tracking.</param>
	public NetStorage (MonoBehaviour behaviour, string host, string trackingCode)
	{
		this.net = new Net (behaviour);
		this.host = host;
		this.trackingCode = trackingCode;
		this.authorization = "a:";

		string filePath = Application.dataPath + "/Assets/track.txt";

		if (Application.platform != RuntimePlatform.WebGLPlayer)
		{
			filePath = "file:///" + filePath;
		}

		WWW www = new WWW(filePath);

		behaviour.StartCoroutine(WaitForRequest(www));
	}

	/*
	* If exist /Assets/tracker.txt in the root of proyect folder with the format
	*          host;trackingCode
	* the tracker will use this host and trackingCode for the connection.
	*/
	private IEnumerator WaitForRequest(WWW www)
	{
		yield return www;
		// check for errors
		if (www.error == null)
		{
			string line = www.text;
			string[] readLine = line.Split(';');
			if (readLine.Length == 2)
			{
				host = readLine[0];
				trackingCode = readLine[1];
			}
		}
	}

	public void SetTracker (Tracker tracker)
	{
		netStartListener = new NetStartListener (tracker, this);
		netStartListener.SetTraceFormatter (tracker.GetTraceFormatter());
		trackHeaders.Add ("Content-Type", "application/json");
	}

	public void Start (Net.IRequestListener startListener)
	{
		Dictionary<string,string> headers = new Dictionary<string, string> ();
		headers.Add ("Authorization", authorization);		
		net.POST (host + start + trackingCode, null, headers, netStartListener);
	}

	public void Send (String data, Net.IRequestListener flushListener)
	{
		net.POST (host + track, System.Text.Encoding.UTF8.GetBytes (data), trackHeaders, flushListener);
	}

	private void SetAuthToken (string authToken)
	{
		trackHeaders["Authorization"] = authToken;
    }

	public class NetStartListener : Tracker.StartListener
	{
		private NetStorage storage;

		public NetStartListener (Tracker tracker, NetStorage storage) : base(tracker)
		{
			this.storage = storage;
		}

		protected override void ProcessData (JSONNode dict)
		{
			storage.SetAuthToken (dict ["authToken"]);
			base.ProcessData (dict);
		}
	}

	public bool IsAvailable()
	{
		if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ||
			Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
		{
			return true;
		}
		return false;
	}
}


