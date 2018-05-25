using uAdventure.Core;

namespace uAdventure.Runner
{
    public interface CustomEffectRunner : Secuence
    {
        IEffect Effect { get; set; }
    }
}
