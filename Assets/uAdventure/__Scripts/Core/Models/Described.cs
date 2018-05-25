using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * The object is described
     */
    public interface Described
    {

        /**
         * Get the description of the object
         * 
         * @return The description of the object
         */
        string getDescription();

        /**
         * Set the description of the object
         * 
         * @param description
         *            The new description
         */
        void setDescription(string description);

    }
}