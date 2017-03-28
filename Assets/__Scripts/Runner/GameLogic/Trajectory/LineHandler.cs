using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Runner
{
    public class LineHandler
    {

        public const float DIVISOR = 10f;

        public Trajectory.Node start, end;
        public Trajectory.Side side;

        private float min_x, max_x, min_y, max_y;
        public MathFunction function;

        private MathFunction scale_function;

        private List<LineHandler> neighbours = new List<LineHandler>();

        public static Trajectory.Node commonPoint(LineHandler l1, LineHandler l2)
        {
            if (l1.containsNode(l2.start))
                return l2.start;
            else
                return l2.end;
        }

        public static Vector2 nodeToVector2(Trajectory.Node node)
        {
            return new Vector2(node.getX() / LineHandler.DIVISOR, 60f - node.getY() / LineHandler.DIVISOR);
        }

        public LineHandler(Trajectory.Node start, Trajectory.Node end, Trajectory.Side side)
        {
            this.start = start;
            this.end = end;
            this.side = side;

            float start_x = start.getX() / LineHandler.DIVISOR, end_x = end.getX() / LineHandler.DIVISOR
                , start_y = 60f - start.getY() / LineHandler.DIVISOR, end_y = 60f - end.getY() / LineHandler.DIVISOR;


            min_x = Mathf.Min(start_x, end_x);
            max_x = Mathf.Max(start_x, end_x);

            min_y = Mathf.Min(start_y, end_y);
            max_y = Mathf.Max(start_y, end_y);


            this.function = new MathFunction(new Vector2(start_x, start_y), new Vector2(end_x, end_y));

            this.scale_function = new MathFunction(new Vector2(start_x, start.getScale()), new Vector2(end_x, end.getScale()));
        }

        public Vector2[] contactPoints(Vector2 point)
        {
            return function.contactPoints(point);
        }

        public bool contains(Vector2 v)
        {
            bool ret = false;
            float x = function.getX(v.y), y = function.getY(v.x);


            if (
                (v.x >= min_x && v.x <= max_x)
                &&
                (v.y >= min_y && v.y <= max_y)
                &&
                (v.x >= (x - 0.01) && v.x <= (x + 0.01))
                &&
                (v.y >= (y - 0.01) && v.y <= (y + 0.01))
            )
                ret = true;

            return ret;
        }

        public bool containsNode(Trajectory.Node node)
        {
            return (start == node) || (end == node);
        }

        public Trajectory.Node getOtherPoint(Trajectory.Node point)
        {
            if (start == point)
                return end;
            else if (end == point)
                return start;

            return null;
        }

        public void addNeighbour(LineHandler n)
        {
            this.neighbours.Add(n);
        }

        public bool isNeighbor(LineHandler line)
        {
            return neighbours.Contains(line);
        }

        public LineHandler[] getNeighborLines()
        {
            return neighbours.ToArray();
        }

        /*public LineHandler[] getNeighborLinesSorted(){
                List<LineHandler> n;
                n.Sort (new LineHandlerComparer());

                return n.ToArray ();
            }*/

        public List<Trajectory.Node> neighbor_nodes;
        public Trajectory.Node[] getNeighborNodes()
        {
            if (neighbor_nodes == null)
            {
                neighbor_nodes = new List<Trajectory.Node>();
                foreach (LineHandler line in neighbours)
                {
                    if (line.containsNode(this.start))
                    {
                        neighbor_nodes.Add(line.end);
                    }
                    else
                        neighbor_nodes.Add(line.start);
                }
            }

            return neighbor_nodes.ToArray();
        }

        public float getScaleFor(Vector2 point)
        {
            return scale_function.getY(point.x);
        }
    }
}