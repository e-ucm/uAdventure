using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.Collections.Generic;
using System;
using RAGE.Analytics;
using RAGE.Analytics.Formats;
using RAGE.Analytics.Exceptions;
using SimpleJSON;

public class TrackerOpened : Tracker
{
	public List<String> GetQueue()
	{
		return base.queue;
	}
}

public class TrackerTest {
	
	private TrackerOpened t;

	private void initTracker(string format){
		t = new TrackerOpened();
		t.traceFormat = format;
		t.Start();
	}

	[Test]
	public void ActionTraceTest() {
		initTracker ("csv");

		t.ActionTrace("Verb", "Type", "ID");
		CheckCSVTrace("Verb,Type,ID");
		t.GetQueue().Clear();

		t.ActionTrace("Verb", "Ty,pe", "ID");
		CheckCSVTrace("Verb,Ty\\,pe,ID");
		t.GetQueue().Clear();

		t.ActionTrace("Verb", "Type", "I,D");
		CheckCSVTrace("Verb,Type,I\\,D");
		t.GetQueue().Clear();

		t.ActionTrace("Ve,rb", "Type", "ID");
		CheckCSVTrace("Ve\\,rb,Type,ID");
		t.GetQueue().Clear();

		//Check that null and empty string throw a controled exception
		Assert.Throws(typeof(TraceException), delegate { t.ActionTrace(null, "Type", "ID"); });
		Assert.Throws(typeof(TraceException), delegate { t.ActionTrace("Verb", null, "ID"); });
		Assert.Throws(typeof(TraceException), delegate { t.ActionTrace("Verb", "Type", null); });

		Assert.Throws(typeof(TraceException), delegate { t.ActionTrace("", "Type", "ID"); });
		Assert.Throws(typeof(TraceException), delegate { t.ActionTrace("Verb", "", "ID"); });
		Assert.Throws(typeof(TraceException), delegate { t.ActionTrace("Verb", "Type", ""); });
	}

	[Test]
	public void TestNullImputs() {
		initTracker ("xapi");

		//Check that null and empty string throw a controled exception
		Assert.Throws(typeof(TraceException), delegate { t.ActionTrace(null, "Type", "ID"); });
		Assert.Throws(typeof(TraceException), delegate { t.ActionTrace("Verb", null, "ID"); });
		Assert.Throws(typeof(TraceException), delegate { t.ActionTrace("Verb", "Type", null); });

		Assert.Throws(typeof(TraceException), delegate { t.ActionTrace("", "Type", "ID"); });
		Assert.Throws(typeof(TraceException), delegate { t.ActionTrace("Verb", "", "ID"); });
		Assert.Throws(typeof(TraceException), delegate { t.ActionTrace("Verb", "Type", ""); });

		Assert.DoesNotThrow(delegate { t.ActionTrace("Verb","Type", "ID"); });

		Assert.Throws(typeof(VerbXApiException), delegate {
			t.ActionTrace("Verb","Type", "ID");
			t.RequestFlush();
			t.Update();
		});

		Assert.Throws(typeof(TraceException), delegate {t.completable.Initialized(null);});
		Assert.Throws(typeof(TraceException), delegate {t.completable.Progressed(null,0.1f);});
		Assert.Throws(typeof(TraceException), delegate {t.completable.Completed(null);});

		Assert.Throws(typeof(TraceException), delegate {t.accessible.Accessed(null);});
		Assert.Throws(typeof(TraceException), delegate {t.accessible.Skipped(null);});

		Assert.Throws(typeof(ValueExtensionException), delegate {t.alternative.Selected(null,null);});
		Assert.Throws(typeof(TraceException), delegate {t.alternative.Selected(null,"o");});
		Assert.Throws(typeof(ValueExtensionException), delegate {t.alternative.Selected("k",null);});
		Assert.Throws(typeof(ValueExtensionException), delegate {t.alternative.Unlocked(null,null);});
		Assert.Throws(typeof(TraceException), delegate {t.alternative.Unlocked(null,"o");});
		Assert.Throws(typeof(ValueExtensionException), delegate {t.alternative.Unlocked("k",null);});

		Assert.Throws(typeof(TraceException), delegate {t.trackedGameObject.Interacted(null);});
		Assert.Throws(typeof(TraceException), delegate {t.trackedGameObject.Used(null);});

		Assert.Throws(typeof(KeyExtensionException), delegate { t.setVar("",""); });
		Assert.Throws(typeof(KeyExtensionException), delegate { t.setVar(null,null); });
		Assert.Throws(typeof(KeyExtensionException), delegate { t.setVar(null,"v"); });
		Assert.Throws(typeof(ValueExtensionException), delegate { t.setVar("k",null); });
		Assert.Throws(typeof(ValueExtensionException), delegate { t.setVar("k",""); });

		Assert.DoesNotThrow(delegate { t.setVar("k","v"); });
	}

	[Test]
	public void TestObsoleteMethods() {
		initTracker ("xapi");

		//Check that null and empty string throw a controled exception
		Assert.Throws(typeof(TraceException), delegate { t.Trace(""); });
		Assert.Throws(typeof(TraceException), delegate { t.Trace("1"); });
		Assert.Throws(typeof(TraceException), delegate { t.Trace("1,2"); });
		Assert.Throws(typeof(TraceException), delegate { t.Trace("1,2,3,4"); });
		Assert.Throws(typeof(TraceException), delegate { t.Trace("1","2"); });
		Assert.Throws(typeof(TraceException), delegate { t.Trace("1","2",null); });
		Assert.Throws(typeof(TraceException), delegate { t.Trace("1","2",""); });
		Assert.Throws(typeof(TraceException), delegate { t.Trace("","",""); });
		Assert.Throws(typeof(TraceException), delegate { t.Trace("1","2","3","4"); });
		Assert.Throws(typeof(TraceException), delegate { t.Trace(null,null); });
		Assert.Throws(typeof(TraceException), delegate { t.Trace("1,2,3,4"); });
		Assert.Throws(typeof(VerbXApiException), delegate { t.Trace("1,2,3"); t.RequestFlush(); t.Update(); });

		t.Trace("Verb", "Type", "ID");
		CheckCSVTrace("Verb,Type,ID");
		t.GetQueue().Clear();

		t.Trace("Verb", "Ty,pe", "ID");
		CheckCSVTrace("Verb,Ty\\,pe,ID");
		t.GetQueue().Clear();

		t.Trace("Verb", "Type", "I,D");
		CheckCSVTrace("Verb,Type,I\\,D");
		t.GetQueue().Clear();

		t.Trace("Ve,rb", "Type", "ID");
		CheckCSVTrace("Ve\\,rb,Type,ID");
		t.GetQueue().Clear();

		t.Trace("Verb,Type,ID");
		CheckCSVTrace("Verb,Type,ID");
		t.GetQueue().Clear();

		t.Trace("Verb,Ty\\,pe,ID");
		CheckCSVTrace("Verb,Ty\\,pe,ID");
		t.GetQueue().Clear();

		t.Trace("Verb,Type,I\\,D");
		CheckCSVTrace("Verb,Type,I\\,D");
		t.GetQueue().Clear();

		t.Trace("Ve\\,rb,Type,ID");
		CheckCSVTrace("Ve\\,rb,Type,ID");
		t.GetQueue().Clear();

		Assert.Throws(typeof(KeyExtensionException), delegate { t.setExtension(null, null); });
		Assert.Throws(typeof(KeyExtensionException), delegate { t.setExtension("", null); });
		Assert.Throws(typeof(ValueExtensionException), delegate { t.setExtension("k", null); });
		Assert.Throws(typeof(ValueExtensionException), delegate { t.setExtension("k", ""); });
		Assert.DoesNotThrow(delegate { t.setExtension("k", 1); });
		Assert.DoesNotThrow(delegate { t.setExtension("k", 1.1f); });
		Assert.DoesNotThrow(delegate { t.setExtension("k", 1.1d); });
		Assert.DoesNotThrow(delegate { t.setExtension("k", "v"); });

		Assert.DoesNotThrow(delegate { t.ActionTrace("Verb","Type", "ID"); });
	}

	[Test]
	public void AlternativeTraceTest()
	{
		initTracker ("csv");

		t.alternative.Selected("question", "alternative");
		CheckCSVTrace("selected,alternative,question,response,alternative");
		t.GetQueue().Clear();
	}

	[Test]
	public void TestTrace_Generic_Csv_Stored_01()
	{
		initTracker ("csv");

		enqueueTrace01 ();
		t.RequestFlush ();
		t.Update ();

		CheckCSVStoredTrace ("accessed,gameobject,ObjectID");
	}

	[Test]
	public void TestTrace_Generic_Csv_Stored_02()
	{
		initTracker ("csv");

		enqueueTrace02 ();
		t.RequestFlush ();
		t.Update ();

		CheckCSVStoredTrace ("initialized,game,ObjectID2,response,TheResponse,score,0.123");
	}

	[Test]
	public void TestTrace_Generic_Csv_Stored_03()
	{
		initTracker ("csv");

		enqueueTrace03 ();
		t.RequestFlush ();
		t.Update ();

		CheckCSVStoredTrace ("selected,zone,ObjectID3,success,false,completion,true,response,AnotherResponse,score,123.456,extension1,value1,extension2,value2,extension3,3,extension4,4.56");
	}

	[Test]
	public void TestTrace_Generic_Csv_Stored_WithComma()
	{
		initTracker ("csv");

		t.setVar ("e1", "ex,2");
		t.setVar ("e,1", "ex,2,");
		t.setVar ("e3", "e3");
		t.ActionTrace("verb","target","id");
		t.RequestFlush ();
		t.Update ();

		CheckCSVStoredTrace ("verb,target,id,e1,ex\\,2,e\\,1,ex\\,2\\,,e3,e3");
	}

	[Test]
	public void TestTrace_Generic_XApi_Stored_01()
	{
		if (System.IO.File.Exists (Tracker.FilePath + ".csv")) {
			System.IO.File.Delete (Tracker.FilePath + ".csv");
		}

		initTracker ("xapi");

		enqueueTrace01 ();
		t.RequestFlush ();
		t.Update ();

		string text =  System.IO.File.ReadAllText (Tracker.FilePath + ".csv");
		text = text.Substring (text.IndexOf ("M\n") +2);

		JSONNode file = JSON.Parse(text);
		JSONNode tracejson = file[new List<JSONNode>(file.Children).Count - 1];

		Assert.AreEqual(new List<JSONNode>(tracejson.Children).Count, 3);
		Assert.AreEqual(tracejson["object"]["id"].Value, "ObjectID");
		Assert.AreEqual(tracejson["object"]["definition"]["type"].Value, "https://w3id.org/xapi/seriousgames/activity-types/game-object");
		Assert.AreEqual(tracejson["verb"]["id"].Value, "https://w3id.org/xapi/seriousgames/verbs/accessed");
	}

	[Test]
	public void TestTrace_Generic_XApi_Stored_02()
	{
		if (System.IO.File.Exists (Tracker.FilePath + ".csv")) {
			System.IO.File.Delete (Tracker.FilePath + ".csv");
		}

		initTracker ("xapi");
		enqueueTrace02 ();
		t.RequestFlush ();
		t.Update ();

		string text =  System.IO.File.ReadAllText (Tracker.FilePath + ".csv");
		text = text.Substring (text.IndexOf ("M\n") +2);

		JSONNode file = JSON.Parse(text);
		JSONNode tracejson = file[new List<JSONNode>(file.Children).Count - 1];

		Assert.AreEqual(tracejson.Count, 4);
		Assert.AreEqual(tracejson["object"]["id"].Value, "ObjectID2");
		Assert.AreEqual(tracejson["object"]["definition"]["type"].Value, "https://w3id.org/xapi/seriousgames/activity-types/serious-game");
		Assert.AreEqual(tracejson["verb"]["id"].Value, "https://w3id.org/xapi/adb/verbs/initialized");
		Assert.AreEqual(tracejson["result"].Count, 2);
		Assert.AreEqual(tracejson["result"]["response"].Value, "TheResponse");
		Assert.AreEqual(float.Parse(tracejson["result"]["score"]["raw"].Value), 0.123f);
	}

	[Test]
	public void TestTrace_Generic_XApi_Stored_03()
	{
		if (System.IO.File.Exists (Tracker.FilePath + ".csv")) {
			System.IO.File.Delete (Tracker.FilePath + ".csv");
		}

		initTracker ("xapi");
		enqueueTrace03 ();
		t.RequestFlush ();
		t.Update ();

		string text =  System.IO.File.ReadAllText (Tracker.FilePath + ".csv");
		text = text.Substring (text.IndexOf ("M\n") +2);

		JSONNode file = JSON.Parse(text);
		JSONNode tracejson = file[new List<JSONNode>(file.Children).Count - 1];

		Assert.AreEqual(new List<JSONNode>(tracejson.Children).Count, 4);
		Assert.AreEqual(tracejson["object"]["id"].Value, "ObjectID3");
		Assert.AreEqual(tracejson["object"]["definition"]["type"].Value, "https://w3id.org/xapi/seriousgames/activity-types/zone");
		Assert.AreEqual(tracejson["verb"]["id"].Value, "https://w3id.org/xapi/adb/verbs/selected");

		Assert.AreEqual(new List<JSONNode>(tracejson["result"].Children).Count, 5);
		Assert.AreEqual(tracejson["result"]["response"].Value, "AnotherResponse");
		Assert.AreEqual(float.Parse(tracejson["result"]["score"]["raw"].Value), 123.456f);
		Assert.AreEqual(bool.Parse(tracejson["result"]["completion"].Value), true);
		Assert.AreEqual(bool.Parse(tracejson["result"]["success"].Value), false);

		Assert.AreEqual(new List<JSONNode>(tracejson["result"]["extensions"].Children).Count, 4);
		Assert.AreEqual(tracejson["result"]["extensions"]["extension1"].Value, "value1");
		Assert.AreEqual(tracejson["result"]["extensions"]["extension2"].Value, "value2");
		Assert.AreEqual(int.Parse(tracejson["result"]["extensions"]["extension3"].Value), 3);
		Assert.AreEqual(float.Parse(tracejson["result"]["extensions"]["extension4"].Value), 4.56f);
	}

	[Test]
	public void TestTrace_Generic_XApi_All()
	{
		if (System.IO.File.Exists (Tracker.FilePath + ".csv")) {
			System.IO.File.Delete (Tracker.FilePath + ".csv");
		}

		initTracker ("xapi");

		enqueueTrace01 ();
		enqueueTrace02 ();
		enqueueTrace03 ();
		t.RequestFlush ();
		t.Update ();

		string text =  System.IO.File.ReadAllText (Tracker.FilePath + ".csv");
		text = text.Substring (text.IndexOf ("M\n") +2);

		JSONNode file = JSON.Parse(text);

		Assert.AreEqual(new List<JSONNode>(file.Children).Count, 3);

		//CHECK THE 1ST TRACe
		JSONNode tracejson = file[0];

		Assert.AreEqual(new List<JSONNode>(tracejson.Children).Count, 3);
		Assert.AreEqual(tracejson["object"]["id"].Value, "ObjectID");
		Assert.AreEqual(tracejson["object"]["definition"]["type"].Value, "https://w3id.org/xapi/seriousgames/activity-types/game-object");
		Assert.AreEqual(tracejson["verb"]["id"].Value, "https://w3id.org/xapi/seriousgames/verbs/accessed");

		//CHECK THE 2ND TRACE
		tracejson = file[1];

		Assert.AreEqual(new List<JSONNode>(tracejson.Children).Count, 4);
		Assert.AreEqual(tracejson["object"]["id"].Value, "ObjectID2");
		Assert.AreEqual(tracejson["object"]["definition"]["type"].Value, "https://w3id.org/xapi/seriousgames/activity-types/serious-game");
		Assert.AreEqual(tracejson["verb"]["id"].Value, "https://w3id.org/xapi/adb/verbs/initialized");
		Assert.AreEqual(new List<JSONNode>(tracejson["result"].Children).Count, 2);
		Assert.AreEqual(tracejson["result"]["response"].Value, "TheResponse");
		Assert.AreEqual(float.Parse(tracejson["result"]["score"]["raw"].Value), 0.123f);

		//CHECK THE 3RD TRACE
		tracejson = file[2];

		Assert.AreEqual(new List<JSONNode>(tracejson.Children).Count, 4);
		Assert.AreEqual(tracejson["object"]["id"].Value, "ObjectID3");
		Assert.AreEqual(tracejson["object"]["definition"]["type"].Value, "https://w3id.org/xapi/seriousgames/activity-types/zone");
		Assert.AreEqual(tracejson["verb"]["id"].Value, "https://w3id.org/xapi/adb/verbs/selected");

		Assert.AreEqual(new List<JSONNode>(tracejson["result"].Children).Count, 5);
		Assert.AreEqual(tracejson["result"]["response"].Value, "AnotherResponse");
		Assert.AreEqual(float.Parse(tracejson["result"]["score"]["raw"].Value), 123.456f);
		Assert.AreEqual(bool.Parse(tracejson["result"]["completion"].Value), true);
		Assert.AreEqual(bool.Parse(tracejson["result"]["success"].Value), false);

		Assert.AreEqual(new List<JSONNode>(tracejson["result"]["extensions"].Children).Count, 4);
		Assert.AreEqual(tracejson["result"]["extensions"]["extension1"].Value, "value1");
		Assert.AreEqual(tracejson["result"]["extensions"]["extension2"].Value, "value2");
		Assert.AreEqual(int.Parse(tracejson["result"]["extensions"]["extension3"].Value), 3);
		Assert.AreEqual(float.Parse(tracejson["result"]["extensions"]["extension4"].Value), 4.56f);
	}

	[Test]
	public void TestAccesible_Csv_01()
	{
		initTracker ("csv");

		t.accessible.Accessed("AccesibleID", AccessibleTracker.Accessible.Cutscene);

		CheckCSVTrace("accessed,cutscene,AccesibleID");
	}

	[Test]
	public void TestAccesible_Csv_02_WithExtensions()
	{
		initTracker ("csv");

		t.setVar("extension1", "value1");
		t.accessible.Skipped("AccesibleID2", AccessibleTracker.Accessible.Screen);

		CheckCSVTrace("skipped,screen,AccesibleID2,extension1,value1");
	}

	[Test]
	public void TestAccesible_XApi_01()
	{
		if (System.IO.File.Exists (Tracker.FilePath + ".csv")) {
			System.IO.File.Delete (Tracker.FilePath + ".csv");
		}

		initTracker ("xapi");

		t.accessible.Accessed("AccesibleID", AccessibleTracker.Accessible.Cutscene);
		t.RequestFlush ();
		t.Update ();

		string text =  System.IO.File.ReadAllText (Tracker.FilePath + ".csv");
		text = text.Substring (text.IndexOf ("M\n") +2);

		JSONNode file = JSON.Parse(text);
		JSONNode tracejson = file[new List<JSONNode>(file.Children).Count - 1];

		Assert.AreEqual(new List<JSONNode>(tracejson.Children).Count, 3);
		Assert.AreEqual(tracejson["object"]["id"].Value, "AccesibleID");
		Assert.AreEqual(tracejson["object"]["definition"]["type"].Value, "https://w3id.org/xapi/seriousgames/activity-types/cutscene");
		Assert.AreEqual(tracejson["verb"]["id"].Value, "https://w3id.org/xapi/seriousgames/verbs/accessed");
	}

	[Test]
	public void TestAccesible_XApi_02_WithExtensions()
	{
		if (System.IO.File.Exists (Tracker.FilePath + ".csv")) {
			System.IO.File.Delete (Tracker.FilePath + ".csv");
		}

		initTracker ("xapi");

		t.setVar("extension1", "value1");
		t.accessible.Skipped("AccesibleID2", AccessibleTracker.Accessible.Screen);
		t.RequestFlush ();
		t.Update ();

		string text =  System.IO.File.ReadAllText (Tracker.FilePath + ".csv");
		text = text.Substring (text.IndexOf ("M\n") +2);

		JSONNode file = JSON.Parse(text);
		JSONNode tracejson = file[new List<JSONNode>(file.Children).Count - 1];

		Assert.AreEqual(new List<JSONNode>(tracejson.Children).Count, 4);
		Assert.AreEqual(tracejson["object"]["id"].Value, "AccesibleID2");
		Assert.AreEqual(tracejson["object"]["definition"]["type"].Value, "https://w3id.org/xapi/seriousgames/activity-types/screen");
		Assert.AreEqual(tracejson["verb"]["id"].Value, "http://id.tincanapi.com/verb/skipped");
		Assert.AreEqual(tracejson["result"]["extensions"]["extension1"].Value, "value1");
	}

	[Test]
	public void TestAlternative_Csv_01()
	{
		initTracker ("csv");

		t.alternative.Selected("AlternativeID", "SelectedOption", AlternativeTracker.Alternative.Path);

		CheckCSVTrace("selected,path,AlternativeID,response,SelectedOption");
	}

	[Test]
	public void TestAlternative_Csv_02_WithExtensions()
	{
		initTracker ("csv");

		t.setVar("SubCompletableScore", 0.8);
		t.alternative.Unlocked("AlternativeID2", "Answer number 3", AlternativeTracker.Alternative.Question);

		CheckCSVTrace("unlocked,question,AlternativeID2,response,Answer number 3,SubCompletableScore,0.8");
	}

	[Test]
	public void TestAlternative_XApi_01()
	{
		if (System.IO.File.Exists (Tracker.FilePath + ".csv")) {
			System.IO.File.Delete (Tracker.FilePath + ".csv");
		}

		initTracker ("xapi");

		t.alternative.Selected("AlternativeID", "SelectedOption", AlternativeTracker.Alternative.Path);
		t.RequestFlush ();
		t.Update ();

		string text =  System.IO.File.ReadAllText (Tracker.FilePath + ".csv");
		text = text.Substring (text.IndexOf ("M\n") +2);

		JSONNode file = JSON.Parse(text);
		JSONNode tracejson = file[new List<JSONNode>(file.Children).Count - 1];

		Assert.AreEqual(new List<JSONNode>(tracejson.Children).Count, 4);
		Assert.AreEqual(tracejson["object"]["id"].Value, "AlternativeID");
		Assert.AreEqual(tracejson["object"]["definition"]["type"].Value, "https://w3id.org/xapi/seriousgames/activity-types/path");
		Assert.AreEqual(tracejson["verb"]["id"].Value, "https://w3id.org/xapi/adb/verbs/selected");
		Assert.AreEqual(tracejson["result"]["response"].Value, "SelectedOption");
	}

	[Test]
	public void TestAlternative_XApi_02_WithExtensions()
	{
		if (System.IO.File.Exists (Tracker.FilePath + ".csv")) {
			System.IO.File.Delete (Tracker.FilePath + ".csv");
		}

		initTracker ("xapi");

		t.setVar("SubCompletableScore", 0.8);
		t.alternative.Unlocked("AlternativeID2", "Answer number 3", AlternativeTracker.Alternative.Question);
		t.RequestFlush ();
		t.Update ();

		string text =  System.IO.File.ReadAllText (Tracker.FilePath + ".csv");
		text = text.Substring (text.IndexOf ("M\n") +2);

		JSONNode file = JSON.Parse(text);
		JSONNode tracejson = file[new List<JSONNode>(file.Children).Count - 1];

		Assert.AreEqual(new List<JSONNode>(tracejson.Children).Count, 4);
		Assert.AreEqual(tracejson["object"]["id"].Value, "AlternativeID2");
		Assert.AreEqual(tracejson["object"]["definition"]["type"].Value, "http://adlnet.gov/expapi/activities/question");
		Assert.AreEqual(tracejson["verb"]["id"].Value, "https://w3id.org/xapi/seriousgames/verbs/unlocked");
		Assert.AreEqual(tracejson["result"]["response"].Value, "Answer number 3");
		Assert.AreEqual(float.Parse(tracejson["result"]["extensions"]["SubCompletableScore"].Value), 0.8f);
	}

	[Test]
	public void TestCompletable_Csv_01()
	{
		initTracker ("csv");

		t.completable.Initialized("CompletableID", CompletableTracker.Completable.Quest);

		CheckCSVTrace("initialized,quest,CompletableID");
	}

	[Test]
	public void TestCompletable_Csv_02()
	{
		initTracker ("csv");

		t.completable.Progressed("CompletableID2", CompletableTracker.Completable.Stage ,0.34f);

		CheckCSVTrace("progressed,stage,CompletableID2,progress,0.34");
	}

	[Test]
	public void TestCompletable_Csv_03()
	{
		initTracker ("csv");

		t.completable.Completed("CompletableID3", CompletableTracker.Completable.Race, true, 0.54f);

		CheckCSVTrace("completed,race,CompletableID3,success,true,score,0.54");
	}

	[Test]
	public void TestCompletable_XApi_01()
	{
		if (System.IO.File.Exists (Tracker.FilePath + ".csv")) {
			System.IO.File.Delete (Tracker.FilePath + ".csv");
		}

		initTracker ("xapi");

		t.completable.Initialized("CompletableID", CompletableTracker.Completable.Quest);
		t.RequestFlush ();
		t.Update ();

		string text =  System.IO.File.ReadAllText (Tracker.FilePath + ".csv");
		text = text.Substring (text.IndexOf ("M\n") +2);

		JSONNode file = JSON.Parse(text);
		JSONNode tracejson = file[new List<JSONNode>(file.Children).Count - 1];

		Assert.AreEqual(new List<JSONNode>(tracejson.Children).Count, 3);
		Assert.AreEqual(tracejson["object"]["id"].Value, "CompletableID");
		Assert.AreEqual(tracejson["object"]["definition"]["type"].Value, "https://w3id.org/xapi/seriousgames/activity-types/quest");
		Assert.AreEqual(tracejson["verb"]["id"].Value, "https://w3id.org/xapi/adb/verbs/initialized");
	}

	[Test]
	public void TestCompletable_XApi_02()
	{
		if (System.IO.File.Exists (Tracker.FilePath + ".csv")) {
			System.IO.File.Delete (Tracker.FilePath + ".csv");
		}

		initTracker ("xapi");

		t.completable.Progressed("CompletableID2", CompletableTracker.Completable.Stage ,0.34f);
		t.RequestFlush ();
		t.Update ();

		string text =  System.IO.File.ReadAllText (Tracker.FilePath + ".csv");
		text = text.Substring (text.IndexOf ("M\n") +2);

		JSONNode file = JSON.Parse(text);
		JSONNode tracejson = file[new List<JSONNode>(file.Children).Count - 1];

		Assert.AreEqual(new List<JSONNode>(tracejson.Children).Count, 4);
		Assert.AreEqual(tracejson["object"]["id"].Value, "CompletableID2");
		Assert.AreEqual(tracejson["object"]["definition"]["type"].Value, "https://w3id.org/xapi/seriousgames/activity-types/stage");
		Assert.AreEqual(tracejson["verb"]["id"].Value, "http://adlnet.gov/expapi/verbs/progressed");
		Assert.AreEqual(tracejson ["result"] ["extensions"] ["https://w3id.org/xapi/seriousgames/extensions/progress"].AsFloat, 0.34f);
	}

	[Test]
	public void TestCompletable_XApi_03()
	{
		if (System.IO.File.Exists (Tracker.FilePath + ".csv")) {
			System.IO.File.Delete (Tracker.FilePath + ".csv");
		}

		initTracker ("xapi");

		t.completable.Completed("CompletableID3", CompletableTracker.Completable.Race, true, 0.54f);
		t.RequestFlush ();
		t.Update ();

		string text =  System.IO.File.ReadAllText (Tracker.FilePath + ".csv");
		text = text.Substring (text.IndexOf ("M\n") +2);

		JSONNode file = JSON.Parse(text);
		JSONNode tracejson = file[new List<JSONNode>(file.Children).Count - 1];

		Assert.AreEqual(tracejson.Count, 4);
		Assert.AreEqual(tracejson["object"]["id"].Value, "CompletableID3");
		Assert.AreEqual(tracejson["object"]["definition"]["type"].Value, "https://w3id.org/xapi/seriousgames/activity-types/race");
		Assert.AreEqual(tracejson["verb"]["id"].Value, "http://adlnet.gov/expapi/verbs/completed");
		Assert.AreEqual(tracejson["result"]["success"].AsBool, true);
		Assert.AreEqual(tracejson["result"]["score"]["raw"].AsFloat, 0.54f);
	}

	[Test]
	public void TestGameObject_Csv_01()
	{
		initTracker ("csv");

		t.trackedGameObject.Interacted("GameObjectID", GameObjectTracker.TrackedGameObject.Npc);

		CheckCSVTrace("interacted,npc,GameObjectID");
	}

	[Test]
	public void TestGameObject_Csv_02()
	{
		initTracker ("csv");

		t.trackedGameObject.Used("GameObjectID2", GameObjectTracker.TrackedGameObject.Item);

		CheckCSVTrace("used,item,GameObjectID2");
	}

	[Test]
	public void TestGameObject_XApi_01()
	{
		if (System.IO.File.Exists (Tracker.FilePath + ".csv")) {
			System.IO.File.Delete (Tracker.FilePath + ".csv");
		}

		initTracker ("xapi");

		t.trackedGameObject.Interacted("GameObjectID", GameObjectTracker.TrackedGameObject.Npc);
		t.RequestFlush ();
		t.Update ();

		string text =  System.IO.File.ReadAllText (Tracker.FilePath + ".csv");
		text = text.Substring (text.IndexOf ("M\n") +2);

		JSONNode file = JSON.Parse(text);
		JSONNode tracejson = file[new List<JSONNode>(file.Children).Count - 1];

		Assert.AreEqual(new List<JSONNode>(tracejson.Children).Count, 3);
		Assert.AreEqual(tracejson["object"]["id"].Value, "GameObjectID");
		Assert.AreEqual(tracejson["object"]["definition"]["type"].Value, "https://w3id.org/xapi/seriousgames/activity-types/non-player-character");
		Assert.AreEqual(tracejson["verb"]["id"].Value, "http://adlnet.gov/expapi/verbs/interacted");
	}

	[Test]
	public void TestGameObject_XApi_02()
	{
		if (System.IO.File.Exists (Tracker.FilePath + ".csv")) {
			System.IO.File.Delete (Tracker.FilePath + ".csv");
		}

		initTracker ("xapi");

		t.trackedGameObject.Used("GameObjectID2", GameObjectTracker.TrackedGameObject.Item);  t.RequestFlush ();
		t.Update ();

		string text =  System.IO.File.ReadAllText (Tracker.FilePath + ".csv");
		text = text.Substring (text.IndexOf ("M\n") +2);

		JSONNode file = JSON.Parse(text);
		JSONNode tracejson = file[new List<JSONNode>(file.Children).Count - 1];

		Assert.AreEqual(new List<JSONNode>(tracejson.Children).Count, 3);
		Assert.AreEqual(tracejson["object"]["id"].Value, "GameObjectID2");
		Assert.AreEqual(tracejson["object"]["definition"]["type"].Value, "https://w3id.org/xapi/seriousgames/activity-types/item");
		Assert.AreEqual(tracejson["verb"]["id"].Value, "https://w3id.org/xapi/seriousgames/verbs/used");
	}

	private void enqueueTrace01(){
		t.ActionTrace("accessed", "gameobject", "ObjectID");
	}

	private void enqueueTrace02(){
		t.setResponse("TheResponse");
		t.setScore(0.123f);
		t.ActionTrace("initialized", "game", "ObjectID2");
	}

	private void enqueueTrace03(){
		t.setResponse("AnotherResponse");
		t.setScore(123.456f);
		t.setSuccess(false);
		t.setCompletion(true);
		t.setVar("extension1", "value1");
		t.setVar("extension2", "value2");
		t.setVar("extension3", 3 );
		t.setVar("extension4", 4.56f);
		t.ActionTrace("selected", "zone", "ObjectID3");
	}

	private void CheckCSVTrace(string trace)
	{
		string traceWithoutTimestamp = removeTimestamp(t.GetQueue()[0]);

		CompareCSV(traceWithoutTimestamp, trace);
		
	}

	private void CheckCSVStoredTrace(string trace){
		string[] lines = System.IO.File.ReadAllLines (Tracker.FilePath + ".csv");

		string traceWithoutTimestamp = removeTimestamp(lines[lines.Length-1]);

		CompareCSV(traceWithoutTimestamp, trace);
	}

	private void CheckXAPIStoredTrace(string trace){
		string[] lines = System.IO.File.ReadAllLines (Tracker.FilePath + ".csv");

		string traceWithoutTimestamp = removeTimestamp(lines[lines.Length-1]);

		CompareCSV(traceWithoutTimestamp, trace);
	}

	private void CompareCSV(string t1, string t2){
		string[] sp1 = Utils.parseCSV(t1);
		string[] sp2 = Utils.parseCSV(t2);

		Assert.AreEqual(sp1.Length, sp2.Length);

		for(int i = 0; i < 3; i++)
			Assert.AreEqual(sp1[i], sp2[i]);
		
		Dictionary<string,string> d1 = new Dictionary<string, string> ();

		if (sp1.Length > 3) {
			for (int i = 3; i < sp1.Length; i += 2) {
				d1.Add (sp1 [i], sp1 [i + 1]);
			}
		
			for (int i = 3; i < sp2.Length; i += 2) {
				Assert.Contains (sp2 [i], d1.Keys);
				Assert.AreEqual (d1 [sp2 [i]], sp2 [i + 1]);
			}
		}
	}

	private string removeTimestamp(string trace){
		return trace.Substring(trace.IndexOf(',') + 1);
	}
}