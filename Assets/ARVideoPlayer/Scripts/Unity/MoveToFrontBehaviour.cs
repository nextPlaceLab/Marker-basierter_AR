using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace nextPlace.ARVideoPlayer
{
    internal class MoveToFrontBehaviour : MonoBehaviour, IPointerClickHandler
    {
        public Camera Camera;
        public MarkerSession MarkerSession;

        public bool _enabled = true;

        [SerializeField]
        private WebLinkBehavior webLinkBehavior;

        [SerializeField]
        private Canvas videoControlCanvas;

        [SerializeField]
        private AnimationCurve _speedCurve;

        public bool IsInFront { get; private set; } = false;

        private bool _IsMoving = false;

        private GameObject _defaultParent;
        private Vector3 orgPosition;
        private Quaternion orgRotation;

        private Vector3 dstPosition;
        private bool surveyDone;

        private void Start()

        {
            webLinkBehavior.URL = MarkerSession.Prefab.Preset.SurveyURL;

            _defaultParent = transform.parent.gameObject;
            orgPosition = transform.localPosition;
            orgRotation = transform.localRotation;
        }

        private void UpdateTargetPosition()
        {
            Vector3 objectSize = Vector3.Scale(transform.localScale, GetComponent<MeshFilter>().mesh.bounds.size);

            var distanceZ = objectSize.z * 0.5f / Mathf.Tan(Camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            var distanceX = objectSize.x / Camera.aspect * 0.5f / Mathf.Tan(Camera.fieldOfView * 0.5f * Mathf.Deg2Rad);

            var distance = Mathf.Max(distanceZ, distanceX);

            dstPosition.z = distance * 1f;

            //var frustumHeight = 2.0f * distance * Mathf.Tan(Camera.fieldOfView * 0.5f * Mathf.Deg2Rad);

            ////Debug.Log("frustrum height, width = " + frustumHeight + ", " + frustumHeight * Camera.aspect + ", " + objectSize);
            ////dstRotation = Quaternion.LookRotation(-Camera.transform.forward, Camera.transform.up);
            ////dstRotation *= Quaternion.Euler(90, 0, 0);
        }

        public void TogglePosition()
        {
            if (_IsMoving) return;
            if (IsInFront)
            {
                IsInFront = false;
                videoControlCanvas.gameObject.SetActive(false);
                transform.parent = _defaultParent.transform;
                MoveToRelativePosition(_defaultParent.transform, orgPosition, orgRotation,TriggerSurvey);
                //TriggerSurvey();
            }
            else
            {
                UpdateTargetPosition();

                IsInFront = true;
                //videoControlCanvas.gameObject.SetActive(true);
                transform.parent = Camera.transform;

                //MoveToRelativePosition(Camera.transform, dstPosition - Camera.transform.position, Quaternion.Euler(90, -90, 90));
                MoveToRelativePosition(Camera.transform, dstPosition, Quaternion.Euler(90, -90, 90),()=>videoControlCanvas.gameObject.SetActive(true));
            }
        }

        public void TriggerSurvey()
        {
            if (!surveyDone)
            {
                surveyDone = true;
                webLinkBehavior.OpenURL();
            }
        }

        private void MoveToRelativePosition(Transform targetTransform, Vector3 localPos, Quaternion localRot, Action OnFinished, int steps = 30)
        {
            _IsMoving = true;
            StartCoroutine(MoveBack(targetTransform, localPos, localRot, OnFinished,steps));
        }

        private IEnumerator MoveBack(Transform targetTransform, Vector3 localPos, Quaternion localRot, Action onFinished, int steps = 30)
        {
            // todo make fps indepent by using time.deltaTime
            var val = 0f;
            var pos = transform.position;
            var rot = transform.localRotation;
            for (int i = 0; i <= steps; i++)

            {
                val = (float)i / steps;
                val = _speedCurve.Evaluate(val);

                transform.position = Vector3.Lerp(pos, targetTransform.position + localPos, val);
                transform.localRotation = Quaternion.Lerp(rot, localRot, val);

                yield return null;
            }
            _IsMoving = false;
            onFinished.Invoke();
        }

        //private void MoveToAbsolutPosition(Vector3 pos, Quaternion rot, int steps = 30)
        //{
        //    _IsMoving = true;
        //    StartCoroutine(MoveAbs(transform.position, transform.rotation, pos, rot, steps));
        //}

        //private IEnumerator MoveAbs(Vector3 p0, Quaternion r0, Vector3 p1, Quaternion r1, int steps = 30)
        //{
        //    var val = 0f;
        //    for (int i = 0; i <= steps; i++)
        //    {
        //        val = (float)i / steps;
        //        val = _speedCurve.Evaluate(val);
        //        transform.position = Vector3.Lerp(p0, p1, val);
        //        transform.rotation = Quaternion.Lerp(r0, r1, val);
        //        yield return null;
        //    }
        //    //for (int i = 0; i <= steps; i++)
        //    //{
        //    //    transform.position = Vector3.Lerp(p0, p1, (float)(i+steps) / 2/steps);
        //    //    //transform.rotation = Quaternion.Lerp(r0, r1, (float) i/ steps);
        //    //    yield return null;
        //    //}
        //    _IsMoving = false;
        //}

        public void OnPointerClick(PointerEventData eventData)
        {
            Log.Info("OnPointerClick");
            if (_enabled && !eventData.dragging)
                TogglePosition();
        }
    }
}