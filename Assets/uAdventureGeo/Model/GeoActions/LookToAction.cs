using UnityEngine;
using System.Collections;
using System;

namespace uAdventure.Geo
{
    public class LookToAction : GeoAction
    {
        private readonly string[] parameters = {"Inside", "Center", "Direction"};

        public LookToAction() : base()
        {
            Inside = true;
            Center = true;
            Direction = Vector2.right;
        }


        public override string Name
        {
            get
            {
                return "LookTo";
            }
        }


        public bool Inside
        {
            get { return (bool)this["Inside"]; }
            set { this["Inside"] = value; }
        }
        public bool Center
        {
            get { return (bool)this["Center"]; }
            set { this["Center"] = value; }
        }
        public Vector2 Direction
        {
            get { return (Vector2)this["Direction"]; }
            set { this["Direction"] = value; }
        }

        public override string[] Parameters
        {
            get { return parameters; }
        }

        public override object Clone()
        {
            return base.Clone();
        }
    }
}