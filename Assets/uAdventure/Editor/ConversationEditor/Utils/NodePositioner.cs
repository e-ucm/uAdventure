using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class NodePositioner
    {
        float radius;
        float margins = 50;

        int nodes;
        Rect window;

        private NodePositioner()
        {

        }
        public NodePositioner(int nodes, Rect max)
        {
            this.nodes = nodes;
            radius = (Mathf.Min(max.width, max.height) / 2) - margins;
            window = max;

        }

        public Rect getRectFor(int node)
        {
            Rect ret = new Rect();
            if (node == 0)
            {
                ret = new Rect(window.width / 2f - 75f, window.height / 2f - 25f, 150f, 0);
            }
            else
            {
                float angle = (node * (2 * Mathf.PI) / nodes) + Mathf.PI / 2f;
                ret = new Rect((window.width / 2f) - radius * Mathf.Sin(angle) - 75f, (window.height / 2f) - radius * Mathf.Cos(angle) - 25f, 150f, 0);
            }

            return ret;
        }
    }
}