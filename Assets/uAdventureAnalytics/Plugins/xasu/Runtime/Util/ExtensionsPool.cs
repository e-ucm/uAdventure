using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinCan;
using TinCan.Json;

namespace Xasu.Util
{
    public static class ExtensionsPool
    {
        private static List<KeyValuePair<string, object>> resultsPool, contextPool;
        private static Dictionary<string, object> staticResultsPool, staticContextPool;

        static ExtensionsPool()
        {
            resultsPool = new List<KeyValuePair<string, object>>();
            contextPool = new List<KeyValuePair<string, object>>();
            staticResultsPool = new Dictionary<string, object>();
            staticContextPool = new Dictionary<string, object>();
        }

        public static void AddResultExtension(string key, object value)
        {
            resultsPool.Add(new KeyValuePair<string, object>(key, value));
        }

        public static void AddContextExtension(string key, object value)
        {
            contextPool.Add(new KeyValuePair<string, object>(key, value));
        }

        public static void SetPermanentResultExtension(string key, object value)
        {
            staticResultsPool[key] = value;
        }

        public static void SetPermanentContextExtension(string key, object value)
        {
            staticContextPool[key] = value;
        }

        public static Statement SetPoolExtensions(this Statement statement)
        {
            SetExtensions(statement);
            return statement;
        }

        public static void SetExtensions(Statement statement)
        {
            if (statement == null)
            {
                throw new ArgumentNullException("Statement is null!");
            }

            InitializeResultAndContext(statement);

            // Add Result extensions
            JObject jobject = statement.result.extensions.ToJObject();
            AddPoolToJObject(staticResultsPool.ToList(), jobject);
            AddPoolToJObject(resultsPool, jobject);
            statement.result.extensions = new TinCan.Extensions(jobject);
            // Empty Results Pool
            resultsPool.Clear();

            // Add Context extensions
            jobject = statement.context.extensions.ToJObject();
            AddPoolToJObject(staticContextPool.ToList(), jobject);
            AddPoolToJObject(contextPool, jobject);
            statement.context.extensions = new TinCan.Extensions(jobject);
            // Empty Context Pool
            contextPool.Clear();
        }

        private static void AddPoolToJObject(List<KeyValuePair<string, object>> pool, JObject jobject)
        {
            for (int i = 0; i < pool.Count; i++)
            {
                jobject.Add(resultsPool[i].Key, JToken.FromObject(resultsPool[i].Value));
            }
        }

        private static void InitializeResultAndContext(Statement statement)
        {
            if (statement.result == null)
            {
                statement.result = new Result();
            }

            if (statement.result.extensions == null)
            {
                statement.result.extensions = new TinCan.Extensions();
            }

            if (statement.context == null)
            {
                if(XasuTracker.Instance.DefaultContext != null)
                {
                    // Workaround to clone the context
                    statement.context = new Context(new StringOfJSON(XasuTracker.Instance.DefaultContext.ToJSON()));
                }
                else
                {
                    statement.context = new Context();
                }
            }

            if (statement.context.extensions == null)
            {
                statement.context.extensions = new TinCan.Extensions();
            }
        }
    }
}
