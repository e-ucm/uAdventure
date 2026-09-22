using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Runner
{
    public class AtrezzoMB : Representable
    {
        protected override void Start()
        {
            base.Start();
            base.SetTexture(Atrezzo.RESOURCE_TYPE_IMAGE);
            base.Adaptate();

            // Disable any kind of colliders

            var collider = GetComponent<Collider>();
            if (collider)
            {
                collider.enabled = false;
            }

            var collider2d = GetComponent<Collider2D>();
            if (collider2d)
            {
                collider2d.enabled = false;
            }

        }
    }
}