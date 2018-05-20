using System;
using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    public class CustomArrow : HasSound, ICloneable
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

        /**
         * @param type
         * @param path
         */
        public CustomArrow(string type, string path)
        {

            this.type = type;
            this.path = path;
        }

        public override bool Equals(System.Object o)
        {

            if (o == null || !(o is CustomArrow))
                return false;
            CustomArrow arrow = (CustomArrow)o;
            if (arrow.type.Equals(type))
            {
                return true;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            CustomArrow ca = (CustomArrow) super.clone( );
            ca.path = ( path != null ? new string(path ) : null );
            ca.type = ( type != null ? new string(type ) : null );
            return ca;
        }
        */
        public string getSoundPath()
        {
            return path;
        }

        public void setSoundPath(string soundPath)
        {
            this.path = soundPath;
        }

        public object Clone()
        {
            CustomArrow ca = (CustomArrow)this.MemberwiseClone();
            ca.path = (path != null ? path : null);
            ca.type = (type != null ? type : null);
            return ca;
        }
    }
}