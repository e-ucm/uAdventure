using uAdventure.Core;

namespace uAdventure.Runner
{
    [CustomEffectRunner(typeof(RemoveObjectFromInventoryEffect))]
    public class RemoveObjectFromInventoryEffectRunner : CustomEffectRunner
    {
        RemoveObjectFromInventoryEffect effect;

        public IEffect Effect { get { return effect; } set { effect = value as RemoveObjectFromInventoryEffect; } }

        public bool execute()
        {
            var element = Game.Instance.GameState.FindElement<Item>(effect.getTargetId());
            InventoryManager.Instance.RemoveElement(element);
            return false;
        }
    }
}