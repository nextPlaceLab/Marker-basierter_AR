using OpenCvSharp;
using UnityEngine;

namespace nextPlace.ARVideoPlayer
{
    internal static class MatHelper
    {
        public static Mat ToMat(this Quaternion q)
        {
            Mat m = new Mat(4, 1, MatType.CV_32FC1, new Scalar(0));
            for (int i = 0; i < 4; i++)
                m.Set(i, 0, q[i]);

            return m;
        }

        public static Quaternion ToQuaternion(this Mat mat)
        {
            var q = new Quaternion();
            for (int i = 0; i < 4; i++)
                q[i] = mat.Get<float>(i);

            return q;
        }

        public static Mat ToMat(this Vector3 vec)
        {
            Mat m = new Mat(3, 1, MatType.CV_32FC1, new Scalar(0));
            for (int i = 0; i < 3; i++)
                m.Set(i, 0, vec[i]);

            return m;
        }

        public static Vector3 ToVector(this Mat mat)
        {
            var vec = new Vector3();
            for (int i = 0; i < 3; i++)
                vec[i] = mat.Get<float>(i);

            return vec;
        }

        public static Mat Convert(this Matrix4x4 matrix)
        {
            //Mat m = new Mat(new Size(4, 4), MatType.CV_32FC1);
            Mat m = new Mat(16, 1, MatType.CV_32FC1, new Scalar(0));
            //m = new Mat<float>()
            //m.Set(0, matrix);
            int index = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    m.Set(index, 0, matrix[i, j]);
                    //Debug.LogFormat("m{0} = {1}", index, m.Get<float>(index, 0));
                    index++;
                }
            }
            return m;
        }
        public static float[,] ToMatrix(this Mat matrix)
        {
            var m = new float[matrix.Rows,matrix.Cols];
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Cols; j++)
                {
                    m[i, j] = matrix.Get<float>(i, j);
                }
            }
            return m;
        }

        public static Matrix4x4 MatrixFromVector(this Mat matrix)
        {
            Matrix4x4 m = new Matrix4x4();
            int index = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    m[i, j] = matrix.Get<float>(index++, 0);
                }
            }
            return m;
        }
    }
}
