using UnityEngine;
using System.Collections.Generic;
using uAdventure.Core;

namespace uAdventure.Runner
{
    public class eFrame
    {

        public Texture2D Image { get; set; }

        public int Duration { get; set; }

        public eFrame()
        {
            Duration = 500;
        }
    }

    public class eAnim : Resource
    {
        public uAdventure.Core.Animation Animation { get; protected set; }

        public List<eFrame> frames;

        public eAnim(string path, ResourceManager.LoadingType type)
        {
            frames = new List<eFrame>();
            var incidences = new List<Incidence>();
            Animation = Loader.LoadAnimation(path, Game.Instance.ResourceManager, incidences);

            foreach (var frame in Animation.getFrames())
            {
                var eframe = new eFrame();
                eframe.Image = Game.Instance.ResourceManager.getImage(frame.getUri());
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