using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public interface EffectEditor
    {
        void draw();
        AbstractEffect Effect { get; set; }
        string EffectName { get; }
        EffectEditor clone();
        bool manages(AbstractEffect c);

        bool Collapsed { get; set; }

        Rect Window { get; set; }
    }
}