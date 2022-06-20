using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace nextPlace.ARVideoPlayer
{
    public enum TrackingState { Nothing, Update, Interpolation }

    public class ARMarkerTracker : ISubject<ARMarkerPoses, IDictionary<int, IList<TrackedMarker>>>
    {
        public enum FilterType { None, KF, EKF };

        public FilterType Filter;
        private List<IObserver<IDictionary<int, IList<TrackedMarker>>>> _observers = new List<IObserver<IDictionary<int, IList<TrackedMarker>>>>();
        private MobileDevice _device;
        private Config _config;

        public IDictionary<int, IList<TrackedMarker>> Marker { get; } = new Dictionary<int, IList<TrackedMarker>>();

        public ARMarkerTracker(MobileDevice mobileDevice, Config config)
        {
            _device = mobileDevice;
            _config = config;
        }

        public void OnNewPoses(ARMarkerPoses markerPoses)
        {
            UpdateMarkerPoses(Marker, markerPoses.MarkerTransforms, markerPoses.DeviceOrientation);
            _observers.ForEach(o => o.OnNext(Marker));
        }

        #region Match detected maker with currently tracked marker

        private void UpdateMarkerPoses(IDictionary<int, IList<TrackedMarker>> markerSessions, IDictionary<int, IList<Matrix4x4>> poses, GeoPose deviceOrientation)
        {
            foreach (var markerId in poses)
            {
                if (!markerSessions.ContainsKey(markerId.Key))
                    markerSessions.Add(markerId.Key, new List<TrackedMarker>());

                ProcessMarkesWithSameId(markerId.Key, markerSessions[markerId.Key], poses[markerId.Key], deviceOrientation);
            }
        }

        private void ProcessMarkesWithSameId(int markerId, IList<TrackedMarker> sessionMarker, IList<Matrix4x4> poses, GeoPose deviceOrientation)
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

                    float distance = Vector3.Distance(sessionMarker[i].Transform.GetPosition(), position);
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
                TrackedMarker markerOnScene = sessionMarker[index];
                if (markerOnScene.bestMatchPoseIndex >= 0)
                {
                    int poseIndex = markerOnScene.bestMatchPoseIndex;
                    UpdatePosition(markerOnScene, poses[poseIndex], deviceOrientation);
                }
                index--;
            }

            //Create objects for markers not matched with any session marker -> newPosesIndices
            // get current processed markerId

            foreach (var poseIndex in newPosesIndices)
            {
                TrackedMarker markerOnScene = new TrackedMarker();
                markerOnScene.Id = markerId;
                UpdatePosition(markerOnScene, poses[poseIndex], deviceOrientation);
                sessionMarker.Add(markerOnScene);
            }
        }
        
        private void UpdatePosition(TrackedMarker markerOnScene, Matrix4x4 transform, GeoPose deviceOrientation)
        {
            transform = OpenCVUtils.TransformToWorldSpace(transform);

            if (Filter != FilterType.None && markerOnScene.poseFilter == null)
            {
                switch (Filter)
                {
                    case FilterType.None:
                        break;

                    case FilterType.KF:
                        markerOnScene.poseFilter = new FilteredPoseNaiv();
                        break;

                    case FilterType.EKF:
                        markerOnScene.poseFilter = new FilteredPose();
                        break;

                    default:
                        break;
                }
            }

            if (Filter != FilterType.None)
                markerOnScene.Transform = markerOnScene.poseFilter.Update(transform);
            else
                markerOnScene.Transform = transform;

            markerOnScene.LastSeen = DateTime.Now;
            markerOnScene.DeviceOrientation = deviceOrientation;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(ARMarkerPoses value)
        {
            OnNewPoses(value);
        }

        public IDisposable Subscribe(IObserver<IDictionary<int, IList<TrackedMarker>>> observer)
        {
            _observers.Add(observer);
            return Disposable.Empty;
        }
    }

    #endregion Match detected maker with currently tracked marker
}