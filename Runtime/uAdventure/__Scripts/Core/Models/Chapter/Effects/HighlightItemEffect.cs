using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    public class HighlightItemEffect : AbstractEffect, HasTargetId
    {

        public const int NO_HIGHLIGHT = 0;

        public const int HIGHLIGHT_BLUE = 1;

        public const int HIGHLIGHT_RED = 2;

        public const int HIGHLIGHT_GREEN = 3;

        public const int HIGHLIGHT_BORDER = 4;

        private string idTarget;

        private int highlightType;

        private bool highlightAnimated;

        public HighlightItemEffect(string idTarget, int type, bool animated) : base()
        {
            this.idTarget = idTarget;
            highlightType = type;
            highlightAnimated = animated;
        }

        public override EffectType getType()
        {
            return EffectType.HIGHLIGHT_ITEM;
        }

        public int getHighlightType()
        {
            return highlightType;
        }

        public void setHighlightType(int highlightType)
        {
            this.highlightType = highlightType;
        }

        public bool isHighlightAnimated()
        {
            return highlightAnimated;
        }

        public void setHighlightAnimated(bool highlightAnimated)
        {
            this.highlightAnimated = highlightAnimated;
        }

        public string getTargetId()
        {
            return idTarget;
        }

        public void setTargetId(string id)
        {
            idTarget = id;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {
            HighlightItemEffect coe = (HighlightItemEffect) super.clone( );
            coe.idTarget = ( idTarget != null ? new string(idTarget ) : null );
            coe.highlightType = highlightType;
            coe.highlightAnimated = highlightAnimated;
            return coe;
        }*/

    }
}