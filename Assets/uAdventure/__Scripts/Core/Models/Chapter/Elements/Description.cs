using System;
using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * 
     * Class to store single description for an element
     * 
     */
    public class Description : Named, Described, Detailed, HasDescriptionSound, ICloneable
    {
        /**
             * Condition
             */
        private Conditions conditions;

        /**
         * The element's name
         */
        private string name;

        /**
         * The path of the sound associated to element's name
         */
        private string nameSoundPath;

        /**
         * The element's brief description
         */
        private string description;

        /**
         * The path of the sound associated to element's description
         */
        private string descriptionSoundPath;

        /**
         * The element's detailed description
         */
        private string detailedDescription;

        /**
         * The path of the sound associated to element's detailed description
         */
        private string detailedDescriptionSoundPath;


        public Description()
        {
            this.name = "";
            this.description = "";
            this.detailedDescription = "";
        }



        /**
         * Returns the element's name
         * 
         * @return the element's name
         */
        public string getName()
        {

            return name;
        }

        /**
         * Returns the element's brief description
         * 
         * @return the element's brief description
         */
        public string getDescription()
        {

            return description;
        }

        /**
         * Returns the element's detailed description
         * 
         * @return the element's detailed description
         */
        public string getDetailedDescription()
        {

            return detailedDescription;
        }

        /**
         * Changes the element's name
         * 
         * @param name
         *            the new name
         */
        public void setName(string name)
        {

            this.name = name;
        }

        /**
         * Changes the element's brief description
         * 
         * @param description
         *            the new brief description
         */
        public void setDescription(string description)
        {

            this.description = description;
        }

        /**
         * Changes the element's detailed description
         * 
         * @param detailedDescription
         *            the new detailed description
         */
        public void setDetailedDescription(string detailedDescription)
        {

            this.detailedDescription = detailedDescription;
        }

        public string getNameSoundPath()
        {

            return nameSoundPath;
        }


        public void setNameSoundPath(string nameSoundPath)
        {

            this.nameSoundPath = nameSoundPath;
            if (nameSoundPath != null)
            {
                AllElementsWithAssets.addAsset(this);
            }
        }


        public string getDescriptionSoundPath()
        {

            return descriptionSoundPath;
        }


        public void setDescriptionSoundPath(string descriptionSoundPath)
        {

            this.descriptionSoundPath = descriptionSoundPath;
            if (descriptionSoundPath != null)
            {
                AllElementsWithAssets.addAsset(this);
            }

        }


        public string getDetailedDescriptionSoundPath()
        {

            return detailedDescriptionSoundPath;
        }


        public void setDetailedDescriptionSoundPath(string detailedDescriptionSoundPath)
        {

            this.detailedDescriptionSoundPath = detailedDescriptionSoundPath;
            if (detailedDescriptionSoundPath != null)
            {
                AllElementsWithAssets.addAsset(this);
            }

        }


        public Conditions getConditions()
        {

            return conditions;
        }

        public void setConditions(Conditions conditions)
        {

            this.conditions = conditions;
        }

        public object Clone()
        {
            Description d = (Description)this.MemberwiseClone();

            d.conditions = (conditions != null ? (Conditions)conditions.Clone() : null);
            d.name = (name != null ? name : null);
            d.nameSoundPath = (nameSoundPath != null ? nameSoundPath : null);
            d.description = (description != null ? description : null);
            d.descriptionSoundPath = (descriptionSoundPath != null ? descriptionSoundPath : null);
            d.detailedDescription = (detailedDescription != null ? detailedDescription : null);
            d.detailedDescriptionSoundPath = (detailedDescriptionSoundPath != null ? detailedDescriptionSoundPath : null);

            return d;
        }
        /*
    @Override
    public Object clone() throws CloneNotSupportedException
    {

       Description d = (Description) super.clone( );

       d.conditions = ( conditions != null ? (Conditions) conditions.clone( ) : null );
       d.name = ( name != null ? new string(name ) : null );
       d.nameSoundPath = ( nameSoundPath != null ? new string(nameSoundPath ) : null );
       d.description = ( description != null ? new string(description ) : null );
       d.descriptionSoundPath = ( descriptionSoundPath != null ? new string(descriptionSoundPath ) : null );
       d.detailedDescription = ( detailedDescription != null ? new string(detailedDescription ) : null );
       d.detailedDescriptionSoundPath = ( detailedDescriptionSoundPath != null ? new string(detailedDescriptionSoundPath ) : null );

       return d;
    }*/
    }
}