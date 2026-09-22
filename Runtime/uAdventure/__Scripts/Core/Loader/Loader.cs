using IMS.MD.v1p2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using uAdventure.Runner;
using UnityFx.Async;
using UnityFx.Async.Promises;

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

        public static IAsyncOperation<AdventureData> LoadAdventureDataAsync(ResourceManager resourceManager, List<Incidence> incidences)
        {
            UnityEngine.Debug.Log("LoadingAdventureData");
            var result = new AsyncCompletionSource<AdventureData>();

            var adventureParser = new AdventureHandler(new AdventureData(), resourceManager, incidences);

            adventureParser.ParseAsync("descriptor.xml")
                .Then(adventureData =>
                {
                    UnityEngine.Debug.Log("Done LoadingAdventureData");
                    result.SetResult(adventureData);
                });

            return result;
        }

        public static Chapter LoadChapter(string filename, ResourceManager resourceManager, List<Incidence> incidences)
        {
            var currentChapter = new Chapter();
            currentChapter.setChapterPath(filename);
            var chapterParser = new ChapterHandler(currentChapter, resourceManager, incidences);

            return chapterParser.Parse(filename);
        }

        public static IAsyncOperation<Chapter> LoadChapterAsync(string filename, ResourceManager resourceManager, List<Incidence> incidences)
        {
            var result = new AsyncCompletionSource<Chapter>();

            var chapterHandler = new ChapterHandler(new Chapter(), resourceManager, incidences);

            chapterHandler.ParseAsync(filename)
                .Then(chapter =>
                {
                    result.SetResult(chapter);
                });

            return result;
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

        public static IAsyncOperation<Animation> LoadAnimationAsync(string filename, ResourceManager resourceManager, List<Incidence> incidences)
        {
            var result = new AsyncCompletionSource<Animation>();

            if (resourceManager.getAnimationsCache().ContainsKey(filename))
            {
                result.SetResult(resourceManager.getAnimationsCache()[filename]); 
            }

            var animationHandler = new AnimationHandler(resourceManager, incidences);

            animationHandler.ParseAsync(filename)
                .Then(anim =>
                {
                    result.SetResult(anim); 
                    if (anim != null)
                    {
                        resourceManager.getAnimationsCache()[filename] = anim;
                    }
                });

            return result;
        }

        public static lomType LoadImsCPMetadata(string imsCPMetadataPath, ResourceManager resourceManager, List<Incidence> incidences)
        {
            var metadata = resourceManager.getText(imsCPMetadataPath);

            var serializer = new XmlSerializer(typeof(lomType));     
            
            // convert string to stream
            byte[] byteArray = Encoding.ASCII.GetBytes(metadata);
            MemoryStream stream = new MemoryStream(byteArray);
            return (lomType)serializer.Deserialize(stream);
        }
    }
}