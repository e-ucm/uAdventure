namespace uAdventure.Core
{
    /**
     * The object has an ID
     */
    public interface HasId
    {

        /**
         * Get the id of the object
         * 
         * @return The objects id
         */
        string getId();

        /**
         * Set the id of the object
         * 
         * @param id
         *            The new id
         */
        void setId(string id);
    }
}