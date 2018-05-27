using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    /**
     * This class holds the information for an animation frame
     */
    public class Frame : Timed, Documented, ICloneable
    {

        /**
         * The xml tag for the sound of the frame
         */
        public const string RESOURCE_TYPE_SOUND = "sound";

        /**
         * The frame is a image
         */
        public const int TYPE_IMAGE = 0;

        /**
         * The frame is a video
         */
        public const int TYPE_VIDEO = 1;

        /**
         * The default time of a frame
         */
        public const int DEFAULT_TIME = 40;

        /**
         * The url/resource path
         */
        private string uri;

        /**
         * Time to display the frame
         */
        private long time;

        /**
         * Type of the frame: {@link #TYPE_IMAGE} or {@link #TYPE_VIDEO}
         */
        private int type;

        /**
         * The image of the frame, buffered when possible
         */
        private Sprite image;

        /**
         * Set of resources for the frame
         */
        private List<ResourcesUni> resources;

        private bool waitforclick;

        private string soundUri;

        private int maxSoundTime;

        private string animationPath;

        /**
         * Creates a new empty frame
         */
        public Frame() :
            this("", DEFAULT_TIME, false)
        {
        }

        /**
         * Creates a new frame with a image uri
         * 
         * @param uri
         *            the uri for the image
         */
        public Frame(string uri) : this(uri, DEFAULT_TIME, false)
        {
        }

        /**
         * Creates a new frame with the given duration
         * 
         * @param time
         *            integer with the duration of the frame
         */
        public Frame(int time) :
            this("", time, false)
        {
        }


        /**
         * Creates a new frame with a image uri, a duration time and the selection for user click waiting
         * 
         * @param uri
         *            The uri for the image
         * @param time
         *            The time (duration) of the frame
         */
        public Frame(string uri, long time, bool waitForClick)
        {
            this.uri = uri;
            type = TYPE_IMAGE;
            this.time = time;
            image = null;
            this.waitforclick = waitForClick;
            resources = new List<ResourcesUni>();
            soundUri = "";
            maxSoundTime = 1000;
        }

        /**
         * Returns the uri/path of the frame resource
         * 
         * @return the uri/path of the frame resource
         */
        public string getUri()
        {

            return uri;
        }

        /**
         * Set the uri/path of the frame resource
         * 
         * @param uri
         *            the uri/path of the frame resource
         */
        public void setUri(string uri)
        {

            this.uri = uri;
            image = null;
        }

        /**
         * Returns the time (duration) of the frame in milliseconds
         * 
         * @return the time (duration) of the frame in milliseconds
         */
        public long getTime()
        {

            return time;
        }

        /**
         * Set the time (duration) of the frame in milliseconds
         * 
         * @param time
         *            the time (duration) of the frame in milliseconds
         */
        public void setTime(long time)
        {

            this.time = time;
        }

        /**
         * Returns the type of the frame
         * 
         * @return the type of the frame
         */
        public int getType()
        {

            return type;
        }

        /**
         * Sets the type of the frame
         * 
         * @param type
         *            the type of the frame
         */
        public void setType(int type)
        {

            this.type = type;
        }

        public string getSoundUri()
        {

            return soundUri;
        }

        public void setSoundUri(string soundUri)
        {

            this.soundUri = soundUri;
        }

        public int getMaxSoundTime()
        {

            return maxSoundTime;
        }

        public void setMaxSoundTime(int maxSoundTime)
        {

            this.maxSoundTime = maxSoundTime;
        }

        /**
         * Adds some resources to the list of resources
         * 
         * @param resources
         *            the resources to add
         */
        public void addResources(ResourcesUni resources)
        {

            this.resources.Add(resources);
        }

        /**
         * Returns the list of resources of the frame
         * 
         * @return The list of resources of the frame
         */
        public List<ResourcesUni> getResources()
        {

            return resources;
        }

        /**
         * Returns the value of waitforclick
         * 
         * @return the value of waitforclick
         */
        public bool isWaitforclick()
        {

            return waitforclick;
        }

        /**
         * Set the value of waitforclick
         * 
         * @param waitforclick
         */
        public void setWaitforclick(bool waitforclick)
        {

            this.waitforclick = waitforclick;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            Frame f = (Frame) super.clone( );
            f.image = image;
            if( resources != null ) {
                f.resources = new List<Resources>();
                for (Resources r : resources)
                    f.resources.add((Resources)r.clone());
            }
            f.time = time;
            f.type = type;
            f.uri = ( uri != null ? new string(uri ) : null );
            f.waitforclick = waitforclick;
            return f;
        }*/

        public void setAbsolutePath(string animationPath)
        {
            this.animationPath = animationPath;
        }

        public string getAbsolutePath()
        {
            return animationPath;
        }

        public string getImageAbsolutePath()
        {
            // TODO
            /*
            string regexp = java.io.File.separator;
            if (regexp.Equals("\\"))
                regexp = "\\\\";
            string temp[] = this.animationPath.split(regexp);
            string filename = "";
            for (int i = 0; i < temp.length - 1; i++)
            {
                filename += temp[i] + java.io.File.separator;
            }

            temp = this.uri.split("/");
            filename += temp[temp.length - 1];
            */
            return string.Empty;
        }

        public string getSoundAbsolutePath()
        {
            // TODO
            /*
            if (soundUri == null || soundUri.Equals(""))
                return null;

            string regexp = java.io.File.separator;
            if (regexp.Equals("\\"))
                regexp = "\\\\";
            string temp[] = this.animationPath.split(regexp);
            string filename = "";
            for (int i = 0; i < temp.length - 1; i++)
            {
                filename += temp[i] + java.io.File.separator;
            }

            temp = this.soundUri.split("/");
            filename += temp[temp.length - 1];

            return filename;        */
            return string.Empty;
        }

        /***************************************************************/
        // Added for accessibility purposes
        /****************************************************************/

        private string documentation = "";
        public string getDocumentation()
        {
            return documentation;
        }

        public void setDocumentation(string documentation)
        {
            this.documentation = documentation;
        }

        public object Clone()
        {

            Frame f = (Frame)this.MemberwiseClone();
            f.image = image;
            if (resources != null)
            {
                f.resources = new List<ResourcesUni>();
                foreach (ResourcesUni r in resources)
                    f.resources.Add((ResourcesUni)r.Clone());
            }
            f.time = time;
            f.type = type;
            f.uri = (uri != null ? uri : null);
            f.waitforclick = waitforclick;
            return f;
        }
    }
}