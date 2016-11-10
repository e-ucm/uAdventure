using UnityEngine;
using System.Collections;

public class ChangeBookPageUriTool : Tool {

    private BookPage bookPage;

    private string newURI;

    private string oldURI;

    public ChangeBookPageUriTool(BookPage bookPage, string newURI)
    {

        this.bookPage = bookPage;
        this.newURI = newURI;
        this.oldURI = bookPage.getUri();
    }

    
    public override bool canRedo()
    {

        return true;
    }

    
    public override  bool canUndo()
    {

        return true;
    }

    
    public override bool combine(Tool other)
    {

        if (other is ChangeBookPageUriTool ) {
            ChangeBookPageUriTool cbput = (ChangeBookPageUriTool)other;
            if (cbput.bookPage == bookPage)
            {
                newURI = cbput.newURI;
                timeStamp = cbput.timeStamp;
                return true;
            }
        }
        return false;
    }

    
    public override bool doTool()
    {

        if (oldURI != null || !newURI.Equals(oldURI))
        {
            bookPage.setUri(newURI);
            return true;
        }
        return false;
    }

    
    public override bool redoTool()
    {

        bookPage.setUri(newURI);
        Controller.getInstance().updatePanel();
        return true;
    }

    
    public override bool undoTool()
    {

        bookPage.setUri(oldURI);
        Controller.getInstance().updatePanel();
        return true;
    }
}
