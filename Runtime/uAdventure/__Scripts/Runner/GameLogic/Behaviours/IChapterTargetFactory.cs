using UnityEngine;
using uAdventure.Core;

namespace uAdventure.Runner
{
    public interface IChapterTargetFactory
    {
        IRunnerChapterTarget Instantiate(IChapterTarget modelObject);
    }
}
