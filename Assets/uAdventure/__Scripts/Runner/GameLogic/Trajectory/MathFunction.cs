using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Runner
{
    public class MathFunction
    {
        Vector2 v1, v2;
        float z = 0;

        public MathFunction(Vector2 v1, Vector2 v2)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.z = (v2.y - v1.y) / (v2.x - v1.x);
        }

        public float getY(float x)
        {
            return (z * x - z * v1.x + v1.y);
        }

        public float getX(float y)
        {
            return ((1 / z) * y - (1 / z) * v1.y + v1.x);
        }

        public Vector2[] contactPoints(Vector2 point)
        {
            List<Vector2> ret = new List<Vector2>();

            if (point.x <= Mathf.Max(v1.x, v2.x) && point.x >= Mathf.Min(v1.x, v2.x))
                ret.Add(new Vector2(point.x, getY(point.x)));

            if (point.y <= Mathf.Max(v1.y, v2.y) && point.x >= Mathf.Min(v1.y, v2.y))
                ret.Add(new Vector2(getX(point.y), point.y));

            return ret.ToArray();
        }
    }
}