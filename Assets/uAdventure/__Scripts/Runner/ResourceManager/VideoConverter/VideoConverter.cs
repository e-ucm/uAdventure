using UnityEngine;
using System.Collections;
using System.Diagnostics;

namespace uAdventure.Runner
{
    public class VideoConverter
    {

        public VideoConverter()
        {


        }

        public void Convert(string video)
        {

            string path = " -i \"" + video + "\" -codec:v libtheora -qscale:v 7 -codec:a libvorbis -qscale:a 5 \"" + video.Remove(video.Length - 4, 4) + ".ogv\"";
            Process foo = new Process();
            foo.StartInfo.FileName = System.IO.Directory.GetCurrentDirectory() + "/ffmpeg/ffmpeg.exe";
            foo.StartInfo.Arguments = path;

            foo.StartInfo.RedirectStandardOutput = true;
            foo.StartInfo.UseShellExecute = false;
            foo.StartInfo.CreateNoWindow = true;
            foo.Start();
            
            if(Application.platform != RuntimePlatform.WebGLPlayer)
            {
                while (!foo.HasExited) { }
            }
        }
    }
}