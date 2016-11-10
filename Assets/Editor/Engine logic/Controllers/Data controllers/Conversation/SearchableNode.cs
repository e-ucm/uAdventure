using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SearchableNode : Searchable
{
    private ConversationNodeView cnv;

    public SearchableNode(ConversationNodeView cnv)
    {

        this.cnv = cnv;
    }

    
    protected override List<Searchable> getPath(Searchable dataControl)
    {

        List<Searchable> path = getPathFromChild(dataControl, getConversationLines().Cast<Searchable>().ToList());
        if (path != null)
            return path;
        if (dataControl is SearchableNode && ((SearchableNode)dataControl).getConversationNodeView() == cnv ) {
            path = new List<Searchable>();
            path.Add(this);
            return path;
        }
        return null;
    }

    private List<ConversationLineController> getConversationLines()
    {

        List<ConversationLineController> lines = new List<ConversationLineController>();
        for (int i = 0; i < cnv.getLineCount(); i++)
        {
            ConversationLineController line = new ConversationLineController(this, i);
            lines.Add(line);
        }
        return lines;
    }

    
    public override void recursiveSearch()
    {

        foreach (ConversationLineController line in getConversationLines())
            line.recursiveSearch();
    }

    public ConversationNodeView getConversationNodeView()
    {

        return cnv;
    }
}
