using System;
using Random = UnityEngine.Random;

namespace uAdventure.Core
{
    /**
     * This class holds the data of a videoscene in eAdventure
     */
     [ChapterTarget]
    public class Videoscene : Cutscene, ICloneable
    {
        /**
             * The tag for the video
             */
        //public const string RESOURCE_TYPE_VIDEO = "video";

        /**
         * Set if the video can be skipped 
         */
        public bool canSkip = false;

        protected Videoscene() : base(GeneralSceneSceneType.VIDEOSCENE, "VideoScene"+Random.Range(100000, 999999))
        {

        }

        /**
         * Creates a new Videoscene
         * 
         * @param id
         *            the id of the videoscene
         */
        public Videoscene(string id) : base(GeneralSceneSceneType.VIDEOSCENE, id)
        {
        }

        public bool isCanSkip()
        {

            return canSkip;
        }


        public void setCanSkip(bool canSkip)
        {

            this.canSkip = canSkip;
        }

        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            Videoscene v = (Videoscene) super.clone( );
            v.canSkip = canSkip;
            return v;
        }*/

        public override object Clone()
        {
            Videoscene v = (Videoscene)base.Clone();
            v.canSkip = canSkip;
            return v;
        }
    }
}