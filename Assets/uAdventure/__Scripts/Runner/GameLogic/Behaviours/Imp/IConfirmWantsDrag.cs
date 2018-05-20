using UnityEngine.EventSystems;

namespace uAdventure.Runner
{
    /// <summary>
    /// When implemented allows the target to confirm if it wants the mouse drag by using the event.
    /// </summary>
    public interface IConfirmWantsDrag : IEventSystemHandler
    {
        void OnConfirmWantsDrag(PointerEventData data);
    }
}