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
using System.IO;
using UnityEngine;

namespace RAGE.Analytics.Storages
{
	public class LocalStorage : Storage
	{
		private const string Separator = "--session--,";
		private string tracesPathPrefix;
		private string tracesFile;
			
		public LocalStorage (string tracesPathPrefix)
		{
			this.tracesPathPrefix = tracesPathPrefix;
		}
					
		public void SetTracker (Tracker tracker)
		{
		}
			
		public void Start (Net.IRequestListener startListener)
		{
			string now = System.DateTime.Now.ToString ().Replace('/', '_').Replace(':', '_');
			tracesFile = tracesPathPrefix + ".csv";	
			Write("\n" + Separator + now + "\n", startListener);
		}

		public void Send (String data, Net.IRequestListener flushListener)
		{
			Write (data, flushListener);
		}

		private void Write (String data, Net.IRequestListener requestListener)
		{

	#if UNITY_WEBGL
			requestListener.Error ("Impossible to use LocalStorage in WebGL version");
	#elif UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
			try {
				File.AppendAllText (tracesFile, data);
				requestListener.Result ("");
			} catch (Exception e) {
				requestListener.Error (e.Message);
			}	
	#endif
		}

		public bool IsAvailable ()
		{
	#if UNITY_WEBGL
		return false;
	#else
			return true;
	#endif
		}

		public void CleanFile ()
		{
	#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
			if (File.Exists (tracesFile))
			{
				File.WriteAllText (tracesFile, "");
			}
	#endif
		}

		public List<string> RecoverData ()
		{
			List<String> tracesList = new List<String>();
	#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
			if (File.Exists(tracesFile))
			{
				string file = tracesFile;
				string data = File.ReadAllText(file);
				String[] traces = data.Split(new Char[] { ';', '\n' }, StringSplitOptions.RemoveEmptyEntries);

				foreach (String s in traces)
				{
					if (!String.IsNullOrEmpty(s) && s.Substring(0, Separator.Length) != Separator)
					{
						tracesList.Add(s);
					}
				}
			}
	#endif
			return tracesList;
		}
	}
}


