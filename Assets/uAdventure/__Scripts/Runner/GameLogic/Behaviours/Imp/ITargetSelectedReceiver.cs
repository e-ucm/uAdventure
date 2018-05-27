using UnityEngine;
using UnityEngine.EventSystems;

namespace uAdventure.Runner
{
    public interface ITargetSelectedHandler : IEventSystemHandler
    {
        void OnTargetSelected(PointerEventData data);
    }
}