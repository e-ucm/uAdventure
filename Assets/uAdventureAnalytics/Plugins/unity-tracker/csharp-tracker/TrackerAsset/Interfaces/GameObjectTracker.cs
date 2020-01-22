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
using System.Collections;
using AssetPackage;
using AssetPackage.Utils;
using AssetPackage.Exceptions;

public class GameObjectTracker : TrackerAsset.IGameObjectTracker
{

    private TrackerAsset tracker;

    public void setTracker(TrackerAsset tracker)
    {
        this.tracker = tracker;
    }


    /* GAMEOBJECT */

    public enum TrackedGameObject
    {
        Enemy,
        Npc,
        Item,
        GameObject
    }

    /// <summary>
    /// Player interacted with a game object.
    /// Type = GameObject 
    /// </summary>
    /// <param name="gameobjectId">Reachable identifier.</param>
    public void Interacted(string gameobjectId)
    {
        if (tracker.Utils.check<TargetXApiException>(gameobjectId, "xAPI Exception: Target ID is null or empty. Ignoring.", "xAPI Exception: Target ID can't be null or empty."))
        {
            tracker.Trace(new TrackerAsset.TrackerEvent(tracker)
            {
                Event = new TrackerAsset.TrackerEvent.TraceVerb(TrackerAsset.Verb.Interacted),
                Target = new TrackerAsset.TrackerEvent.TraceObject(TrackedGameObject.GameObject.ToString().ToLower(), gameobjectId)
            });
        }
    }

    /// <summary>
    /// Player interacted with a game object.
    /// </summary>
    /// <param name="gameobjectId">TrackedGameObject identifier.</param>
    public void Interacted(string gameobjectId, TrackedGameObject type)
    {
        if (tracker.Utils.check<TargetXApiException>(gameobjectId, "xAPI Exception: Target ID is null or empty. Ignoring.", "xAPI Exception: Target ID can't be null or empty."))
        {
            tracker.Trace(new TrackerAsset.TrackerEvent(tracker)
            {
                Event = new TrackerAsset.TrackerEvent.TraceVerb(TrackerAsset.Verb.Interacted),
                Target = new TrackerAsset.TrackerEvent.TraceObject(type.ToString().ToLower(), gameobjectId)
            });
        }
    }

    /// <summary>
    /// Player interacted with a game object.
    /// Type = GameObject 
    /// </summary>
    /// <param name="gameobjectId">Reachable identifier.</param>
    public void Used(string gameobjectId)
    {
        if (tracker.Utils.check<TargetXApiException>(gameobjectId, "xAPI Exception: Target ID is null or empty. Ignoring.", "xAPI Exception: Target ID can't be null or empty."))
        {
            tracker.Trace(new TrackerAsset.TrackerEvent(tracker)
            {
                Event = new TrackerAsset.TrackerEvent.TraceVerb(TrackerAsset.Verb.Used),
                Target = new TrackerAsset.TrackerEvent.TraceObject(TrackedGameObject.GameObject.ToString().ToLower(), gameobjectId)
            });
        }
    }

    /// <summary>
    /// Player interacted with a game object.
    /// </summary>
    /// <param name="gameobjectId">TrackedGameObject identifier.</param>
    public void Used(string gameobjectId, TrackedGameObject type)
    {
        if (tracker.Utils.check<TargetXApiException>(gameobjectId, "xAPI Exception: Target ID is null or empty. Ignoring.", "xAPI Exception: Target ID can't be null or empty."))
        {
            tracker.Trace(new TrackerAsset.TrackerEvent(tracker)
            {
                Event = new TrackerAsset.TrackerEvent.TraceVerb(TrackerAsset.Verb.Used),
                Target = new TrackerAsset.TrackerEvent.TraceObject(type.ToString().ToLower(), gameobjectId)
            });
        }
    }
}
