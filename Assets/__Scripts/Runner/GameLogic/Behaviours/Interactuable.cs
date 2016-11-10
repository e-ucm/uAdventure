using UnityEngine;
using System.Collections;

public enum InteractuableResult{
    IGNORES, DOES_SOMETHING, REQUIRES_MORE_INTERACTION
}

public interface Interactuable{
    InteractuableResult Interacted (RaycastHit hit = new RaycastHit());
	bool canBeInteracted();
	void setInteractuable(bool state);
}

