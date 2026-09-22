using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    public class ChapterSummary : Titled, Described, ICloneable
    {
        /**
            * Chapter's title.
            */
        private string title;

        /**
         * Chapter's description.
         */
        private string description;

        /**
         * Adaptation profile's name, if there is any.
         */
        // this attribute isn't in descriptor, now is in chapter.xml, as "eAdventure" element attribute.
        // don't move to "Chapter" class for no get it dirty.
        private string adaptationName;

        /**
         * Assessment profile's name, if there is any.
         */
        // this attribute isn't in descriptor, now is in chapter.xml, as "eAdventure" element attribute.
        // don't move to "Chapter" class for no get it dirty.
        private string assessmentName;

        /**
         * List of assessment profiles in chapter.
         */
        protected List<AssessmentProfile> assessmentProfiles;

        /**
         * List of adaptation profiles in chapter.
         */
        protected List<AdaptationProfile> adaptationProfiles;

        /**
         * Relative path to the zip where it was contained. Used for replacing
         */
        private string path;

        /**
         * Empty constructor. Sets values to null or blank
         */
        public ChapterSummary()
        {

            title = null;
            description = "";
            adaptationName = "";
            assessmentName = "";
            assessmentProfiles = new List<AssessmentProfile>();
            adaptationProfiles = new List<AdaptationProfile>();
        }

        /**
         * Constructor with title for the chapter. Sets empty values..
         * 
         * @param title
         *            Title for the chapter
         */
        public ChapterSummary(string title) : this()
        {
            this.title = title;
        }

        /**
         * Returns the title of the chapter
         * 
         * @return Chapter's title
         */
        public string getTitle()
        {

            return title;
        }

        /**
         * Returns the description of the chapter.
         * 
         * @return Chapter's description
         */
        public string getDescription()
        {

            return description;
        }

        /**
         * Returns the name of the adaptation file.
         * 
         * @return the name of the adaptation file
         */
        public string getAdaptationName()
        {

            return adaptationName;
        }

        /**
         * Returns the name of the assessment file.
         * 
         * @return the name of the assessment file
         */
        public string getAssessmentName()
        {

            return assessmentName;
        }

        /**
         * Sets the title of the chapter.
         * 
         * @param title
         *            New title for the chapter
         */
        public void setTitle(string title)
        {

            this.title = title;
        }

        /**
         * Sets the description of the chapter.
         * 
         * @param description
         *            New description for the chapter
         */
        public void setDescription(string description)
        {

            this.description = description;
        }

        /**
         * Changes the name of the adaptation file.
         * 
         * @param adaptationName
         *            the new name of the adaptation file
         */
        public void setAdaptationName(string adaptationName)
        {

            this.adaptationName = adaptationName;
        }

        /**
         * Changes the name of the assessment file.
         * 
         * @param assessmentName
         *            the new name of the assessment file
         */
        public void setAssessmentName(string assessmentName)
        {

            this.assessmentName = assessmentName;
        }

        /**
         * @return the path of the capt
         */
        public string getChapterPath()
        {

            return path;
        }

        /**
         * @param path
         *            the path to set
         */
        public void setChapterPath(string path)
        {

            this.path = path;
        }

        /**
         * Returns true if an assessment profile has been defined for the chapter
         * 
         * @return
         */
        public bool hasAssessmentProfile()
        {

            return this.assessmentName != null && !this.assessmentName.Equals("");
        }

        /**
         * Returns true if an adaptation profile has been defined for the chapter
         * 
         * @return
         */
        public bool hasAdaptationProfile()
        {

            return this.adaptationName != null && !this.adaptationName.Equals("");
        }

        /**
         * Adds new assessment profile
         * 
         * @param assessProfile
         *            the new assessment profile to add
         */
        public void addAssessmentProfile(AssessmentProfile assessProfile)
        {

            assessmentProfiles.Add(assessProfile);
        }

        /**
         * Adds new adaptation profile
         * 
         * @param adaptProfile
         *            the new assessment profile to add
         */
        public void addAdaptationProfile(AdaptationProfile adaptProfile)
        {

            adaptationProfiles.Add(adaptProfile);
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            ChapterSummary cs = (ChapterSummary) super.clone( );
            cs.adaptationName = ( adaptationName != null ? new string(adaptationName ) : null );
            cs.assessmentName = ( assessmentName != null ? new string(assessmentName ) : null );
            cs.description = ( description != null ? new string(description ) : null );
            cs.path = ( path != null ? new string(path ) : null );
            cs.title = ( title != null ? new string(title ) : null );
            cs.adaptationProfiles = ( adaptationProfiles != null ? new List<AdaptationProfile>( ) : null );
            cs.assessmentProfiles = ( assessmentProfiles != null ? new List<AssessmentProfile>( ) : null );
            return cs;
        }*/

        /**
         * @return the assessmentProfiles
         */
        public List<AssessmentProfile> getAssessmentProfiles()
        {

            return assessmentProfiles;
        }

        /**
         * @return the adaptationProfiles
         */
        public List<AdaptationProfile> getAdaptationProfiles()
        {

            return adaptationProfiles;
        }

        /**
         * Return the selected assessment profile
         * 
         * @return the selected assessment profile
         */
        public AssessmentProfile getSelectedAssessmentProfile()
        {

            foreach (AssessmentProfile profile in assessmentProfiles)
            {
                if (profile.getName().Equals(assessmentName))
                    return profile;
            }
            return null;
        }

        /**
         * Return the selected adaptation profile
         * 
         * @return the selected adaptation profile
         */
        public AdaptationProfile getSelectedAdaptationProfile()
        {

            foreach (AdaptationProfile profile in adaptationProfiles)
            {
                if (profile.getName().Equals(adaptationName))
                    return profile;
            }
            return null;
        }

        /**
         * Return all adaptation profiles names
         * 
         * @return An array of all adaptation profiles names
         */
        public string[] getAdaptationProfilesNames()
        {

            string[] names = new string[adaptationProfiles.Count];
            for (int i = 0; i < adaptationProfiles.Count; i++)
                names[i] = adaptationProfiles[i].getName();
            return names;
        }

        /**
         * Return all assessment profiles names
         * 
         * @return An array of all assessment profiles names
         */
        public string[] getAssessmentProfilesNames()
        {

            string[] names = new string[assessmentProfiles.Count];
            for (int i = 0; i < assessmentProfiles.Count; i++)
                names[i] = assessmentProfiles[i].getName();
            return names;
        }

        public virtual object Clone()
        {
            ChapterSummary cs = (ChapterSummary)this.MemberwiseClone();
            cs.adaptationName = (adaptationName != null ? adaptationName : null);
            cs.assessmentName = (assessmentName != null ? assessmentName : null);
            cs.description = (description != null ? description : null);
            cs.path = (path != null ? path : null);
            cs.title = (title != null ? title : null);
            cs.adaptationProfiles = (adaptationProfiles != null ? new List<AdaptationProfile>() : null);
            cs.assessmentProfiles = (assessmentProfiles != null ? new List<AssessmentProfile>() : null);
            return cs;
        }
    }
}