using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace uAdventure.Core
{
    /**
     * Represents the whole data of an adventure game. This includes: Descriptor
     * Information (adventure title, description, path of each chapter, adaptation &
     * assessment configuration, gui configuration) List of Chapters List of
     * AssessmentProfiles List of AdaptationProfiles
     */
    public class AdventureData : DescriptorData, ICloneable
    {
        /**
         * List of chapters. Contains the main data of the adventures.
         */
        protected List<Chapter> chapters;

        // ------------------------------
        // EXTENSIONS
        // ------------------------------

        Dictionary<Type, IList> extensionObjects;

        /**
         * Default constructor
         */
        public AdventureData() : base()
        {
            this.chapters = new List<Chapter>();
            extensionObjects = new Dictionary<Type, IList>();
            contents = null;
        }

        /**
         * Adds a new chapter.
         * 
         * @param chapter
         */
        public void addChapter(Chapter chapter)
        {
            chapters.Add(chapter);
        }

        /**
         * @return the chapters
         */
        public List<Chapter> getChapters()
        {

            /*List<Chapter> chapters = new List<Chapter>();
            for (ChapterSummary summary: contents){
                chapters.add((Chapter)summary);
            }*/
            return chapters;
        }

        /**
         * @param chapters
         *            the chapters to set
         */
        public void setChapters(List<Chapter> chapters)
        {

            /*this.contents = new List<ChapterSummary>();
            for (Chapter chapter: chapters){
                contents.add(chapter);
            }*/
            this.chapters = chapters;
        }

        /*
         * Redefine ChapterSummaries handling so no data is duplicated
         *  
         */
        public override void addChapterSummary(ChapterSummary chapter)
        {

            if (chapter is Chapter)
            {
                chapters.Add((Chapter)chapter);
            }
        }

        /**
         * Returns if the chapter.xml has adaptation and/or assessment data
         * 
         * @return
         */
        public bool hasAdapOrAssesData()
        {

            for (int i = 0; i < chapters.Count; i++)
            {
                string[] apn = chapters[i].getAdaptationProfilesNames();
                if (apn.Length > 0)
                    return true;
                string[] aspn = chapters[i].getAssessmentProfilesNames();
                if (aspn.Length > 0)
                    return true;
            }

            return false;
        }

        /**
         * Returns the list of chapters of the game
         * 
         * @return List of chapters of the game
         */
        public override List<ChapterSummary> getChapterSummaries()
        {

            List<ChapterSummary> summary = new List<ChapterSummary>();
            foreach (Chapter chapter in chapters)
            {
                summary.Add(chapter);
            }
            return summary;
        }

        /// <summary>
        /// Obtain extension objects by type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> getObjects<T>()
        {
            if (!extensionObjects.ContainsKey(typeof(T)))
            {
                var exists = new List<T>(getObjects().FindAll(o => o is T).Cast<T>());
                if (exists.Count == 0)
                    extensionObjects.Add(typeof(T), new List<T>());
                else
                    return exists;
            }

            return extensionObjects[typeof(T)] as List<T>;
        }

        public List<object> getObjects()
        {

            // Let's return all the objects
            List<object> allObjects = new List<object>();
            foreach (var kp in extensionObjects)
            {
                allObjects.AddRange(kp.Value.Cast<object>());
            }

            return allObjects;
        }

        public IList getObjects(Type t)
        {
            var listType = typeof(List<>).MakeGenericType(t);

            if (!extensionObjects.ContainsKey(t))
                extensionObjects.Add(t, Activator.CreateInstance(listType) as IList);

            return extensionObjects[t] as IList;
        }

        public List<Type> getObjectTypes()
        {
            return new List<Type>(extensionObjects.Keys);
        }

        public override object Clone()
        {
            AdventureData ad = (AdventureData)base.Clone();
            ad.buttons = new List<CustomButton>();
            foreach (CustomButton cb in buttons)
                ad.buttons.Add((CustomButton)cb.Clone());
            foreach (CustomArrow ca in arrows)
                ad.arrows.Add((CustomArrow)ca.Clone());
            ad.chapters = new List<Chapter>();
            foreach (Chapter c in chapters)
            	ad.chapters.Add((Chapter) c.Clone());
            ad.commentaries = commentaries;
            ad.contents = new List<ChapterSummary>();
            foreach (ChapterSummary cs in contents)
                ad.contents.Add((ChapterSummary)cs.Clone());
            ad.cursors = new List<CustomCursor>();
            foreach (CustomCursor cc in cursors)
                ad.cursors.Add((CustomCursor)cc.Clone());
            ad.description = description;
            ad.guiCustomized = guiCustomized;
            ad.guiType = guiType;
            ad.playerMode = playerMode;
            ad.playerName = playerName;
            ad.title = title;
            return ad;
        }

    }
}