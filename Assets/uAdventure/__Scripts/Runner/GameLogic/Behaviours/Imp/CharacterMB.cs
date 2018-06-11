using UnityEngine;
using uAdventure.Core;
using System.Collections.Generic;

namespace uAdventure.Runner
{

    [RequireComponent(typeof(Representable))]
    [RequireComponent(typeof(Mover))]
    public class CharacterMB : InteractiveElement
    {
        private static readonly int[] availableActions = { Action.CUSTOM, Action.TALK_TO, Action.EXAMINE };
        protected override int[] AvailableActions { get { return availableActions; } }

        private Representable representable;

        protected override void Start()
        {
            representable = GetComponent<Representable>();
            representable.Play("stand");
        }

        protected override Rectangle GetInteractionArea(SceneMB sceneMB)
        {
            var size = representable.Size;
            var context = representable.Context;

            var topLeft = new Vector2(context.getX() - size.x / 2f, context.getY() - size.y);
            Rectangle area = new InfluenceArea((int)topLeft.x - 20, (int)topLeft.y - 20, (int)size.x + 40, (int)size.y + 40);
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
    }
}