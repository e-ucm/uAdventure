using uAdventure.Core;

namespace uAdventure.Runner
{
    [CustomEffectRunner(typeof(AddObjectToInventoryEffect))]
    public class AddObjectToInventoryEffectRunner : CustomEffectRunner
    {
        AddObjectToInventoryEffect effect;

        public IEffect Effect { get { return effect; } set { effect = value as AddObjectToInventoryEffect; } }

        public bool execute()
        {
            var element = Game.Instance.GameState.FindElement<Item>(effect.getTargetId());
            InventoryManager.Instance.AddElement(element);
            return false;
        }
    }
}