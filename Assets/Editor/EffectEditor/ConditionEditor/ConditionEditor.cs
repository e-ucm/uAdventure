using UnityEngine;
using System.Collections;

public interface ConditionEditor {
    void draw(Condition c);
    bool manages(Condition c);
    string conditionName();
    Condition InstanceManagedCondition();
    bool Collapsed { get; set; }
    bool Avaiable { get; set; }
    Rect Window { get; set; }
}
