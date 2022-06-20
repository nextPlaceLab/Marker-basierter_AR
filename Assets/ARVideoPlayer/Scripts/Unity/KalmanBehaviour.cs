using System;
using UnityEngine;
using UnityEngine.Events;

namespace nextPlace.ARVideoPlayer
{
    public class KalmanBehaviour : MonoBehaviour
    {
        public FilteredPose pose;

        public float MeasureNoise = 1e-3f;
        public float ProcessNoise = 1e-3f;

        public event Action<Vector3> onPlayerCreated;
        private void Start()
        {
            onPlayerCreated += (v) => { Log.Info("Event called 1 -> vec = {0}", v); };
            onPlayerCreated += (v) => { Log.Info("Event called 2 -> vec = {0}", v); };
        }

        [ContextMenu("Test event")]
        private void testEvent()
        {
            onPlayerCreated.Invoke(new Vector3(1, 2, 3));
        }

        private void OnValidate()
        {
            pose = FilteredPose.current;
            if (pose != null)
            {
                pose.SetMeasurementNoise(MeasureNoise);
                pose.SetProcessNoise(ProcessNoise);
            }
        }
    }
}