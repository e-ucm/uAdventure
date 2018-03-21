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
            // Add the element
            Game.Instance.GameState.addRemovedElement(effect.getTargetId());
            // Refresh the scene
            Game.Instance.RunTarget(Game.Instance.GameState.CurrentTarget);
            return false;
        }
    }
}