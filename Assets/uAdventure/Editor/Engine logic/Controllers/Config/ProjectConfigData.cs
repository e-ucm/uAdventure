using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ProjectConfigData
    {

        private const string FILE_NAME = "project.xml";

        private static Properties properties;

        private static List<ProjectConfigDataConsumer> consumers = new List<ProjectConfigDataConsumer>();

        public static void init()
        {

            consumers = new List<ProjectConfigDataConsumer>();
            properties = new Properties(Controller.Instance.ProjectFolder+ "/" + FILE_NAME);
            storeToXML();
        }

        public static void addConsumer(ProjectConfigDataConsumer consumer)
        {
            consumers.Add(consumer);
        }

        public static void LoadFromXML()
        {

            properties = new Properties(Controller.Instance.ProjectFolder+ "/" + FILE_NAME);
            foreach (ProjectConfigDataConsumer consumer in consumers)
            {
                consumer.updateData();
            }
            //LOMConfigData.loadData();
        }

        public static void storeToXML()
        {
            if (properties == null)
                properties = new Properties(Controller.Instance.ProjectFolder+ "/" + FILE_NAME);
            properties.Save();
        }

        public static string getProperty(string key)
        {

            if (properties.GetProperty(key) != null)
            {
                return properties.GetProperty(key);
            }
            else
                return null;
        }

        public static void setProperty(string key, string value)
        {

            properties.SetProperty(key, value);
        }

        public static bool existsKey(string key)
        {
            bool exists = (properties.GetProperty(key) != null);
            return exists;
        }

        public static void registerConsumer(ProjectConfigDataConsumer newConsumer)
        {

            consumers.Add(newConsumer);
        }
    }
}