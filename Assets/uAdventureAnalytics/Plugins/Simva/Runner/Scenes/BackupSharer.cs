
using AssetPackage;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace uAdventure.Simva
{
    public class BackupSharer : MonoBehaviour
    {
        public void Share()
        {
            string traces = "";
            string filename = "";

            try
            {
                filename = ((TrackerAssetSettings)TrackerAsset.Instance.Settings).BackupFile;
                traces = SimvaExtension.Instance.SimvaBridge.Load(((TrackerAssetSettings)TrackerAsset.Instance.Settings).BackupFile);
            }
            catch (System.Exception ex){ Debug.Log("Couldn't read traces: " + ex.Message); }

            if (string.IsNullOrEmpty(traces))
            {
                DirectoryInfo info = new DirectoryInfo(Application.temporaryCachePath);
                FileInfo[] files = info.GetFiles().OrderBy(p => p.CreationTime).Reverse().ToArray();
                foreach (FileInfo file in files)
                {
                    traces = File.ReadAllText(file.FullName);
                    filename = file.Name;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(traces))
            {
                string filePath = Path.Combine(Application.temporaryCachePath, filename);
                File.WriteAllBytes(filePath, Encoding.UTF8.GetBytes(traces));

                new NativeShare().AddFile(filePath)
                    .SetSubject("Backup de " + filename).SetText("Backup adjunto")
                    .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
                    .Share();
            }
        }
    }
}
