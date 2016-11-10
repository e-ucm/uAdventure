using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConversationLineController : Searchable
{
    private SearchableNode searchableNode;

    private int line;

    public ConversationLineController(SearchableNode searchableNode, int i)
    {
        this.searchableNode = searchableNode;
        this.line = i;
    }

    
    protected override List<Searchable> getPath(Searchable dataControl)
    {

        if (dataControl is ConversationLineController && ((ConversationLineController)dataControl).searchableNode.getConversationNodeView() == searchableNode.getConversationNodeView() && ((ConversationLineController)dataControl).line == line ) {
            List<Searchable> path = new List<Searchable>();
            path.Add(this);
            return path;
        }
        return null;
    }

    
    public override void recursiveSearch()
    {

        check(searchableNode.getConversationNodeView().getLineName(line), TC.get("Search.LineName"));
        check(searchableNode.getConversationNodeView().getLineText(line), TC.get("Search.LineText"));
    }

    public int getLine()
    {

        return line;
    }
}
