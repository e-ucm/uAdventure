using System;
using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * This class holds the data of an item in eAdventure
     */
    public class Atrezzo : Element, Named, HasDescriptionSound, ICloneable
    {

        /**
         * The tag of the item's image
         */
        public const string RESOURCE_TYPE_IMAGE = "image";

        /**
         * Creates a new Atrezzo item
         * 
         * @param id
         *            the id of the atrezzo item
         */
        public Atrezzo(string id) : base(id)
        {
            descriptions.Add(new Description());

        }

        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            Atrezzo a = (Atrezzo) super.clone( );
            return a;
        }
        */

        public override object Clone()
        {
            Atrezzo a = (Atrezzo)base.Clone();
            return a;
        }

        public string getName()
        {

            return this.descriptions[0].getName();

        }


        public void setName(string name)
        {

            this.descriptions[0].setName(name);

        }


        public string getDescriptionSoundPath()
        {

            // TODO Auto-generated method stub
            return null;
        }


        public string getDetailedDescriptionSoundPath()
        {

            // TODO Auto-generated method stub
            return null;
        }


        public string getNameSoundPath()
        {

            return this.descriptions[0].getNameSoundPath();
        }


        public void setDescriptionSoundPath(string descriptionSoundPath)
        {



        }


        public void setDetailedDescriptionSoundPath(string detailedDescriptionSoundPath)
        {

            // TODO Auto-generated method stub

        }


        public void setNameSoundPath(string nameSoundPath)
        {

            this.descriptions[0].setNameSoundPath(nameSoundPath);

        }


    }
}