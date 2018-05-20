using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class DeletePointTool : Tool
    {
        private Rectangle rectangle;

        private Vector2 oldPoint;

        private int oldIndex;

        private InfluenceAreaDataControl iadc;

        private InfluenceArea oldInfluenceArea;

        private InfluenceArea newInfluenceArea;

        public DeletePointTool(Rectangle rectangle, Vector2 point)
        {

            this.rectangle = rectangle;
            this.oldPoint = point;
            this.oldIndex = rectangle.getPoints().IndexOf(point);
        }

        public DeletePointTool(Rectangle rectangle, Vector2 point, InfluenceAreaDataControl iadc)
        {

            this.rectangle = rectangle;
            this.oldPoint = point;
            this.oldIndex = rectangle.getPoints().IndexOf(point);
            this.iadc = iadc;
            oldInfluenceArea = (InfluenceArea)iadc.getContent();
        }

        public override bool canRedo()
        {

            return true;
        }


        public override bool canUndo()
        {

            return true;
        }


        public override bool combine(Tool other)
        {

            return false;
        }


        public override bool doTool()
        {

            if (rectangle.isRectangular())
                return false;
            if (rectangle.getPoints().Contains(oldPoint))
            {
                rectangle.getPoints().Remove(oldPoint);

                if (iadc != null && rectangle.getPoints().Count > 3)
                {
                    int minX = int.MaxValue;
                    int minY = int.MaxValue;
                    int maxX = 0;
                    int maxY = 0;
                    foreach (Vector2 point in rectangle.getPoints())
                    {
                        if (point.x > maxX)
                            maxX = (int)point.x;
                        if (point.x < minX)
                            minX = (int)point.x;
                        if (point.y > maxY)
                            maxY = (int)point.y;
                        if (point.y < minY)
                            minY = (int)point.y;
                    }
                    newInfluenceArea = new InfluenceArea();
                    newInfluenceArea.setX(-20);
                    newInfluenceArea.setY(-20);
                    newInfluenceArea.setHeight(maxY - minY + 40);
                    newInfluenceArea.setWidth(maxX - minX + 40);

                    ActiveArea aa = (ActiveArea)rectangle;
                    aa.setInfluenceArea(newInfluenceArea);
                    iadc.setInfluenceArea(newInfluenceArea);
                }

                return true;
            }
            return false;
        }


        public override bool redoTool()
        {

            if (rectangle.getPoints().Contains(oldPoint))
            {
                rectangle.getPoints().Remove(oldPoint);
                if (iadc != null)
                {
                    ActiveArea aa = (ActiveArea)rectangle;
                    aa.setInfluenceArea(newInfluenceArea);
                    iadc.setInfluenceArea(newInfluenceArea);
                }
                Controller.Instance.reloadPanel();
                return true;
            }
            return false;
        }


        public override bool undoTool()
        {

            rectangle.getPoints().Insert(oldIndex, oldPoint);
            if (iadc != null)
            {
                ActiveArea aa = (ActiveArea)rectangle;
                aa.setInfluenceArea(oldInfluenceArea);
                iadc.setInfluenceArea(oldInfluenceArea);
            }
            Controller.Instance.reloadPanel();
            return true;
        }
    }
}