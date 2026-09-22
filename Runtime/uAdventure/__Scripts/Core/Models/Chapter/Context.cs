using System;
using UnityEngine;

namespace uAdventure.Core
{
    public abstract class Context : Documented, HasTargetId, ICloneable
    {

        /**
         * Id of the element referenced
         */
        [SerializeField]
        private string idTarget;

        /**
         * Conditions for the element to be placed
         */
        [SerializeField]
        private Conditions conditions;

        /**
         * Scale of the referenced element
         */
        [SerializeField]
        private float scale;

        /**
         * Orientation of the referenced element
         */
        [SerializeField]
        private Orientation orientation;

        /**
         * Orientation of the referenced element
         */
        [SerializeField]
        private bool glow;

        /**
         * Documentation of the element reference.
         */
        [SerializeField]
        private string documentation;

        protected Context(string idTarget)
        {
            this.scale = 1;
            this.idTarget = idTarget;
            this.conditions = new Conditions();
            this.orientation = Orientation.S;
            this.glow = false;
        }

        public string TargetId
        {
            get { return idTarget; }
            set { this.idTarget = value; }
        }

        public Conditions Conditions
        {
            get { return conditions; }
            set { this.conditions = value; }
        }

        public string Documentation
        {
            get { return documentation; }
            set { this.documentation = value; }
        }

        public float Scale
        {
            get { return scale; }
            set { this.scale = value; }
        }

        public Orientation Orientation
        {
            get { return orientation; }
            set { this.orientation = value; }
        }

        public bool Glow
        {
            get { return glow; }
            set { this.glow = value; }
        }


        public string getDocumentation()
        {
            return Documentation;
        }

        public string getTargetId()
        {
            return TargetId;
        }

        public void setDocumentation(string documentation)
        {
            Documentation = documentation;
        }

        public void setTargetId(string id)
        {
            TargetId = id;
        }

        public virtual object Clone()
        {
            var context = this.MemberwiseClone() as Context;
            context.conditions = (conditions != null ? (Conditions)conditions.Clone() : null);
            return context;
        }
    }
}
