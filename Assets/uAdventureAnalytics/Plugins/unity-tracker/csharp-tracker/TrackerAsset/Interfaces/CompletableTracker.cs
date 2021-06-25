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

public class CompletableTracker : IGameObjectTracker
{

    private TrackerAsset tracker;

    public void setTracker(TrackerAsset tracker)
    {
        this.tracker = tracker;
    }

    /* COMPLETABLES */

    public enum Completable
    {
        Game,
        Session,
        Level,
        Quest,
        Stage,
        Combat,
        StoryNode,
        DialogNode,
        DialogFragment,
        Race,
        Completable
    }

    /// <summary>
    /// Player initialized a completable.
    /// </summary>
    /// <param name="completableId">Completable identifier.</param>
    public TrackerEvent Initialized(string completableId)
    {
        if (tracker.Utils.check<TargetXApiException>(completableId, "xAPI Exception: Target ID is null or empty. Ignoring.", "xAPI Exception: Target ID can't be null or empty."))
        {
            return tracker.Trace(new TrackerEvent(tracker)
            {
                Event = new TrackerEvent.TraceVerb(Verb.Initialized),
                Target = new TrackerEvent.TraceObject(Completable.Completable.ToString().ToLower(), completableId)
            });
        }
        return null;
    }

    /// <summary>
    /// Player initialized a completable.
    /// </summary>
    /// <param name="completableId">Completable identifier.</param>
    /// <param name="type">Completable type.</param>
    public TrackerEvent Initialized(string completableId, Completable type)
    {
        if (tracker.Utils.check<TargetXApiException>(completableId, "xAPI Exception: Target ID is null or empty. Ignoring.", "xAPI Exception: Target ID can't be null or empty."))
        {
            return tracker.Trace(new TrackerEvent(tracker)
            {
                Event = new TrackerEvent.TraceVerb(Verb.Initialized),
                Target = new TrackerEvent.TraceObject(type.ToString().ToLower(), completableId)
            });
        }
        return null;
    }

    /// <summary>
    /// Player progressed a completable.
    /// Type = Completable
    /// </summary>
    /// <param name="completableId">Completable identifier.</param>
    /// <param name="value">New value for the completable's progress.</param>
    public TrackerEvent Progressed(string completableId, float value)
    {
        if (tracker.Utils.check<TargetXApiException>(completableId, "xAPI Exception: Target ID is null or empty. Ignoring.", "xAPI Exception: Target ID can't be null or empty."))
        {
            tracker.setProgress(value);
            return tracker.Trace(new TrackerEvent(tracker)
            {
                Event = new TrackerEvent.TraceVerb(Verb.Progressed),
                Target = new TrackerEvent.TraceObject(Completable.Completable.ToString().ToLower(), completableId)
            });
        }
        return null;
    }

    /// <summary>
    /// Player progressed a completable.
    /// </summary>
    /// <param name="completableId">Completable identifier.</param>
    /// <param name="value">New value for the completable's progress.</param>
    /// <param name="type">Completable type.</param>
    public TrackerEvent Progressed(string completableId, Completable type, float value)
    {
        if (tracker.Utils.check<TargetXApiException>(completableId, "xAPI Exception: Target ID is null or empty. Ignoring.", "xAPI Exception: Target ID can't be null or empty."))
        {
            tracker.setProgress(value);
            return tracker.Trace(new TrackerEvent(tracker)
            {
                Event = new TrackerEvent.TraceVerb(Verb.Progressed),
                Target = new TrackerEvent.TraceObject(type.ToString().ToLower(), completableId)
            });
        }
        return null;
    }

    /// <summary>
    /// Player completed a completable.
    /// Type = Completable
    /// Success = true
    /// Score = 1
    /// </summary>
    /// <param name="completableId">Completable identifier.</param>
    public TrackerEvent Completed(string completableId)
    {
        if (tracker.Utils.check<TargetXApiException>(completableId, "xAPI Exception: Target ID is null or empty. Ignoring.", "xAPI Exception: Target ID can't be null or empty."))
        {
            return tracker.Trace(new TrackerEvent(tracker)
            {
                Event = new TrackerEvent.TraceVerb(Verb.Completed),
                Target = new TrackerEvent.TraceObject(Completable.Completable.ToString().ToLower(), completableId),
                Result = new TrackerEvent.TraceResult()
                {
                    Success = true,
                    Score = 1f
                }
            });
        }
        return null;
    }

    /// <summary>
    /// Player completed a completable.
    /// Success = true
    /// Score = 1
    /// </summary>
    /// <param name="completableId">Completable identifier.</param>
    /// <param name="type">Completable type.</param>
    public TrackerEvent Completed(string completableId, Completable type)
    {
        if (tracker.Utils.check<TargetXApiException>(completableId, "xAPI Exception: Target ID is null or empty. Ignoring.", "xAPI Exception: Target ID can't be null or empty."))
        {
            return tracker.Trace(new TrackerEvent(tracker)
            {
                Event = new TrackerEvent.TraceVerb(Verb.Completed),
                Target = new TrackerEvent.TraceObject(type.ToString().ToLower(), completableId),
                Result = new TrackerEvent.TraceResult()
                {
                    Success = true,
                    Score = 1f
                }
            });
        }
        return null;
    }

    /// <summary>
    /// Player completed a completable.
    /// Score = 1
    /// </summary>
    /// <param name="completableId">Completable identifier.</param>
    /// <param name="type">Completable type.</param>
    /// <param name="success">Completable success.</param>
    public TrackerEvent Completed(string completableId, Completable type, bool success)
    {
        if (tracker.Utils.check<TargetXApiException>(completableId, "xAPI Exception: Target ID is null or empty. Ignoring.", "xAPI Exception: Target ID can't be null or empty."))
        {
            tracker.setSuccess(success);
            return tracker.Trace(new TrackerEvent(tracker)
            {
                Event = new TrackerEvent.TraceVerb(Verb.Completed),
                Target = new TrackerEvent.TraceObject(type.ToString().ToLower(), completableId),
                Result = new TrackerEvent.TraceResult()
                {
                    Score = 1f
                }
            });
        }
        return null;
    }

    /// <summary>
    /// Player completed a completable.
    /// </summary>
    /// <param name="completableId">Completable identifier.</param>
    /// <param name="type">Completable type.</param>
    /// <param name="score">Completable score.</param>
    public TrackerEvent Completed(string completableId, Completable type, float score)
    {
        if (tracker.Utils.check<TargetXApiException>(completableId, "xAPI Exception: Target ID is null or empty. Ignoring.", "xAPI Exception: Target ID can't be null or empty."))
        {
            tracker.setScore(score);
            return tracker.Trace(new TrackerEvent(tracker)
            {
                Event = new TrackerEvent.TraceVerb(Verb.Completed),
                Target = new TrackerEvent.TraceObject(type.ToString().ToLower(), completableId),
                Result = new TrackerEvent.TraceResult()
                {
                    Success = true
                }
            });
        }
        return null;
    }

    /// <summary>
    /// Player completed a completable.
    /// </summary>
    /// <param name="completableId">Completable identifier.</param>
    /// <param name="type">Completable type.</param>
    /// <param name="success">Completable success.</param>
    /// <param name="score">Completable score.</param>
    public TrackerEvent Completed(string completableId, Completable type, bool success, float score)
    {
        if (tracker.Utils.check<TargetXApiException>(completableId, "xAPI Exception: Target ID is null or empty. Ignoring.", "xAPI Exception: Target ID can't be null or empty."))
        {
            tracker.setSuccess(success);
            tracker.setScore(score);
            return tracker.Trace(new TrackerEvent(tracker)
            {
                Event = new TrackerEvent.TraceVerb(Verb.Completed),
                Target = new TrackerEvent.TraceObject(type.ToString().ToLower(), completableId)
            });
        }
        return null;
    }

}
