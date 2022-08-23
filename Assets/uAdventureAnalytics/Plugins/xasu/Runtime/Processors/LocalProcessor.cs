using System;
using System.Threading.Tasks;
using TinCan;
using Xasu.Config;
using CircularBuffer;
using System.IO;
using UnityEngine;
using Xasu.Processors.Formatter;
using Xasu.Util;
using System.Linq;
using System.Threading;

namespace Xasu.Processors
{
    // Oriented to save data in Application.persistentDataPath
    public class LocalProcessor : IProcessor
    {
        internal CircularBuffer<TraceTask> localQueue;
        protected readonly TraceFormats traceFormat;
        protected readonly TCAPIVersion version;
        protected readonly string file;
        public string ErrorMessage { get; protected set; }

        public virtual ProcessorState State { get; protected set; }

        public int TracesCompleted { get; protected set; }

        public int TracesFailed { get; protected set; }

        public virtual int TracesPending { get { return localQueue.Size; } }

        public LocalProcessor(string filename, TraceFormats traceFormat, bool useTempDataPath = false)
            : this(filename, traceFormat, TCAPIVersion.V103, useTempDataPath)
        {
        }

        public LocalProcessor(string filename, TraceFormats traceFormat, TCAPIVersion version, bool useTempDataPath)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentException("Filename is null or empty!");
            }

            this.localQueue = new CircularBuffer<TraceTask>(1000);
            this.file = (useTempDataPath ? Application.temporaryCachePath : Application.persistentDataPath) + "/" + filename;
            this.traceFormat = traceFormat;
            this.version = version;
            State = ProcessorState.Created;
        }

        public Task<Statement> Enqueue(Statement statement)
        {
            var completionSource = new TaskCompletionSource<Statement>();
            
            localQueue.PushBack(
                new TraceTask {
                    completionSource = completionSource,
                    statement = statement 
                }
            );
            return completionSource.Task;
        }

        public virtual Task Process(bool complete = false)
        {
            // Only process in Working o Fallback modes
            if (State != ProcessorState.Working && State != ProcessorState.Fallback)
            {
                return Task.FromResult(0);
            }

            // Only process if there are any completed traces
            if (!localQueue.Completed().Any())
            {
                return Task.FromResult(0);
            }

            var writer = File.AppendText(file);
            TraceTask traceTask = default;
            var toPop = 0;
            try
            {
                foreach (var t in localQueue.Completed())
                {
                    traceTask = t;
                    writer.WriteLine(TraceFormatter.Format(t.statement, traceFormat, version));
                    toPop++;
                    TracesCompleted++;
                    Debug.Log(string.Format("[TRACKER ({0}): {1}] Done with statement. {2} ", Thread.CurrentThread.ManagedThreadId, this.GetType(), t.statement.id));
                    t.completionSource.SetResult(t.statement);
                }
            }
            catch (IOException ex)
            {
                TracesFailed++;
                Debug.LogError(string.Format("[TRACKER: {0}] Failed to write statement. {1}: {2} ", this.GetType(), ex.GetType(), ex.Message));
                traceTask.completionSource?.SetException(ex);
                State = ProcessorState.Errored;
                ErrorMessage = ex.ToString();
            }

            // After finishing iteration we pop the same amount of traces
            while (toPop > 0)
            {
                localQueue.PopFront();
                toPop--;
            }

            writer.Flush();
            writer.Close();

            return Task.FromResult(0);
        }

        public virtual async Task Finalize(IProgress<float> progress)
        {
            await Process();
            State = ProcessorState.Finalized;
        }

        public virtual Task Init()
        {
            try
            {
                if (!File.Exists(file))
                {
                    // Create and close
                    File.Create(file).Close();
                }
            }
            // UnauthorizedAccessException
            // ArgumentException
            // ArgumentNullException
            // PathTooLongException
            // DirectoryNotFoundException
            // IOException
            // NotSupportedException
            catch (SystemException se)
            {
                State = ProcessorState.Errored;
                ErrorMessage = se.ToString();
            }

            State = ProcessorState.Working;

            return Task.FromResult(true);
        }

        public virtual Task Reset()
        {
            State = ProcessorState.Working;
            return Task.FromResult(true);
        }
    }
}
