using UnityEngine;
using uAdventure.Core;

namespace uAdventure.Runner
{
    [RequireComponent(typeof(Representable))]
    public class ObjectMB : InteractiveElement
    {
        private static readonly int[] availableActions = { Action.CUSTOM, Action.DRAG_TO, Action.EXAMINE, Action.GRAB, Action.USE };
        protected override int[] AvailableActions { get { return availableActions; } }

        private Representable representable;

        protected void Start()
        {
            representable = GetComponent<Representable>();
            representable.SetTexture(Item.RESOURCE_TYPE_IMAGE);
        }

        protected override Rectangle GetInteractionArea(SceneMB sceneMB)
        {
            var size = representable.Size;
            var context = representable.Context;

            var topLeft = new Vector2(context.getX() - size.x / 2f, context.getY() - size.y);
            Rectangle area = new InfluenceArea((int)topLeft.x - 20, (int)topLeft.y - 20, (int) size.x + 40, (int) size.y + 40);
            var scene = sceneMB.SceneData as Scene;
            if (scene != null && scene.getTrajectory() == null)
            {
                // If no trajectory I have to move the area to the trajectory for it to be connected
                area = area.MoveAreaToTrajectory(sceneMB.Trajectory);
            }
            else if (context.getInfluenceArea() != null && context.getInfluenceArea().isExists())
            {
                area = context.getInfluenceArea().MoveArea(topLeft);
            }
            return area;
        }

        protected override Item.BehaviourType GetBehaviourType()
        {
            return (representable.Element as Item).getBehaviour();
        }
    }
}