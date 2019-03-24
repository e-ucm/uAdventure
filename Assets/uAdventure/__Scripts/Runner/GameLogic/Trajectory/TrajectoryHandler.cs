using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using Dijkstras;
using System.Linq;

namespace uAdventure.Runner
{
    public class TrajectoryHandler
    {

        private List<LineHandler> lines = new List<LineHandler>();
        private Trajectory trajectory;

        public TrajectoryHandler(Trajectory trajectory)
        {
            if (trajectory == null)
                return;

            this.trajectory = trajectory;

            lines = new List<LineHandler>();
            foreach (Trajectory.Side side in trajectory.getSides())
            {
                lines.Add(new LineHandler(trajectory.getNodeForId(side.getIDStart())
                    , trajectory.getNodeForId(side.getIDEnd())
                    , side));
            }
            updateNeighbours();
        }

        private void updateNeighbours()
        {
            foreach (LineHandler line1 in lines)
            {
                foreach (LineHandler line2 in lines)
                {
                    if (line1 != line2 && !line1.isNeighbor(line2) && (line1.containsNode(line2.start) || line1.containsNode(line2.end)))
                    {
                        line1.addNeighbour(line2);
                        line2.addNeighbour(line1);
                    }
                }
            }
        }

        public Vector2 closestPoint(Vector2 v)
        {
            Vector2 ret = new Vector2(0f, 0f);

            float distance = float.MaxValue, current = float.MaxValue;
            

            foreach (LineHandler handler in lines)
            {
                var closestPointToLine = handler.closestPoint(v);
                current = Vector2.Distance(v, closestPointToLine);
                if (current < distance)
                {
                    distance = current;
                    ret = closestPointToLine;
                }
            }

            return ret;
        }

        public LineHandler containingLine(Vector2 point)
        {
            foreach (LineHandler handler in lines)
            {
                if (handler.contains(point))
                {
                    return handler;
                }
            }

            return null;
        }

        public Trajectory.Node getLastNode()
        {
            return lines[lines.Count - 1].end;
        }

        public Trajectory.Node getInitialNode()
        {
            return trajectory.getInitial();
        }

        public MovementPoint[] route(Vector2 origin, Vector2[] destiny)
        {
            MovementPoint[] bestRoute = null;
            MovementPoint[] r;
            float maxDistance = float.MaxValue;

            foreach(var d in destiny)
            {
                r = route(origin, d);
                var distance = r.Sum(m => m.distance);
                if(distance < maxDistance)
                {
                    bestRoute = r;
                    maxDistance = distance;
                }
            }

            return bestRoute;
        }

        public MovementPoint[] route(Vector2 origin, Vector2 destiny)
        {
            List<MovementPoint> ret = new List<MovementPoint>();

            LineHandler origin_line = containingLine(origin),
                destiny_line = containingLine(destiny);

            Vector2 closest = Vector2.zero;
            if (origin_line == null)
            {
                closest = closestPoint(origin);
                origin_line = containingLine(closest);
            }

            if (destiny_line == null)
            {
                destiny = closestPoint(destiny);
                destiny_line = containingLine(destiny);
            }

            if (closest != Vector2.zero)
                ret.Add(new MovementPoint()
                {
                    destination = closest,
                    scale = origin_line.getScaleFor(closest),
                    distance = (closest - origin).magnitude
                });

            if (origin_line != null && destiny_line != null)
            {
                //######################################################
                // IF ORIGIN_LINE AND DESTINY_LINE ARE THE SAME
                // Return only the destiny point, dont have to go
                // to other node
                //######################################################
                if (origin_line == destiny_line)
                {
                    ret.Add(new MovementPoint()
                    {
                        destination = destiny,
                        scale = destiny_line.getScaleFor(destiny),
                        distance = destiny_line.getSegmentDistance(origin, destiny),

                    });
                }
                else
                {
                    /*List<KeyValuePair<Vector2, float>> tmpRoute = new List<KeyValuePair<Vector2, float>>(reach(origin_line, destiny_line));
                    //tmpRoute.Reverse ();
                    ret.AddRange(tmpRoute);
                    ret.Add(new KeyValuePair<Vector2, float>(destiny, destiny_line.getScaleFor(destiny)));*/

                    ret = route_d(origin, destiny, origin_line, destiny_line);
                }
            }

            return ret.ToArray();
        }

        public KeyValuePair<Vector2, float>[] reach(LineHandler origin, LineHandler destiny)
        {
            List<KeyValuePair<Vector2, float>> route = new List<KeyValuePair<Vector2, float>>();
            //Scene s = (Scene) sd;

            //WE STORE IN A BOOLEAN IF WE HAVE VISITED OR NOT THAT LINE
            //        Dictionary<LineHandler, bool> visited_lines = new Dictionary<LineHandler, bool>();
            //        Dictionary<Trajectory.Node, bool> visited_nodes = new Dictionary<Trajectory.Node, bool>();
            //
            //        foreach (LineHandler line in lines)
            //            visited_lines.Add (line, false);
            //
            //        reach (origin, destiny, visited_lines, route);

            Dictionary<LineHandler, KeyValuePair<float, LineHandler>> stickered = stick(origin);

            LineHandler current = destiny;
            while (current != origin)
            {
                KeyValuePair<float, LineHandler> node = stickered[current];
                Trajectory.Node tmp = LineHandler.commonPoint(current, node.Value);
                route.Add(new KeyValuePair<Vector2, float>(LineHandler.nodeToVector2(tmp), tmp.getScale()));
                current = node.Value;
            }

            route.Reverse();

            return route.ToArray();
        }

        public Dictionary<LineHandler, KeyValuePair<float, LineHandler>> stick(LineHandler origin)
        {
            Dictionary<LineHandler, KeyValuePair<float, LineHandler>> stickered = new Dictionary<LineHandler, KeyValuePair<float, LineHandler>>();
            Dictionary<LineHandler, float> costs = new Dictionary<LineHandler, float>();
            LineHandler current = origin, previous = origin;

            stickered.Add(origin, new KeyValuePair<float, LineHandler>(0, null));

            float current_cost = 0;// TODO why? , total_cost = 0;
            while (stickered.Count < lines.Count)
            {
                current_cost = current.side.getLength();
                //total_cost = stickered[current].Key;
                foreach (LineHandler line in current.getNeighborLines())
                {
                    if (!stickered.ContainsKey(line))
                        if (costs.ContainsKey(line))
                        {
                            if (costs[line] > current_cost)
                            {
                                costs[line] = current_cost;
                            }
                        }
                        else
                        {
                            costs.Add(line, current_cost);
                        }
                }

                //obtenemos la mas corta
                float min = float.MaxValue;
                LineHandler selected = origin;
                foreach (KeyValuePair<LineHandler, float> line in costs)
                {
                    if (line.Value < min)
                    {
                        selected = line.Key;
                        min = line.Value;
                    }
                }

                costs.Remove(selected);

                //establecemos la mas corta como principal y la añadimos a vecinas
                stickered.Add(selected, new KeyValuePair<float, LineHandler>(stickered[previous].Key + current.side.getLength(), current));
                previous = current;
                current = selected;
            }

            return stickered;
        }

        /*public bool reach(Trajectory.Node origin, LineHandler origin_line, LineHandler destiny, Dictionary<Trajectory.Node, bool> visited_nodes,  Dictionary<LineHandler, bool> visited_lines, List<Vector2> route){
            bool reached = true;
            if(visited_nodes[origin] == false && !destiny.containsNode(origin)){
                visited_nodes [origin] = true;
                foreach (LineHandler line in origin_line.getNeighborLines()) {
                    if(visited_lines[line] == false && line.containsNode(origin)){
                        visited_lines[line.Key] = true;
                        if (!visited_nodes[line.Key.start] && reach (line.Key.start, destiny, visited_nodes, visited_lines, route))
                            route.Add (new Vector2(line.Key.start.getX () / 10f, 60 - line.Key.start.getY () / 10f));
                        else if (!visited_nodes[line.Key.end] && reach (line.Key.end, destiny, visited_nodes, visited_lines, route))
                            route.Add (new Vector2(line.Key.end.getX () / 10f, 60 - line.Key.end.getY () / 10f));
                        else
                            reached = false;
                    }
                }
            }
            return reached;
        }*/

        /*public bool reach(Trajectory.Node origin, LineHandler origin_line, LineHandler destiny, Dictionary<Trajectory.Node, bool> visited_nodes,  Dictionary<LineHandler, bool> visited_lines, List<Vector2> route){
            bool reached = true;
            if (!destiny.containsNode (origin)) {
                visited_nodes [origin] = true;
                foreach (LineHandler line in origin_line.getNeighborLines()) {
                    if (!visited_lines [line] && !visited_nodes [line.getOtherPoint (origin)] && reach (line.getOtherPoint (origin), origin_line, destiny, visited_nodes, visited_lines, route)) {
                        Trajectory.Node n = line.getOtherPoint (origin);
                        route.Add (new Vector2(n.getX () / 10f, 60 - n.getY () / 10f));
                    }
                }
            }
            return reached;
        }*/

        public bool reach(LineHandler origin, LineHandler destiny, Dictionary<LineHandler, bool> visited, List<KeyValuePair<Vector2, float>> route)
        {
            bool ret = false;

            if (origin == destiny)
                ret = true;
            else if (visited[origin])
                ret = false;
            else
            {
                visited[origin] = true;
                foreach (LineHandler neighbor in origin.getNeighborLines())
                {
                    if (reach(neighbor, destiny, visited, route))
                    {
                        Trajectory.Node point = LineHandler.commonPoint(origin, neighbor);
                        route.Add(new KeyValuePair<Vector2, float>(LineHandler.nodeToVector2(point), point.getScale()));
                        ret = true;
                        break;
                    }
                }
            }

            return ret;

        }

        public List<MovementPoint> route_d(Vector2 origin, Vector2 destiny, LineHandler originline, LineHandler destinyline)
        {
            Graph<string> g = new Graph<string>();

            Dictionary<string, Dictionary<string, float>> d = new Dictionary<string, Dictionary<string, float>>();
            Dictionary<string, Dictionary<string, MovementPoint>> ps = new Dictionary<string, Dictionary<string, MovementPoint>>();
            foreach (Trajectory.Node n in this.trajectory.getNodes())
            {
                d.Add(n.getID(), new Dictionary<string, float>());
                ps.Add(n.getID(), new Dictionary<string, MovementPoint>());
            }

            foreach (Trajectory.Side s in this.trajectory.getSides())
            {
                var start = trajectory.getNodeForId(s.getIDStart());

                var end = trajectory.getNodeForId(s.getIDEnd());

                d[s.getIDStart()].Add(s.getIDEnd(), s.getLength());
                d[s.getIDEnd()].Add(s.getIDStart(), s.getLength());

                ps[s.getIDStart()].Add(s.getIDEnd(), new MovementPoint()
                {
                    distance = s.getLength(),
                    scale = end.getScale(),
                    destination = new Vector2(end.getX(), end.getY())
                });
                ps[s.getIDEnd()].Add(s.getIDStart(), new MovementPoint()
                {
                    distance = s.getLength(),
                    scale = start.getScale(),
                    destination = new Vector2(start.getX(), start.getY())
                });
            }

            Trajectory.Node no = new Trajectory.Node("origin", Mathf.RoundToInt(origin.x), Mathf.RoundToInt(origin.y), 1f);
            Trajectory.Node nd = new Trajectory.Node("destiny", Mathf.RoundToInt(destiny.x), Mathf.RoundToInt(destiny.y), 1f);

            Vector2 oStartV2 = LineHandler.nodeToVector2(originline.start), oEndV2 = LineHandler.nodeToVector2(originline.end),
                    dStartV2 = LineHandler.nodeToVector2(destinyline.start), dEndV2 = LineHandler.nodeToVector2(destinyline.end);

            float od1 = originline.getSegmentDistance(origin, oStartV2),
                od2 = originline.getSegmentDistance(origin, oEndV2);
            float dd1 = destinyline.getSegmentDistance(destiny, dStartV2),
                dd2 = destinyline.getSegmentDistance(destiny, dEndV2);

            d[originline.start.getID()].Add(no.getID(), od1);
            d[originline.end.getID()].Add(no.getID(), od2);
            d.Add("origin", new Dictionary<string, float>() { { originline.start.getID(), od1 }, { originline.end.getID(), od2 } });

            ps[originline.start.getID()].Add(no.getID(), new MovementPoint()
            {
                distance = od1,
                scale = originline.getScaleFor(origin),
                destination = origin
            });
            ps[originline.end.getID()].Add(no.getID(), new MovementPoint()
            {
                distance = od2,
                scale = originline.getScaleFor(origin),
                destination = origin
            });
            ps.Add("origin", new Dictionary<string, MovementPoint>() {
                { originline.start.getID(), new MovementPoint()
                    {
                        distance = od1,
                        scale = originline.getScaleFor(LineHandler.nodeToVector2(originline.start)),
                        destination = LineHandler.nodeToVector2(originline.start)
                    }
                },
                { originline.end.getID(), new MovementPoint()
                    {
                        distance = od2,
                        scale = originline.getScaleFor(LineHandler.nodeToVector2(originline.end)),
                        destination = LineHandler.nodeToVector2(originline.end)
                    }
                }
            });

            d[destinyline.start.getID()].Add(nd.getID(), dd1);
            d[destinyline.end.getID()].Add(nd.getID(), dd2);
            d.Add("destiny", new Dictionary<string, float>() { { destinyline.start.getID(), dd1 }, { destinyline.end.getID(), dd2 } });
            
            ps[destinyline.start.getID()].Add(nd.getID(), new MovementPoint()
            {
                distance = dd1,
                scale = destinyline.getScaleFor(destiny),
                destination = destiny
            });
            ps[destinyline.end.getID()].Add(nd.getID(), new MovementPoint()
            {
                distance = dd2,
                scale = destinyline.getScaleFor(destiny),
                destination = destiny
            });
            ps.Add("destiny", new Dictionary<string, MovementPoint>() {
                { destinyline.start.getID(), new MovementPoint()
                    {
                        distance = dd1,
                        scale = destinyline.getScaleFor(LineHandler.nodeToVector2(destinyline.start)),
                        destination = LineHandler.nodeToVector2(destinyline.start)
                    }
                },
                { destinyline.end.getID(), new MovementPoint()
                    {
                        distance = dd2,
                        scale = destinyline.getScaleFor(LineHandler.nodeToVector2(destinyline.end)),
                        destination = LineHandler.nodeToVector2(destinyline.end)
                    }
                }
            });

            g.set_vertices(d);

            List<string> l = g.shortest_path("origin", "destiny");
            l.Reverse();

            List<MovementPoint> ret = new List<MovementPoint>();

            string last = "origin";
            foreach(string n in l)
            {
                ret.Add(ps[last][n]);
                last = n;
            }

            return ret;
        }

        public static TrajectoryHandler GetAccessibleTrajectory(Vector2 position, TrajectoryHandler original)
        {
            position = original.closestPoint(position);
            var line = original.containingLine(position);
            if(line == null)
            {
                // Return empty trajectory
                return new TrajectoryHandler(new Trajectory());
            }

            var output = new Trajectory();
            var toOpen = new Queue<LineHandler>();
            var opened = new Dictionary<LineHandler, bool>();
            var added = new Dictionary<string, bool>();

            toOpen.Enqueue(line);
            while (toOpen.Count != 0)
            {
                var current = toOpen.Dequeue();
                opened[current] = true;

                // First we add both nodes and the side
                if (!added.ContainsKey(current.start.getID()))
                {
                    output.addNode(current.start.getID(), current.start.getX(), current.start.getY(), current.start.getScale());
                    added.Add(current.start.getID(), true);
                }
                if (!added.ContainsKey(current.end.getID()))
                {
                    output.addNode(current.end.getID(), current.end.getX(), current.end.getY(), current.end.getScale());
                    added.Add(current.end.getID(), true);
                }
                output.addSide(current.start.getID(), current.end.getID(), (int)current.getDistance());

                // Then we add the neightbor lines to expand
                foreach (var side in current.getNeighborLines())
                {
                    if (opened.ContainsKey(side) && opened[side]) continue;
                    else toOpen.Enqueue(side);
                }
            }

            return new TrajectoryHandler(output);
        }

        public static Trajectory CreateBlockedTrajectory(Trajectory original, Rectangle[] blockingObjects)
        {
            Trajectory r = original;
            
            foreach(var blockingObject in blockingObjects)
            {
                r = CreateBlockedTrajectory(r, blockingObject);
            }

            return r;
        }

        public static Trajectory CreateBlockedTrajectory(Trajectory original, Rectangle blockingObject)
        {
            var trajectory = new Trajectory();

            // Fist we add the nodes
            foreach(var node in original.getNodes())
            {
                if (!Inside(LineHandler.nodeToVector2(node), blockingObject))
                    trajectory.addNode(node.getID(), node.getX(), node.getY(), node.getScale());
            }

            // Then we add the sides
            foreach(var side in original.getSides())
            {
                //Dividing them with the rect
                DivideSideByRect(blockingObject, original, side, trajectory);
            }

            return trajectory;
        }

        public static void DivideSideByRect(Rectangle rect, Trajectory original, Trajectory.Side side, Trajectory outputTrajectory)
        {
            var startNode = original.getNodeForId(side.getIDStart());
            var endNode = original.getNodeForId(side.getIDEnd());

            Math3DFunc scaleFunc = new Math3DFunc(
                new Vector3(startNode.getX(), startNode.getY(), startNode.getScale()),
                new Vector3(  endNode.getX(),   endNode.getY(),   endNode.getScale()));
            Math3DFunc lengthFunc = new Math3DFunc(
                new Vector3(startNode.getX(), startNode.getY(), 0),
                new Vector3(endNode.getX(), endNode.getY(), side.getLength()));

            var startInside = outputTrajectory.getNodeForId(side.getIDStart()) == null;
            var endInside = outputTrajectory.getNodeForId(side.getIDEnd()) == null;

            if (startInside && endInside)
                return;

            Vector2 start = LineHandler.nodeToVector2(startNode),
                end = LineHandler.nodeToVector2(endNode);
            
            Vector2[] intersections;
            if (LineRectangleIntersections(rect, start, end, out intersections))
            {
                if (!startInside)
                {
                    var cs = ClosestPoint(start, intersections);
                    var csId = createRandomNodeId(cs.x, cs.y);
                    outputTrajectory.addNode(csId, (int)cs.x, (int)cs.y, scaleFunc.getZ(cs.x, cs.y));
                    outputTrajectory.addSide(side.getIDStart(), csId, (int)lengthFunc.getZ(cs.x, cs.y));
                }
                if (!endInside)
                {
                    var ce = ClosestPoint(end, intersections);
                    var ceId = createRandomNodeId(ce.x, ce.y);
                    outputTrajectory.addNode(ceId, (int)ce.x, (int)ce.y, scaleFunc.getZ(ce.x, ce.y));
                    outputTrajectory.addSide(side.getIDEnd(), ceId, (int) (side.getLength() - lengthFunc.getZ(ce.x, ce.y)));
                }
            }
            else
            {
                outputTrajectory.addSide(side.getIDStart(), side.getIDEnd(), (int) side.getLength());
            }
        }

        public static string createRandomNodeId(float x, float y)
        {
            return "tmpNode" + (int)x + ";" + (int)y + "_" + Random.Range(0,1000); 
        }

        public static Vector2 ClosestPoint(Vector2 point, Vector2[] points)
        {
            if (points.Length == 0)
                return Vector2.zero;

            float distance;
            float maxDistance = float.MaxValue;
            int r = 0;

            for (int i = 0, end  = points.Length; i < end; ++i)
            {
                distance = (point - points[i]).sqrMagnitude;
                if(distance < maxDistance)
                {
                    maxDistance = distance;
                    r = i;
                }
            }

            return points[r];
        }
        public static bool TrajectoryRectangleIntersections(Rectangle rect, TrajectoryHandler trajectory, out Vector2[] intersections)
        {
            List<Vector2> intersectionsList = new List<Vector2>();

            Vector2[] currentIntersections;
            foreach(var side in trajectory.lines)
            {
                if (LineRectangleIntersections(rect, LineHandler.nodeToVector2(side.start), 
                    LineHandler.nodeToVector2(side.end), out currentIntersections))
                {
                    intersectionsList.AddRange(currentIntersections);
                }
            }

            intersections = intersectionsList.ToArray();

            return intersections.Length > 0;
        }

        public static bool LineRectangleIntersections(Rectangle rect, Vector2 p1, Vector2 p2, out Vector2[] intersections)
        {
            List<Vector2> intersectionsList = new List<Vector2>();
            List<Vector2> points = rect.getPoints();

            if (rect.isRectangular())
            {
                points = new List<Vector2>(4);
                var uRect = new Rect(rect.getX(), rect.getY(), rect.getWidth(), rect.getHeight());
                points.AddRange(uRect.ToPoints());
            }

            Vector2 intersection;
            for (int i = 0, end = points.Count; i < end; ++i)
            {
                if (LineSegmentsIntersection(p1, p2, points[i], points[(i + 1) % end], out intersection))
                {
                    intersectionsList.Add(intersection);
                }
            }

            intersections = intersectionsList.ToArray();

            return intersections.Length > 0;
        }

        private static bool Inside(Vector2 point, Rectangle rect)
        {
            if (rect.isRectangular())
            {
                // - Rectangular inside case
                var aux = point - new Vector2(rect.getX(), rect.getY());
                return aux.x > 0 && aux.y > 0 && aux.x < rect.getWidth() && aux.y < rect.getHeight();
            }
            else
            {
                // - Polygon inside case
                // Move the polygon to make the point rest in the center
                var points = rect.getPoints().ConvertAll(p => p - point); 
                bool inside = false;
                for (int i = 0; i < points.Count; i++)
                {
                    // Check the times it cuts with the axis
                    if (((points[i].y > 0) != (points[(i + 1) % points.Count].y > 0))
                    && ((points[i].y > 0) == (points[i].y * points[(i + 1) % points.Count].x > points[(i + 1) % points.Count].y * points[i].x)))
                        inside = !inside;
                }
                return inside;
            }
        }

        // FROM https://github.com/setchi/Unity-LineSegmentsIntersection/blob/master/Assets/LineSegmentIntersection/Scripts/Math2d.cs
        public static bool LineSegmentsIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector3 p4, out Vector2 intersection)
        {
            intersection = Vector2.zero;

            var d = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);

            if (d == 0.0f)
            {
                return false;
            }

            var u = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;
            var v = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / d;

            if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
            {
                return false;
            }

            intersection.x = p1.x + u * (p2.x - p1.x);
            intersection.y = p1.y + u * (p2.y - p1.y);

            return true;
        }

        private static bool same_sign(float a, float b)
        {
            return ((a * b) >= 0f);
        }

        private class Math3DFunc
        {
            private Vector3 p;
            private Vector3 d;

            public Math3DFunc(Vector3 v1, Vector3 v2)
            {
                p = v1;
                d = v2 - v1;
            }

            public float getX(float t)
            {
                return p.x + d.x * t;
            }

            public float getY(float t)
            {
                return p.y + d.y * t;
            }

            public float getZ(float t)
            {
                return p.z + d.z * t;
            }

            public float getX(float y, float z)
            {
                if(d.y != 0)
                {
                    return (y - p.y) * d.x / d.y + p.x;
                }
                else if(d.z != 0)
                {
                    return (z - p.z) * d.x / d.z + p.x;
                }

                return p.x;
            }

            public float getY(float x, float z)
            {
                if (d.y != 0)
                {
                    return (x - p.x) * d.y / d.x + p.y;
                }
                else if (d.z != 0)
                {
                    return (z - p.z) * d.y / d.z + p.y;
                }

                return p.y;
            }

            public float getZ(float x, float y)
            {
                if (d.y != 0)
                {
                    return (y - p.y) * d.z / d.y + p.z;
                }
                else if (d.x != 0)
                {
                    return (x - p.x) * d.z / d.x + p.z;
                }

                return p.x;
            }


        }
    }
}