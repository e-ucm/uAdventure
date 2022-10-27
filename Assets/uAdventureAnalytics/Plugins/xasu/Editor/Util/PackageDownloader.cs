using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.Networking;
using Xasu.Requests;
using System.IO.Compression;

public class PackageDownloader
{

    private static PackageDownloader instance;
    public static PackageDownloader Instance { get { return instance ?? (instance = new PackageDownloader()); } }

    private PackageDownloader() { }

    public async Task<bool> DownloadPackage(string name, string folderName, string url)
    {
        if (!EditorUtility.DisplayDialog("Package needed!", name + " package will be downloaded from '" + url + "'. Do you want to continue?", "Ok", "Cancel"))
        {
            return false;
        }

        var projectPath = Directory.GetCurrentDirectory().Replace("\\", "/");
        var downloadPath = projectPath + folderName;

        var progress = new Progress<float>();
        progress.ProgressChanged += (_, p) =>
        {

            if (EditorUtility.DisplayCancelableProgressBar("Downloading", "Downloading " + name + "...", p))
            {
                EditorUtility.ClearProgressBar();
                throw new TaskCanceledException("The download has been canceled by the user!");
            }
        };

        try
        {
            var request = await RequestsUtility.DoRequest(UnityWebRequest.Get(url), progress);

            var downloadFileName = name.Replace(" ", "").Trim() + ".zip";

            if (!Directory.Exists(downloadPath))
            {
                Directory.CreateDirectory(downloadPath);
            }

            // Write the zip file
            EditorUtility.DisplayProgressBar("Copying", "Copying downloaded file " + name + "...", 100);
            File.WriteAllBytes(downloadPath + "/" + downloadFileName, request.downloadHandler.data);
            // Unzip it
            EditorUtility.DisplayProgressBar("Extracting...", "Extracting " + name + " to " + downloadPath, 0f);
            DecompressFile(downloadPath + "/" + downloadFileName, downloadPath);
            EditorUtility.DisplayProgressBar("Extracting...", "Extracting " + name + " to " + downloadPath, 1f);
            EditorUtility.ClearProgressBar();
            // Delete the zip
            File.Delete(downloadPath + "/" + downloadFileName);
            return true;

        }
        catch (TaskCanceledException ex)
        {
            EditorUtility.DisplayDialog("Canceled!", ex.Message, "Ok");
        }
        catch (Exception ex)
        {
            EditorUtility.DisplayDialog("Error!", "Download failed! Check your connection and try again. " +
                "If the problem persist download it manually and put it in the " + folderName + " folder at the root of the project. (" +
                (ex.Message) + ")", "Ok");
        }
        return false;
    }

    private static void DecompressFile(string CompressedFileName, string DecompressedFileName)
    {
        var buffer = new byte[1024];
        using (FileStream zipToOpen = new FileStream(CompressedFileName, FileMode.Open))
        {
            using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
            {
                foreach (var entry in archive.Entries)
                {
                    var directoryName = Path.GetDirectoryName(DecompressedFileName + "/" + entry.FullName);
                    Directory.CreateDirectory(directoryName);
                    if (entry.FullName.EndsWith("/")) //We skip because it is a directory
                        continue;
                    using (var file = File.OpenWrite(DecompressedFileName + "/" + entry.FullName))
                    using (var s = entry.Open())
                    {
                        while (s.CanRead)
                        {
                            var read = s.Read(buffer, 0, 1024);
                            if (read == 0)
                                break;
                            file.Write(buffer, 0, read);
                        }
                    }
                }
            }
        }
    }
}