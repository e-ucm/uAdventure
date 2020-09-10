using uAdventure.Runner;

namespace uAdventure.Core.XmlUpgrader
{
    public interface ITransformer
    {
        string TargetFile { get; }
        int TargetVersion { get; }
        int DestinationVersion { get; }
        string Upgrade(string input, string path, ResourceManager resourceManager);
        //IAsyncOperation<string> UpgradeAsync(string input, string path, ResourceManager resourceManager);
    }
}
