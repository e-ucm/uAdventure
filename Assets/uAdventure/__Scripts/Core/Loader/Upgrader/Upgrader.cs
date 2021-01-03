using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using uAdventure.Runner;
using UnityFx.Async;
using UnityFx.Async.Promises;

namespace uAdventure.Core.XmlUpgrader
{
    public class Upgrader
    {
        private static readonly ITransformer[] transformers = 
        {
            new Chapter0To1Transformer(),
            new Chapter1To2Transformer(),
            new Chapter2To3Transformer(),
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
            return CheckUpgraders(fileVersion, path);
        }

        public IAsyncOperation<bool> NeedsUpgradeAsync(string path)
        {
            UnityEngine.Debug.Log("Checking Upgrade Async");
            var result = new AsyncCompletionSource<bool>();
            GetFileVersionAsync(path, resourceManager)
                .Then(version =>
                {
                    UnityEngine.Debug.Log("Done Checking Upgrade Async");
                    result.SetResult(CheckUpgraders(version, path));
                });

            return result;
        }

        private bool CheckUpgraders(int fileVersion, string path)
        {
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

        public IAsyncOperation<string> UpgradeAsync(string path)
        {
            var result = new AsyncCompletionSource<string>();

            GetFileVersionAsync(path, resourceManager)
                .Then(version =>
                {
                    Dictionary<int, ITransformer> transformers = null;
                    foreach (var upgradableFile in orderedTransformers.Keys)
                    {
                        if (Regex.IsMatch(path, upgradableFile))
                        {
                            transformers = orderedTransformers[upgradableFile];
                            break;
                        }
                    }

                    if (transformers == null)
                    {
                        result.SetResult(null);
                    }
                    else
                    {
                        var maxVersion = transformers.Values.Max(t => t.DestinationVersion);
                        resourceManager.getTextAsync(path)
                            .Then(input =>
                            {
                                while (version < maxVersion)
                                {
                                    if (!transformers.ContainsKey(version))
                                    {
                                        incidences.Add(new Incidence(Incidence.XML_INCIDENCE, Incidence.XML_INCIDENCE, path,
                                            Incidence.IMPORTANCE_CRITICAL, "Upgrader not found to upgrade from version "
                                            + version + " towards " + maxVersion, false, new UpgraderVersionNotFoundException()
                                            {
                                                TargetFile = path,
                                                TargetVersion = version,
                                                MaxVersion = maxVersion
                                            }));
                                        result.SetResult(null);
                                    }

                                    input = transformers[version].Upgrade(input, path, resourceManager);
                                    version = transformers[version].DestinationVersion;
                                }
                                result.SetResult(input);
                            });
                    }
                });

            return result;
        }

        private static int GetFileVersion(string path, ResourceManager resourceManager)
        {
            var xmlText = resourceManager.getText(path);
            return ExtractFileVersion(xmlText);
        }
        private static IAsyncOperation<int> GetFileVersionAsync(string path, ResourceManager resourceManager)
        {
            UnityEngine.Debug.Log("Getting File Version Async");
            var result = new AsyncCompletionSource<int>();
            resourceManager.getTextAsync(path)
                .Then(text =>
                {
                    UnityEngine.Debug.Log("Done Getting File Version Async");
                    result.SetResult(ExtractFileVersion(text));
                });

            return result;
        }

        private static int ExtractFileVersion(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return -1;
            }

            var xmlDocument = new XmlDocument();
            xmlDocument.XmlResolver = null;
            xmlDocument.LoadXml(text);

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