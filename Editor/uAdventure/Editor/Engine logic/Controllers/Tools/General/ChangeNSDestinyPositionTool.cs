using UnityEngine;
using System.Collections;
using System;

using uAdventure.Core;

namespace uAdventure.Editor
{
    /**
     * Edition tool for changing the destiny position of a NextScene
     */

    public class ChangeNSDestinyPositionTool : ChangePositionTool
    {

        public ChangeNSDestinyPositionTool(Positioned nextScene, int newX, int newY) : base(nextScene, newX, newY)
        {
            this.addListener(new ChangePositionToolListenerImplementation());
        }

        public class ChangePositionToolListenerImplementation : ChangePositionToolListener
        {
            public void positionUpdated(int newX, int newY)
            {
                // TODO: implentation ?
            }
        }
    }
}