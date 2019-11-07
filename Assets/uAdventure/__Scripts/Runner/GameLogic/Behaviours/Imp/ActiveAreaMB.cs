using uAdventure.Core;
using UnityEngine;

namespace uAdventure.Runner
{
    [RequireComponent(typeof(Area))]
    public class ActiveAreaMB : InteractiveElement
    {
        private static readonly int[] restrictedActions = { Action.CUSTOM, Action.EXAMINE, Action.USE };

        protected override int[] AvailableActions { get { return restrictedActions; } }

        protected override Rectangle GetInteractionArea(SceneMB sceneMB)
        {
            var aad = Element as ActiveArea;
            var scene = sceneMB.SceneData as Scene;
            Rectangle area = null;
            if (scene != null && scene.getTrajectory() == null)
            {
                // If no trajectory I have to move the area to the trajectory for it to be connected
                area = aad.MoveAreaToTrajectory(sceneMB.Trajectory);
            }
            else
            {
                area = new InfluenceArea(aad.getX() - 20, aad.getY() - 20, aad.getWidth() + 40, aad.getHeight() + 40);
                if (aad.getInfluenceArea() != null && aad.getInfluenceArea().isExists())
                {
                    var points = aad.isRectangular() ? aad.ToRect().ToPoints() : aad.getPoints().ToArray();
                    var topLeft = points.ToRect().position;
                    area = aad.getInfluenceArea().MoveArea(topLeft);
                }
            }
            return area;
        }

        protected override Item.BehaviourType GetBehaviourType()
        {
            return (Element as ActiveArea).getBehaviour();
        }
    }
}