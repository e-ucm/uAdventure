using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using System.IO;
using Xasu.Processors;
using Xasu.Util;
using Xasu.Auth;
using Xasu.Config;
using Xasu.Auth.Protocols;
using Xasu.Exceptions;
using TinCan;
using UnityEngine;

namespace Xasu
{
    public class XasuTracker : Singleton<XasuTracker>
    {
        public bool AutoStart = false;

        public float processingLoopTime = 1; // In Seconds

        private bool processing = false;
        private string processingLock = "CoolLock";
        private bool flushRequested = false;
        private bool finalizeRequested = false;
        private float currentTime;
        private IProcessor[] traceProcessors;
        private TrackerStatus trackerStatus;

        private string errorLogFilename;

        public IAsyncLRS LRS { get; set; }
        public TrackerConfig TrackerConfig { get; private set; }
        public Agent DefaultActor { get; set; }
        public Context DefaultContext { get; set; }
        public string DefaultIdPrefix { get; set; }

        protected override void Awake()
        {
            base.Awake();
            trackerStatus = new TrackerStatus();
            string withOutSpecialCharacters = new string(Application.productName.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-').ToArray());
            DefaultIdPrefix = "https://" + withOutSpecialCharacters.Replace(' ','_') + "/";
        }
        protected async void Start()
        {
            if (AutoStart)
            {
                await Init();
            }
        }

        public TrackerStatus Status 
        { 
            get 
            {
                trackerStatus.Update();
                return trackerStatus;
            } 
        }

        public async Task Init()
        {
            // Init with local file config
            await Init(await TrackerConfigLoader.LoadLocalAsync());
        }

        public async Task Init(TrackerConfig trackerConfig, IAuthProtocol onlineAuthorization = null, IAuthProtocol backupAuthorization = null)
        {
            try
            {
                Debug.Log("[TRACKER] Initializing...");

                TrackerConfig = trackerConfig;
                errorLogFilename = Application.persistentDataPath + "/tracker_errors.log";
                var processors = new List<IProcessor>();

                // Working Modes and Backup
                IAuthProtocol onlineAuthProtocol = null, backupAuthProtocol = null;
                IProcessor onlineProcessor = null, localProcessor = null, backupProcessor = null;

                // TODO: Implement a ProcessorFactory that performs generic initialization
                if (TrackerConfig.Online)
                {
                    onlineAuthProtocol = onlineAuthorization ?? await AuthManager.InitAuth(TrackerConfig.AuthProtocol, TrackerConfig.AuthParameters, null); // TODO: Auth Policies
                    if (onlineAuthProtocol?.State == AuthState.Errored)
                    {
                        LogError("[TRACKER] Failed to initialize auth for LRS: " + onlineAuthProtocol.ErrorMessage);
                        return;
                    }

                    if (onlineAuthProtocol is Cmi5Protocol)
                    {
                        Debug.Log("[TRACKER] Initializing cmi5 online processor...");
                        onlineProcessor = new Cmi5Processor(TrackerConfig.BatchSize, onlineAuthProtocol, false);
                    }
                    else
                    {
                        Debug.Log("[TRACKER] Initializing online processor...");
                        onlineProcessor = new OnlineProcessor(TrackerConfig.LRSEndpoint, TCAPIVersion.V103,
                            TrackerConfig.BatchSize, onlineAuthProtocol, TrackerConfig.Fallback);
                    }

                    await onlineProcessor.Init();
                    processors.Add(onlineProcessor);
                }

                if (TrackerConfig.Offline)
                {
                    Debug.Log("[TRACKER] Initializing local processor...");
                    localProcessor = new LocalProcessor(TrackerConfig.FileName, TrackerConfig.TraceFormat);

                    await localProcessor.Init();
                    processors.Add(localProcessor);
                }

                if (TrackerConfig.Backup)
                {
                    if(backupAuthorization != null)
                    {
                        backupAuthProtocol = backupAuthorization;
                    }
                    else if (!string.IsNullOrEmpty(TrackerConfig.BackupAuthProtocol))
                    {
                        backupAuthProtocol = TrackerConfig.BackupAuthProtocol == "same" 
                            ? onlineAuthProtocol 
                            : await AuthManager.InitAuth(TrackerConfig.AuthProtocol, TrackerConfig.AuthParameters, null);
                    }

                    if (backupAuthProtocol != null && backupAuthProtocol.State == AuthState.Errored)
                    {
                        LogError("[TRACKER] Failed to initialize auth for backup: " + backupAuthProtocol.ErrorMessage);
                        return;
                    }

                    Debug.Log("[TRACKER] Initializing backup processor...");
                    backupProcessor = new BackupProcessor(TrackerConfig.BackupFileName, TrackerConfig.BackupTraceFormat, 
                        TrackerConfig.BackupEndpoint, TrackerConfig.BackupRequestConfig, backupAuthProtocol, null); // TODO: Backup policy

                    await backupProcessor.Init();
                    processors.Add(backupProcessor);
                }

                // Actor is obtained from authorization (e.g. OAuth contains username, CMI-5 obtains agent)
                DefaultActor = onlineAuthProtocol != null ? onlineAuthProtocol.Agent : new Agent { name = "Dummy User", mbox = "dummy@user.com" };

                traceProcessors = processors.ToArray();

                Status.Monitor(onlineProcessor, localProcessor, backupProcessor, onlineAuthProtocol, backupAuthProtocol);

                if (traceProcessors.Length == 0)
                {
                    Debug.LogWarning("[TRACKER] The tracker has been initialized with no output streams! " +
                        "Please active either online, offline and/or backup in the configuration!");
                }

                // Start the processing
                if (processors.Count > 0)
                {
                    Debug.Log("[TRACKER] Started!");
                    ProcessingLoop().WrapErrors();
                }
            }
            catch (Exception ex)
            {
                Status.InitException = ex;
                LogError("[TRACKER] Init exception!", ex);
                throw;
            }
            
        }

        public async Task Finalize(IProgress<float> progress = null)
        {
            if (Status.State == TrackerState.Uninitialized)
            {
                throw new InvalidOperationException("The tracker is not initialized!");
            }

            if (Status.State == TrackerState.Errored)
            {
                throw new InvalidOperationException("The tracker cannot be finalized in 'Errored' state. " +
                    "(Check the tracker status for more information)");
            }

            if (Status.State == TrackerState.Finalized)
            {
                throw new InvalidOperationException("The tracker is already finalized. " +
                    "(Check the tracker status for more information)");
            }

            finalizeRequested = true;
            
            try
            {
                await LockProcessing();
                var localProgress = new Progress<float>();
                float processorsDone = 0;
                float totalProcessors = (float)traceProcessors.Length;
                localProgress.ProgressChanged += (_, p) =>
                {
                    progress?.Report((processorsDone + p) / totalProcessors);
                };

                foreach (var p in traceProcessors
                    .Where(tp => tp.State == ProcessorState.Working || tp.State == ProcessorState.Fallback))
                {
                    await p.Finalize(localProgress);
                    processorsDone++;
                }

                progress?.Report(1f);
                UnlockProcessing();
            }
            catch (Exception ex)
            {
                Status.FinalizeException = ex;
                LogError("[TRACKER] Finalize failed!", ex);
                UnlockProcessing();
                throw;
            }
        }

        public async Task Flush()
        {
            if (Status.State == TrackerState.Finalized || Status.State == TrackerState.Uninitialized)
            {
                // Ignoring....
                return;
            }

            if (Status.State == TrackerState.Errored)
            {
                throw new InvalidOperationException("Flushing the tracker is not allowed in error state.");
            }

            flushRequested = true;

            while(flushRequested)
            {
                await Task.Yield();
                if (Status.LoopException != null) 
                {
                    throw new TrackerException("An exception ocurred during trace submission!", Status.LoopException);
                }
                else if (Status.State == TrackerState.Errored)
                {
                    throw new TrackerException("The tracker entered in error state! (Check the tracker status for more information)");
                }
            }

        }


        public Task<Statement> Enqueue(Statement statement)
        {
            if (Status.State == TrackerState.Uninitialized)
            {
                throw new InvalidOperationException("The tracker is not initialized! Initialize it using Init()");
            }

            if (Status.State == TrackerState.Finalized)
            {
                Debug.LogWarning("The tracker has been finalized. Traces enqueued won't be send!");
            }

            if (Status.State == TrackerState.Errored)
            {
                Debug.LogWarning("The tracker is in an errored state. Traces won't be send! (Check the tracker status for more information)");
            }

            if (statement == null)
            {
                throw new ArgumentNullException("Statement must be different than null!");
            }

            statement.SetPoolExtensions();
            AddDefaultsToTrace(statement);

            // When all processors are done we notify the listener
            return Task.WhenAll(traceProcessors.Select(p => p.Enqueue(statement)))
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        Debug.Log(t.Exception.GetType().ToString());
                        LogError(string.Format("[TRACKER ({0})] Couldn't send statement with id \"{1}\".",
                            Thread.CurrentThread.ManagedThreadId, statement.id), t.Exception);
                        throw t.Exception;
                    }

                    Debug.Log(string.Format("[TRACKER ({0})] All processors done with statement {1}", Thread.CurrentThread.ManagedThreadId, statement.id));

                    // All tasks return the same statement
                    return t.Result[0];
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public async Task ResetState()
        {
            foreach(var p in traceProcessors)
            {
                await p.Reset();
            }

            Status.InitException = null;
            Status.LoopException = null;
            Status.FinalizeException = null;
        }


        #region Private Methods

        private async Task ProcessingLoop()
        {
            try
            {
                while (true)
                {
                    await Task.Yield();
                    currentTime += Time.deltaTime;
                    var isFlushRequested = flushRequested;
                    if (HasToSendTraces())
                    {
                        await LockProcessing();
                        currentTime = 0;
                        foreach (var p in traceProcessors)
                        {
                            if(p.State != ProcessorState.Working && p.State != ProcessorState.Fallback)
                            {
                                continue;
                            }

                            await p.Process(isFlushRequested);
                        }

                        // If it was a flush, we turn off the flag
                        if (isFlushRequested)
                        {
                            flushRequested = false;
                        }
                        UnlockProcessing();
                    }

                }
            } 
            catch (Exception ex)
            {
                Status.LoopException = ex;
                LogError("[TRACKER] Main loop exception!", ex);
                UnlockProcessing();
            }
        }

        private void AddDefaultsToTrace(Statement statement)
        {
            // If we do not have an ID we create one so all processors store the trace with the same id
            if (statement.id == null || !statement.id.HasValue)
            {
                statement.id = Guid.NewGuid();
            }

            // Set the actor in case noone is provided
            if (statement.actor == null)
            {
                statement.actor = DefaultActor;
            }

            // Set the timestamp
            if (statement.timestamp == null || !statement.timestamp.HasValue)
            {
                statement.timestamp = DateTime.Now;
            }

            if (statement.context == null)
            {
                statement.context = DefaultContext;
            }
        }

        private async Task LockProcessing()
        {
            lock (processingLock)
            {
                if (!processing)
                {
                    processing = true;
                    return;
                }
            }

            while (processing)
            {
                await Task.Yield();

                lock (processingLock)
                {
                    if (!processing)
                    {
                        processing = true;
                        return;
                    }
                }
            }
        }

        private void UnlockProcessing()
        {
            lock (processingLock)
            {
                processing = false;
            }
        }

        private bool HasToSendTraces()
        {
            return (currentTime > processingLoopTime || flushRequested) && !finalizeRequested;
        }

#endregion

        internal void LogError(string error, Exception ex = null)
        {
            // Output unity console log
            Debug.LogError(error);
            if(ex!= null)
            {
                Debug.LogException(new TrackerException(error, ex));
            }

            // Output internal file log
            if (!File.Exists(errorLogFilename))
            {
                // Simplified disposal
                using (var _ = File.Create(errorLogFilename)) { }
            }

            var appendLines = ex != null ? new string[] { error, ex.ToString() } : new string[] { error };
            File.AppendAllLines(errorLogFilename, appendLines);
        }
    }
}

