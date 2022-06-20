using OpenCvSharp;
using UnityEngine;

namespace nextPlace.ARVideoPlayer
{
    public class QuaternionEKF : IFilter<Quaternion, Quaternion>
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

        private KalmanFilter rotFilter = new KalmanFilter(8, 4);
        private bool _IsInit = true;

        public QuaternionEKF()
        {
            rotFilter.TransitionMatrix.SetArray(0, 0, rotTransition);
            rotFilter.MeasurementMatrix.SetArray(0, 0, rotMeasurement);
            rotFilter.MeasurementNoiseCov.SetIdentity(1e-1);// bis 09.02 1e-3
            rotFilter.ProcessNoiseCov.SetIdentity(1e-5);
            rotFilter.ErrorCovPost.SetIdentity(1);
        }

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
        public void Correct(Quaternion rot)
        {
            rotFilter.Correct(rot.ToMat());
        }

        public Quaternion Predict()
        {
            var rot = rotFilter.Predict();
            return rot.ToQuaternion();
        }

        public Quaternion Update(Quaternion matrix)
        {
            // predict oder postState?
            Predict();
            Correct(matrix);
            //return Predict();
            return rotFilter.StatePost.ToQuaternion();
        }
    }
}