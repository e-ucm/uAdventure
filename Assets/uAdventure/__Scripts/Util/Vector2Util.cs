using UnityEngine;

namespace uAdventure.Runner
{
    public static class Vector2Util
    {
        public static Orientation ToOrientation(this Vector2 source, Vector2 target)
        {
            Orientation o = Orientation.S;

            float angle = ToAngle(source, target);

            if (angle >= 45 && angle < 135)
            {
                o = Orientation.N;
            }
            else if (angle >= 135 && angle < 225)
            {
                o = Orientation.O;
            }
            else if (angle >= 225 && angle < 315)
            {
                o = Orientation.S;
            }
            else if (angle >= 315 || angle < 45)
            {
                o = Orientation.E;
            }

            return o;
        }
        public static Orientation ToOrientation(this Vector2 direction, bool onlyHorizontal)
        {
            Orientation o = Orientation.E;
            if (onlyHorizontal)
            {
                float angle = ToAngle(direction);
                if (angle >= 90 && angle < 270)
                {
                    o = Orientation.O;
                }
            } 
            else
            {
                o = ToOrientation(direction);
            }
            
            return o;
        }

        public static Orientation ToOrientation(this Vector2 direction)
        {
            Orientation o = Orientation.S;

            float angle = ToAngle(direction);

            if (angle >= 45 && angle < 135)
            {
                o = Orientation.N;
            }
            else if (angle >= 135 && angle < 225)
            {
                o = Orientation.O;
            }
            else if (angle >= 225 && angle < 315)
            {
                o = Orientation.S;
            }
            else if (angle >= 315 || angle < 45)
            {
                o = Orientation.E;
            }

            return o;
        }
        
        public static float ToAngle(this Vector2 source, Vector2 target)
        {
            return (target - source).ToAngle();
        }

        public static float ToAngle(this Vector2 direction)
        {
            Vector2 horizon = new Vector2(1, 0);

            float angle = Vector2.Angle(horizon, direction);
            Vector3 cross = Vector3.Cross(horizon, direction);

            if (cross.z > 0)
            {
                angle = 360 - angle;
            }

            return angle;
        }
    }
}
