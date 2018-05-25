namespace uAdventure.Core
{
    public interface HasTargetId
    {
        /**
         * Get the target id of the object
         * 
         * @return The objects target id
         */
        string getTargetId();

        /**
         * Set the target id of the object
         * 
         * @param id
         *            The objects new target id
         */
        void setTargetId(string id);
    }
}