using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public interface ConversationNodeEditor
    {
        void setParent(ConversationEditorWindow parent);

        void draw();
        ConversationNode Node { get; set; }
        string NodeName { get; }
        ConversationNodeEditor clone();
        bool manages(ConversationNode c);

        bool Collapsed { get; set; }

        Rect Window { get; set; }
    }
}
