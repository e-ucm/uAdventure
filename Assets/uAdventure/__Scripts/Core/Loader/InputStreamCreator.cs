using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * Constructs the InputStream of a file located in a parent structure, which is
     * abstracted by this entity Entities aiming to use Loader must implement this
     * interface
     * 
     */
    public interface InputStreamCreator
    {
        //TODO
        // changed InputStream to string 
        // commented URL section

        /**
         * Builds the inputStream Object of a filePath which is stored in "parent",
         * where the implementation of parent is undefined
         * 
         * @param filePath
         *            Path of the file
         */
        string buildInputStream(string filePath);

        /**
         * If filePath is a directory in parentPath, the list of its children is
         * given
         * 
         * @param filePath
         * @return
         */
        string[] listNames(string filePath);

        // URL buildURL(string path);
    }
}