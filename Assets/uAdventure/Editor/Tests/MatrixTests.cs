using System;
using NUnit.Framework;
using UnityEngine;
using uAdventure.Core;

namespace uAdventure.Test
{
    public class MatrixTests
    {
        [Test]
        public void SerializeSingleFlagConditionTest()
        {
            var matrix = Matrix4x4.identity;

            var viewPort = new Rect(400, 200, 800, 600);

            //Camera.main.worldToCameraMatrix

            //matrix = matrix * Matrix4x4.TRS(viewPort.position, Quaternion.identity, viewPort.size);

            matrix = matrix * Matrix4x4.Ortho(0, 300, 300, 0, -1, -3);

            matrix = matrix * Matrix4x4.Inverse(Matrix4x4.TRS(
                new Vector3(150, 150, 1), 
                Quaternion.Euler(0,0,0),
                new Vector3(1, 1, -1)
            ));
            matrix = matrix * Matrix4x4.TRS(new Vector3(150, 150, -1), Quaternion.identity, Vector3.one);

            var point = new Vector3(0, 0, 0);

            point = matrix.MultiplyPoint(point);

            Debug.Log(point);

        }
    }
}
