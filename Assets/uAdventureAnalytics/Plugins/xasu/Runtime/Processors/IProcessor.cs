
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TinCan;

namespace Xasu.Processors
{
    public interface IProcessor
    {
        ProcessorState State { get; }
        int TracesPending { get; }
        int TracesCompleted { get; }
        int TracesFailed { get; }
        string ErrorMessage { get; }
        Task Init();
        Task<Statement> Enqueue(Statement statement);
        Task Process(bool complete = false);
        Task Finalize(IProgress<float> progress);
        Task Reset();
    }
}
