using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using uAdventure.Runner;
using UnityFx.Async;
using UnityFx.Async.Promises;

namespace uAdventure.Core
{
    public abstract class XmlHandler<T> : FileHandler<T>
    {
        private string path = "none";


        protected XmlHandler(ResourceManager resourceManager, List<Incidence> incidences) : base(default(T), resourceManager, incidences)
        {
        }

        protected XmlHandler(T content, ResourceManager resourceManager, List<Incidence> incidences) : base(content, resourceManager, incidences)
        {
        }


        public override T Parse(string path)
        {
            string xml;
            if (upgrader.NeedsUpgrade(path))
            {
                xml = upgrader.Upgrade(path);
            }
            else
            {
                xml = resourceManager.getText(path);
            }

            if (string.IsNullOrEmpty(xml))
            {
                incidences.Add(new Incidence(Incidence.XML_INCIDENCE, Incidence.XML_INCIDENCE,
                    path, Incidence.IMPORTANCE_CRITICAL, "XML file not found", false, new FileNotFoundException()));
                return default(T);
            }

            this.path = path;

            return ParseXml(xml);
        }

        public override IAsyncOperation<T> ParseAsync(string path)
        {
            UnityEngine.Debug.Log("Parsing Async");
            var request = new AsyncCompletionSource<T>();

            upgrader.NeedsUpgradeAsync(path)
                .Then(needs =>
                {
                    if (needs)
                    {
                        return upgrader.UpgradeAsync(path);
                    }
                    else
                    {
                        return resourceManager.getTextAsync(path);
                    }
                })
                .Then(text =>
                {
                    if (string.IsNullOrEmpty(text))
                    {
                        incidences.Add(new Incidence(Incidence.XML_INCIDENCE, Incidence.XML_INCIDENCE,
                            path, Incidence.IMPORTANCE_CRITICAL, "XML file not found", false, new FileNotFoundException()));
                        request.SetResult(default(T));
                    }

                    this.path = path;

                    var result = ParseXml(text);

                    UnityEngine.Debug.Log("Done Parsing Async");
                    request.SetResult(result);
                });

            return request; 
        }

        public virtual T ParseXml(string content)
        {
            try
            {
                XmlDocument xmld = new XmlDocument();
                xmld.XmlResolver = null; 
                xmld.LoadXml(content);
                return ParseXml(xmld);
            }
            catch (Exception ex)
            {
                incidences.Add(new Incidence(Incidence.XML_INCIDENCE, Incidence.XML_INCIDENCE,
                    path, Incidence.IMPORTANCE_CRITICAL, "Error loading XML file", false, ex));

                path = "none";
                return default(T);
            }
        }

        protected abstract T ParseXml(XmlDocument doc);
    }
}
