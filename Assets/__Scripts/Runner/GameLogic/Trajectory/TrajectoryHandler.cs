using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrajectoryHandler {

	private List<LineHandler> lines = new List<LineHandler>();

	public TrajectoryHandler(Trajectory trajectory){
		lines = new List<LineHandler> ();
		foreach (Trajectory.Side side in trajectory.getSides()) {
			lines.Add (new LineHandler (trajectory.getNodeForId (side.getIDStart ())
				, trajectory.getNodeForId (side.getIDEnd ())
				, side));
		}
		updateNeighbours ();
	}

	private void updateNeighbours(){
		foreach (LineHandler line1 in lines) {
			foreach (LineHandler line2 in lines) {
				if (line1 != line2 && !line1.isNeighbor (line2) && (line1.containsNode(line2.start) || line1.containsNode(line2.end)) ) {
					line1.addNeighbour (line2);
					line2.addNeighbour (line1);
				}
			}
		}
	}

	public Vector2 closestPoint(Vector2 v){
		Vector2 ret = new Vector2(0f,0f);

		float distance = float.MaxValue, current = float.MaxValue;

		LineHandler tmp = lines[0];

		foreach (LineHandler handler in lines) {
			foreach (Vector2 collisions in handler.contactPoints(v)) {
				current = Vector2.Distance (v, collisions);
				if (current < distance) {
					distance = current;
					ret = collisions;
					tmp = handler;
				}
			}
		}

		bool contains = tmp.contains (ret);

		return ret;
	}

	public Trajectory.Node getLastNode(){
		return lines [lines.Count - 1].end;
	}

	public KeyValuePair<Vector2,float>[] route(Vector2 origin, Vector2 destiny){
		List<KeyValuePair<Vector2,float>> ret = new List<KeyValuePair<Vector2,float>> ();

		LineHandler origin_line = null, destiny_line = null;
		foreach (LineHandler handler in lines) {
			if (origin_line == null && handler.contains (origin))
				origin_line = handler;

			if (destiny_line == null && handler.contains (destiny))
				destiny_line = handler;

			if (origin_line != null && destiny_line != null)
				break;
		}

		Vector2 closest = Vector2.zero;
		if (origin_line == null) {
			closest = closestPoint (PlayerMB.Instance.getPosition());
			foreach (LineHandler handler in lines) {
				if (origin_line == null && handler.contains (closest)) {
					origin_line = handler;
					break;
				}
			}
		}

		if (closest != Vector2.zero)
			ret.Add (new KeyValuePair<Vector2,float>(closest,origin_line.getScaleFor(closest)));

		if (origin_line != null && destiny_line != null) {
			//######################################################
			// IF ORIGIN_LINE AND DESTINY_LINE ARE THE SAME
			// Return only the destiny point, dont have to go
			// to other node
			//######################################################
			if (origin_line == destiny_line) {
				ret.Add (new KeyValuePair<Vector2,float>(destiny,destiny_line.getScaleFor(destiny)));
			} else {
				List<KeyValuePair<Vector2,float>> tmpRoute = new List<KeyValuePair<Vector2, float>>(reach (origin_line, destiny_line));
				//tmpRoute.Reverse ();
				ret.AddRange(tmpRoute);
				ret.Add (new KeyValuePair<Vector2,float>(destiny,destiny_line.getScaleFor(destiny)));
			}
		}

		return ret.ToArray ();
	}

	public KeyValuePair<Vector2,float>[] reach(LineHandler origin, LineHandler destiny){
		List<KeyValuePair<Vector2,float>> route = new List<KeyValuePair<Vector2,float>> ();
		//Scene s = (Scene) sd;

		//WE STORE IN A BOOLEAN IF WE HAVE VISITED OR NOT THAT LINE
		//        Dictionary<LineHandler, bool> visited_lines = new Dictionary<LineHandler, bool>();
		//        Dictionary<Trajectory.Node, bool> visited_nodes = new Dictionary<Trajectory.Node, bool>();
		//
		//        foreach (LineHandler line in lines)
		//            visited_lines.Add (line, false);
		//
		//        reach (origin, destiny, visited_lines, route);

		Dictionary<LineHandler, KeyValuePair<float,LineHandler>> stickered = stick (origin);

		LineHandler current = destiny;
		while (current != origin) {
			KeyValuePair<float,LineHandler> node = stickered [current];
			Trajectory.Node tmp = LineHandler.commonPoint (current, node.Value);
			route.Add (new KeyValuePair<Vector2, float>(LineHandler.nodeToVector2 (tmp),tmp.getScale()));
			current = node.Value;
		}

		route.Reverse ();

		return route.ToArray ();
	}

	public Dictionary<LineHandler, KeyValuePair<float,LineHandler>> stick(LineHandler origin){
		Dictionary<LineHandler, KeyValuePair<float,LineHandler>> stickered = new Dictionary<LineHandler, KeyValuePair<float, LineHandler>>();
		Dictionary<LineHandler,float> costs = new Dictionary<LineHandler, float>();
		LineHandler current = origin, previous = origin;

		stickered.Add (origin, new KeyValuePair<float, LineHandler> (0, null));

		float current_cost = 0, total_cost = 0;
		while (stickered.Count < lines.Count) {
			current_cost = current.side.getLength ();
			total_cost = stickered [current].Key;
			foreach (LineHandler line in current.getNeighborLines ()) {
				if(!stickered.ContainsKey(line))
				if (costs.ContainsKey (line)) {
					if (costs [line] > current_cost) {
						costs [line] = current_cost;
					}
				} else {
					costs.Add (line, current_cost);
				}
			}

			//obtenemos la mas corta
			float min = float.MaxValue;
			LineHandler selected = origin;
			foreach (KeyValuePair<LineHandler,float> line in costs) {
				if (line.Value < min) {
					selected = line.Key;
					min = line.Value;
				}
			}

			costs.Remove(selected);

			//establecemos la mas corta como principal y la añadimos a vecinas
			stickered.Add(selected, new KeyValuePair<float, LineHandler>(stickered[previous].Key + current.side.getLength(),current));
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

	public bool reach(LineHandler origin, LineHandler destiny, Dictionary<LineHandler, bool> visited, List<KeyValuePair<Vector2,float>> route){
		bool ret = false;

		if (origin == destiny)
			ret = true;
		else if (visited[origin])
			ret = false;
		else{
			visited [origin] = true;
			foreach (LineHandler neighbor in origin.getNeighborLines()) {
				if (reach (neighbor, destiny, visited, route)) {
					Trajectory.Node point = LineHandler.commonPoint (origin, neighbor);
					route.Add (new KeyValuePair<Vector2,float>(LineHandler.nodeToVector2 (point),point.getScale()));
					ret = true;
					break;
				}
			}
		}

		return ret;

	}
}
