using Newtonsoft.Json.Linq;
using Polly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TinCan;
using TinCan.Documents;
using TinCan.LRSResponses;
using Xasu.Requests;
using Xasu.Auth.Protocols;

namespace Xasu
{

    public class UnityLRS : IAsyncLRS
    {
        public string endpoint { get; set; }
        public TCAPIVersion version { get; set; }
        public IAuthProtocol auth { get; set; }
        public Dictionary<String, String> extended { get; set; } = new Dictionary<String, String>();

        public AsyncPolicy policy { get; set; }

        public UnityLRS(string endpoint)
        {
            this.endpoint = endpoint;
            this.version = TCAPIVersion.latest();
        }

        // TODO: handle special properties we recognize, such as content type, modified since, etc.
        private async Task<MyHttpResponse> MakeAsyncRequest(MyHttpRequest req)
        {
            // Set URL
            req.url = string.Empty;
            if (req.resource.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
            {
                req.url = req.resource;
            }
            else
            {
                req.url = endpoint.ToString();
                if (!req.url.EndsWith("/") && !req.resource.StartsWith("/"))
                {
                    req.url += "/";
                }
                req.url += req.resource;
            }

            // Set query params
            foreach(var extendedQueryParameters in extended)
            {
                if (!req.queryParams.ContainsKey(extendedQueryParameters.Key))
                {
                    req.queryParams.Add(extendedQueryParameters.Key, extendedQueryParameters.Value);
                }
            }

            // Set other headers
            req.headers["X-Experience-API-Version"] = version.ToString();

            // Set the authorization protocol
            if(req.authorization == null)
            {
                req.authorization = auth;
            }

            // Set the policy
            if(req.policy == null)
            {
                req.policy = policy;
            }

            return await RequestsUtility.DoRequest(req);
        }

        private async Task<MyHttpResponse> GetDocument(String resource, Dictionary<String, String> queryParams, Document document)
        {
            var req = new MyHttpRequest
            {
                method = "GET",
                resource = resource,
                queryParams = queryParams
            };

            var res = await MakeAsyncRequest(req);
            if (res.status == (int)HttpStatusCode.OK)
            {
                document.content = res.content;
                document.contentType = res.contentType;
                document.timestamp = res.lastModified;
                document.etag = res.etag;
            }

            return res;
        }

        private async Task<ProfileKeysLRSResponse> GetProfileKeys(String resource, Dictionary<String, String> queryParams)
        {
            var r = new ProfileKeysLRSResponse();

            var req = new MyHttpRequest
            {
                method = "GET",
                resource = resource,
                queryParams = queryParams
            };

            var res = await MakeAsyncRequest(req);
            if (res.status != (int)HttpStatusCode.OK)
            {
                r.success = false;
                r.httpException = res.ex;
                r.SetErrMsgFromBytes(res.content);
                return r;
            }

            r.success = true;

            var keys = JArray.Parse(Encoding.UTF8.GetString(res.content));
            if (keys.Count > 0)
            {
                r.content = new List<String>();
                foreach (JToken key in keys)
                {
                    r.content.Add((String)key);
                }
            }

            return r;
        }

        private async Task<LRSResponse> SaveDocument(String resource, Dictionary<String, String> queryParams, Document document)
        {
            var r = new LRSResponse();

            var req = new MyHttpRequest
            {
                method = "PUT",
                resource = resource,
                queryParams = queryParams,
                contentType = document.contentType,
                content = document.content
            };

            var res = await MakeAsyncRequest(req);
            if (res.status != (int)HttpStatusCode.NoContent)
            {
                r.success = false;
                r.httpException = res.ex;
                r.SetErrMsgFromBytes(res.content);
                return r;
            }

            r.success = true;

            return r;
        }

        private async Task<LRSResponse> DeleteDocument(String resource, Dictionary<String, String> queryParams)
        {
            var r = new LRSResponse();

            var req = new MyHttpRequest
            {
                method = "DELETE",
                resource = resource,
                queryParams = queryParams
            };

            var res = await MakeAsyncRequest(req);
            if (res.status != (int)HttpStatusCode.NoContent)
            {
                r.success = false;
                r.httpException = res.ex;
                r.SetErrMsgFromBytes(res.content);
                return r;
            }

            r.success = true;

            return r;
        }

        private async Task<StatementLRSResponse> GetStatement(Dictionary<String, String> queryParams)
        {
            var r = new StatementLRSResponse();

            var req = new MyHttpRequest
            {
                method = "GET",
                resource = "statements",
                queryParams = queryParams
            };

            var res = await MakeAsyncRequest(req);
            if (res.status != (int)HttpStatusCode.OK)
            {
                r.success = false;
                r.httpException = res.ex;
                r.SetErrMsgFromBytes(res.content);
                return r;
            }

            r.success = true;
            r.content = new Statement(new TinCan.Json.StringOfJSON(Encoding.UTF8.GetString(res.content)));

            return r;
        }

        public async Task<AboutLRSResponse> About()
        {
            var r = new AboutLRSResponse();

            var req = new MyHttpRequest
            {
                method = "GET",
                resource = "about"
            };

            var res = await MakeAsyncRequest(req);
            if (res.status != (int)(int)HttpStatusCode.OK)
            {
                r.success = false;
                r.httpException = res.ex;
                r.SetErrMsgFromBytes(res.content);
                return r;
            }

            r.success = true;
            r.content = new About(Encoding.UTF8.GetString(res.content));

            return r;
        }

        public async Task<StatementLRSResponse> SaveStatement(Statement statement)
        {
            var r = new StatementLRSResponse();
            var req = new MyHttpRequest { resource = "statements" };

            if (statement.id == null)
            {
                req.method = "POST";
            }
            else
            {
                req.method = "PUT";
                req.queryParams.Add("statementId", statement.id.ToString());
            }

            req.contentType = "application/json";
            req.content = Encoding.UTF8.GetBytes(statement.ToJSON(version));

            var res = await MakeAsyncRequest(req);
            if (statement.id == null)
            {
                if (res.status != (int)HttpStatusCode.OK)
                {
                    r.success = false;
                    r.httpException = res.ex;
                    r.SetErrMsgFromBytes(res.content);
                    return r;
                }

                var ids = JArray.Parse(Encoding.UTF8.GetString(res.content));
                statement.id = new Guid((String)ids[0]);
            }
            else
            {
                if (res.status != (int)HttpStatusCode.NoContent)
                {
                    r.success = false;
                    r.httpException = res.ex;
                    r.SetErrMsgFromBytes(res.content);
                    return r;
                }
            }

            r.success = true;
            r.content = statement;

            return r;
        }
        public async Task<StatementLRSResponse> VoidStatement(Guid id, Agent agent)
        {
            var voidStatement = new Statement
            {
                actor = agent,
                verb = new Verb
                {
                    id = new Uri("http://adlnet.gov/expapi/verbs/voided"),
                    display = new LanguageMap()
                },
                target = new StatementRef { id = id }
            };
            voidStatement.verb.display.Add("en-US", "voided");

            return await SaveStatement(voidStatement);
        }
        public async Task<StatementsResultLRSResponse> SaveStatements(List<Statement> statements)
        {
            var r = new StatementsResultLRSResponse();

            var req = new MyHttpRequest
            {
                resource = "statements",
                method = "POST",
                contentType = "application/json"
            };

            var jarray = new JArray();
            foreach (Statement st in statements)
            {
                jarray.Add(st.ToJObject(version));
            }
            req.content = Encoding.UTF8.GetBytes(jarray.ToString());

            var res = await MakeAsyncRequest(req);
            if (res.status != (int)HttpStatusCode.OK)
            {
                r.success = false;
                r.httpException = res.ex;
                r.SetErrMsgFromBytes(res.content);
                return r;
            }

            var ids = JArray.Parse(Encoding.UTF8.GetString(res.content));
            for (int i = 0; i < ids.Count; i++)
            {
                statements[i].id = new Guid((String)ids[i]);
            }

            r.success = true;
            r.content = new StatementsResult(statements);

            return r;
        }
        public async Task<StatementLRSResponse> RetrieveStatement(Guid id)
        {
            var queryParams = new Dictionary<String, String>
            {
                { "statementId", id.ToString() }
            };

            return await GetStatement(queryParams);
        }
        public async Task<StatementLRSResponse> RetrieveVoidedStatement(Guid id)
        {
            var queryParams = new Dictionary<String, String>
            {
                { "voidedStatementId", id.ToString() }
            };

            return await GetStatement(queryParams);
        }
        public async Task<StatementsResultLRSResponse> QueryStatements(StatementsQuery query)
        {
            var r = new StatementsResultLRSResponse();

            var req = new MyHttpRequest
            {
                method = "GET",
                resource = "statements",
                queryParams = query.ToParameterMap(version)
            };

            var res = await MakeAsyncRequest(req);
            if (res.status != (int)HttpStatusCode.OK)
            {
                r.success = false;
                r.httpException = res.ex;
                r.SetErrMsgFromBytes(res.content);
                return r;
            }

            r.success = true;
            r.content = new StatementsResult(new TinCan.Json.StringOfJSON(Encoding.UTF8.GetString(res.content)));

            return r;
        }
        public async Task<StatementsResultLRSResponse> MoreStatements(StatementsResult result)
        {
            var r = new StatementsResultLRSResponse();

            var req = new MyHttpRequest
            {
                method = "GET",
                //resource = endpoint.GetLeftPart(UriPartial.Authority)
            };
            if (!req.resource.EndsWith("/"))
            {
                req.resource += "/";
            }
            req.resource += result.more;

            var res = await MakeAsyncRequest(req);
            if (res.status != (int)HttpStatusCode.OK)
            {
                r.success = false;
                r.httpException = res.ex;
                r.SetErrMsgFromBytes(res.content);
                return r;
            }

            r.success = true;
            r.content = new StatementsResult(new TinCan.Json.StringOfJSON(Encoding.UTF8.GetString(res.content)));

            return r;
        }

        // TODO: since param
        public Task<ProfileKeysLRSResponse> RetrieveStateIds(Activity activity, Agent agent, Nullable<Guid> registration = null)
        {
            var queryParams = new Dictionary<String, String>
            {
                { "activityId", activity.id.ToString() },
                { "agent", agent.ToJSON(version) }
            };
            if (registration != null)
            {
                queryParams.Add("registration", registration.ToString());
            }

            return GetProfileKeys("activities/state", queryParams);
        }

        public async Task<StateLRSResponse> RetrieveState(String id, Activity activity, Agent agent, Nullable<Guid> registration = null)
        {
            var r = new StateLRSResponse();

            var queryParams = new Dictionary<String, String>
            {
                { "stateId", id },
                { "activityId", activity.id.ToString() },
                { "agent", agent.ToJSON(version) }
            };

            var state = new StateDocument
            {
                id = id,
                activity = activity,
                agent = agent
            };

            if (registration != null)
            {
                queryParams.Add("registration", registration.ToString());
                state.registration = registration;
            }

            var resp = await GetDocument("activities/state", queryParams, state);
            if (resp.status != (int)HttpStatusCode.OK && resp.status != (int)HttpStatusCode.NotFound)
            {
                r.success = false;
                r.httpException = resp.ex;
                r.SetErrMsgFromBytes(resp.content);
                return r;
            }
            r.success = true;
            r.content = state;

            return r;
        }
        public async Task<LRSResponse> SaveState(StateDocument state)
        {
            var queryParams = new Dictionary<String, String>
            {
                { "stateId", state.id },
                { "activityId", state.activity.id.ToString() },
                { "agent", state.agent.ToJSON(version) }
            };
            if (state.registration != null)
            {
                queryParams.Add("registration", state.registration.ToString());
            }

            return await SaveDocument("activities/state", queryParams, state);
        }
        public async Task<LRSResponse> DeleteState(StateDocument state)
        {
            var queryParams = new Dictionary<String, String>
            {
                { "stateId", state.id },
                { "activityId", state.activity.id.ToString() },
                { "agent", state.agent.ToJSON(version) }
            };
            if (state.registration != null)
            {
                queryParams.Add("registration", state.registration.ToString());
            }

            return await DeleteDocument("activities/state", queryParams);
        }
        public async Task<LRSResponse> ClearState(Activity activity, Agent agent, Nullable<Guid> registration = null)
        {
            var queryParams = new Dictionary<String, String>
            {
                { "activityId", activity.id.ToString() },
                { "agent", agent.ToJSON(version) }
            };
            if (registration != null)
            {
                queryParams.Add("registration", registration.ToString());
            }

            return await DeleteDocument("activities/state", queryParams);
        }

        // TODO: since param
        public async Task<ProfileKeysLRSResponse> RetrieveActivityProfileIds(Activity activity)
        {
            var queryParams = new Dictionary<String, String>
        {
            { "activityId", activity.id.ToString() }
        };

            return await GetProfileKeys("activities/profile", queryParams);
        }
        public async Task<ActivityProfileLRSResponse> RetrieveActivityProfile(String id, Activity activity)
        {
            var r = new ActivityProfileLRSResponse();

            var queryParams = new Dictionary<String, String>();
            queryParams.Add("profileId", id);
            queryParams.Add("activityId", activity.id.ToString());

            var profile = new ActivityProfileDocument
            {
                id = id,
                activity = activity
            };

            var resp = await GetDocument("activities/profile", queryParams, profile);
            if (resp.status != (int)HttpStatusCode.OK && resp.status != (int)HttpStatusCode.NotFound)
            {
                r.success = false;
                r.httpException = resp.ex;
                r.SetErrMsgFromBytes(resp.content);
                return r;
            }
            r.success = true;
            r.content = profile;

            return r;
        }
        public async Task<LRSResponse> SaveActivityProfile(ActivityProfileDocument profile)
        {
            var queryParams = new Dictionary<String, String>
            {
                { "profileId", profile.id },
                { "activityId", profile.activity.id.ToString() }
            };

            return await SaveDocument("activities/profile", queryParams, profile);
        }
        public async Task<LRSResponse> DeleteActivityProfile(ActivityProfileDocument profile)
        {
            var queryParams = new Dictionary<String, String>
            {
                { "profileId", profile.id },
                { "activityId", profile.activity.id.ToString() }
            };
            // TODO: need to pass Etag?

            return await DeleteDocument("activities/profile", queryParams);
        }

        // TODO: since param
        public async Task<ProfileKeysLRSResponse> RetrieveAgentProfileIds(Agent agent)
        {
            var queryParams = new Dictionary<String, String>();
            queryParams.Add("agent", agent.ToJSON(version));

            return await GetProfileKeys("agents/profile", queryParams);
        }
        public async Task<AgentProfileLRSResponse> RetrieveAgentProfile(String id, Agent agent)
        {
            var r = new AgentProfileLRSResponse();

            var queryParams = new Dictionary<String, String>
            {
                { "profileId", id },
                { "agent", agent.ToJSON(version) }
            };

            var profile = new AgentProfileDocument
            {
                id = id,
                agent = agent
            };

            var resp = await GetDocument("agents/profile", queryParams, profile);
            if (resp.status != (int)HttpStatusCode.OK && resp.status != (int)HttpStatusCode.NotFound)
            {
                r.success = false;
                r.httpException = resp.ex;
                r.SetErrMsgFromBytes(resp.content);
                return r;
            }
            r.success = true;
            r.content = profile;

            return r;
        }
        public async Task<LRSResponse> SaveAgentProfile(AgentProfileDocument profile)
        {
            var queryParams = new Dictionary<String, String>
            {
                { "profileId", profile.id },
                { "agent", profile.agent.ToJSON(version) }
            };

            return await SaveDocument("agents/profile", queryParams, profile);
        }
        public async Task<LRSResponse> DeleteAgentProfile(AgentProfileDocument profile)
        {
            var queryParams = new Dictionary<String, String>
            {
                { "profileId", profile.id },
                { "agent", profile.agent.ToJSON(version) }
            };
            // TODO: need to pass Etag?

            return await DeleteDocument("agents/profile", queryParams);
        }
    }
}
