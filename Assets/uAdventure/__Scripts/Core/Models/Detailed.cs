namespace uAdventure.Core
{
    /**
     * The object has a detailed description
     */
    public interface Detailed
    {

        /**
         * Set the detailed description of the object
         * 
         * @param detailedDescription
         *            The new detailed description
         */
        void setDetailedDescription(string detailedDescription);

        /**
         * Get the detailed description of the object
         * 
         * @return The detailed description of the object
         */
        string getDetailedDescription();

    }
}