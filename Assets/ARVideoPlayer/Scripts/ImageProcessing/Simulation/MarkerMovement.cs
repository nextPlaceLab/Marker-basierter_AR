using UnityEngine;

namespace nextPlace.ARVideoPlayer
{
    internal class MarkerMovement : MonoBehaviour
    {
        public bool MoveLinear;
        public float freqX;
        public float freqY;
        public float freqZ;

        public float RX;
        public float RY;
        public float RZ;

        public bool MoveAngular;
        public float freqRX;
        public float freqRY;
        public float freqRZ;

        public float FRX;
        public float FRY;
        public float FRZ;

        private Vector3 pos0;
        private Vector3 rot0;

        private void Start()
        {
            pos0 = transform.position;
            rot0 = transform.rotation.eulerAngles;
        }

        private void Update()
        {
            Vector3 pos, rot;
            BoxDistortion(out pos, out rot);
            //SineDistortion(out pos, out rot);

            if (MoveAngular)
                transform.eulerAngles = rot0 + rot;
            if (MoveLinear)
                transform.position = pos0 + pos;
        }

        private void SineDistortion(out Vector3 pos, out Vector3 rot)
        {
            pos = new Vector3(RX * Mathf.Sin(freqX * Time.fixedTime),
                            RY * Mathf.Sin(freqY * Time.fixedTime),
                            RZ * Mathf.Sin(freqZ * Time.fixedTime));
            rot = new Vector3(FRX * Mathf.Sin(freqRX * Time.fixedTime),
                FRY * Mathf.Sin(freqRY * Time.fixedTime),
                FRZ * Mathf.Sin(freqRZ * Time.fixedTime));
        }

        private void BoxDistortion(out Vector3 pos, out Vector3 rot)
        {
            pos = new Vector3((Random.Range(0f, 1f) - 0.5f) * RX,
                            (Random.Range(0f, 1f) - 0.5f) * RY,
                            (Random.Range(0f, 1f) - 0.5f) * RZ);

            rot = new Vector3((Random.Range(0f, 1f) - 0.5f) * FRX,
                            (Random.Range(0f, 1f) - 0.5f) * FRY,
                            (Random.Range(0f, 1f) - 0.5f) * FRZ);

            //Log.Info("pos = {0}, rot = {1}", pos, rot);
        }
    }
}