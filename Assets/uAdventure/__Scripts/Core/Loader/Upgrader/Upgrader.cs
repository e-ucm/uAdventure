using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using uAdventure.Runner;

namespace uAdventure.Core.XmlUpgrader
{
    public class Upgrader
    {
        private static readonly ITransformer[] transformers = 
        {
            new Chapter0To1Transformer(),
            new Chapter1To2Transformer(),
            new AnimationNonExistentTo0Transformer()
        };
        private readonly Dictionary<string, Dictionary<int, ITransformer>> orderedTransformers;
        private readonly ResourceManager resourceManager;
        private readonly List<Incidence> incidences;

        public Upgrader(ResourceManager resourceManager, List<Incidence> incidences)
        {
            this.resourceManager = resourceManager;
            this.incidences = incidences;
            this.orderedTransformers = new Dictionary<string, Dictionary<int, ITransformer>>();

            foreach (var transformer in transformers)
            {
                var regFile = WildCardToRegular(transformer.TargetFile);
                if (!orderedTransformers.ContainsKey(regFile))
                {
                    orderedTransformers[regFile] = new Dictionary<int, ITransformer>();
                }
                orderedTransformers[regFile].Add(transformer.TargetVersion, transformer);
            }
        }

        public bool NeedsUpgrade(string path)
        {
            var fileVersion = GetFileVersion(path, resourceManager);

            foreach (var upgradableFile in orderedTransformers.Keys)
            {
                if (Regex.IsMatch(path, upgradableFile))
                {
                    var fileTransformers = orderedTransformers[upgradableFile];
                    var maxVersion = fileTransformers.Values.Max(t => t.DestinationVersion);
                    return fileVersion < maxVersion;
                }
            }

            return false;
        }

        public string Upgrade(string path)
        {
            var fileVersion = GetFileVersion(path, resourceManager);

            foreach (var upgradableFile in orderedTransformers.Keys)
            {
                if (Regex.IsMatch(path, upgradableFile))
                {
                    var fileTransformers = orderedTransformers[upgradableFile];
                    var maxVersion = fileTransformers.Values.Max(t => t.DestinationVersion);
                    string input = resourceManager.getText(path);

                    while (fileVersion < maxVersion)
                    {
                        if (!fileTransformers.ContainsKey(fileVersion))
                        {
                            incidences.Add(new Incidence(Incidence.XML_INCIDENCE, Incidence.XML_INCIDENCE, path,
                                Incidence.IMPORTANCE_CRITICAL, "Upgrader not found to upgrade from version "
                                + fileVersion + " towards " + maxVersion, false, new UpgraderVersionNotFoundException()
                                {
                                    TargetFile = path,
                                    TargetVersion = fileVersion,
                                    MaxVersion = maxVersion
                                }));
                            return null;
                        }

                        input = fileTransformers[fileVersion].Upgrade(input, path, resourceManager);
                        fileVersion = fileTransformers[fileVersion].DestinationVersion;
                    }

                    return input;
                }
            }

            return null;
        }

        private static int GetFileVersion(string path, ResourceManager resourceManager)
        {
            var xmlText = resourceManager.getText(path);
            if (string.IsNullOrEmpty(xmlText))
            {
                return -1; 
            }

            var xmlDocument = new XmlDocument();
            xmlDocument.XmlResolver = null;
            xmlDocument.LoadXml(xmlText);

            XmlElement root = xmlDocument.DocumentElement;

            return ExParsers.ParseDefault(root.GetAttribute("version"), 0);
        }

        // If you want to implement both "*" and "?"
        private static string WildCardToRegular(string value)
        {
            return "^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$";
        }

    }

    public class UpgraderVersionNotFoundException : System.Exception
    {
        public string TargetFile { get; set; }
        public int TargetVersion { get; set; }
        public int MaxVersion { get; set; }
    }
}