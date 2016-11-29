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
            base.setTexture(Atrezzo.RESOURCE_TYPE_IMAGE);
            base.Positionate();
        }

        // Update is called once per frame
        protected override void Update()
        {

        }
    }
}