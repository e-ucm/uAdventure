namespace uAdventure.Core
{
    /**
     * The object has associated
     *
     */

    public interface HasSound
    {

        /**
         * Added for v1.4 - soundPath for accessibility purposes
         * @return
         */
        string getSoundPath();

        /**
         * Added for v1.4 - soundPath for accessibility purposes
         * @return
         */
        void setSoundPath(string soundPath);
    }
}