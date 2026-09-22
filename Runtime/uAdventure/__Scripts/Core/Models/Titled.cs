namespace uAdventure.Core
{
    /**
     * The object has a title
     */
    public interface Titled
    {

        /**
         * Set the objects title
         * 
         * @param title
         *            The new title
         */
        void setTitle(string title);

        /**
         * Get the objects title
         * 
         * @return The objects title
         */
        string getTitle();
    }
}