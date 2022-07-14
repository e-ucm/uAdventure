using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using Ionic.Zip;
using System.Text;
using System.IO;

public class ZipUtil
{
	static float progress = 0;
	public static float Progress{
		get{ return progress; }
	}

#if UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern void unzip (string zipFilePath, string location);

	[DllImport("__Internal")]
	private static extern void zip (string zipFilePath);

	[DllImport("__Internal")]
	private static extern void addZipFile (string addFile);
#else
	ZipFile current;

	static void zip_ExtractProgress(object sender, ExtractProgressEventArgs e)
	{
		if(e.EventType == ZipProgressEventType.Extracting_AfterExtractEntry)
			progress = (float)e.EntriesExtracted / (float)e.EntriesTotal;
	}
#endif

	public static void Unzip (string zipFilePath, string location)
	{
		Debug.Log ("Entrando en unzip");
#if UNITY_ANDROID
		using (AndroidJavaClass zipper = new AndroidJavaClass ("com.tsw.zipper")) {
			zipper.CallStatic ("unzip", zipFilePath, location);
		}
#elif UNITY_IPHONE
		unzip (zipFilePath, location);
#else
		Debug.Log ("UNZIP WINDOWS");
		Directory.CreateDirectory (location);

		Debug.Log ("DIRECTORIO CREADO");

		using (ZipFile zip = ZipFile.Read (zipFilePath)) {
			//current = zip;
			zip.ExtractAll (location, ExtractExistingFileAction.OverwriteSilently);
		}

		Debug.Log ("TERMINADO");

		/*int n;
		using(ZipFile zip = ZipFile.Read(zipFilePath))
		{                
			zip.ExtractProgress += zip_ExtractProgress;
			n = 0;
			foreach (ZipEntry entry in zip)
			{
				n++;
				entry.Extract(location, ExtractExistingFileAction.OverwriteSilently);                    
				}
		}*/
#endif
	}

	public static void Zip (string zipFileName, params string[] files)
	{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
		string path = Path.GetDirectoryName(zipFileName);
		Directory.CreateDirectory (path);
		
		using (ZipFile zip = new ZipFile()) {
			foreach (string file in files) {
				zip.AddFile(file, "");
			}
			zip.Save (zipFileName);
		}
#elif UNITY_ANDROID
		using (AndroidJavaClass zipper = new AndroidJavaClass ("com.tsw.zipper")) {
			{
				zipper.CallStatic ("zip", zipFileName, files);
			}
		}
#elif UNITY_IPHONE
		foreach (string file in files) {
			addZipFile (file);
		}
		zip (zipFileName);
#endif
	}
}
