namespace uAdventure.Core
{
    /**
     * The object has a name
     */
    public interface Named
    {

        /**
         * Set the name of the object
         * 
         * @param name
         *            The new name of the object
         */
        void setName(string name);

        /**
         * Get the name of the object
         * 
         * @return The name of the object
         */
        string getName();
    }
}