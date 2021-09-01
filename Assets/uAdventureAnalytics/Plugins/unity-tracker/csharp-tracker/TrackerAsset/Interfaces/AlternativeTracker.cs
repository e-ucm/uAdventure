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
using static AssetPackage.TrackerAsset;
using System.Collections.Generic;

public class AlternativeTracker : TrackerAsset.IGameObjectTracker
{

    private TrackerAsset tracker;

    public void setTracker(TrackerAsset tracker)
    {
        this.tracker = tracker;
    }

    /* ALTERNATIVES */

    public enum Alternative
    {
        Question,
        Menu,
        Dialog,
        Path,
        Arena,
        Alternative
    }

    /// <summary>
    /// Player selected an option in a presented alternative
    /// Type = Alternative
    /// </summary>
    /// <param name="alternativeId">Alternative identifier.</param>
    /// <param name="optionId">Option identifier.</param>
    public TrackerEvent Selected(string alternativeId, string optionId)
    {
        bool check = true;

        check &= tracker.Utils.check<TargetXApiException>(alternativeId, "xAPI Exception: Target ID is null or empty. Ignoring.", "xAPI Exception: Target ID can't be null or empty.");
        check &= tracker.Utils.check<ValueExtensionException>(optionId, "xAPI Exception: Selected alternative is null or empty", "xAPI Exception: Selected alternative can't be null or empty");

        if (check)
        {
            return tracker.Trace(new TrackerEvent(tracker)
            {
                Event = new TrackerEvent.TraceVerb(Verb.Selected),
                Target = new TrackerEvent.TraceObject(Alternative.Alternative.ToString().ToLower(), alternativeId),
                Result = new TrackerEvent.TraceResult()
                {
                    Response = optionId
                }
            });
        }
        return null;
    }

    /// <summary>
    /// Player selected an option in a presented alternative
    /// </summary>
    /// <param name="alternativeId">Alternative identifier.</param>
    /// <param name="optionId">Option identifier.</param>
    /// <param name="type">Alternative type.</param>
    public TrackerEvent Selected(string alternativeId, string optionId, Alternative type)
    {
        bool check = true;

        check &= tracker.Utils.check<TargetXApiException>(alternativeId, "xAPI Exception: Target ID is null or empty. Ignoring.", "xAPI Exception: Target ID can't be null or empty.");
        check &= tracker.Utils.check<ValueExtensionException>(optionId, "xAPI Exception: Selected alternative is null or empty", "xAPI Exception: Selected alternative can't be null or empty");

        if (check)
        {
            return tracker.Trace(new TrackerAsset.TrackerEvent(tracker)
            {
                Event = new TrackerEvent.TraceVerb(Verb.Selected),
                Target = new TrackerEvent.TraceObject(type.ToString().ToLower(), alternativeId),
                Result = new TrackerEvent.TraceResult()
                {
                    Response = optionId
                }
            });
        }
        return null;
    }

    /// <summary>
    /// Player unlocked an option
    /// Type = Alternative
    /// </summary>
    /// <param name="alternativeId">Alternative identifier.</param>
    /// <param name="optionId">Option identifier.</param>
    public TrackerEvent Unlocked(string alternativeId, string optionId)
    {
        bool check = true;

        check &= tracker.Utils.check<TargetXApiException>(alternativeId, "xAPI Exception: Target ID is null or empty. Ignoring.", "xAPI Exception: Target ID can't be null or empty.");
        check &= tracker.Utils.check<ValueExtensionException>(optionId, "xAPI Exception: Selected alternative is null or empty", "xAPI Exception: Selected alternative can't be null or empty");

        if (check)
        {
            return tracker.Trace(new TrackerEvent(tracker)
            {
                Event = new TrackerEvent.TraceVerb(Verb.Unlocked),
                Target = new TrackerEvent.TraceObject(Alternative.Alternative.ToString().ToLower(), alternativeId),
                Result = new TrackerEvent.TraceResult()
                {
                    Response = optionId
                }
            });
        }
        return null;
    }

    /// <summary>
    /// Player unlocked an option
    /// </summary>
    /// <param name="alternativeId">Alternative identifier.</param>
    /// <param name="optionId">Option identifier.</param>
    /// <param name="type">Alternative type.</param>
    public TrackerEvent Unlocked(string alternativeId, string optionId, Alternative type)
    {
        bool check = true;

        check &= tracker.Utils.check<TargetXApiException>(alternativeId, "xAPI Exception: Target ID is null or empty. Ignoring.", "xAPI Exception: Target ID can't be null or empty.");
        check &= tracker.Utils.check<ValueExtensionException>(optionId, "xAPI Exception: Selected alternative is null or empty", "xAPI Exception: Selected alternative can't be null or empty");

        if (check)
        {
            return tracker.Trace(new TrackerEvent(tracker)
            {
                Event = new TrackerEvent.TraceVerb(Verb.Unlocked),
                Target = new TrackerEvent.TraceObject(type.ToString().ToLower(), alternativeId),
                Result = new TrackerEvent.TraceResult()
                {
                    Response = optionId
                }
            });
        }
        return null;
    }

}
