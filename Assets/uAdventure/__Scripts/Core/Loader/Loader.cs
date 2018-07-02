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
            var adventureParser = new AdventureHandler(new AdventureData(), resourceManager, incidences);
            var adventureData = adventureParser.Parse("descriptor.xml");

            return adventureData;
        }

        public static Chapter LoadChapter(string filename, ResourceManager resourceManager, List<Incidence> incidences)
        {
            var currentChapter = new Chapter();
            currentChapter.setChapterPath(filename);
            var chapterParser = new ChapterHandler(currentChapter, resourceManager, incidences);

            return chapterParser.Parse(filename);
        }

        public static Animation LoadAnimation(string filename, ResourceManager resourceManager, List<Incidence> incidences)
        {
            if (resourceManager.getAnimationsCache().ContainsKey(filename))
            {
                return resourceManager.getAnimationsCache()[filename];
            }

            var animationHandler = new AnimationHandler(resourceManager, incidences);
            var anim = animationHandler.Parse(filename);

            if (anim != null)
            {
                resourceManager.getAnimationsCache()[filename] = anim;
            }

            return anim;
        }
    }
}