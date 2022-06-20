using myField.Utils;
using UnityEngine;

namespace nextPlace.ARVideoPlayer
{
    public class Billboard : MonoBehaviour
    {
        public float offset = 270;

        [SerializeField]
        private Transform _eye;

        private Transform _t;

        public Transform Eye { get => _eye; set => _eye = value; }

        private void Start()
        {
            if (_eye == null)
                _eye = Camera.main.transform;
            _t = gameObject.transform;
        }

        private void Update()
        {
            var dir = _t.position - _eye.position;
            var ha = Vector3Utils.GetHorizontalAngle(dir);
            var va = Vector3Utils.GetVerticalAngle(dir) + offset;

            _t.rotation = Quaternion.Euler(va, ha, 0);
        }
    }
}