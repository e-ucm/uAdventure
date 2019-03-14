using System;
using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * This class holds the data of a slidescene in eAdventure
     */
    [ChapterTarget]
    public class Slidescene : Cutscene, ICloneable
    {

        /**
        * The tag for the slides
        */
        //public const string RESOURCE_TYPE_SLIDES = "slides";

        /**
         * The tag for the background music
         */
        // public const string RESOURCE_TYPE_MUSIC = "bgmusic";


        protected Slidescene() : base(GeneralSceneSceneType.SLIDESCENE, "Slidescene" + UnityEngine.Random.Range(100000, 999999))
        {

        }
        /**
         * Creates a new Slidescene
         * 
         * @param id
         *            the id of the slidescene
         */
        public Slidescene(string id) : base(GeneralSceneSceneType.SLIDESCENE, id)
        {
        }

        public override object Clone()
        {
            Slidescene s = (Slidescene)base.Clone();
            return s;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            Slidescene s = (Slidescene) super.clone( );
            return s;
        }*/
    }
}