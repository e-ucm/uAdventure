using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    /**
     * This class holds the data of an exit in eAdventure
     */
    public class Exit : Documented, Rectangle, Positioned, ICloneable
    {

        public const int NO_TRANSITION = 0;

        public const int TOP_TO_BOTTOM = 1;

        public const int BOTTOM_TO_TOP = 2;

        public const int LEFT_TO_RIGHT = 3;

        public const int RIGHT_TO_LEFT = 4;

        public const int FADE_IN = 5;

        /**
         * X position of the upper left corner of the exit
         */
        private int x;

        /**
         * Y position of the upper left corner of the exit
         */
        private int y;

        /**
         * Width of the exit
         */
        private int width;

        /**
         * Height of the exit
         */
        private int height;

        /**
         * Documentation of the exit.
         */
        private string documentation;

        /**
         * List of nextscenes of the exit
         */
        private List<NextScene> nextScenes;

        /**
         * Default exit look (it can exists or not)
         */
        private ExitLook defaultExitLook;

        private bool rectangular;

        private List<Vector2> points;

        private InfluenceArea influenceArea;

        /**
         * Id of the target scene
         */
        private string nextSceneId;

        /**
         * X position in which the player should appear in the new scene
         */
        private int destinyX;

        /**
         * Y position in which the player should appear in the new scene
         */
        private int destinyY;

        /**
         * Conditions of the next scene
         */
        private Conditions conditions;

        /**
         * Effects triggered before exiting the current scene
         */
        private Effects effects;

        /**
         * Post effects triggered after exiting the current scene
         */
        private Effects postEffects;

        private Effects notEffects;

        private bool hasNotEffects;

        private int transitionType;

        private int transitionTime;

        private float destinyScale;

        /**
         * Creates a new Exit
         * 
         * @param x
         *            The horizontal coordinate of the upper left corner of the exit
         * @param y
         *            The vertical coordinate of the upper left corner of the exit
         * @param width
         *            The width of the exit
         * @param height
         *            The height of the exit
         */
        public Exit(bool rectangular, int x, int y, int width, int height)
        {

            this.x = x;
            this.y = y;
            this.destinyScale = float.MinValue;
            this.width = width;
            this.height = height;

            documentation = null;
            points = new List<Vector2>();
            nextScenes = new List<NextScene>();
            this.rectangular = rectangular;
            influenceArea = new InfluenceArea();

            destinyX = int.MinValue;
            destinyY = int.MinValue;
            conditions = new Conditions();
            effects = new Effects();
            postEffects = new Effects();
            notEffects = new Effects();
            hasNotEffects = false;
            transitionType = NO_TRANSITION;
            transitionTime = 0;
            defaultExitLook = new ExitLook();
        }

        public Exit(string targetId) : this(false, 0, 0, 20, 20)
        {
            this.nextSceneId = targetId;
        }

        /**
         * @return the nextSceneId
         */
        public string getNextSceneId()
        {

            return nextSceneId;
        }

        /**
         * @param nextSceneId
         *            the nextSceneId to set
         */
        public void setNextSceneId(string nextSceneId)
        {

            this.nextSceneId = nextSceneId;
        }

        /**
         * @return the destinyX
         */
        public int getDestinyX()
        {

            return destinyX;
        }

        /**
         * @param destinyX
         *            the destinyX to set
         */
        public void setDestinyX(int destinyX)
        {

            this.destinyX = destinyX;
        }

        /**
         * @return the destinyY
         */
        public int getDestinyY()
        {

            return destinyY;
        }

        /**
         * @param destinyY
         *            the destinyY to set
         */
        public void setDestinyY(int destinyY)
        {

            this.destinyY = destinyY;
        }

        /**
         * @return the conditions
         */
        public Conditions getConditions()
        {

            return conditions;
        }

        /**
         * @param conditions
         *            the conditions to set
         */
        public void setConditions(Conditions conditions)
        {

            this.conditions = conditions;
        }

        /**
         * @return the effects
         */
        public Effects getEffects()
        {

            return effects;
        }

        /**
         * @param effects
         *            the effects to set
         */
        public void setEffects(Effects effects)
        {

            this.effects = effects;
        }

        /**
         * @return the postEffects
         */
        public Effects getPostEffects()
        {

            return postEffects;
        }

        /**
         * @param postEffects
         *            the postEffects to set
         */
        public void setPostEffects(Effects postEffects)
        {

            this.postEffects = postEffects;
        }

        /**
         * @return the notEffects
         */
        public Effects getNotEffects()
        {

            return notEffects;
        }

        /**
         * @param notEffects
         *            the notEffects to set
         */
        public void setNotEffects(Effects notEffects)
        {

            this.notEffects = notEffects;
        }

        /**
         * @return the hasNotEffects
         */
        public bool isHasNotEffects()
        {

            return hasNotEffects;
        }

        /**
         * @param hasNotEffects
         *            the hasNotEffects to set
         */
        public void setHasNotEffects(bool hasNotEffects)
        {

            this.hasNotEffects = hasNotEffects;
        }

        /**
         * @return the transitionType
         */
        public int getTransitionType()
        {

            return transitionType;
        }

        /**
         * @param transitionType
         *            the transitionType to set
         */
        public void setTransitionType(int transitionType)
        {

            this.transitionType = transitionType;
        }

        /**
         * @return the transitionTime
         */
        public int getTransitionTime()
        {

            return transitionTime;
        }

        /**
         * @param transitionTime
         *            the transitionTime to set
         */
        public void setTransitionTime(int transitionTime)
        {

            this.transitionTime = transitionTime;
        }

        /**
         * @param height
         *            the height to set
         */
        public void setHeight(int height)
        {

            this.height = height;
        }

        /**
         * @param nextScenes
         *            the nextScenes to set
         */
        public void setNextScenes(List<NextScene> nextScenes)
        {

            this.nextScenes = nextScenes;
        }

        /**
         * Returns the horizontal coordinate of the upper left corner of the exit
         * 
         * @return the horizontal coordinate of the upper left corner of the exit
         */
        public int getX()
        {

            if (rectangular)
                return x;
            else
            {
                int minX = int.MaxValue;
                foreach (Vector2 point in points)
                {
                    if (point.x < minX)
                        minX = (int)point.x;
                }
                return minX;
            }
        }

        /**
         * Returns the horizontal coordinate of the bottom right of the exit
         * 
         * @return the horizontal coordinate of the bottom right of the exit
         */
        public int getY()
        {

            if (rectangular)
                return y;
            else
            {
                int minY = int.MaxValue;
                foreach (Vector2 point in points)
                {
                    if (point.y < minY)
                        minY = (int)point.y;
                }
                return minY;
            }
        }

        /**
         * Returns the width of the exit
         * 
         * @return Width of the exit
         */
        public int getWidth()
        {

            if (rectangular)
                return width;
            else
            {
                int maxX = int.MinValue;
                int minX = int.MaxValue;
                foreach (Vector2 point in points)
                {
                    if (point.x > maxX)
                        maxX = (int)point.x;
                    if (point.x < minX)
                        minX = (int)point.x;
                }
                return maxX - minX;

            }
        }

        /**
         * Returns the height of the exit
         * 
         * @return Height of the exit
         */
        public int getHeight()
        {

            if (rectangular)
                return height;
            else
            {
                int maxY = int.MinValue;
                int minY = int.MaxValue;
                foreach (Vector2 point in points)
                {
                    if (point.y > maxY)
                        maxY = (int)point.y;
                    if (point.y < minY)
                        minY = (int)point.y;
                }
                return maxY - minY;
            }
        }

        /**
         * Set the values of the exit.
         * 
         * @param x
         *            X coordinate of the upper left point
         * @param y
         *            Y coordinate of the upper left point
         * @param width
         *            Width of the exit area
         * @param height
         *            Height of the exit area
         */
        public void setValues(int x, int y, int width, int height)
        {

            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        /**
         * Returns the documentation of the exit.
         * 
         * @return the documentation of the exit
         */
        public string getDocumentation()
        {

            return documentation;
        }

        /**
         * Returns the list of the next scenes from this scene
         * 
         * @return the list of the next scenes from this scene
         */
        public List<NextScene> getNextScenes()
        {

            return nextScenes;
        }

        /**
         * Changes the documentation of this exit.
         * 
         * @param documentation
         *            The new documentation
         */
        public void setDocumentation(string documentation)
        {

            this.documentation = documentation;
        }

        /**
         * Adds a next scene to the list of next scenes
         * 
         * @param nextScene
         *            the next scene to add
         */
        public void addNextScene(NextScene nextScene)
        {

            nextScenes.Add(nextScene);
        }

        /**
         * @return the defaultExitLook
         */
        public ExitLook getDefaultExitLook()
        {

            return defaultExitLook;
        }

        /**
         * @param defaultExitLook
         *            the defaultExitLook to set
         */
        public void setDefaultExitLook(ExitLook defaultExitLook)
        {

            this.defaultExitLook = defaultExitLook;
        }

        /**
         * Returns whether a point is inside the exit
         * 
         * @param x
         *            the horizontal positon
         * @param y
         *            the vertical position
         * @return true if the point (x, y) is inside the exit, false otherwise
         */
        public bool isPointInside(int x, int y)
        {

            if (rectangular)
                return x > getX0() && x < getX1() && y > getY0() && y < getY1();
            else
            {
                return PolygonHelper.ContainsPoint(getPoints(), new Vector2(x, y));
            }
        }

        /**
         * Returns the horizontal coordinate of the upper left corner of the exit
         * 
         * @return the horizontal coordinate of the upper left corner of the exit
         */
        public int getX0()
        {

            return getX();
        }

        /**
         * Returns the vertical coordinate of the upper left corner of the exit
         * 
         * @return the vertical coordinate of the upper left corner of the exit
         */
        public int getX1()
        {

            return getX() + getWidth();
        }

        /**
         * Returns the horizontal coordinate of the bottom right of the exit
         * 
         * @return the horizontal coordinate of the bottom right of the exit
         */
        public int getY0()
        {

            return getY();
        }

        /**
         * Returns the vertical coordinate of the bottom right of the exit
         * 
         * @return the vertical coordinate of the bottom right of the exit
         */
        public int getY1()
        {

            return getY() + getHeight();
        }

        public void setDestinyScale(float destinyScale)
        {
            this.destinyScale = destinyScale;
        }

        public float getDestinyScale()
        {
            return destinyScale;
        }

        /*
@Override
public Object clone() throws CloneNotSupportedException
{

   Exit e = (Exit) super.clone( );
   e.defaultExitLook = ( defaultExitLook != null ? (ExitLook) defaultExitLook.clone( ) : null );
   e.documentation = ( documentation != null ? new string(documentation ) : null );
   e.height = height;
   if( nextScenes != null ) {
       e.nextScenes = new List<NextScene>( );
       for( NextScene ns : nextScenes )
           e.nextScenes.add( (NextScene) ns.clone( ) );
   }
e.influenceArea = ( influenceArea != null ? (InfluenceArea) influenceArea.clone( ) : null );
   e.width = width;
   e.x = x;
   e.y = y;
   e.rectangular = rectangular;
   if( points != null ) {
       e.points = new List<Point>( );
       for( Point p : points )
           e.points.add( (Point) p.clone( ) );
   }
   e.conditions = ( conditions != null ? (Conditions) conditions.clone( ) : null );
   e.effects = ( effects != null ? (Effects) effects.clone( ) : null );
   e.postEffects = ( postEffects != null ? (Effects) postEffects.clone( ) : null );
   e.notEffects = ( notEffects != null ? (Effects) notEffects.clone( ) : null );
   e.destinyX = destinyX;
   e.destinyY = destinyY;
   e.hasNotEffects = hasNotEffects;
   e.nextSceneId = ( nextSceneId != null ? new string(nextSceneId ) : null );
   e.transitionTime = new int(transitionTime );
e.transitionType = new int(transitionType );
   return e;
}*/

        public bool isRectangular()
        {

            return rectangular;
        }

        public void setRectangular(bool rectangular)
        {

            this.rectangular = rectangular;
        }

        public List<Vector2> getPoints()
        {

            return points;
        }

        public void addPoint(Vector2 point)
        {

            points.Add(point);
        }

        public InfluenceArea getInfluenceArea()
        {

            return influenceArea;
        }

        public void setInfluenceArea(InfluenceArea influeceArea)
        {

            this.influenceArea = influeceArea;
        }

        public bool hasPlayerPosition()
        {

            return (destinyX != int.MinValue) && (destinyY != int.MinValue);
        }

        public int getPositionX()
        {

            return destinyX;
        }

        public int getPositionY()
        {

            return destinyY;
        }

        public void setPositionX(int newX)
        {

            this.destinyX = newX;
        }

        public void setPositionY(int newY)
        {

            this.destinyY = newY;
        }

        public object Clone()
        {

            Exit e = (Exit)this.MemberwiseClone();
            e.defaultExitLook = (defaultExitLook != null ? (ExitLook)defaultExitLook.Clone() : null);
            e.documentation = (documentation != null ? documentation : null);
            e.height = height;
            if (nextScenes != null)
            {
                e.nextScenes = new List<NextScene>();
                foreach (NextScene ns in nextScenes)
                    e.nextScenes.Add((NextScene)ns.Clone());
            }
            e.influenceArea = (influenceArea != null ? (InfluenceArea)influenceArea.Clone() : null);
            e.width = width;
            e.x = x;
            e.y = y;
            e.rectangular = rectangular;
            if (points != null)
            {
                e.points = new List<Vector2>();
                foreach (Vector2 p in points)
                    e.points.Add(new Vector2(p.x, p.y));
            }
            e.conditions = (conditions != null ? (Conditions)conditions.Clone() : null);
            e.effects = (effects != null ? (Effects)effects.Clone() : null);
            e.postEffects = (postEffects != null ? (Effects)postEffects.Clone() : null);
            e.notEffects = (notEffects != null ? (Effects)notEffects.Clone() : null);
            e.destinyX = destinyX;
            e.destinyY = destinyY;
            e.hasNotEffects = hasNotEffects;
            e.nextSceneId = (nextSceneId != null ? nextSceneId : null);
            e.transitionTime = transitionTime;
            e.transitionType = transitionType;
            return e;
        }
    }
}