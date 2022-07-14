using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TinCan;

namespace Xasu.HighLevel
{
    public class StatementPromise
    {
        public Statement Statement { get; private set; }
        public Task<Statement> Promise { get; private set; }

        public TaskAwaiter<Statement> GetAwaiter() { return Promise.GetAwaiter(); }

        public StatementPromise(Statement statement, Task<Statement> task)
        {
            this.Statement = statement;
            this.Promise = task;
        }

        public StatementPromise WithSuccess(bool success)
        {
            Statement.result.success = success;
            return this;
        }

        public StatementPromise WithScore(double score)
        {
            Statement.result.score = new Score
            {
                scaled = score
            };
            return this;
        }


        public StatementPromise WithResultExtensions(Dictionary<string, object> extensions)
        {
            Statement.result.extensions = AddExtensions(Statement.result.extensions, extensions);
            return this;
        }
        public StatementPromise WithContextExtensions(Dictionary<string, object> extensions)
        {
            Statement.context.extensions = AddExtensions(Statement.result.extensions, extensions);
            return this;
        }

        private Extensions AddExtensions(Extensions traceExtensions, Dictionary<string, object> extensions)
        {
            var jObject = traceExtensions.ToJObject(TinCan.TCAPIVersion.V103);
            foreach (var stateExtension in extensions)
            {
                if (stateExtension.Value is int)
                {
                    jObject.Add(stateExtension.Key, (int)stateExtension.Value);
                }
                else
                {
                    jObject.Add(stateExtension.Key, stateExtension.Value.ToString());
                }
            }
            return new TinCan.Extensions(jObject);
        }
    }
}
