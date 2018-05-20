using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    /**
     * Utility class that is used to fix consistency problems in trajectories
     */
    public class TrajectoryFixer
    {

        /** If a trajectory has only one node, the player is not displayed. To fix that situation, the node is duplicated with a slightly different location.
         * Both old and new nodes are linked through a side. 
         * 
         * @param scene
         */
        public static void fixSingleNode(Scene scene)
        {
            Trajectory trajectory = scene.getTrajectory();
            if (trajectory != null)
            {
                if (trajectory.getNodes().Count == 1)
                {
                    Trajectory.Node node1 = trajectory.getNodes()[0];
                    trajectory.addNode(node1.getID() + "Dupl", node1.getX() + 1, node1.getY(), node1.getScale());
                    trajectory.addSide(node1.getID(), node1.getID() + "Dupl", 1);
                }
            }
        }


        /**
         * Checks consistency between nodes and references in sides. If a node is referenced to but does not exist, it is created on position 400,300.
         * @param scene
         */
        private static void fixMissingNodes(Scene scene)
        {
            Trajectory trajectory = scene.getTrajectory();
            if (trajectory != null)
            {
                List<string> nodeIds = new List<string>();
                // Put all node ids referenced in sides in a single array list
                foreach (Trajectory.Side side in trajectory.getSides())
                {
                    if (!nodeIds.Contains(side.getIDStart()))
                    {
                        nodeIds.Add(side.getIDStart());
                    }
                    if (!nodeIds.Contains(side.getIDEnd()))
                    {
                        nodeIds.Add(side.getIDEnd());
                    }
                }

                // Remove all ids belonging to an existing nodes. The result is a list containing the strings of
                // all missing nodes
                foreach (Trajectory.Node node in trajectory.getNodes())
                {
                    if (nodeIds.Contains(node.getID()))
                    {
                        nodeIds.Remove(node.getID());
                    }
                }

                // Add all missing nodes to the trajectory
                foreach (string missingNodeId in nodeIds)
                {
                    if (trajectory.getNodes().Count > 0)
                    {
                        trajectory.addNode(missingNodeId, 400, 300, 1);
                    }
                }
            }

        }

        /**
         * Ensures that all nodes in a trajectory have a different location. If two nodes are found in the same position, first's x coordinate
         * is incremented by 1 px. This is important as nodes with equal position makes the trajectory algorithm to crash.
         * @param scene
         */
        private static void fixNodesWithSameLocation(Scene scene)
        {
            Trajectory trajectory = scene.getTrajectory();
            if (trajectory != null)
            {
                // Iterate through nodes. 
                for (int i = 0; i < trajectory.getNodes().Count; i++)
                {
                    Trajectory.Node node1 = trajectory.getNodes()[i];
                    for (int j = 0; j < trajectory.getNodes().Count; j++)
                    {
                        Trajectory.Node node2 = trajectory.getNodes()[j];
                        if (i != j && node1.getX() == node2.getX() && node1.getY() == node2.getY())
                        {
                            node1.setValues(node1.getX() + 1, node1.getY(), node1.getScale());
                            j = 0;
                        }
                    }

                }

            }
        }

        /**
         * Checks pairs of nodes with duplicate ids. One of the nodes is removed.
         * @param nodes
         */
        private static void fixDuplicateNodes(Scene scene)
        {
            Trajectory trajectory = scene.getTrajectory();
            if (trajectory != null)
            {
                // Iterate through nodes. 
                for (int i = 0; i < trajectory.getNodes().Count; i++)
                {
                    Trajectory.Node node1 = trajectory.getNodes()[i];
                    for (int j = 0; j < trajectory.getNodes().Count; j++)
                    {
                        Trajectory.Node node2 = trajectory.getNodes()[j];
                        if (i != j && node1.getID().Equals(node2.getID()))
                        {
                            trajectory.getNodes().RemoveAt(j);
                            i--; j--;
                        }
                    }

                }
            }

        }

        /**
         * Invokes all possible checks for the trajectory
         * @param scene
         */
        public static void fixTrajectory(Scene scene)
        {
            fixSingleNode(scene);
            fixDuplicateNodes(scene);
            fixMissingNodes(scene);
            fixNodesWithSameLocation(scene);
        }
    }
}