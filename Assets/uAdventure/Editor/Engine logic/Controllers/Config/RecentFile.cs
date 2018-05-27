using System;
using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class RecentFile
    {
        private string absolutePath;

        private DateTime date;

        /**
         * @param absolutePath
         * @param date
         */
        public RecentFile(string absolutePath, DateTime date)
        {

            this.absolutePath = absolutePath;
            this.date = date;
        }

        public RecentFile(string absolutePath)
        {

            this.absolutePath = absolutePath;
            date = new DateTime();
        }

        public RecentFile(string absolutePath, string date)
        {
            this.absolutePath = absolutePath;
            this.date = DateTime.Parse(date);
        }

        /**
         * @return the absolutePath
         */
        public string getAbsolutePath()
        {

            return absolutePath;
        }

        /**
         * @param absolutePath
         *            the absolutePath to set
         */
        public void setAbsolutePath(string absolutePath)
        {

            this.absolutePath = absolutePath;
        }

        /**
         * @return the date
         */
        public DateTime getDate()
        {

            return date;
        }

        /**
         * @param date
         *            the date to set
         */
        public void setDate(DateTime date)
        {

            this.date = date;
        }
    }
}