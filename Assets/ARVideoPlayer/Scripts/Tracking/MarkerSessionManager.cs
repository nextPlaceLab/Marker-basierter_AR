using System;
using System.Collections.Generic;
using System.Diagnostics;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace nextPlace.ARVideoPlayer
{
    public class MarkerSessionManager : MonoBehaviour, IObserver<IDictionary<int, IList<Matrix4x4>>>, IObservable<IDictionary<int, IList<MarkerOnScene>>>

    {
        private const bool _IsDebug = false;

        // manages current session, i.e. keeps track of all detected marker
        public Camera ARCamera;

        public Image StatusImage;

        private Dictionary<int, IList<MarkerOnScene>> markerSessions = new Dictionary<int, IList<MarkerOnScene>>();
        private List<IObserver<Dictionary<int, IList<MarkerOnScene>>>> _observer = new List<IObserver<Dictionary<int, IList<MarkerOnScene>>>>();
        private Config _config;

        public void SetConfig(Config config)
        {
            _config = config;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        //private void Awake()
        //{
        //    var markers = new int[] { 5, 23, 24 };
        //    foreach (var item in markers)
        //    {
        //        markerSessions.Add(item, new List<MarkerOnScene>());
        //    }
        //}
        // evtl ist es sinnvoll, den kalman filter zu reseten, wenn ein marker
        // nicht mehr sichtbar ist. überhaupt ist das ein sinnvoller status -> sichtbar in aktuellem frame

        private FPSMeasure fps = new FPSMeasure("MarkerSession");
        public void OnNext(IDictionary<int, IList<Matrix4x4>> poses)
        {
            if (_IsDebug)
                Log.Info("On new poses");

            try
            {
                Match(poses);

                _observer.ForEach(o => o.OnNext(markerSessions));

                fps.Lap();
            }
            catch (Exception e)
            {
                Log.Error("Exception raised: {0}", e);
            }
        }

        private void Match(IDictionary<int, IList<Matrix4x4>> poses)
        {
            foreach (var markerId in poses)
            {
                if (!markerSessions.ContainsKey(markerId.Key))
                    markerSessions.Add(markerId.Key, new List<MarkerOnScene>());

                ProcessMarkesWithSameId(markerId.Key,markerSessions[markerId.Key], poses[markerId.Key]);
            }
        }

        private void ProcessMarkesWithSameId(int markerId ,IList<MarkerOnScene> sessionMarker, IList<Matrix4x4> poses)
        {
            if (poses.Count == 0) return;

            foreach (var item in sessionMarker)
                item.bestMatchPoseIndex = -1;

            int index = poses.Count - 1;

            // Match poses with existing session marker
            // otherwise add pose to new poses indices
            var newPosesIndices = new HashSet<int>();
            while (index >= 0)
            {
                //int poseIndex = foundedMarkers[index];
                var position = MatrixHelper.GetPosition(poses[index]);

                float minDistance = float.MaxValue;
                int bestMatchSessionIndex = -1;
                for (int i = 0; i < sessionMarker.Count; i++)
                {
                    if (sessionMarker[i].bestMatchPoseIndex >= 0)
                    {
                        // each sessionMarker may only be matched to one detected pose
                        // thus continue if session marker is already matched
                        continue;
                    }

                    float distance = Vector3.Distance(sessionMarker[i].transform.GetPosition(), position);
                    if (distance < minDistance)
                        bestMatchSessionIndex = i;
                }

                if (bestMatchSessionIndex >= 0)
                    sessionMarker[bestMatchSessionIndex].bestMatchPoseIndex = index;
                else
                    newPosesIndices.Add(index);

                --index;
            }

            // update position of existing sessionMarker
            index = sessionMarker.Count - 1;
            while (index >= 0)
            {
                MarkerOnScene markerOnScene = sessionMarker[index];
                if (markerOnScene.bestMatchPoseIndex >= 0)
                {
                    int poseIndex = markerOnScene.bestMatchPoseIndex;
                    UpdatePosition(markerOnScene, poses[poseIndex]);
                }
                index--;
            }

            //Create objects for markers not matched with any session marker -> newPosesIndices
            

            foreach (var poseIndex in newPosesIndices)
            {
                MarkerOnScene markerOnScene = new MarkerOnScene();
                markerOnScene.markerId = markerId;
                UpdatePosition(markerOnScene, poses[poseIndex]);
                sessionMarker.Add(markerOnScene);
            }
        }

        private void Update()
        {
            var anyMarkerVisible = false;
            foreach (var ms in markerSessions)
            {
                if (_IsDebug)
                    Log.Info("Marker on scene: {0},{1}", ms.Key, ms.Value.Count);
                if (ms.Value.Count > 0) anyMarkerVisible = true;
            }

            StatusImage.color = anyMarkerVisible ? Color.green : Color.red;
        }

        private void UpdatePosition(MarkerOnScene markerOnScene, Matrix4x4 transform)
        {
            transform = OpenCVUtils.TransformToWorldSpace(transform);
            if (markerOnScene.poseFilter == null)
            {
                if (_config.UseKF)
                    markerOnScene.poseFilter = new FilteredPoseNaiv();
                else
                    markerOnScene.poseFilter = new FilteredPose();
            }
            if (_config.UseEKF)
                markerOnScene.transform = markerOnScene.poseFilter.Update(transform);
            else
                markerOnScene.transform = transform;

            markerOnScene.LastSeen = DateTime.Now;
        }

        public IDisposable Subscribe(IObserver<IDictionary<int, IList<MarkerOnScene>>> observer)
        {
            _observer.Add(observer);
            return Disposable.Empty;
        }
    }
}