namespace uAdventure.Core
{
    public enum HasDescriptionSoundEnum
    { NAME_PATH = 0, DESCRIPTION_PATH = 1, DETAILED_DESCRIPTION_PATH = 2 }
    public interface HasDescriptionSound
    {

        /*
        public const int NAME_PATH = 0;

        public const int DESCRIPTION_PATH = 1;

        public const int DETAILED_DESCRIPTION_PATH = 2;
        */

        /**
         * Get the description sound's path
         * 
         * @return the path of the sound associated to the description
         */
        string getDescriptionSoundPath();

        /**
         * Set the description sound's path
         * 
         * @param descriptionSoundPath
         *          The new path of the sound associated to the description
         * 
         */
        void setDescriptionSoundPath(string descriptionSoundPath);

        /**
         * Get the detailed description sound's path
         * 
         * @return the path of the sound associated to the description
         */
        string getDetailedDescriptionSoundPath();

        /**
         * Set the description sound's path
         * 
         * @param descriptionSoundPath
         *          The new path of the sound associated to the description
         * 
         */
        void setDetailedDescriptionSoundPath(string detailedDescriptionSoundPath);

        /**
         * Get the name sound's path
         * 
         * @return the path of sound associated to the name
         */
        string getNameSoundPath();

        /**
         * Set the name sound's path
         * 
         * @param nameSoundPath
         *          The new path of the sound associated to the name
         * 
         */
        void setNameSoundPath(string nameSoundPath);
    }

}