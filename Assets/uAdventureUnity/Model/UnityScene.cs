
using uAdventure.Core;

namespace uAdventure.Unity
{
    public class UnityScene : HasId, IChapterTarget
    {
        public string Id { get; set; }
        public string Scene { get; set; }

        public bool allowsSavingGame()
        {
            return true;
        }

        public string getId()
        {
            return Id;
        }

        public string getXApiClass()
        {
            return "";
        }

        public string getXApiType()
        {
            return "";
        }

        public void setId(string id)
        {
            Id = id;
        }
    }
}
