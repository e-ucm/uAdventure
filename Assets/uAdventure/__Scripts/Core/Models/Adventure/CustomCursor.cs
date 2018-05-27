using System;
using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    public class CustomCursor : ICloneable
    {

        private string type;

        private string path;

        /**
         * @return the type
         */
        public string getType()
        {

            return type;
        }

        /**
         * @param type
         *            the type to set
         */
        public void setType(string type)
        {

            this.type = type;
        }

        /**
         * @return the path
         */
        public string getPath()
        {

            return path;
        }

        /**
         * @param path
         *            the path to set
         */
        public void setPath(string path)
        {

            this.path = path;
        }

        public object Clone()
        {
            CustomCursor cc = (CustomCursor)this.MemberwiseClone();
            cc.path = (path != null ? path : null);
            cc.type = (type != null ? type : null);
            return cc;
        }

        /**
         * @param type
         * @param path
         */
        public CustomCursor(string type, string path)
        {

            this.type = type;
            this.path = path;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            CustomCursor cc = (CustomCursor) super.clone( );
            cc.path = ( path != null ? new string(path ) : null );
            cc.type = ( type != null ? new string(type ) : null );
            return cc;
        }
        */
    }
}