using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    /**
     * The list of resources of an element in the game under certain conditions.
     */

    public class ResourcesUni : ICloneable
    {

        /**
            * Conditions of the resource
            */
        private Conditions conditions;

        /**
         * List of single assets stored in the resources structure
         */
        private Dictionary<string, string> assets;

        private string name;

        /**
         * Creates a new Resources.
         */

        public ResourcesUni()
        {
            assets = new Dictionary<string, string>();
            // add the hash map of asset AllElements with asset 
            AllElementsWithAssets.addAsset(assets);
            conditions = new Conditions();
            name = "No name";
        }

        /**
         * Inserts a new asset in the resources.
         * 
         * @param type
         *            Identifier of the asset
         * @param path
         *            Path of the asset
         */

        public bool addAsset(string type, string path)
        {
            bool alreadyExists = existAsset(type) && getAssetPath(type).Equals(path);
            // Remove the asset (if it was present), and insert the new one
            deleteAsset(type);
            assets.Add(type, path);
            return !alreadyExists;
        }

        /**
         * Adds an asset to the resources.
         * 
         * @param asset
         *            the asset to be added
         */

        public bool addAsset(Asset asset)
        {
            return addAsset(asset.getType(), asset.getPath());
        }

        /**
         * Deletes the given asset from the resources block.
         * 
         * @param type
         *            Identifier of the asset
         */

        public void deleteAsset(string type)
        {
            assets.Remove(type);
        }

        /**
         * Returns the number of assets contained.
         * 
         * @return Number of assets
         */

        public int getAssetCount()
        {

            return assets.Count;
        }

        /**
         * Returns an array with all the types of the resources.
         * 
         * @return Array with the resources types
         */

        public string[] getAssetTypes()
        {

            string[] types = new string[assets.Count];
            assets.Keys.CopyTo(types, 0);
            return types;
        }

        /**
         * Returns an array with all the values of the resources.
         * 
         * @return Array with the resources values
         */

        public string[] getAssetValues()
        {
            string[] values = new string[assets.Count];
            assets.Values.CopyTo(values, 0);
            return values;
        }

        /**
         * Returns the path associated with the given type.
         * 
         * @param type
         *            Identifier of the asset
         * @return Path to the asset if present, null otherwise
         */

        public string getAssetPath(string type)
        {

            if (assets.ContainsKey(type))
                return assets[type];
            else
                return null;
        }

        /**
         * Returns the conditions to the resources.
         * 
         * @return the conditions to the resources
         */

        public Conditions getConditions()
        {

            return conditions;
        }

        /**
         * Sets the conditions to the resources.
         * 
         * @param conditions
         *            the new conditions.
         */

        public void setConditions(Conditions conditions)
        {

            this.conditions = conditions;
        }

        /**
         * Returns whether exists an asset of the given type.
         * 
         * @param type
         *            the type of the asset
         * @return whether exists an asset of the given type
         */

        public bool existAsset(string type)
        {

            bool existAsset = false;

            existAsset = assets.ContainsKey(type) && assets[type] != null;

            return existAsset;
        }

        public void setName(string name)
        {
            this.name = name;
        }

        public string getName()
        {
            return name;
        }

        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            Resources r = (Resources) super.clone( );
            if( assets != null ) {
                r.assets = new HashMap<string, string>();
                for (string s : assets.keySet())
                {
                    string s2 = (assets.get(s) != null ? new string(assets.get(s)) : null);
                    r.assets.put(new string(s), s2);
                }
            }
            r.name = ( name != null ? new string(name) : null);
            r.conditions = ( conditions != null ? (Conditions) conditions.clone( ) : null );
            return r;
        }*/

        /**
         * Deletes all the resources in the structure. Needed for undo tool
         * 
         * @return
         */

        public void clearAssets()
        {
            assets.Clear();
        }

        public object Clone()
        {
            ResourcesUni r = (ResourcesUni)this.MemberwiseClone();
            if (assets != null)
            {
                r.assets = new Dictionary<string, string>();
                foreach (string s in assets.Keys)
                {
                    string s2 = (assets[s] != null ? assets[s] : null);
                    r.assets.Add(s, s2);
                }
            }
            r.name = (name != null ? name : null);
            r.conditions = (conditions != null ? (Conditions)conditions.Clone() : null);
            return r;
        }
    }
}