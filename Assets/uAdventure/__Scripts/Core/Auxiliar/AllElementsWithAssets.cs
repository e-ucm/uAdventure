using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    /**
     * 
     * This class is used to store all the element with asset present in a chapter when it is imported.
     *
     */
    public class AllElementsWithAssets
    {


        /**
         * Check if store or not in allAssets list
         */
        private static bool storePath = false;

        /**
         * Store all the elements with assets an their references
         */
        private static List<System.Object> allAssets = new List<System.Object>();

        /**
         * change the storePath attribute
         * @param store
         */
        public static void setStorePath(bool store)
        {
            storePath = store;
        }

        /**
         * Store the element with asset if storePath att allows for it
         * 
         * @param value
         * @param key
         */
        public static void addAsset(System.Object element)
        {
            if (storePath)
            {
                bool found = false;
                foreach (System.Object o in allAssets)
                {
                    if (o == element)
                    {
                        found = true; break;
                    }
                }
                if (!found)
                {
                    allAssets.Add(element);
                }
            }
        }

        /**
         * Clear the allAssets structure
         */
        public static void resetAllAssets()
        {
            allAssets.Clear();
        }

        /**
         * Return the allAssets
         */
        public static List<System.Object> getAllAssets()
        {
            return allAssets;
        }
    }
}