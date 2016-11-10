/*
 * Copyright 2016 e-UCM (http://www.e-ucm.es/)
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * This project has received funding from the European Unionâ€™s Horizon
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
using System.Collections.Generic;
using SimpleJSON;
using System;

public class XApiFormat : Tracker.ITraceFormatter
{

    private Dictionary<string, string> verbIds = new Dictionary<string, string>()
    {
        { Tracker.Verb.Initialized.ToString().ToLower(), "https://w3id.org/xapi/adb/verbs/initialized"},
        { Tracker.Verb.Progressed.ToString().ToLower(), "http://adlnet.gov/expapi/verbs/progressed"},
        { Tracker.Verb.Completed.ToString().ToLower(), "http://adlnet.gov/expapi/verbs/completed"},
        { Tracker.Verb.Accessed.ToString().ToLower(), "https://w3id.org/xapi/seriousgames/verbs/accessed"},
        { Tracker.Verb.Skipped.ToString().ToLower(), "http://id.tincanapi.com/verb/skipped"},
        { Tracker.Verb.Selected.ToString().ToLower(), "https://w3id.org/xapi/adb/verbs/selected"},
        { Tracker.Verb.Unlocked.ToString().ToLower(), "https://w3id.org/xapi/seriousgames/verbs/unlocked"},
        { Tracker.Verb.Interacted.ToString().ToLower(), "http://adlnet.gov/expapi/verbs/interacted"},
        { Tracker.Verb.Used.ToString().ToLower(), "https://w3id.org/xapi/seriousgames/verbs/used"}
    };

    private Dictionary<string, string> objectIds = new Dictionary<string, string>()
    {
        // Completable
        { CompletableTracker.Completable.Game.ToString().ToLower(), "https://w3id.org/xapi/seriousgames/activity-types/serious-game" },
        { CompletableTracker.Completable.Session.ToString().ToLower(), "https://w3id.org/xapi/seriousgames/activity-types/session"},
        { CompletableTracker.Completable.Level.ToString().ToLower(), "https://w3id.org/xapi/seriousgames/activity-types/level"},
        { CompletableTracker.Completable.Quest.ToString().ToLower(), "https://w3id.org/xapi/seriousgames/activity-types/quest"},
        { CompletableTracker.Completable.Stage.ToString().ToLower(), "https://w3id.org/xapi/seriousgames/activity-types/stage"},
        { CompletableTracker.Completable.Combat.ToString().ToLower(), "https://w3id.org/xapi/seriousgames/activity-types/combat"},
        { CompletableTracker.Completable.StoryNode.ToString().ToLower(), "https://w3id.org/xapi/seriousgames/activity-types/story-node"},
        { CompletableTracker.Completable.Race.ToString().ToLower(), "https://w3id.org/xapi/seriousgames/activity-types/race"},
        { CompletableTracker.Completable.Completable.ToString().ToLower(), "https://w3id.org/xapi/seriousgames/activity-types/completable"},

        // Acceesible
        { AccessibleTracker.Accessible.Screen.ToString().ToLower(), "https://w3id.org/xapi/seriousgames/activity-types/screen" },
        { AccessibleTracker.Accessible.Area.ToString().ToLower(), "https://w3id.org/xapi/seriousgames/activity-types/area"},
        { AccessibleTracker.Accessible.Zone.ToString().ToLower(), "https://w3id.org/xapi/seriousgames/activity-types/zone"},
        { AccessibleTracker.Accessible.Cutscene.ToString().ToLower(), "https://w3id.org/xapi/seriousgames/activity-types/cutscene"},
        { AccessibleTracker.Accessible.Accessible.ToString().ToLower(), "https://w3id.org/xapi/seriousgames/activity-types/accessible"},

        // Alternative
        { AlternativeTracker.Alternative.Question.ToString().ToLower(), "http://adlnet.gov/expapi/activities/question" },
        { AlternativeTracker.Alternative.Menu.ToString().ToLower(), "https://w3id.org/xapi/seriousgames/activity-types/menu"},
        { AlternativeTracker.Alternative.Dialog.ToString().ToLower(), "https://w3id.org/xapi/seriousgames/activity-types/dialog-tree"},
        { AlternativeTracker.Alternative.Path.ToString().ToLower(), "https://w3id.org/xapi/seriousgames/activity-types/path"},
        { AlternativeTracker.Alternative.Arena.ToString().ToLower(), "https://w3id.org/xapi/seriousgames/activity-types/arena"},
        { AlternativeTracker.Alternative.Alternative.ToString().ToLower(), "https://w3id.org/xapi/seriousgames/activity-types/alternative"},

        // GameObject
        { GameObjectTracker.TrackedGameObject.Enemy.ToString().ToLower(), "https://w3id.org/xapi/seriousgames/activity-types/enemy" },
        { GameObjectTracker.TrackedGameObject.Npc.ToString().ToLower(), "https://w3id.org/xapi/seriousgames/activity-types/non-player-character"},
        { GameObjectTracker.TrackedGameObject.Item.ToString().ToLower(), "https://w3id.org/xapi/seriousgames/activity-types/item"},
        { GameObjectTracker.TrackedGameObject.GameObject.ToString().ToLower(), "https://w3id.org/xapi/seriousgames/activity-types/game-object"}
    };

    private Dictionary<string, string> extensionIds = new Dictionary<string, string>()
    {
        { Tracker.Extension.Health.ToString().ToLower(), "https://w3id.org/xapi/seriousgames/extensions/health"},
        { Tracker.Extension.Position.ToString().ToLower(), "https://w3id.org/xapi/seriousgames/extensions/position"},
        { Tracker.Extension.Progress.ToString().ToLower(), "https://w3id.org/xapi/seriousgames/extensions/progress"}
    };

    private List<JSONNode> statements = new List<JSONNode> ();
	private string objectId;
	private JSONNode actor;

public void StartData (JSONNode data)
	{
		actor = data ["actor"];
		objectId = data ["objectId"].ToString();
		if (!objectId.EndsWith ("/")) {
            objectId = objectId.Replace("\"", "");
			objectId += "/";
            UnityEngine.Debug.Log("objectId::: " + objectId);
		}
	}

	public string Serialize (List<string> traces)
	{
		statements.Clear ();

		foreach (string trace in traces) {
			statements.Add (CreateStatement (trace));
		}

		string result = "[";
		foreach (JSONNode statement in statements) {
			result += statement.ToString () + ",";
		}
		return result.Substring (0, result.Length - 1).Replace ("[\n\t]", "").Replace (": ", ":") + "]";
	}

	private JSONNode CreateStatement (string trace)
	{
		string[] parts = trace.Split (',');
		string timestamp = new System.DateTime (1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).AddMilliseconds (long.Parse (parts [0])).ToString ("yyyy-MM-ddTHH:mm:ss.fffZ");

		JSONNode statement = JSONNode.Parse ("{\"timestamp\": \"" + timestamp + "\"}");

		if (actor != null) {
			statement.Add ("actor", actor);		      
		}
		statement.Add ("verb", CreateVerb (parts [1]));


		statement.Add ("object", CreateObject(parts));

		if (parts.Length > 4) {
            // Parse extensions

            int extCount = parts.Length - 4;
            if(extCount > 0 && extCount % 2 == 0)
            {
                // Extensions come in <key, value> pairs

                JSONClass extensions = new JSONClass();
                JSONNode extensionsChild = null;

                for (int i = 4; i < parts.Length; i+= 2)
                {
                    string key = parts[i];
                    string value = parts[i + 1];
                    if (key.Equals("") || value.Equals(""))
                    {
                        continue;
                    }
                    if (key.Equals(Tracker.Extension.Score.ToString().ToLower()))
                    {

                        JSONClass score = new JSONClass();
                        float valueResult = 0f;
                        float.TryParse(value, out valueResult);
                        score.Add("raw", new JSONData(valueResult));
                        extensions.Add("score", score);
                    }
                    else if (key.Equals(Tracker.Extension.Success.ToString().ToLower()))
                    {
                        bool valueResult = false;
                        bool.TryParse(value, out valueResult);
                        extensions.Add("success", new JSONData(valueResult));
                    }
                    else if (key.Equals(Tracker.Extension.Completion.ToString().ToLower()))
                    {
                        bool valueResult = false;
                        bool.TryParse(value, out valueResult);
                        extensions.Add("completion", new JSONData(valueResult));
                    }
                    else if (key.Equals(Tracker.Extension.Response.ToString().ToLower()))
                    {
                        extensions.Add("response", new JSONData(value));
                    }
                    else
                    {
                        if(extensionsChild == null)
                        {
                            extensionsChild = JSONNode.Parse("{}");
                            extensions.Add("extensions", extensionsChild);
                        }

                        string id = key;
                        bool tbool;
                        int tint;
                        float tfloat;
                        double tdouble;

                        if (extensionIds.ContainsKey(key))
                            id = extensionIds[key];

                        if (int.TryParse(value, out tint))
                            extensionsChild.Add(id, new JSONData(tint));
                        else if(float.TryParse(value, out tfloat))
                            extensionsChild.Add(id, new JSONData(tfloat));
                        else if(double.TryParse(value, out tdouble))
                            extensionsChild.Add(id, new JSONData(tdouble));
                        else if(bool.TryParse(value, out tbool))
                            extensionsChild.Add(id, new JSONData(tbool));
                        else
                            extensionsChild.Add(id, new JSONData(value));
                    }
                }
                statement.Add("result", extensions);
            }
		}

        return statement;
	}

	private JSONNode CreateVerb (string ev)
	{

        string id = ev;
        verbIds.TryGetValue(ev, out id);

		JSONNode verb = JSONNode.Parse ("{id: }");
		verb ["id"] = id;

		return verb;
	}

	private JSONNode CreateObject (string[] parts)
	{
		string type = parts [2];

        string id = parts[3];
        string typeKey = type;
        objectIds.TryGetValue(type, out typeKey);

        JSONNode obj = JSONNode.Parse("{id: }");
        obj["id"] = objectId + id;

        JSONNode definition = JSONNode.Parse("{type: }");
        definition["type"] = typeKey;

        obj.Add("definition", definition);

        return obj;
	}
}


