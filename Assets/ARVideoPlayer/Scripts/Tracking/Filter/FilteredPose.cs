using OpenCvSharp;
using UnityEngine;

namespace nextPlace.ARVideoPlayer
{
    public interface IFilter<T0, T1>
    {
        void Correct(T0 measurement);

        T1 Predict();

        T1 Update(T0 measurement);
    }

    public interface IPoseFilter : IFilter<Matrix4x4, Matrix4x4> { }

    public class FilteredPose : IPoseFilter
    {
        private float[] rotTransition = {
            1, 0, 0, 0, 1, 0, 0, 0,
            0, 1, 0, 0, 0, 1, 0, 0,
            0, 0, 1, 0, 0, 0, 1, 0,
            0, 0, 0, 1, 0, 0, 0, 1,
            0, 0, 0, 0, 1, 0, 0, 0,
            0, 0, 0, 0, 0, 1, 0, 0,
            0, 0, 0, 0, 0, 0, 1, 0,
            0, 0, 0, 0, 0, 0, 0, 1
        };
        private float[] rotMeasurement = {
            1, 0, 0, 0, 0, 0, 0, 0,
            0, 1, 0, 0, 0, 0, 0, 0,
            0, 0, 1, 0, 0, 0, 0, 0,
            0, 0, 0, 1, 0, 0, 0, 0
        };

        private float[] posTransition = {
            1, 0, 0, 1, 0, 0,
            0, 1, 0, 0, 1, 0,
            0, 0, 1, 0, 0, 1,
            0, 0, 0, 1, 0, 0,
            0, 0, 0, 0, 1, 0,
            0, 0, 0, 0, 0, 1
        };
        private float[] posMeasurement = {
            1, 0, 0, 0, 0, 0,
            0, 1, 0, 0, 0, 0,
            0, 0, 1, 0, 0, 0,
        };

        private KalmanFilter rotFilter = new KalmanFilter(8, 4);
        private KalmanFilter posFilter = new KalmanFilter(6, 3);
        private bool _IsInit = true;

        public FilteredPose()
        {
            rotFilter.TransitionMatrix.SetArray(0, 0, rotTransition);
            //var m = rotFilter.TransitionMatrix.ToMatrix();
            //Debug.Log("m =" + m);
            //rotFilter.MeasurementMatrix.SetIdentity(1);
            rotFilter.MeasurementMatrix.SetArray(0,0,rotMeasurement);
            rotFilter.MeasurementNoiseCov.SetIdentity(1e-3);
            rotFilter.ProcessNoiseCov.SetIdentity(1e-5);
            rotFilter.ErrorCovPost.SetIdentity(1);

            //posFilter.TransitionMatrix.SetIdentity(1);
            posFilter.TransitionMatrix.SetArray(0, 0, posTransition);
            //posFilter.MeasurementMatrix.SetIdentity(1);
            posFilter.MeasurementMatrix.SetArray(0,0,posMeasurement);
            posFilter.MeasurementNoiseCov.SetIdentity(1e-4);
            posFilter.ProcessNoiseCov.SetIdentity(1e-5);
            posFilter.ErrorCovPost.SetIdentity(1);
            current = this;
        }
        public static FilteredPose current;
        ///-----------------------
        public void SetMeasurementNoise(float val)
        {
            rotFilter.MeasurementNoiseCov.SetIdentity(val);
        }

        public void SetProcessNoise(float val)
        {
            rotFilter.ProcessNoiseCov.SetIdentity(val);
        }

        ///-----------------------

        public void Correct(Matrix4x4 matrix)
        {
            var rot = MatrixHelper.GetQuaternion(matrix);
            var pos = MatrixHelper.GetPosition(matrix);

            rotFilter.Correct(rot.ToMat());
            posFilter.Correct(pos.ToMat());
        }

        public Matrix4x4 Predict()
        {
            var rot = rotFilter.Predict();
            var pos = posFilter.Predict();
            return MatrixHelper.GetMatrix(rot.ToQuaternion(), pos.ToVector());
        }

        public Matrix4x4 Update(Matrix4x4 matrix)
        {
            // predict oder postState?
            Predict();
            Correct(matrix);
            //return Predict();
            return MatrixHelper.GetMatrix(rotFilter.StatePost.ToQuaternion(), posFilter.StatePost.ToVector());
        }
    }

    public class FilteredPoseNaiv : IPoseFilter
    {
        // Kalman filtered Pose
        private float[] A = new float[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

        private KalmanFilter k = new KalmanFilter(16, 16);

        public FilteredPoseNaiv()
        {
            //k.TransitionMatrix.SetIdentity(1);
            //k.MeasurementMatrix.SetIdentity(1);
            //k.MeasurementNoiseCov.SetIdentity(1e-1);
            //k.ProcessNoiseCov.SetIdentity(1e-5);
            //k.ErrorCovPost.SetIdentity(1);

            k.TransitionMatrix.SetIdentity(1);
            k.MeasurementMatrix.SetIdentity(1);
            k.MeasurementNoiseCov.SetIdentity(1e-3);
            k.ProcessNoiseCov.SetIdentity(1e-5);
            k.ErrorCovPost.SetIdentity(1);
        }

        public void Correct(Matrix4x4 matrix)
        {
            var m = MatHelper.Convert(matrix);
            k.Correct(m);
        }

        public Matrix4x4 Predict()
        {
            // predict oder postState?
            return MatHelper.MatrixFromVector(k.Predict());
        }

        public Matrix4x4 Update(Matrix4x4 matrix)
        {
            Correct(matrix);
            return Predict();
        }

        //private Matrix4x4 Kalman(Matrix4x4 matrix)
        //{
        //    var m = Convert(matrix);
        //    k.Correct(m);
        //    var m0 = Convert(k.Predict());
        //    //var m0 = Convert(m);
        //    return m0;
        //}
    }
}