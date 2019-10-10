using uAdventure.Core;

namespace uAdventure.Runner
{
    [CustomEffectRunner(typeof(RemoveElementEffect))]
    public class RemoveElementEffectRunner : CustomEffectRunner
    {
        RemoveElementEffect effect;
        private bool waitingRunTarget = false;

        public IEffect Effect { get { return effect; } set { effect = value as RemoveElementEffect; } }

        public bool execute()
        {
            if (!waitingRunTarget)
            {
                // Add the element
                Game.Instance.GameState.AddRemovedElement(effect.getTargetId());
                // Refresh the scene
                Game.Instance.RunTarget(Game.Instance.GameState.CurrentTarget);
                waitingRunTarget = true;
            }
            else
            {
                waitingRunTarget = false;
            }
            return waitingRunTarget;
        }
    }
}