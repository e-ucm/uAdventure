using uAdventure.Core;

namespace uAdventure.Runner
{
    [CustomEffectRunner(typeof(RemoveElementEffect))]
    public class RemoveElementEffectRunner : CustomEffectRunner
    {
        RemoveElementEffect effect;

        public IEffect Effect { get { return effect; } set { effect = value as RemoveElementEffect; } }

        public bool execute()
        {
            Game.Instance.GameState.AddRemovedElement(effect.getTargetId());
            return false;
        }
    }
}