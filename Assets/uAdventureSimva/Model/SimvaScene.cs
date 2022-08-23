using uAdventure.Core;

namespace uAdventure.Simva
{
    public class SimvaScene : IChapterTarget
    {
        private string id;

        protected SimvaScene(string id)
        {
            this.id = id;
        }

        public bool allowsSavingGame()
        {
            return false;
        }

        public string getId()
        {
            return id;
        }

        public string getXApiClass()
        {
            return null;
        }

        public string getXApiType()
        {
            return null;
        }
    }
}
