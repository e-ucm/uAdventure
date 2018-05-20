using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace uAdventure.Core
{
    public class Incidence
    {

        /**
            * Type of the affected resource: XML or Asset file.
            */
        public const int XML_INCIDENCE = 0;

        public const int ASSET_INCIDENCE = 1;

        /**
         * Affected resource "sub-type". For XML: Assessment, Adaptation, Chapter or
         * Descriptor can fail.
         */
        public const int ASSESSMENT_INCIDENCE = 0;

        public const int ADAPTATION_INCIDENCE = 1;

        public const int CHAPTER_INCIDENCE = 2;

        public const int DESCRIPTOR_INCIDENCE = 3;

        /**
         * Importance: Low, Medium, High or Critical
         */
        public const int IMPORTANCE_LOW = 0;

        public const int IMPORTANCE_MEDIUM = 1;

        public const int IMPORTANCE_HIGH = 2;

        public const int IMPORTANCE_CRITICAL = 3;

        /**
         * Type of the affected resource: {@link #XML_INCIDENCE},
         * {@link #ASSET_INCIDENCE}
         */
        private int type;

        /**
         * Affected area (sub-type). For XML: {@link #ASSESSMENT_INCIDENCE},
         * {@link #ADAPTATION_INCIDENCE}, {@link #CHAPTER_INCIDENCE},
         * {@link #DESCRIPTOR_INCIDENCE} For Asset: Category of the asset (see
         * AssetsController)
         */
        private int affectedArea;

        /**
         * Path of the affected File
         */
        private string affectedResource;

        /**
         * Importance of the incidence. Low-important incidences will not affect
         * directly the game. Medium-important incidences will vary slightly the
         * behaviour of the game, but not significantly. High important incidences
         * will make some parts of the games unplayable and Critical ones will make
         * them completely unreadable.
         */
        private int importance;

        /**
         * string containing a message which describes the problem found reading the
         * damaged file
         */
        private string message;

        /**
         * Indicates if the damaged file was referenced inside the XML or just was
         * contained in the zip / folder
         */
        private bool referenced;

        /**
         * the exception
         */
        private Exception exception;

        /**
         * @return the exception
         */
        public Exception getException()
        {

            return exception;
        }

        /**
         * @param exception
         *            the exception to set
         */
        public void setException(Exception exception)
        {

            this.exception = exception;
        }

        /**
         * @return the type
         */
        public int getType()
        {

            return type;
        }

        /**
         * @param type
         *            the type to set
         */
        public void setType(int type)
        {

            this.type = type;
        }

        /**
         * @return the affectedArea
         */
        public int getAffectedArea()
        {

            return affectedArea;
        }

        /**
         * @param affectedArea
         *            the affectedArea to set
         */
        public void setAffectedArea(int affectedArea)
        {

            this.affectedArea = affectedArea;
        }

        /**
         * @return the affectedResource
         */
        public string getAffectedResource()
        {

            return affectedResource;
        }

        /**
         * @param affectedResource
         *            the affectedResource to set
         */
        public void setAffectedResource(string affectedResource)
        {

            this.affectedResource = affectedResource;
        }

        /**
         * @return the importance
         */
        public int getImportance()
        {

            return importance;
        }

        /**
         * @param importance
         *            the importance to set
         */
        public void setImportance(int importance)
        {

            this.importance = importance;
        }

        /**
         * @return the message
         */
        public string getMessage()
        {

            return message;
        }

        /**
         * @param message
         *            the message to set
         */
        public void setMessage(string message)
        {

            this.message = message;
        }

        /**
         * @return the referenced
         */
        public bool isReferenced()
        {

            return referenced;
        }

        /**
         * @param referenced
         *            the referenced to set
         */
        public void setReferenced(bool referenced)
        {

            this.referenced = referenced;
        }

        /**
         * @param type
         * @param affectedArea
         * @param affectedResource
         * @param importance
         * @param message
         * @param referenced
         */
        public Incidence(int type, int affectedArea, string affectedResource, int importance, string message, bool referenced, Exception exception)
        {

            this.type = type;
            this.affectedArea = affectedArea;
            this.affectedResource = affectedResource;
            this.importance = importance;
            this.message = message;
            this.referenced = referenced;
            this.exception = exception;
        }

        public static Incidence createDescriptorIncidence(string message, Exception exception)
        {

            int type = XML_INCIDENCE;
            int affectedArea = DESCRIPTOR_INCIDENCE;
            string affectedResource = "descriptor.xml";
            int importance = IMPORTANCE_CRITICAL;
            bool referenced = true;
            return new Incidence(type, affectedArea, affectedResource, importance, message, referenced, exception);
        }

        public static Incidence createAssessmentIncidence(bool referenced, string message, string profilePath, Exception exception)
        {

            int type = XML_INCIDENCE;
            int affectedArea = ASSESSMENT_INCIDENCE;
            string affectedResource = profilePath;
            int importance = IMPORTANCE_LOW;
            if (referenced)
                importance = IMPORTANCE_MEDIUM;
            return new Incidence(type, affectedArea, affectedResource, importance, message, referenced, exception);

        }

        public static Incidence createAdaptationIncidence(bool referenced, string message, string profilePath, Exception exception)
        {

            int type = XML_INCIDENCE;
            int affectedArea = ADAPTATION_INCIDENCE;
            string affectedResource = profilePath;
            int importance = IMPORTANCE_LOW;
            if (referenced)
                importance = IMPORTANCE_MEDIUM;
            return new Incidence(type, affectedArea, affectedResource, importance, message, referenced, exception);
        }

        public static Incidence createChapterIncidence(string message, string chapterPath, Exception exception)
        {

            int type = XML_INCIDENCE;
            int affectedArea = CHAPTER_INCIDENCE;
            string affectedResource = chapterPath;
            int importance = IMPORTANCE_HIGH;
            bool referenced = true;
            return new Incidence(type, affectedArea, affectedResource, importance, message, referenced, exception);
        }

        public static Incidence createAssetIncidence(bool notPresent, int assetType, string message, string assetPath, Exception exception)
        {

            int type = ASSET_INCIDENCE;
            int affectedArea = assetType;
            int importance = IMPORTANCE_MEDIUM;
            return new Incidence(type, affectedArea, assetPath, importance, message, notPresent, exception);
        }

        public static void sortIncidences(List<Incidence> incidences)
        {

            if (incidences != null && incidences.Count > 0)
                sortIncidences(incidences, 0);
        }

        private static void sortIncidences(List<Incidence> incidences, int j)
        {

            if (incidences != null)
            {
                int max = incidences[j].importance;
                for (int i = j + 1; i < incidences.Count && max < IMPORTANCE_CRITICAL; i++)
                {
                    if (incidences[i].importance > max)
                    {
                        max = incidences[i].importance;
                        Incidence temp = incidences[j];
                        incidences.RemoveAt(j);
                        incidences.Insert(j, incidences[i]);
                        incidences.Insert(i, temp);
                    }
                }
            }
        }
    }
}