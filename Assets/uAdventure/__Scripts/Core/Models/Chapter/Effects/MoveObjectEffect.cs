using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    public class MoveObjectEffect : AbstractEffect, HasTargetId
    {
        private string idTarget;

        private int x;

        private int y;

        private float scale;

        private int translateSpeed;

        private int scaleSpeed;

        private bool animated;

        public MoveObjectEffect(string idTarget, int x, int y, float scale, bool animated, int translateSpeed, int scaleSpeed) : base()
        {
            this.idTarget = idTarget;
            this.x = x;
            this.y = y;
            this.scale = scale;
            this.translateSpeed = translateSpeed;
            this.scaleSpeed = scaleSpeed;
            this.animated = animated;
        }

        public override EffectType getType()
        {
            return EffectType.MOVE_OBJECT;
        }

        public string getTargetId()
        {
            return idTarget;
        }

        public void setTargetId(string id)
        {
            idTarget = id;
        }

        public int getX()
        {
            return x;
        }

        public void setX(int x)
        {
            this.x = x;
        }

        public int getY()
        {
            return y;
        }

        public void setY(int y)
        {
            this.y = y;
        }

        public float getScale()
        {
            return scale;
        }

        public void setScale(float scale)
        {
            this.scale = scale;
        }

        public int getTranslateSpeed()
        {
            return translateSpeed;
        }

        public void setTranslateSpeed(int translateSpeed)
        {
            this.translateSpeed = translateSpeed;
        }

        public int getScaleSpeed()
        {
            return scaleSpeed;
        }

        public void setScaleSpeed(int scaleSpeed)
        {
            this.scaleSpeed = scaleSpeed;
        }

        public bool isAnimated()
        {
            return animated;
        }

        public void setAnimated(bool animated)
        {
            this.animated = animated;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {
            MoveObjectEffect coe = (MoveObjectEffect) super.clone( );
            coe.idTarget = ( idTarget != null ? new string(idTarget ) : null );
            coe.scale = scale;
            coe.x = x;
            coe.y = y;
            coe.animated = animated;
            coe.translateSpeed = translateSpeed;
            coe.scaleSpeed = scaleSpeed;
            return coe;
        }*/
    }
}