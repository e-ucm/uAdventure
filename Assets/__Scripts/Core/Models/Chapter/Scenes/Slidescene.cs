using System;
using UnityEngine;
using System.Collections;

/**
 * This class holds the data of a slidescene in eAdventure
 */
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
