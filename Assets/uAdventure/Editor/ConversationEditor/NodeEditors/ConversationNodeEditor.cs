using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public interface ConversationNodeEditor
    {
        void setParent(ConversationEditor parent);

        void draw();
        ConversationNode Node { get; set; }
        string NodeName { get; }
        ConversationNodeEditor clone();
        bool manages(ConversationNode c);

        Rect Window { get; set; }
    }
}
