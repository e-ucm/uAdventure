using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uAdventure.Core;

namespace uAdventure.Runner
{
    public static class ElementsUtil
    {
        public static IEnumerable<ResourcesUni> Checked(this IEnumerable<ResourcesUni> elements)
        {
            return elements.Where(e => ConditionChecker.check(e.getConditions()));
        }

        public static IEnumerable<ElementReference> Checked(this IEnumerable<ElementReference> elements)
        {
            return elements.Where(e => ConditionChecker.check(e.Conditions));
        }

        public static bool IsRemoved(this HasTargetId element)
        {
            return Game.Instance.GameState.GetRemovedElements().Contains(element.getTargetId());
        }

        public static bool IsRemoved(this HasId element)
        {
            return Game.Instance.GameState.GetRemovedElements().Contains(element.getId());
        }

        public static IEnumerable<HasTargetId> NotRemoved(this IEnumerable<HasTargetId> elements)
        {
            return elements.Where(e => !e.IsRemoved());
        }
        public static IEnumerable<ElementReference> NotRemoved(this IEnumerable<ElementReference> elements)
        {
            return elements.Where(e => !e.IsRemoved());
        }
        public static IEnumerable<ActiveArea> NotRemoved(this IEnumerable<ActiveArea> activeAreas)
        {
            return activeAreas.Where(aa => !aa.IsRemoved());
        }
    }
}
