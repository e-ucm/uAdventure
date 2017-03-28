using uAdventure.Core;

namespace uAdventure.Runner
{
    public interface CustomEffectRunner : Secuence
    {
        Effect Effect { get; set; }
    }
}
