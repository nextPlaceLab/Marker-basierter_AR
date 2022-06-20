using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace nextPlace.ARVideoPlayer
{
    class OpenCVUtils
    {
        public static Matrix4x4 TransformToWorldSpace(Matrix4x4 openCVTransform)
        {
            Matrix4x4 matrixY = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, -1, 1));
            Matrix4x4 matrixZ = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, 1, -1));
            Matrix4x4 matrix = matrixY * openCVTransform * matrixZ;
            return matrix;
        }
    }
}
