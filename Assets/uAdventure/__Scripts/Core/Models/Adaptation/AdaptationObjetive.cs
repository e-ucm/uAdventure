using System;
using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * This class is used for adaptation sent by LMS, using SCORM data model
     * 
     */
    public class AdaptationObjetive : AdaptedState, ICloneable
    {

        private string objetiveId;

        public AdaptationObjetive() : base()
        {
            objetiveId = string.Empty;
        }

        public AdaptationObjetive(string id) : base()
        {
            objetiveId = string.Empty;
        }

        public string getObjetiveId()
        {

            return objetiveId;
        }

        public void setObjetiveId(string objetiveId)
        {

            this.objetiveId = objetiveId;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            AdaptationObjetive ao = (AdaptationObjetive) super.clone( );
            ao.objetiveId = ( objetiveId != null ? new string(objetiveId ) : null );
            return ao;
        }*/
        public override object Clone()
        {
            AdaptationObjetive ao = (AdaptationObjetive)this.MemberwiseClone();
            ao.objetiveId = (objetiveId != null ? objetiveId : null);
            return ao;
        }
    }
}
