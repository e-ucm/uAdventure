using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TinCan;

namespace Xasu.HighLevel
{
    public class GameObjectTracker : AbstractHighLevelTracker<GameObjectTracker>
    {
        /// <summary>
        /// Types of GameObjects
        /// </summary>

        private readonly Dictionary<Enum, string> typeIds = new Dictionary<Enum, string>
        {
            { TrackedGameObject.Enemy,      "https://w3id.org/xapi/seriousgames/activity-types/enemy" },
            { TrackedGameObject.Npc,        "https://w3id.org/xapi/seriousgames/activity-types/non-player-character"},
            { TrackedGameObject.Item,       "https://w3id.org/xapi/seriousgames/activity-types/item"},
            { TrackedGameObject.GameObject, "https://w3id.org/xapi/seriousgames/activity-types/game-object"}
        };

        /// <summary>
        /// Verbs associated with the GameObject High Level API
        /// </summary>
        public enum Verb
        {
            Interacted,
            Used
        }

        private readonly Dictionary<Enum, string> verbIds = new Dictionary<Enum, string>
        {
            { Verb.Interacted,  "http://adlnet.gov/expapi/verbs/interacted"     },
            { Verb.Used,        "https://w3id.org/xapi/seriousgames/verbs/used" }
        };

        public enum TrackedGameObject
        {
            Enemy,
            Npc,
            Item,
            GameObject
        }
        protected override Dictionary<Enum, string> VerbIds => verbIds;

        protected override Dictionary<Enum, string> TypeIds => typeIds;

        protected override Dictionary<Enum, string> ExtensionIds => null;


        /// <summary>
        /// Player interacted with a game object.
        /// Type = GameObject 
        /// </summary>
        /// <param name="gameobjectId">Identifier.</param>
        public StatementPromise Interacted(string gameobjectId)
        {
            return Interacted(gameobjectId, TrackedGameObject.GameObject);
        }

        /// <summary>
        /// Player interacted with a game object.
        /// </summary>
        /// <param name="gameobjectId">Identifier.</param>
        /// <param name="type">TrackedGameObject type.</param>
        public StatementPromise Interacted(string gameobjectId, TrackedGameObject type)
        {
            return Enqueue(new Statement
            {
                verb = GetVerb(Verb.Interacted),
                target = GetTargetActivity(gameobjectId, type)
            });
        }

        /// <summary>
        /// Player interacted with a game object.
        /// Type = GameObject 
        /// </summary>
        /// <param name="gameobjectId">Reachable identifier.</param>
        public StatementPromise Used(string gameobjectId)
        {
            return Used(gameobjectId, TrackedGameObject.GameObject);
        }

        /// <summary>
        /// Player interacted with a game object.
        /// </summary>
        /// <param name="gameobjectId">TrackedGameObject identifier.</param>
        public StatementPromise Used(string gameobjectId, TrackedGameObject type)
        {
            return Enqueue(new Statement
            {
                verb = GetVerb(Verb.Used),
                target = GetTargetActivity(gameobjectId, type)
            });
        }
    }
}
