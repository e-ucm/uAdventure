using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public interface IEffectEditor
    {
        void draw();
        IEffect Effect { get; set; }
        string EffectName { get; }
        IEffectEditor clone();
        bool manages(IEffect c);
        bool Collapsed { get; set; }
        Rect Window { get; set; }
        bool Usable { get; }
    }
}