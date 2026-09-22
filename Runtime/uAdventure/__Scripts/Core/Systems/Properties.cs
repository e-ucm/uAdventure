using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace uAdventure.Core
{
    public class Properties
    {
        private Dictionary<string, string> list;
        private string filename;

        public Properties()
        {
            list = new Dictionary<string, string>();
        }

        public Properties(string file)
        {
            Reload(file);
        }

        public string this[string field]
        {
            get
            {
                return (list.ContainsKey(field)) ? (list[field]) : (null);
            }
            set
            {
                if (!list.ContainsKey(field))
                    list.Add(field, value);
                else
                    list[field] = value;
            }
        }

        public string GetProperty(string field, string defValue)
        {
            return GetProperty(field) ?? defValue;
        }

        public string GetProperty(string field)
        {
            return this[field];
        }

        public void SetProperty(string field, System.Object value)
        {
            this[field] = value.ToString();
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
            file.WriteLine(ToString());
            file.Close();
        }

        public override string ToString()
        {
            string s = "<properties>\n";
            s += "<comment> Project Configuration </comment>\n";
            foreach (string prop in list.Keys.ToArray())
                if (!string.IsNullOrEmpty(list[prop]))
                    s += "<entry key=\"" + prop + "\">" + list[prop] + "</entry>\n";
            s += "</properties>\n";
            return s;
        }

        public void Reload()
        {
            Reload(this.filename);
        }

        public void Reload(string filename)
        {
            this.filename = filename;
            list = new Dictionary<string, string>();
            if (System.IO.File.Exists(filename))
                LoadFromFile(filename);
            else
                System.IO.File.Create(filename).Close();
        }

        public void ReloadFromString(string data)
        {
            LoadFromString(data);
        }

        private void LoadFromString(string data)
        {
            Load(new XmlTextReader(new StringReader(data)));
        }

        private void LoadFromFile(string file)
        {
            Load(new XmlTextReader(file));
        }

        private void Load(XmlTextReader reader)
        {
            string key = "", value = "";
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

        public Dictionary<string, string>.KeyCollection KeySet
        {
            get { return list.Keys; }
        }
    }
}