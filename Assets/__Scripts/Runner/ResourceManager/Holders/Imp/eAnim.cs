using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using uAdventure.Core;

namespace uAdventure.Runner
{
    public class eFrame
    {

        private Texture2D image;
        public Texture2D Image
        {
            get { return image; }
            set { image = value; }
        }

        private int duration = 500;
        public int Duration
        {
            get { return duration; }
            set { duration = value; }
        }
    }

    public class eAnim : Resource
    {
        public uAdventure.Core.Animation Animation
        {
            get; protected set;
        }

        public List<eFrame> frames;
        string path;

        public eAnim(string path, ResourceManager.LoadingType type)
        {
            this.path = path;
            frames = new List<eFrame>();
            Animation = Loader.loadAnimation(path, new ResourceManager.ResourceImageLoader());

            foreach (var frame in Animation.getFrames())
            {
                var eframe = new eFrame();
                eframe.Image = ResourceManager.Instance.getImage(frame.getUri());
                eframe.Duration = (int)frame.getTime();
                frames.Add(eframe);
            }
        }

        public bool Loaded()
        {
            return this.frames.Count > 0;
        }
    }
}