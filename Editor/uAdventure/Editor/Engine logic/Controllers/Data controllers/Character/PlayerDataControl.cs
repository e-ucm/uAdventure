using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class PlayerDataControl : NPCDataControl
    {

        /**
           * Contained player data.
           */
        private Player player;

        /**
         * Constructor.
         * 
         * @param player
         *            Contained player data
         */
        public PlayerDataControl(Player player) : base(player)
        {
            this.player = player;
        }


        public override System.Object getContent()
        {

            return player;
        }

        public override bool canBeDeleted()
        {

            return false;
        }


        public override bool canBeMoved()
        {

            return false;
        }


        public override bool canBeRenamed()
        {

            return false;
        }


        public override bool addElement(int type, string id)
        {

            bool elementAdded = false;

            if (type == Controller.RESOURCES && !Controller.Instance.PlayTransparent)
            {
                return base.addElement(type, id);
            }

            return elementAdded;
        }


        public override bool buildResourcesTab()
        {

            return !Controller.Instance.PlayTransparent;
        }


        public override string renameElement(string name)
        {

            return null;
        }



        public override bool canBeDuplicated()
        {

            return false;

        }
    }
}