namespace uAdventure.Geo
{
    public class ProgressController
    {
        private readonly float todo;
        private int done;

        public ProgressController(int todo)
        {
            this.todo = todo;
        }

        public int Current
        {
            get { return done; }
        }

        public bool Step()
        {
            return Step(1);
        }

        public bool Step(int count)
        {
            done += count;
            return done >= todo;
        }

        public float Progress
        {
            get { return done / todo; }
        }

        public void Restart()
        {
            done = 0;
        }
    }
}
