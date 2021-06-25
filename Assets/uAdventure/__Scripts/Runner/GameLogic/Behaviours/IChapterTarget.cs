using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    public interface IChapterTarget
    {
        string getId();
        string getXApiClass();
        string getXApiType();
        bool allowsSavingGame();
    }
}

