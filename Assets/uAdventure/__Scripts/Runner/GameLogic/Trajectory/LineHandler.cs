using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using uAdventure.Core;

namespace uAdventure.Runner
{
    public class LineHandler
    {
        public Trajectory.Node start, end;
        public Trajectory.Side side;
        public MathFunction function;

        private readonly MathFunction scale_function;

        private readonly List<LineHandler> neighbours = new List<LineHandler>();

        public static Trajectory.Node commonPoint(LineHandler l1, LineHandler l2)
        {
            if (l1.containsNode(l2.start))
                return l2.start;
            else
                return l2.end;
        }

        public static Vector2 nodeToVector2(Trajectory.Node node)
        {
            return new Vector2(node.getX(), node.getY());
        }

        public LineHandler(Trajectory.Node start, Trajectory.Node end, Trajectory.Side side)
        {
            this.start = start;
            this.end = end;
            this.side = side;

            float start_x = start.getX(), end_x = end.getX()
                , start_y = start.getY() , end_y = end.getY();

            this.function = new MathFunction(new Vector2(start_x, start_y), new Vector2(end_x, end_y));

            this.scale_function = new MathFunction(new Vector2(start_x, start.getScale()), new Vector2(end_x, end.getScale()));
        }

        public Vector2[] contactPoints(Vector2 point)
        {
            return function.contactPoints(point);
        }

        public bool contains(Vector2 v)
        {
            var closest = closestPoint(v);
            return (closest - v).sqrMagnitude < 0.001f;
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

        public float getDistance()
        {
            return side.getLength();
        }

        public Vector2 closestPoint(Vector2 P)
        {
            return GetClosestPointOnLineSegment(nodeToVector2(start), nodeToVector2(end), P);
        }

        //http://stackoverflow.com/questions/3120357/get-closest-point-to-a-line
        public static Vector2 GetClosestPointOnLineSegment(Vector2 A, Vector2 B, Vector2 P)
        {
            Vector2 AP = P - A;       //Vector from A to P   
            Vector2 AB = B - A;       //Vector from A to B  

            float magnitudeAB = AB.sqrMagnitude;     //Magnitude of AB vector (it's length squared)     
            float ABAPproduct = Vector2.Dot(AP, AB);    //The DOT product of a_to_p and a_to_b     
            float distance = ABAPproduct / magnitudeAB; //The normalized "distance" from a to your closest point  

            if (distance < 0)     //Check if P projection is over vectorAB     
            {
                return A;
            }
            else if (distance > 1)
            {
                return B;
            }
            else
            {
                return A + AB * distance;
            }
        }
        public static Vector2d GetClosestPointOnLineSegment(Vector2d A, Vector2d B, Vector2d P)
        {
            return GetClosestPointOnLineSegment(A.ToVector2(), B.ToVector2(), P.ToVector2()).ToVector2d();
        }

        public float getSegmentDistance(Vector2 from, Vector2 to)
        {
            return getDistance() * ((to - from).magnitude / (nodeToVector2(end) - nodeToVector2(start)).magnitude);
        }

        public float getScaleFor(Vector2 point)
        {
            return scale_function.getY(point.x);
        }
    }
}