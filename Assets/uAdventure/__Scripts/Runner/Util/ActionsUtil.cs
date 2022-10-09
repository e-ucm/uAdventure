using System.Collections.Generic;
using System.Linq;
using uAdventure.Core;

namespace uAdventure.Runner
{

    public interface ICheckable
    {
        Conditions Conditions { get; }
    }

    public static class ActionsUtil
    {
        public static IEnumerable<T> Checked<T>(this IEnumerable<T> checkables) where T : ICheckable
        {
            return checkables.Where(c => ConditionChecker.check(c.Conditions));
        }

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
        public static IEnumerable<Action> DistinctTarget(this IEnumerable<Action> actions)
        {
            // This probably too
            return from action in actions
                   group action by new
                   {
                       Type = action.getType(),
                       Name = (action is CustomAction) ? ((CustomAction)action).getName() : "",
                       TargetId = action.getTargetId()
                   } into sameTypeAction
                   select sameTypeAction.First();
        }

        public static IEnumerable<Action> Valid(this IEnumerable<Action> actions, IEnumerable<int> restricted = null)
        {
            var unrestricted = actions.Checked();

            if (restricted != null)
                return unrestricted.Restrict(restricted);

            return unrestricted;
        }

        private static Dictionary<Description, Action> examineActions = new Dictionary<Description, Action>();

        public static void AddExamineIfNotExists(Element element, List<Action> actions)
        {
            if (actions.Any(a => a.getType() == Action.EXAMINE))
            {
                return;
            }

            var description = element.getDescriptions()
                .Find(d => ConditionChecker.check(d.getConditions()));

            if (description == null)
            {
                return;
            }

            if (!examineActions.ContainsKey(description))
            {
                var textToShow = description.getDetailedDescription();
                if (string.IsNullOrEmpty(textToShow))
                    textToShow = description.getDescription();
                if (string.IsNullOrEmpty(textToShow))
                    textToShow = description.getName();

                // Only add the examine if there's text to show
                if (!string.IsNullOrEmpty(textToShow))
                {
                    var action = new Action(Action.EXAMINE)
                    {
                        Effects = new Effects
                        {
                            new SpeakPlayerEffect(textToShow)
                        }
                    };
                    examineActions.Add(description, action);
                }
            }

            if (examineActions.ContainsKey(description))
            {
                actions.Add(examineActions[description]);
            }
        }
    }
}