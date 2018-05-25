using System;
using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    public class CustomButton : HasSound, ICloneable
    {

        private string type;

        private string path;

        private string action;

        /**
         * @return the action
         */

        public string getAction()
        {

            return action;
        }

        /**
         * @param action
         *            the action to set
         */

        public void setAction(string action)
        {

            this.action = action;
        }

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

        public CustomButton(string action, string type, string path)
        {

            this.action = action;
            this.type = type;
            this.path = path;
        }


        public override bool Equals(System.Object o)
        {

            if (o == null || !(o is CustomButton))
                return false;
            CustomButton button = (CustomButton)o;
            if (button.action.Equals(action) && button.type.Equals(type))
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

            CustomButton cb = (CustomButton) super.clone( );
            cb.action = ( action != null ? new string(action ) : null );
            cb.path = ( path != null ? new string(path ) : null );
            cb.type = ( type != null ? new string(type ) : null );
            return cb;
        }*/

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
            CustomButton cb = (CustomButton)this.MemberwiseClone();
            cb.action = (action != null ? action : null);
            cb.path = (path != null ? path : null);
            cb.type = (type != null ? type : null);
            return cb;
        }
    }
}