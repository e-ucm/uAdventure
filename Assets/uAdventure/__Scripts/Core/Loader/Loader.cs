using UnityEngine;
using System.Collections.Generic;
using System;
using uAdventure.Runner;
using uAdventure.Core.XmlUpgrader;

namespace uAdventure.Core
{
    /**
     * This class loads the e-Adventure data from a XML file
     */
    public class Loader
    {
        private Loader()
        {
        }

        public static AdventureData LoadAdventureData(ResourceManager resourceManager, List<Incidence> incidences)
        {
            var adventureDataTemp = new AdventureData();
            try
            {
                var adventureParser = new AdventureHandler(adventureDataTemp, resourceManager, incidences);
                var upgrader = new Upgrader(resourceManager, incidences);
                if (upgrader.NeedsUpgrade("descriptor.xml"))
                {
                    var upgraded = upgrader.Upgrade("descriptor.xml");
                    adventureParser.ParseXml(upgraded);
                }
                else
                {
                    adventureParser.Parse("descriptor.xml");
                }
                adventureDataTemp = adventureParser.getAdventureData();

            }
            catch (Exception e) { Debug.LogError(e); }

            return adventureDataTemp;
        }

        public static Chapter LoadChapter(string filename, ResourceManager resourceManager, List<Incidence> incidences)
        {

            var currentChapter = new Chapter();
            currentChapter.setChapterPath(filename);
            ChapterHandler chapterParser = new ChapterHandler(currentChapter, resourceManager, incidences);
            var upgrader = new Upgrader(resourceManager, incidences);
            if (upgrader.NeedsUpgrade(filename))
            {
                var upgraded = upgrader.Upgrade(filename);
                chapterParser.ParseXml(upgraded);
            }
            else
            {
                chapterParser.Parse(filename);
            }

            return currentChapter;
        }
        /**
         * Loads an animation from a filename
         * 
         * @param filename
         *            The xml descriptor for the animation
         * @return the loaded Animation
         */
        public static Animation LoadAnimation(string filename, ResourceManager resourceManager, List<Incidence> incidences)
        {
            if (resourceManager.getAnimationsCache().ContainsKey(filename))
            {
                return resourceManager.getAnimationsCache()[filename];
            }

            AnimationHandler animationHandler = new AnimationHandler(resourceManager, incidences);

            var upgrader = new Upgrader(resourceManager, incidences);
            if (upgrader.NeedsUpgrade(filename))
            {
                var upgraded = upgrader.Upgrade(filename);
                animationHandler.ParseXml(upgraded);
            }
            else
            {
                animationHandler.Parse(filename);
            }

            Animation anim = animationHandler.GetAnimation();

            if (anim != null)
            {
                resourceManager.getAnimationsCache()[filename] = anim;
            }

            return anim;
        }
    }
}