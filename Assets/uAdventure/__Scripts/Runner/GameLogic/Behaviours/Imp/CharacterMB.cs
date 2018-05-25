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
            var context = representable.Context;
            var texture = representable.Texture;
            var scene = sceneMB.sceneData as Scene;
            var topLeft = new Vector2(context.getX() - texture.width / 2f, context.getY() - texture.height);
            Rectangle area = new InfluenceArea((int)topLeft.x - 20, (int)topLeft.y - 20, texture.width + 40, texture.height + 40);
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