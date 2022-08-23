using Polly;
using Polly.CircuitBreaker;
using Polly.Contrib.WaitAndRetry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TinCan;
using UnityEngine;
using Xasu.Auth.Protocols;
using Xasu.Exceptions;
using Xasu.Requests;
using Xasu.Util;

namespace Xasu.Processors
{
    // Oriented to send traces to an online LRS and, when disconnected, use Application.temporaryCachePath as buffer
    // (WebGL will use persistentDataPath when needed to store traces in IndexedDB)
    public class OnlineProcessor : LocalProcessor
    {
        private float trackerProcessingTime;
        private float minTrackerProcessingTime = 0.1f;

        private List<TraceTask> lastTraceTasks;
        private List<Statement> lastStatements;
        private readonly string lrsEndpoint;
        private readonly bool fallback;
        private readonly int batchSize;
        private readonly IAuthProtocol authProtocol;
        private readonly string failedTracesFile;
        private readonly string fallbackTmpFile;

        private int currentBatchSize;
        protected IAsyncLRS lrs;
        private bool hasFallbackTraces;
        protected ICircuitBreakerPolicy networkCircuitBreaker, apiCircuitBreaker;

        public int TracesToFallback = 0, TracesFromFallbackSent = 0, TracesFromFallbackFailed = 0;

        public override int TracesPending { get { return base.TracesPending + TracesToFallback - (TracesFromFallbackSent + TracesFromFallbackFailed); } }

        public OnlineProcessor(Uri lrsEndpoint, TCAPIVersion version, int batchSize, IAuthProtocol authProtocol,
            bool fallback) : this(lrsEndpoint.ToString(), version, batchSize, authProtocol, fallback)
        {
        }

        public OnlineProcessor(string lrsEndpoint, TCAPIVersion version, int batchSize, IAuthProtocol authProtocol, 
            bool fallback) : base("fallback.log", Config.TraceFormats.XAPI, version, true)
        {
            failedTracesFile = Application.persistentDataPath + "/failed_traces.log";
            fallbackTmpFile = Application.temporaryCachePath + "/fallback.tmp";
            lastTraceTasks = new List<TraceTask>();
            lastStatements = new List<Statement>();
            this.lrsEndpoint = lrsEndpoint;
            this.fallback = fallback;
            this.batchSize = currentBatchSize = batchSize;
            this.authProtocol = authProtocol;
        }

        public override async Task Init()
        {
            trackerProcessingTime = XasuTracker.Instance.processingLoopTime;

            // Init Auth
            this.lrs = new UnityLRS(lrsEndpoint)
            {
                auth = authProtocol,
                policy = ConfigureLRSPolicy()
            };
            await base.Init();

            // Restore fallback traces when restarting the traces
            if (new System.IO.FileInfo(file).Length != 0)
            {
                hasFallbackTraces = true;
            }
        }

        public override async Task Process(bool complete = false)
        {
            do
            {

                // Only process in Working o Fallback modes
                if (State != ProcessorState.Working && State != ProcessorState.Fallback)
                {
                    return;
                }

                // Normal behavior
                if (CircuitsClosed())
                {
                    // First process fallback saved traces
                    // Only process if there are traces to process

                    var hasTracesReady = LoadFallbackTracesToLists(currentBatchSize) || LoadTracesToLists(currentBatchSize);
                    if (!hasTracesReady)
                    {
                        return;
                    }

                    try
                    {
                        var response = await lrs.SaveStatements(lastStatements);

                        if (response.success)
                        {
                            // Enlarge batch size until we reach the maximum size
                            currentBatchSize = Mathf.Min(batchSize, currentBatchSize * 2);
                            XasuTracker.Instance.processingLoopTime = Mathf.Min(trackerProcessingTime, XasuTracker.Instance.processingLoopTime * 2f);

                            if (hasFallbackTraces)
                            {
                                PopFallbackTraces();
                            }
                            else
                            {
                                NotifyCompletedAndPopTraces();
                            }
                        }
                        // Response.success == false
                        else if (response.httpException is APIException apiEx)
                        {
                            switch ((HttpStatusCode)apiEx.HttpCode)
                            {
                                // Normal LRS Issues
                                case HttpStatusCode.BadRequest: // Malformed statements 
                                case HttpStatusCode.Conflict: // One statement has the same id
                                case HttpStatusCode.RequestEntityTooLarge: // The request is too large
                                    // In any case we send half of the batch until it's accepted or 
                                    // a single statement is rejected and continue
                                    if (lastStatements.Count == 1)
                                    {
                                        if (hasFallbackTraces)
                                        {
                                            MoveFailedTraceLocallyAndLogError(apiEx);
                                        }
                                        else
                                        {
                                            NotifyFailedAndPopTrace(apiEx);
                                        }
                                    }
                                    else
                                    {
                                        XasuTracker.Instance.LogError("[TRACKER: Online Processor] Failed to submit traces. Reducing flush size. Error: " + response.errMsg);
                                        currentBatchSize = Mathf.Max(1, currentBatchSize / 2);
                                        XasuTracker.Instance.processingLoopTime = Mathf.Max(minTrackerProcessingTime, XasuTracker.Instance.processingLoopTime / 2f);
                                    }
                                    apiCircuitBreaker.Reset();
                                    break;

                                // Authorization issues are forwarded to the auth protocol
                                case HttpStatusCode.Unauthorized:
                                    State = ProcessorState.Errored;
                                    ErrorMessage = "Unauthorized: " + apiEx.Message;
                                    authProtocol?.Unauthorized(apiEx);
                                    break;
                                case HttpStatusCode.Forbidden:
                                    State = ProcessorState.Errored;
                                    ErrorMessage = "Forbidden: " + apiEx.Message;
                                    authProtocol?.Forbidden(apiEx);
                                    break;
                                default:
                                    XasuTracker.Instance.LogError("[TRACKER: Online Processor] Failed to submit traces with API (" + apiEx.HttpCode + ") response: " + response.errMsg);
                                    break;
                            }
                        }
                        else
                        {
                            XasuTracker.Instance.LogError("[TRACKER: Online Processor] Failed to submit traces with response: " + response.errMsg);
                        }
                    }
                    catch (NetworkException networkException)
                    {
                        XasuTracker.Instance.LogError("[TRACKER: Online Processor] Network failed: " + networkException.Message);
                    }
                    catch (BrokenCircuitException)
                    {
                        Debug.LogWarning("The tracker tried to send traces while the circuit was open, generating an exception. Resetting the circuit...");
                        networkCircuitBreaker.Reset();
                        apiCircuitBreaker.Reset();
                    }
                }

                // Fallback behavior
                if (!CircuitsClosed() && fallback)
                {
                    do
                    {
                        State = ProcessorState.Fallback;
                        // The fallback mode saves traces locally until the circuit is closed again
                        hasFallbackTraces = true;
                        var prevSize = localQueue.Size;
                        await base.Process();
                        TracesToFallback += prevSize - localQueue.Size;
                    } while (complete && localQueue.Size > 0);
                }

                // When requesting a complete flush traces can be stored in fallback, but finalize has to stop, and we must raise an exception
                if (complete && !CircuitsClosed())
                {
                    if (apiCircuitBreaker.CircuitState == CircuitState.Open || apiCircuitBreaker.CircuitState == CircuitState.Isolated)
                    {
                        throw apiCircuitBreaker.LastException;
                    }
                    else
                    {
                        throw networkCircuitBreaker.LastException;
                    }
                }

            } while (complete && (hasFallbackTraces || localQueue.Size > 0));
        }

        protected bool CircuitsClosed()
        {
            return (networkCircuitBreaker.CircuitState == CircuitState.Closed || networkCircuitBreaker.CircuitState == CircuitState.HalfOpen)
                && (apiCircuitBreaker.CircuitState == CircuitState.Closed || apiCircuitBreaker.CircuitState == CircuitState.HalfOpen);
        }

        public override async Task Finalize(IProgress<float> progress)
        {
            // Reset the circuits in case they are closed
            ResetCircuits();

            progress?.Report(0);
            float total = TracesPending;

            // Asynchronous processing
            var task = Process(true);
            while ((hasFallbackTraces || TracesPending > 0) && !task.IsCompleted)
            {
                await Task.Yield();
                progress?.Report((total - TracesPending) / total);
            }
        }

        private AsyncPolicy ConfigureLRSPolicy()
        {
            // Retry policy handles both connection and API exceptions
            var retryDelay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 5);
            var retryPolicy = Policy
                .Handle<APIException>(apiEx => 
                    apiEx.HttpCode >= HttpStatus.InternalServerError ||
                    apiEx.HttpCode == HttpStatus.PreconditionFailed ||
                    apiEx.HttpCode == HttpStatus.TooManyRequests
                  )
                  .Or<NetworkException>()
                  .UnityWaitAndRetryAsync(retryDelay);

            // Connection breaker restarts after 10 seconds
            var connectionBreaker = Policy
                .Handle<NetworkException>()
                .CircuitBreakerAsync(1, TimeSpan.FromSeconds(10));
            networkCircuitBreaker = connectionBreaker;

            // API breaker restarts after 30 seconds
            var apiBreaker = Policy
                .Handle<APIException>(apiEx => apiEx.HttpCode != 404)
                .CircuitBreakerAsync(1, TimeSpan.FromSeconds(30));
            apiCircuitBreaker = apiBreaker;

            // Final policy: Retry -> Network Circuit -> API Circuit
            return apiBreaker
                .WrapAsync(connectionBreaker)
                .WrapAsync(retryPolicy);
        }

        private bool LoadTracesToLists(int n)
        {
            lastTraceTasks.Clear();
            lastStatements.Clear();
            for (int i = 0; i < Math.Min(n, localQueue.Size); i++)
            {
                var traceTask = localQueue[i];
                if (traceTask.statement.IsPartial())
                {
                    break;
                }
                // Get statements from the end of the queue
                lastTraceTasks.Add(traceTask);
                lastStatements.Add(traceTask.statement);
            }

            return lastStatements.Count > 0;
        }

        private void NotifyCompletedAndPopTraces()
        {
            foreach(var trace in lastTraceTasks)
            {
                TracesCompleted++;
                Debug.Log(string.Format("[TRACKER ({0}): {1}] Done with statement. {2} ", Thread.CurrentThread.ManagedThreadId, this.GetType(), trace.statement.id));
                trace.completionSource.SetResult(trace.statement);
                localQueue.PopFront();
            }
        }

        private void NotifyFailedAndPopTrace(Exception ex)
        {
            foreach (var trace in lastTraceTasks)
            {
                TracesFailed++;
                XasuTracker.Instance.LogError(string.Format("[TRACKER ({0}): {1}] Statement failed. {2} ", Thread.CurrentThread.ManagedThreadId, this.GetType(), trace.statement.id), ex);
                trace.completionSource.SetException(ex);
                localQueue.PopFront();
            }
        }

        // TODO: catch exceptions
        private bool LoadFallbackTracesToLists(int n)
        {
            if (!hasFallbackTraces)
            {
                return false;
            }

            lastTraceTasks.Clear();
            lastStatements.Clear();

            using (StreamReader reader = File.OpenText(file))
            {
                string line = String.Empty;
                while (lastStatements.Count < n && (line = reader.ReadLine()) != null)
                {
                    var statement = new Statement(new TinCan.Json.StringOfJSON(line));
                    lastStatements.Add(statement);
                }
            }

            return lastStatements.Count > 0;
        }


        // TODO: catch exceptions
        private void PopFallbackTraces()
        {
            bool tracesPending = false;
            using (var newFallbackStream = File.OpenWrite(fallbackTmpFile))
            using (var oldFallbackStream = File.OpenRead(file))
            using (var newFallback = new StreamWriter(newFallbackStream))
            using (var oldFallback = new StreamReader(oldFallbackStream))
            {
                // Skip the sent traces
                for (int i = 0; i < lastStatements.Count; i++)
                {
                    TracesFromFallbackSent++;
                    Debug.Log(string.Format("[TRACKER ({0}): {1}] Done submitting fallback statement. {2} ", Thread.CurrentThread.ManagedThreadId, this.GetType(), lastStatements[i].id));
                    oldFallback.ReadLine();
                }

                // Move the rest of the traces to a new file
                string line;
                while ((line = oldFallback.ReadLine()) != null)
                {
                    newFallback.WriteLine(line);
                    tracesPending = true;
                }

                newFallback.Flush();
            }

            // Replace the old fallback with the new file
            File.Replace(fallbackTmpFile, file, file+".bac");

            if (!tracesPending)
            {
                hasFallbackTraces = false;
                State = ProcessorState.Working;
            }
        }

        private void MoveFailedTraceLocallyAndLogError(Exception ex)
        {
            bool tracesPending = false;
            try
            {
                if (!File.Exists(failedTracesFile))
                {
                    // "using" simplifies disposing
                    using (var _ = File.Create(failedTracesFile)) { }
                }

                using (var newFallbackStream = File.OpenWrite(fallbackTmpFile))
                using (var oldFallbackStream = File.OpenRead(file))
                using (var newFallback = new StreamWriter(newFallbackStream))
                using (var failedTraces = File.AppendText(failedTracesFile))
                using (var oldFallback = new StreamReader(oldFallbackStream))
                { 
                    // Skip the sent traces (Count should be == 1)
                    for (int i = 0; i < lastStatements.Count; i++)
                    {
                        TracesFromFallbackFailed++;
                        // Log the error in the main log
                        XasuTracker.Instance.LogError(string.Format("[TRACKER ({0}): {1}] Failed to send fallback trace with id \"{2}\".",
                            Thread.CurrentThread.ManagedThreadId, this.GetType(), lastStatements[i].id), ex);

                        // Move the cursor
                        var failedTrace = oldFallback.ReadLine();
                        failedTraces.WriteLine(failedTrace);
                    }

                    string line;
                    while ((line = oldFallback.ReadLine()) != null)
                    {
                        newFallback.WriteLine(line);
                        tracesPending = true;
                    }

                    failedTraces.Flush();
                    newFallback.Flush();
                }

                // Replace the old fallback with the new file
                File.Replace(fallbackTmpFile, file, file + ".bac");
            }
            catch (SystemException se)
            {
                XasuTracker.Instance.LogError(string.Format("[TRACKER ({0}): {1}] Failed to write traces in fallback errors.",
                    Thread.CurrentThread.ManagedThreadId, this.GetType()), se);
            }

            if (!tracesPending)
            {
                hasFallbackTraces = false;
                State = ProcessorState.Working;
            }
        }

        public override Task Reset()
        {
            State = hasFallbackTraces ? ProcessorState.Fallback : ProcessorState.Working;
            ResetCircuits();
            return Task.FromResult(true);
        }

        private void ResetCircuits()
        {
            networkCircuitBreaker.Reset();
            apiCircuitBreaker.Reset();
        }
    }
}
