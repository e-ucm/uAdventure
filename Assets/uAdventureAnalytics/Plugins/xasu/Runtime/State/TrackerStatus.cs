using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using Xasu.Auth.Protocols;
using Xasu.Processors;
using Xasu.Util;

namespace Xasu
{
    public class TrackerStatus
    {
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]

        public TrackerState State { get; private set; }
        public string ErrorMessage { get; private set; }
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]

        public AuthState OnlineAuthState { get; private set; }
        public string OnlineAuthErrorMessage { get; private set; }
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]

        public ProcessorState OnlineState { get; private set; }
        public int OnlinePending { get; private set; }
        public int OnlineCompleted { get; private set; }
        public int OnlineFailed { get; private set; }
        public int ToFallback { get; private set; }
        public int FallbackSent { get; private set; }
        public int FallbackFailed { get; private set; }
        public string OnlineErrorMessage { get; private set; }
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]

        public ProcessorState LocalState { get; private set; }
        public int LocalPending { get; private set; }
        public int LocalCompleted { get; private set; }
        public int LocalFailed { get; private set; }
        public string LocalErrorMessage { get; private set; }
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]

        public AuthState BackupAuthState { get; private set; }
        public string BackupAuthErrorMessage { get; private set; }
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]

        public ProcessorState BackupState { get; private set; }
        public int BackupPending { get; private set; }
        public int BackupCompleted { get; private set; }
        public int BackupFailed { get; private set; }
        public string BackupErrorMessage { get; private set; }

        public bool IsNetworkRequired { get; private set; }

        private IProcessor onlineProcessor, localProcessor, backupProcessor;
        private IAuthProtocol onlineAuthProtocol, backupAuthProtocol;

        private bool monitoring = false;

        public TrackerStatus()
        {
            State = TrackerState.Uninitialized;
        }

        public void Monitor(IProcessor onlineProcessor, IProcessor localProcessor, IProcessor backupProcessor, IAuthProtocol onlineAuthProtocol, IAuthProtocol backupAuthProtocol)
        {
            monitoring = true;
            this.onlineProcessor = onlineProcessor;
            this.localProcessor = localProcessor;
            this.backupProcessor = backupProcessor;
            this.onlineAuthProtocol = onlineAuthProtocol;
            this.backupAuthProtocol = backupAuthProtocol;
        }

        public Exception InitException { get; set; }
        public Exception LoopException { get; set; }
        public Exception FinalizeException { get; set; }

        public void Update()
        {
            if (!monitoring)
            {
                return;
            }

            State = TrackerState.Normal;

            OnlineState = onlineProcessor?.State ?? ProcessorState.Disabled;
            LocalState = localProcessor?.State ?? ProcessorState.Disabled;
            BackupState = backupProcessor?.State ?? ProcessorState.Disabled;

            if (OnlineState == ProcessorState.Finalized
                && LocalState == ProcessorState.Finalized
                && BackupState == ProcessorState.Finalized)
            {
                State = TrackerState.Finalized;
            }

            // Init Exception
            if (InitException != null)
            {
                State = TrackerState.Errored;
                ErrorMessage = InitException.ToString();
            }

            // Loop Exception
            if (LoopException != null)
            {
                State = TrackerState.Errored;
                ErrorMessage = LoopException.ToString();
            }

            // Finalize Exception
            if (FinalizeException != null)
            {
                State = TrackerState.Errored;
                ErrorMessage = FinalizeException.ToString();
            }

            // Online Status
            if(onlineProcessor != null)
            {
                OnlinePending = onlineProcessor.TracesPending;
                OnlineCompleted = onlineProcessor.TracesCompleted;
                OnlineFailed = onlineProcessor.TracesFailed;
                ToFallback = ((OnlineProcessor)onlineProcessor).TracesToFallback;
                FallbackSent = ((OnlineProcessor)onlineProcessor).TracesFromFallbackSent;
                FallbackFailed = ((OnlineProcessor)onlineProcessor).TracesFromFallbackFailed;

                IsNetworkRequired = onlineProcessor.TracesPending > 0 && !NetworkInfo.IsWorking();
            }
            if (onlineAuthProtocol != null)
            {
                OnlineAuthState = onlineAuthProtocol.State;
                OnlineAuthErrorMessage = onlineAuthProtocol.State != AuthState.Working ? onlineAuthProtocol.ErrorMessage : "";
                if (onlineAuthProtocol.State != AuthState.Working)
                {
                    State = TrackerState.Errored;
                }
            }

            if (OnlineState == ProcessorState.Errored)
            {
                State = TrackerState.Errored;
                OnlineErrorMessage = onlineProcessor.ErrorMessage;
            }
            else if (OnlineState == ProcessorState.Fallback)
            {
                State = TrackerState.Normal;
                OnlineErrorMessage = onlineProcessor.ErrorMessage;
            }

            // Local Status
            if (localProcessor != null)
            {
                LocalPending = localProcessor.TracesPending;
                LocalCompleted = localProcessor.TracesCompleted;
                LocalFailed = localProcessor.TracesFailed;
            }
            if (LocalState == ProcessorState.Errored)
            {
                State = TrackerState.Errored;
                LocalErrorMessage = localProcessor.ErrorMessage;
            }

            // Backup Status
            if (backupProcessor != null)
            {
                BackupPending = backupProcessor.TracesPending;
                BackupCompleted = backupProcessor.TracesCompleted;
                BackupFailed = backupProcessor.TracesFailed;
            }
            if (backupAuthProtocol != null)
            {
                BackupAuthState = backupAuthProtocol.State;
                BackupAuthErrorMessage = backupAuthProtocol.State != AuthState.Working ? backupAuthProtocol.ErrorMessage : "";
                if (backupAuthProtocol.State != AuthState.Working)
                {
                    State = TrackerState.Errored;
                }
            }

            if (BackupState == ProcessorState.Errored)
            {
                State = TrackerState.Errored;
                BackupErrorMessage = backupProcessor.ErrorMessage;
            }
        }
    }
}
