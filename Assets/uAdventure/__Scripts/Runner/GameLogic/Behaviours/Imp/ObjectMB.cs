using UnityEngine;
using uAdventure.Core;
using System.Linq;

namespace uAdventure.Runner
{
    [RequireComponent(typeof(Representable))]
    [RequireComponent(typeof(Mover))]
    public class ObjectMB : InteractiveElement
    {
        private static readonly int[] availableActions = { Action.CUSTOM, Action.DRAG_TO, Action.EXAMINE, Action.GRAB, Action.USE };
        protected override int[] AvailableActions { get { return availableActions; } }

        private Representable representable;

		protected override void Start()
        {
            base.Start();
            representable = GetComponent<Representable>();
            OnPointerLeave();
            representable.SetTexture(Item.RESOURCE_TYPE_IMAGE);
        }

        protected override void OnPointerEnter()
        {
            if (representable.Element.getResources().Checked().First().existAsset(Item.RESOURCE_TYPE_IMAGEOVER))
            {
                representable.SetTexture(Item.RESOURCE_TYPE_IMAGEOVER);
            }
        }

        protected override void OnPointerLeave()
        {
            representable.SetTexture(Item.RESOURCE_TYPE_IMAGE);
        }

        protected override Rectangle GetInteractionArea(SceneMB sceneMB)
        {
            var size = representable.Size;
            var context = representable.Context;
            var elemRef = context as ElementReference;

            if (elemRef == null)
            {
                return null;
            }

            var topLeft = new Vector2(elemRef.getX() - size.x / 2f, elemRef.getY() - size.y);
            Rectangle area = new InfluenceArea((int)topLeft.x - 20, (int)topLeft.y - 20, (int) size.x + 40, (int) size.y + 40);
            var scene = sceneMB.SceneData as Scene;
            if (scene != null && scene.getTrajectory() == null)
            {
                // If no trajectory I have to move the area to the trajectory for it to be connected
                area = area.MoveAreaToTrajectory(sceneMB.Trajectory);
            }
            else if (elemRef.getInfluenceArea() != null && elemRef.getInfluenceArea().isExists())
            {
                area = elemRef.getInfluenceArea().MoveArea(topLeft);
            }
            return area;
        }

        protected override Item.BehaviourType GetBehaviourType()
        {
            return (representable.Element as Item).getBehaviour();
        }
    }
}