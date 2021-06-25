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
using AssetPackage;
using AssetPackage.Exceptions;
using static AssetPackage.TrackerAsset;

public class AccessibleTracker : TrackerAsset.IGameObjectTracker
{

    private TrackerAsset tracker;

    public void setTracker(TrackerAsset tracker)
    {
        this.tracker = tracker;
    }


    /* ACCESSIBLES */

    public enum Accessible
    {
        Screen,
        Area,
        Zone,
        Cutscene,
        Inventory,
        Accessible
    }

    /// <summary>
    /// Player accessed a reachable.
    /// Type = Accessible 
    /// </summary>
    /// <param name="reachableId">Reachable identifier.</param>
    public TrackerEvent Accessed(string reachableId)
    {
        if (tracker.Utils.check<TargetXApiException>(reachableId, "xAPI Exception: Target ID is null or empty. Ignoring.", "xAPI Exception: Target ID can't be null or empty."))
        {
            return tracker.Trace(new TrackerAsset.TrackerEvent(tracker)
            {
                Event = new TrackerAsset.TrackerEvent.TraceVerb(TrackerAsset.Verb.Accessed),
                Target = new TrackerAsset.TrackerEvent.TraceObject(Accessible.Accessible.ToString().ToLower(), reachableId)
            });
        }
        return null;
    }

    /// <summary>
    /// Player accessed a reachable.
    /// </summary>
    /// <param name="reachableId">Reachable identifier.</param>
    /// <param name="type">Reachable type.</param>
    public TrackerEvent Accessed(string reachableId, Accessible type)
    {
        if (tracker.Utils.check<TargetXApiException>(reachableId, "xAPI Exception: Target ID is null or empty. Ignoring.", "xAPI Exception: Target ID can't be null or empty."))
        {
            return tracker.Trace(new TrackerAsset.TrackerEvent(tracker)
            {
                Event = new TrackerAsset.TrackerEvent.TraceVerb(TrackerAsset.Verb.Accessed),
                Target = new TrackerAsset.TrackerEvent.TraceObject(type.ToString().ToLower(), reachableId)
            });
        }
        return null;
    }

    /// <summary>
    /// Player skipped a reachable.
    /// Type = Accessible
    /// </summary>
    /// <param name="reachableId">Reachable identifier.</param>
    public TrackerEvent Skipped(string reachableId)
    {
        if (tracker.Utils.check<TargetXApiException>(reachableId, "xAPI Exception: Target ID is null or empty. Ignoring.", "xAPI Exception: Target ID can't be null or empty."))
        {
            return tracker.Trace(new TrackerAsset.TrackerEvent(tracker)
            {
                Event = new TrackerAsset.TrackerEvent.TraceVerb(TrackerAsset.Verb.Skipped),
                Target = new TrackerAsset.TrackerEvent.TraceObject(Accessible.Accessible.ToString().ToLower(), reachableId)
            });
        }
        return null;
    }

    /// <summary>
    /// Player skipped a reachable.
    /// </summary>
    /// <param name="reachableId">Reachable identifier.</param>
    /// <param name="type">Reachable type.</param>
    public TrackerEvent Skipped(string reachableId, Accessible type)
    {
        if (tracker.Utils.check<TargetXApiException>(reachableId, "xAPI Exception: Target ID is null or empty. Ignoring.", "xAPI Exception: Target ID can't be null or empty."))
        {
            return tracker.Trace(new TrackerAsset.TrackerEvent(tracker)
            {
                Event = new TrackerAsset.TrackerEvent.TraceVerb(TrackerAsset.Verb.Skipped),
                Target = new TrackerAsset.TrackerEvent.TraceObject(type.ToString().ToLower(), reachableId)
            });
        }
        return null;
    }


}
