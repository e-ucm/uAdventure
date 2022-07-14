using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinCan;
using TinCan.Documents;
using TinCan.LRSResponses;
using UnityEngine;
using Xasu.Auth.Protocols;
using Xasu.CMI5;
using Xasu.Exceptions;
using Xasu.Util;

namespace Xasu.Processors
{
    public class Cmi5Processor : OnlineProcessor
    {
        private const string retrieveStateDocumentError = "[CMI5] Failed to retrieve 'LMS.LaunchData' state document! Message: {0}";
        private const string retrieveAgentDocumentError = "[CMI5] Failed to retrieve 'cmi5LearnerPreferences' agent document! Message: {0}";
        private const string saveInitializedStatementError = "[CMI5] Failed to send 'initialized' statement! Message: {0}";
        private const string saveTerminatedStatementError = "[CMI5] Failed to send 'terminated' statement! Message: {0}";

        private DateTime initialized;

        public StateDocument StateDocument { get; private set; }
        public JObject StateDocumentData { get; private set; }
        public AgentProfileDocument AgentProfileDocument { get; private set; }

        public Cmi5Processor(int batchSize, IAuthProtocol authProtocol, bool fallback)
            : base(Cmi5Helper.Endpoint, TCAPIVersion.V103, batchSize, authProtocol, fallback)
        {
        }

        public override async Task Init()
        {
            // Init the Online processor so it prepares the LRS
            await base.Init();

            // Get 'LMS.LaunchData' state document
            var stateResponse = await lrs.RetrieveState("LMS.LaunchData", Cmi5Helper.Activity, Cmi5Helper.Actor, Cmi5Helper.Registration);
            if (!stateResponse.success)
            {
                State = ProcessorState.Errored;
                throw new Cmi5Exception(string.Format(retrieveStateDocumentError, stateResponse.errMsg), stateResponse.httpException);
            }

            if (!CircuitsClosed()) apiCircuitBreaker.Reset();

            StateDocument = stateResponse.content;
            if(StateDocument != null && StateDocument.content != null)
            {
                StateDocumentData = JObject.Parse(Encoding.UTF8.GetString(StateDocument.content));
            }
            Cmi5Helper.SetStateDocument(StateDocument);

            // Get 'cmi5LearnerPreferences' state document
            var agentResponse = await lrs.RetrieveAgentProfile("cmi5LearnerPreferences", Cmi5Helper.Actor);
            if (!agentResponse.success)
            {
                State = ProcessorState.Errored;
                throw new Cmi5Exception(string.Format(retrieveAgentDocumentError, agentResponse.errMsg), agentResponse.httpException);
            }
            AgentProfileDocument = agentResponse.content;
            if (!CircuitsClosed()) apiCircuitBreaker.Reset();

            // Setup the context
            XasuTracker.Instance.DefaultContext = Cmi5Helper.Cmi5Allowed;

            State = ProcessorState.Working;
            // Send initialized statement
            var initializedResponse = await SendInitializedAUStatement();
            if (!initializedResponse.success)
            {
                State = ProcessorState.Errored;
                throw new Cmi5Exception(string.Format(saveInitializedStatementError, initializedResponse.errMsg), initializedResponse.httpException);
            }
            if (!CircuitsClosed()) apiCircuitBreaker.Reset();

            State = ProcessorState.Working;
        }

        public override async Task Finalize(IProgress<float> progress)
        {
            await base.Finalize(progress);
            var terminatedResponse = await SendTerminatedAUStatement();

            if (!terminatedResponse.success)
            {
                State = ProcessorState.Errored;
                throw new Cmi5Exception(string.Format(saveTerminatedStatementError, terminatedResponse.errMsg), terminatedResponse.httpException);
            }

            if (Cmi5Helper.ReturnURL != null)
            {
                if (Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    WebGLUtility.OpenUrl(Cmi5Helper.ReturnURL.ToString());
                }
                else
                {
                    Application.OpenURL(Cmi5Helper.ReturnURL.ToString());
                }
            }
        }

        // PRIVATE METHODS

        private async Task<StatementLRSResponse> SendInitializedAUStatement()
        {
            var statement = new Statement
            {
                id = Guid.NewGuid(),
                verb = new Verb
                {
                    id = new Uri("http://adlnet.gov/expapi/verbs/initialized"),
                    display = new LanguageMap(new Dictionary<string, string>
                    {
                        { "en-US", "initialized"}
                    })
                },
                actor = Cmi5Helper.Actor,
                target = Cmi5Helper.Activity,
                context = Cmi5Helper.Cmi5Context,
                timestamp = initialized = DateTime.Now,
            };

            return await lrs.SaveStatement(statement);
        }
        private async Task<StatementLRSResponse> SendTerminatedAUStatement()
        {
            var statement = new Statement
            {
                id = Guid.NewGuid(),
                verb = new Verb
                {
                    id = new Uri("http://adlnet.gov/expapi/verbs/terminated"),
                    display = new LanguageMap(new Dictionary<string, string>
                    {
                        { "en-US", "terminated"}
                    })
                },
                actor = Cmi5Helper.Actor,
                target = Cmi5Helper.Activity,
                context = Cmi5Helper.Cmi5Context,
                timestamp = DateTime.Now,
                result = new Result
                {
                    duration = DateTime.Now - initialized
                }
            };

            return await lrs.SaveStatement(statement);
        }

    }
}
