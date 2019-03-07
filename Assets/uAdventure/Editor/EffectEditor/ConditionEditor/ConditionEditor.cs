using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public interface ConditionEditor
    {
        void draw(Condition c);
        bool manages(Condition c);
        string conditionName();
        Condition InstanceManagedCondition();
        bool Collapsed { get; set; }
        bool Available { get; }
        Rect Window { get; set; }
    }
}