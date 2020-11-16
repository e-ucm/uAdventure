using uAdventure.Core;
using UnityEngine;

namespace uAdventure.Runner
{
    [RequireComponent(typeof(Area))]
    public class ActiveAreaMB : InteractiveElement
    {
        private static readonly int[] restrictedActions = { Action.CUSTOM, Action.GRAB, Action.EXAMINE, Action.USE };
        
        protected override int[] AvailableActions { get { return restrictedActions; } }

        protected override void OnConditionChanged(string condition, int value)
        {
            var aad = Element as ActiveArea;

            gameObject.SetActive(!aad.IsRemoved() && ConditionChecker.check(aad.getConditions()));
        }

        protected override Rectangle GetInteractionArea(SceneMB sceneMB)
        {
            var aad = Element as ActiveArea;
            var scene = sceneMB.SceneData as Scene;
            Rectangle rect = null;
            if (scene != null && scene.getTrajectory() == null)
            {
                // If no trajectory I have to move the area to the trajectory for it to be connected
                rect = aad.MoveAreaToTrajectory(sceneMB.Trajectory);
            }
            else
            {
                rect = new InfluenceArea(aad.getX() - 20, aad.getY() - 20, aad.getWidth() + 40, aad.getHeight() + 40);
                if (aad.getInfluenceArea() != null && aad.getInfluenceArea().isExists())
                {
                    var points = aad.isRectangular() ? aad.ToRect().ToPoints() : aad.getPoints().ToArray();
                    var topLeft = points.ToRect().position;
                    rect = aad.getInfluenceArea().MoveArea(topLeft);
                }
            }
            return rect;
        }

        protected override Item.BehaviourType GetBehaviourType()
        {
            return (Element as ActiveArea).getBehaviour();
        }
    }
}