using System;
using uAdventure.Core;

namespace uAdventure.QR
{
    public class QR : HasId, Documented
    {
        public QR(string id)
        {
            Id = id;
            Conditions = new Conditions();
            Effects = new Effects();
            Documentation = "";
            Content = "";
        }

        // ID of the QR code in order to manage them
        public string Id { get; set; }
        // Documentation for the editor
        public string Documentation { get; set; }
        // Main information of the QR code
        public string Content { get; set; }

        public Conditions Conditions { get; set; }
        public Effects Effects { get; set; }

        public string getDocumentation()
        {
            return Documentation;
        }

        public string getId()
        {
            return Id;
        }

        public void setDocumentation(string documentation)
        {
            Documentation = documentation;
        }

        public void setId(string id)
        {
            Id = id;
        }
    }
}