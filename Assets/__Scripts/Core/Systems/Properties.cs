using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

public class Properties
{
    private Dictionary<string, string> list;
    private string filename;

    public Properties(string file)
    {
        reload(file);
    }

    public string getProperty(string field, string defValue)
    {
        return (getProperty(field) == null) ? (defValue) : (getProperty(field));
    }

    public string getProperty(string field)
    {
        return (list.ContainsKey(field)) ? (list[field]) : (null);
    }

    public void setProperty(string field, System.Object value)
    {
        if (!list.ContainsKey(field))
            list.Add(field, value.ToString());
        else
            list[field] = value.ToString();
    }

    public void Save()
    {
        Save(this.filename);
    }

    public void Save(string filename)
    {
        Debug.Log("Saving properties to: " + new FileInfo(filename).FullName);
        this.filename = filename;

        if (!System.IO.File.Exists(filename))
            System.IO.File.Create(filename).Close();

        System.IO.StreamWriter file = new System.IO.StreamWriter(filename);
        file.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?><!DOCTYPE properties SYSTEM \"http://java.sun.com/dtd/properties.dtd\">");
        file.WriteLine("<properties>");
        file.WriteLine("<comment> Project Configuration </comment>");
        foreach (string prop in list.Keys.ToArray())
            if (!string.IsNullOrEmpty(list[prop]))
                file.WriteLine("<entry key=\"" + prop + "\">" + list[prop] + "</entry>");
        file.Write("</properties>");
        file.Close();
    }

    public void reload()
    {
        reload(this.filename);
    }

    public void reload(string filename)
    {
        this.filename = filename;
        list = new Dictionary<string, string>();
        if (System.IO.File.Exists(filename))
            loadFromFile(filename);
        else
            System.IO.File.Create(filename).Close();
    }

    private void loadFromFile(string file)
    {
        string key = "", value = "";
        XmlTextReader reader = new XmlTextReader(file);
        while (reader.Read())
        {
            switch (reader.NodeType)
            {
                case XmlNodeType.Element: // The node is an element.

                    while (reader.MoveToNextAttribute()) // Read the attributes.
                    {
                        if (reader.Name.Equals("key"))
                            key = reader.Value;
                    }
                    break;
                case XmlNodeType.Text: //Display the text in each element.
                    if (!key.Equals(""))
                        value = reader.Value;
                    break;
                case XmlNodeType.EndElement: //Display the end of the element.
                    break;
            }
            if (key != "" && value != "")
            {
                list.Add(key, value);
                key = value = "";
            }
        }
        reader.Close();
    }
    public Dictionary<string, string>.KeyCollection getKeyset()
    {
        return list.Keys;
    }
}