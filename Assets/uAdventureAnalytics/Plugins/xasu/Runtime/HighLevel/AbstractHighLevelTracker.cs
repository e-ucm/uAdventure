using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using TinCan;

namespace Xasu.HighLevel
{
    public abstract class AbstractHighLevelTracker<T> : IHighLevelTracker where T : class, new()
    {
        protected static T instance;
        public static T Instance { get { return instance ?? (instance = new T()); } }

        public XasuTracker Tracker { get; set; }

        protected abstract Dictionary<Enum, string> VerbIds { get; }
        protected abstract Dictionary<Enum, string> TypeIds { get; }
        protected abstract Dictionary<Enum, string> ExtensionIds { get; }


        protected Verb GetVerb(Enum verb)
        {
            var verbDisplay = verb.ToString().ToLower();
            return new Verb
            {
                id = new Uri(VerbIds[verb]),
                display = new LanguageMap(new Dictionary<string, string>
                {
                    { "en", verbDisplay }
                })
            };
        }

        protected StatementTarget GetTargetActivity(string id, Enum type, string name = null, string description = null)
        {
            if (!Uri.IsWellFormedUriString(id, UriKind.Absolute))
            {
                id = XasuTracker.Instance.DefaultIdPrefix + id;
            }

            return new Activity
            {
                id = id,
                definition = new ActivityDefinition
                {
                    name = !string.IsNullOrEmpty(name) ? new LanguageMap(new Dictionary<string, string>
                    {
                        { "en-US", name}
                    }) : null,
                    description = !string.IsNullOrEmpty(description) ? new LanguageMap(new Dictionary<string, string>
                    {
                        { "en-US", description}
                    }) : null,
                    type = new Uri(TypeIds[type])
                }
            };
        }

        protected TinCan.Extensions GetExtensions(Dictionary<Enum, object> extensions)
        {
            var jobject = new JObject();
            foreach (var ex in extensions)
            {
                jobject.Add(ExtensionIds[ex.Key], JToken.FromObject(ex.Value));
            }
            
            return new TinCan.Extensions(jobject);
        }

        protected Result SetResultExtensions(Result result, Dictionary<Enum, object> extensions)
        {
            result.extensions = GetExtensions(extensions);
            return result;
        }

        protected Context SetContextExtensions(Context context, Dictionary<Enum, object> extensions)
        {
            context.extensions = GetExtensions(extensions);
            return context;
        }

        protected static StatementPromise Enqueue(Statement statement)
        {
            return new StatementPromise(statement, XasuTracker.Instance.Enqueue(statement));
        }

    }
}
