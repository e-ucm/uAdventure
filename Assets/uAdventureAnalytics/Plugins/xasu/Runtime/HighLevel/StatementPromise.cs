using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TinCan;
using UnityEngine;
using Xasu.Util;

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
                ExtensionUtil.AddExtensionToJObject(stateExtension, jObject);
            }

            try
            {
                return new TinCan.Extensions(jObject);
            }
            catch(System.Exception ex)
            {
                if (XasuTracker.Instance.TrackerConfig.StrictMode)
                {
                    throw;
                }
                else
                {
                    Debug.LogWarning("[STRICT=OFF] Error adding extensions to trace. Ignoring...");
                    Debug.LogException(ex);
                }
                return traceExtensions;
            }
        }
    }
}
