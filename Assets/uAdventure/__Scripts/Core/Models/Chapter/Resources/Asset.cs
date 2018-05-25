using System;
using UnityEngine;

namespace uAdventure.Core
{
    /**
     * Defines an asset of any type (background, slides, image, icon, [...], or the
     * animatinos of the characters.
     */

    public class Asset : ICloneable
    {

        /**
         * String with the type of the asset
         */
        private string type;

        /**
         * Path of the asset
         */
        private string path;

        /**
         * Creates a new asset
         * 
         * @param type
         *            the type of the asset
         * @param path
         *            the path of the asset
         */

        public Asset(string type, string path)
        {

            this.type = type;
            this.path = path;
        }

        /**
         * Returns the type of the asset.
         * 
         * @return the type of the asset
         */

        public string getType()
        {

            return type;
        }

        /**
         * Returns the path of the asset.
         * 
         * @return the path of the asset
         */

        public string getPath()
        {

            return path;
        }

        public object Clone()
        {
            Asset a = (Asset)this.MemberwiseClone();
            a.path = (path != null ? path : null);
            a.type = (type != null ? type : null);
            return a;
        }

        /*
    @Override
    public Object clone() throws CloneNotSupportedException
    {

       Asset a = (Asset) super.clone( );
       a.path = ( path != null ? new String(path ) : null );
       a.type = ( type != null ? new String(type ) : null );
       return a;
    }*/
    }
}