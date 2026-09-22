using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class AddNewPointTool : Tool
    {


        private Rectangle rectangle;

        private Vector2 newPoint;

        private InfluenceAreaDataControl iadc;

        private InfluenceArea oldInfluenceArea;

        private InfluenceArea newInfluenceArea;

        public AddNewPointTool(Rectangle rectangle, int x, int y)
        {

            this.rectangle = rectangle;
            newPoint = new Vector2(x, y);
        }

        public AddNewPointTool(Rectangle rectangle, int x, int y, InfluenceAreaDataControl iadc)
        {

            this.rectangle = rectangle;
            this.iadc = iadc;
            oldInfluenceArea = (InfluenceArea)iadc.getContent();
            newPoint = new Vector2(x, y);
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
            {
                return false;
            }
            rectangle.getPoints().Add(newPoint);

            if (iadc != null)
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


        public override bool redoTool()
        {

            rectangle.getPoints().Add(newPoint);
            if (iadc != null)
            {
                ActiveArea aa = (ActiveArea)rectangle;
                aa.setInfluenceArea(newInfluenceArea);
                iadc.setInfluenceArea(newInfluenceArea);
            }
            Controller.Instance.updatePanel();
            return true;
        }


        public override bool undoTool()
        {

            rectangle.getPoints().Remove(newPoint);
            if (iadc != null)
            {
                ActiveArea aa = (ActiveArea)rectangle;
                aa.setInfluenceArea(oldInfluenceArea);
                iadc.setInfluenceArea(oldInfluenceArea);
            }
            Controller.Instance.updatePanel();
            return true;
        }
    }
}