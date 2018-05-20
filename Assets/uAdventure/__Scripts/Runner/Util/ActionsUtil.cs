using System.Collections.Generic;
using System.Linq;
using uAdventure.Core;

namespace uAdventure.Runner
{
    public static class ActionsUtil
    {
        public static IEnumerable<Action> Checked(this IEnumerable<Action> actions)
        {
            return actions.Where(a => ConditionChecker.check(a.getConditions()));
        }

        public static IEnumerable<Action> Restrict(this IEnumerable<Action> actions, IEnumerable<int> restricted)
        {
            foreach(var a in actions)
            {
                if (restricted.Any(a.getType().Equals))
                {
                    yield return a;
                }
            }
        }

        public static IEnumerable<Action> Distinct(this IEnumerable<Action> actions)
        {
            // This probably too
            return from action in actions
                   group action by new
                   {
                       Type = action.getType(),
                       Name = (action is CustomAction) ? ((CustomAction)action).getName() : ""
                   } into sameTypeAction
                   select sameTypeAction.First();
        }

        public static IEnumerable<Action> Valid(this IEnumerable<Action> actions, IEnumerable<int> restricted = null)
        {
            var unrestricted = actions.Checked().Distinct();

            if (restricted != null)
                return unrestricted.Restrict(restricted);

            return unrestricted;
        }

        public static void AddExamineIfNotExists(Element element, List<Action> actions)
        {
            if (!actions.Any(a => a.getType() == Action.EXAMINE))
            {
                var description = element.getDescriptions()
                    .Find(d => ConditionChecker.check(d.getConditions()));

                if (description != null)
                {
                    actions.Add(
                        new Action(Action.EXAMINE)
                        {
                            Effects = new Effects
                            {
                                new SpeakPlayerEffect(description.getDetailedDescription())
                            }
                        }
                    );
                }
            }
        }
    }
}