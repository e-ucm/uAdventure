using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System;

namespace uAdventure.Core
{
    /**
     * This class loads the e-Adventure data from a XML file
     */
    public class Loader
    {
        /**
            * AdventureData structure which has been previously read. (For Debug
            * execution)
            */
        private static AdventureData adventureData;

        /**
         * Cache the SaxParserFactory
         */
        //private static SAXParserFactory factory = SAXParserFactory.newInstance();

        /**
         * Private constructor
         */
        private Loader()
        {

        }

        public static AdventureData loadAdventureData(string path, List<Incidence> incidences)
        {
            AdventureData adventureDataTemp = new AdventureData();
            try
            {
                AdventureHandler_ adventureParser = new AdventureHandler_(adventureDataTemp);

                // Read and close the input stream
                adventureParser.Parse(path + "/descriptor.xml");
                //descriptorIS.close();

                adventureDataTemp = adventureParser.getAdventureData();

            }
            catch (Exception e) { Debug.LogError(e); }

            return adventureDataTemp;
        }

        /**
         * @param adventureData
         *            the adventureData to set
         */
        public static void setAdventureData(AdventureData adventureData)
        {

            Loader.adventureData = adventureData;
        }

        private static Dictionary<string, Animation> animationLoadCache;

        /**
         * Loads an animation from a filename
         * 
         * @param filename
         *            The xml descriptor for the animation
         * @return the loaded Animation
         */
        public static Animation loadAnimation(InputStreamCreator isCreator, string filename, ImageLoaderFactory imageloader)
        {
            if (animationLoadCache == null) animationLoadCache = new Dictionary<string, Animation>();

            if (animationLoadCache.ContainsKey(filename))
                return animationLoadCache[filename];

            AnimationHandler_ animationHandler = new AnimationHandler_(isCreator, imageloader);
            
            try
            {
                string descriptorIS = null;

                descriptorIS = isCreator.buildInputStream("Assets/Resources/CurrentGame/" + filename);
                if (!descriptorIS.EndsWith(".eaa"))
                    descriptorIS += ".eaa";
                animationHandler.Parse(descriptorIS);


            }
            catch (Exception e) { Debug.LogError(e); }

            Animation anim = null;
            if (animationHandler.getAnimation() != null)
                anim = animationHandler.getAnimation();
            else
                anim = new Animation("anim" + (new System.Random()).Next(1000), imageloader);

            animationLoadCache.Add(filename, anim);
            return anim;
        }

        /**
         * Returns true if the given file contains an eAdventure game from a newer
         * version. Essentially, it looks for the "ead.properties" file in the new
         * eAdventure games. If it's found, then returns true
         * 
         * @param f
         *            the file to check
         * @return if the game requires a newer version
         */
        //public static bool requiresNewVersion(java.io.File f)
        //{
        //    bool isOldProject = true;
        //    FileInputStream in = null;
        //    ZipInputStream zipIn = null;
        //    try
        //    {
        //        in = new FileInputStream(f);
        //        zipIn = new ZipInputStream( in );
        //        ZipEntry zipEntry = null;
        //        while ((zipEntry = zipIn.getNextEntry()) != null)
        //        {
        //            if (zipEntry.getName().endsWith("ead.properties"))
        //            {
        //                isOldProject = false;
        //            }
        //        }
        //        zipIn.close();
        //    }
        //    catch (IOException e)
        //    {

        //    }
        //    finally
        //    {
        //        if ( in != null ) {
        //            try
        //            {
        //                in.close();
        //            }
        //            catch (IOException e)
        //            {

        //            }
        //        }

        //        if (zipIn != null)
        //        {
        //            try
        //            {
        //                zipIn.close();
        //            }
        //            catch (IOException e)
        //            {
        //            }
        //        }
        //    }
        //    return !isOldProject;
        //}
    }
}