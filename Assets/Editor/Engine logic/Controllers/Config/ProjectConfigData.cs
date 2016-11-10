using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectConfigData{

    private const string FILE_NAME = "project.xml";

    private static Properties properties;

    private static List<ProjectConfigDataConsumer> consumers = new List<ProjectConfigDataConsumer>();

    public static void init()
    {

        consumers = new List<ProjectConfigDataConsumer>();
        properties = new Properties(Controller.getInstance().getProjectFolder() + "/" + FILE_NAME);
        storeToXML();
    }

    public static void addConsumer(ProjectConfigDataConsumer consumer)
    {
        consumers.Add(consumer);
    }

    public static void loadFromXML()
    {

        properties = new Properties(Controller.getInstance().getProjectFolder() + "/" + FILE_NAME);
        foreach (ProjectConfigDataConsumer consumer in consumers)
        {
            consumer.updateData();
        }
        //LOMConfigData.loadData();
    }

    public static void storeToXML()
    {
        if(properties == null)
            properties = new Properties(Controller.getInstance().getProjectFolder() + "/" + FILE_NAME);
        properties.Save();
    }

    public static string getProperty(string key)
    {

        if (properties.getProperty(key) != null)
        {
            return properties.getProperty(key);
        }
        else
            return null;
    }

    public static void setProperty(string key, string value)
    {

        properties.setProperty(key, value);
    }

    public static bool existsKey(string key)
    {
        bool exists = (properties.getProperty(key)!= null);
        return exists;
    }

    public static void registerConsumer(ProjectConfigDataConsumer newConsumer)
    {

        consumers.Add(newConsumer);
    }
}
