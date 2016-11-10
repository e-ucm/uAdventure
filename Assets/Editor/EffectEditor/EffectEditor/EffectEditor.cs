using UnityEngine;
using System.Collections;

public interface EffectEditor {
	void draw();
    AbstractEffect Effect { get; set; }
	string EffectName{ get; }
	EffectEditor clone();
	bool manages(AbstractEffect c);

    bool Collapsed { get; set; }

    Rect Window { get; set; }
}
