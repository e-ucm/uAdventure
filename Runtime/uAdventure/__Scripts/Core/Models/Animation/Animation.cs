using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace uAdventure.Core
{
    /**
     * This class holds an "animation" data. An animation must have at least a start
     * transition, a end transition and a frame.
     * 
     */
    public class Animation : Documented, HasId, ICloneable
    {

        /**
         * The xml tag for the background music of the animation
         */
        public const string RESOURCE_TYPE_MUSIC = "music";

        public const int ENGINE = 0;

        public const int EDITOR = 1;

        public const int PREVIEW = 2;

        /**
         * Set of frames for the animation
         */
        private List<Frame> frames;

        /**
         * Set of transitions for the animation
         */
        private List<Transition> transitions;

        /**
         * Documentation of the animation
         */
        private string documentation;

        /**
         * Set of resources for the animation
         */
        private List<ResourcesUni> resources;

        /**
         * Id of the animation
         */
        private string id;

        /**
         * bool that indicates that the animation will be used as slides
         */
        private bool slides;

        /**
         * bool that indicates that the animation will use transitions. If true,
         * transitions will be ignored
         */
        private bool useTransitions;

        private int skippedFrames;

        private bool mirror;

        private bool fullscreen;

        private string newSound = null;

        private int soundMaxTime = 1000;

        private int lastSoundFrame = -1;

        private string animationPath;

        // private BufferedImage temp;

        private int temp_w;

        private int temp_h;

        /**
         * Creates a new Animation. By default, adds the given frame to the timeline, along with
         * two blank tranisitions.
         * 
         * @param id
         *            the id of the animation
         * @param frame
         *              The frame to add to the timeline            
         * @param factory
         *              Object used to create the images for the frames
         *     
         */
        public Animation(string id, Frame frame)
        {
            this.id = id;
            resources = new List<ResourcesUni>();
            frames = new List<Frame>();
            transitions = new List<Transition>();
            frames.Add(frame);
            transitions.Add(new Transition());
            transitions.Add(new Transition());
            skippedFrames = 0;
            useTransitions = true;
            slides = false;
        }


        /**
         * Creates a new Animation with a blank frame.
         * Equivalent to Animation (id, new Frame(factory), factory).
         * 
         * @param id
         *            the id of the animation
         * @param factory
         *              Object used to create the images for the frames
         */
        public Animation(string id) :
            this(id, new Frame())
        {
        }

        /**
         * Creates a new Animation with a blank frame with a specific duration.
         * Equivalent to Animation (id, new Frame(factory, time), factory).
         * 
         * @param id
         *            the id of the animation
         * @param time
         *              The duration of the frame to be created.
         * @param factory
         *              Object used to create the images for the frames
         */
        public Animation(string id, int time) :
            this(id, new Frame(time))
        {
        }

        /**
         * Returns the frame at a given position (null if it doesn't exist)
         * 
         * @param i
         *            index of the frame
         * @return frame at given position (null if it doesn't exist)
         */
        public Frame getFrame(int i)
        {

            if (frames.Count <= i || i < 0)
                return null;
            return frames[i];
        }

        /**
         * Returns the transition for a given frame
         * 
         * @param i
         *            index of the frame
         * @return transition for tha frame (null if it doesn't exist)
         */
        public Transition getTranstionForFrame(int i)
        {

            if (frames.Count <= i - 1 || i < 0)
                return null;
            return transitions[i + 1];
        }

        /**
         * Returns the start transition
         * 
         * @return the start transition
         */
        public Transition getStartTransition()
        {

            return transitions[0];
        }

        /**
         * Returns the end transition
         * 
         * @return the end transition
         */
        public Transition getEndTransition()
        {

            return transitions[transitions.Count - 1];
        }

        /**
         * Adds a new frame after the one at the given index. If the index is
         * invalid it adds it at the end. If the frame is null it creates a new one.
         * 
         * @param after
         *            index of the previous frame
         * @param frame
         *            the frame to add (a new one is created if null)
         * @return the added frame
         */
        public Frame addFrame(int after, Frame frame)
        {

            if (after >= frames.Count || after < 0)
                after = frames.Count - 1;
            if (frame == null)
                frame = new Frame();

            frame = (Frame)frame.Clone();

            if (frames.Count == 1 && frames[0].getUri().Equals(""))
            {
                frames.RemoveAt(0);
                frames.Add(frame);
            }
            else
            {
                frames.Insert(after + 1, frame);
                transitions.Insert(after + 2, new Transition());
            }
            return frame;
        }

		/// <summary>
		/// Adds the frame in the end of the animation
		/// </summary>
		/// <returns>The frame.</returns>
		/// <param name="frame">Frame.</param>
		public Frame addFrame(Frame frame){
			frames.Add ((Frame)frame.Clone());
			return frame;
		}

        /**
        * Adds a new frame after the one at the given index. If the index is
        * invalid it adds it at the end. If the frame is null it creates a new one.
        * 
        * @param after
        *            index of the previous frame
        * @param frame
        *            the frame to add (a new one is created if null)
        */
        public void moveLeft(int ind)
        {

            if (ind < frames.Count && ind > 0)
                Swap(frames, ind, ind - 1);
        }

        /**
           * Adds a new frame after the one at the given index. If the index is
           * invalid it adds it at the end. If the frame is null it creates a new one.
           * 
           * @param after
           *            index of the previous frame
           * @param frame
           *            the frame to add (a new one is created if null)
           * @return the added frame
           */
        public void moveRight(int ind)
        {
            if (ind < frames.Count - 1 && ind >= 0)
                Swap(frames, ind, ind + 1);
        }

        /**
         * Removes the frame at a given index from the animation.
         * 
         * @param index
         *            the index of the frame to remove
         */
        public void removeFrame(int index)
        {

            if (frames.Count <= index)
                return;
            frames.RemoveAt(index);
            transitions.RemoveAt(index + 1);
        }



        /**
         * Returns the documentation of the animation
         * 
         * @return the documentation of the animation
         */
        public string getDocumentation()
        {

            return documentation;
        }

        /**
         * Sets the documentation of the animation
         * 
         * @param documentation
         *            The new documentation
         */
        public void setDocumentation(string documentation)
        {

            this.documentation = documentation;
        }

        /**
         * Adds some resources to the list of resources
         * 
         * @param resources
         *            the resources to add
         */
        public void addResources(ResourcesUni resources)
        {

            this.resources.Add(resources);
        }

        /**
         * Returns the animation's id
         * 
         * @return the animation's id
         */
        public string getId()
        {

            return id;
        }

        /**
         * Returns the list of animation frames
         * 
         * @return the list of animation frames
         */
        public List<Frame> getFrames()
        {

            return frames;
        }

        /**
         * Returns the list of animation transitions
         * 
         * @return the list of animation transitions
         */
        public List<Transition> getTransitions()
        {

            return transitions;
        }

        public List<ResourcesUni> getResources()
        {

            return resources;
        }

        public bool isEmptyAnimation()
        {
            return this.id == SpecialAssetPaths.ASSET_EMPTY_ANIMATION.Split('/').Last();
        }

        /**
         * @return the slides
         */
        public bool isSlides()
        {

            return slides;
        }

        /**
         * @param slides
         *            the slides to set
         */
        public void setSlides(bool slides)
        {

            this.slides = slides;
        }

        /**
         * @return the useTransitions
         */
        public bool isUseTransitions()
        {

            return useTransitions;
        }

        /**
         * @param useTransitions
         *            the useTransitions to set
         */
        public void setUseTransitions(bool useTransitions)
        {

            this.useTransitions = useTransitions;
        }

        public string getNewSound()
        {

            string temp = newSound;
            newSound = null;
            return temp;
        }

        public int getSoundMaxTime()
        {

            return soundMaxTime;
        }

        /**
         * Returns true if the animation has already played once.
         * 
         * @param elapsedTime
         *            The time passed for the animation
         * @return True if the animation has already played once.
         */
        public bool finishedFirstTime(long elapsedTime)
        {
            //TODO
            /*
            int temp = skippedFrames;
            for (int i = 0; i < frames.size(); i++)
            {
                if (frames[i].isWaitforclick())
                    temp--;
                if (frames[i].getTime() > elapsedTime || (frames[i].isWaitforclick() && temp < 0 && slides))
                {
                    return false;
                }
                if (i == frames.size() - 1)
                    return true;
                elapsedTime -= frames[i].getTime();
                if (transitions.get(i + 1).getTime() > elapsedTime && useTransitions)
                {
                    return false;
                }
                if (useTransitions)
                    elapsedTime -= transitions.get(i + 1).getTime();
            }*/
            return true;

        }

        /**
         * Returns the total time of the animation (the "waitforclick" property of
         * the frames is ignored)
         * 
         * @return Total time of the animation
         */
        public long getTotalTime()
        {
            //TODO
            /*
            long temp = 0;
            for (int i = 0; i < frames.size(); i++)
            {
                temp += frames[i].getTime();
                if (i < frames.size() - 1 && useTransitions)
                    temp += transitions.get(i + 1).getTime();
            }
            return temp;*/
            return 0;
        }

        public void setMirror(bool mirror)
        {

            this.mirror = mirror;
        }

        /**
         * Returns the time that must pass for the animation to get into the next
         * frame of transition.
         * 
         * @param accumulatedTime
         *            Time that the animation has been playing
         * 
         * @return Time till the next frame or transition
         */
        public long skipFrame(long elapsedTime)
        {

            //elapsedTime = elapsedTime % getTotalTime();
            if (!slides)
                return elapsedTime;

            long tempTime = 0;
            int temp = ++skippedFrames;
            for (int i = 0; i < frames.Count; i++)
            {
                if (frames[i].isWaitforclick())
                    temp--;
                if (frames[i].getTime() > elapsedTime || (frames[i].isWaitforclick() && temp < 1))
                {
                    return tempTime + frames[i].getTime();
                }
                tempTime += frames[i].getTime();
                if (i == frames.Count - 1)
                    return 0;
                elapsedTime -= frames[i].getTime();
                if (transitions[i + 1].getTime() > elapsedTime)
                {
                    skippedFrames--;
                    return tempTime += transitions[i + 1].getTime();
                }
                tempTime += transitions[i + 1].getTime();
                elapsedTime -= transitions[i + 1].getTime();
            }
            skippedFrames = 0;
            return 0;
        }

        public void setFullscreen(bool b)
        {

            this.fullscreen = b;
        }

        public void restart()
        {

            skippedFrames = 0;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            Animation a = (Animation) super.clone( );
            a.documentation = ( documentation != null ? new string(documentation ) : null );
            if( frames != null ) {
                a.frames = new List<Frame>( );
                for( Frame f : frames )
                    a.frames.add( (Frame) f.clone( ) );
            }
    a.fullscreen = fullscreen;
            a.id = ( id != null ? new string(id ) : null );
            a.mirror = mirror;
            if( resources != null ) {
                a.resources = new List<Resources>( );
                for( Resources r : resources )
                    a.resources.add( (Resources) r.clone( ) );
            }
            a.skippedFrames = skippedFrames;
            a.slides = slides;
            if( transitions != null ) {
                a.transitions = new List<Transition>( );
                for( Transition t : transitions )
                    a.transitions.add( (Transition) t.clone( ) );
            }
            a.useTransitions = useTransitions;
            return a;
        }*/

        public void setId(string id)
        {

            this.id = id;
        }

        public void setAbsolutePath(string animationPath)
        {

            this.animationPath = animationPath;
            foreach (Frame frame in frames)
                frame.setAbsolutePath(animationPath);
        }

        public string getAboslutePath()
        {

            return animationPath;
        }

        /***************************************************************/
        // Added for accessibility purposes
        /****************************************************************/
        /**
         * Returns the image in a given moment, or null if the animation has
         * finished.
         * 
         * @param elapsedTime
         *            Time elapsed since the animation began
         * @return The image to draw, in a loop
         */
        public string getDocumentation(long elapsedTime, int where)
        {

            int temp = skippedFrames;

            // check to see if the all the waiting frames have been
            // skipped
            int temp2 = 0;
            for (int i = 0; i < frames.Count; i++)
            {
                if (frames[i].isWaitforclick())
                    temp2++;
            }
            if (!slides || temp2 <= skippedFrames)
                elapsedTime = elapsedTime % getTotalTime();

            for (int i = 0; i < frames.Count; i++)
            {
                if (frames[i].isWaitforclick())
                    temp--;
                if (frames[i].getTime() > elapsedTime || (frames[i].isWaitforclick() && temp < 0 && slides))
                {
                    if (lastSoundFrame != i)
                    {
                        newSound = frames[i].getSoundUri();
                        soundMaxTime = frames[i].getMaxSoundTime();
                        lastSoundFrame = i;
                    }
                    return frames[i].getDocumentation();
                }
                if (i == frames.Count - 1)
                    return null;
                elapsedTime -= frames[i].getTime();
                if (transitions[i + 1].getTime() > elapsedTime && useTransitions)
                {
                    return null;
                }
                if (useTransitions)
                    elapsedTime -= transitions[i + 1].getTime();
            }
            return null;
        }

        public object Clone()
        {
            Animation a = (Animation)this.MemberwiseClone();
            a.documentation = (documentation != null ? documentation : null);
            if (frames != null)
            {
                a.frames = new List<Frame>();
                foreach (Frame f in frames)
                    a.frames.Add((Frame)f.Clone());
            }
            a.fullscreen = fullscreen;
            a.id = (id != null ? id : null);
            a.mirror = mirror;
            if (resources != null)
            {
                a.resources = new List<ResourcesUni>();
                foreach (ResourcesUni r in resources)
                    a.resources.Add((ResourcesUni)r.Clone());
            }
            a.skippedFrames = skippedFrames;
            a.slides = slides;
            if (transitions != null)
            {
                a.transitions = new List<Transition>();
                foreach (Transition t in transitions)
                    a.transitions.Add((Transition)t.Clone());
            }
            a.useTransitions = useTransitions;
            return a;
        }

        public static void Swap(IList<Frame> list, int indexA, int indexB)
        {
            Frame tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }
    }
}