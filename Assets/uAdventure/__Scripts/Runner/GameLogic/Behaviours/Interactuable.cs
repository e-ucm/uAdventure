using UnityEngine;
using System.Collections;

using uAdventure.Core;
using UnityEngine.EventSystems;

namespace uAdventure.Runner
{
    public enum InteractuableResult
    {
        IGNORES, DOES_SOMETHING, REQUIRES_MORE_INTERACTION
    }

    public interface Interactuable
    {
        InteractuableResult Interacted(PointerEventData pointerData = null);
        bool canBeInteracted();
        void setInteractuable(bool state);
    }
}