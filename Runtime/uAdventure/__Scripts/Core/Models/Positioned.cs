namespace uAdventure.Core
{
    /**
     * The object has a position in the x and y axis
     */
    public interface Positioned
    {

        /**
         * Get the position along the x axis
         * 
         * @return The position along the x axis
         */
        int getPositionX();

        /**
         * Get the position along the y axis
         * 
         * @return The position along the y axis
         */
        int getPositionY();

        /**
         * Set the position along the x axis
         * 
         * @param newX
         *            The new position along the x axis
         */
        void setPositionX(int newX);

        /**
         * Set the position along the y axis
         * 
         * @param newY
         *            The new position along the y axis
         */
        void setPositionY(int newY);
    }
}