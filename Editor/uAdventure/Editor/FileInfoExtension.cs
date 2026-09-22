

using System.IO;

static class FileInfoExtension
{
    public static bool IsuAdventureRelevant(this FileInfo fileInfo)
    {
        bool relevant = true;
        switch (fileInfo.Extension.ToLowerInvariant())
        {
            case ".ds_store": relevant = false; break;
        }
        return relevant;
    }
}
