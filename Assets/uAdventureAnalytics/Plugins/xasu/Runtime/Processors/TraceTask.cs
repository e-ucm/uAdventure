using System.Threading.Tasks;
using TinCan;

namespace Xasu.Processors
{

    internal struct TraceTask
    {
        public TaskCompletionSource<Statement> completionSource;
        public Statement statement;
    }
}
