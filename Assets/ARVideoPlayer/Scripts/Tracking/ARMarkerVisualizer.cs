using System;
using System.Collections.Generic;
using UnityEngine;

namespace nextPlace.ARVideoPlayer
{
    public class ARMarkerVisualizer : MonoBehaviour, IObserver<IDictionary<int, IList<TrackedMarker>>>
    {
        public int HideDelayMS = 300;
        public Camera ARCamera;
        public MobileDevice MobileDevice;
        public GameObject PrefabParent;
        public List<MarkerGOPreset> MarkerPresets;

        private Dictionary<int, MarkerGOPreset> _prefabs = new Dictionary<int, MarkerGOPreset>();
        private Dictionary<int, MarkerSession> _markerSessions = new Dictionary<int, MarkerSession>();
        private bool isPoseUpdated;
        private const bool _IsDebug = false;

        public bool UseGyroSync { get; set; }

        private void Start()
        {
            foreach (var item in MarkerPresets)
                _prefabs.Add(item.MarkerId, item);
        }

        private void OnNewPosition(IDictionary<int, IList<TrackedMarker>> tracked)
        {
            isPoseUpdated = true;
            foreach (var markers in tracked)
            {
                if (!_prefabs.ContainsKey(markers.Key))
                {
                    Log.Error("No prefab assigned for markerID = {0}", markers.Key);
                    continue;
                }

                if (markers.Value.Count > 0)
                {
                    //continue;

                    if (!_markerSessions.ContainsKey(markers.Key))
                    {
                        var go = InstantiateMakerPrefab(markers.Key);
                        var mb = go.GetComponent<MarkerBehaviour>();
                        if (mb != null)
                        {
                            // create new MarkerSession
                            var ms = new MarkerSession(markers.Value[0], mb);
                            // pass in dependencies
                            mb.ARCamera = ARCamera;
                            mb.Preset = _prefabs[markers.Key];
                            mb.MarkerSession = ms;
                            _markerSessions.Add(ms.Marker.Id, ms);
                        }
                        else
                        {
                            Log.Error("MarkerBehaviour component not found");
                        }
                    }

                    // take always the first marker of given id -> [0]!
                    _markerSessions[markers.Key].Marker.LastSeen = markers.Value[0].LastSeen;
                    if (_IsDebug)
                        Log.Info("MarkerId = {1}, Pose Age = {0}", _markerSessions[markers.Key].Age, markers.Key);
                    //ApplyTransform(_sessionPrefabs[marker.Key].MarkerGO.gameObject, marker.Value[0].Transform);
                }
            }
        }

        private void Update()
        {
            //if (isPoseUpdated)
            //{
            //    isPoseUpdated = false;
            //    UIDisplay.current.SetTrackingState(TrackingState.Update);
            //}
            //else
            //    UIDisplay.current.SetTrackingState(TrackingState.Interpolation);

            foreach (var item in _markerSessions)
            {
                var isVisible = (DateTime.Now - item.Value.Marker.LastSeen).TotalMilliseconds < HideDelayMS;
                item.Value.Prefab.SetVisible(isVisible);
                if (isVisible)
                    item.Value.UpdatePosition(MobileDevice.CurrentPose, UseGyroSync);
            }
        }

        private GameObject InstantiateMakerPrefab(int markerId)
        {
            var go = Instantiate(_prefabs[markerId].Prefab);
            go.transform.parent = PrefabParent.transform;

            return go;
        }

        //private static void ApplyTransform(GameObject gameObject, Matrix4x4 matrix)
        //{
        //    gameObject.transform.localPosition = MatrixHelper.GetPosition(matrix);
        //    gameObject.transform.localRotation = MatrixHelper.GetQuaternion(matrix);
        //    gameObject.transform.localScale = MatrixHelper.GetScale(matrix);

        //    //gameObject.transform.localPosition = matrix.GetColumn(3);
        //    //gameObject.transform.localRotation = matrix.rotation;
        //    //gameObject.transform.localScale = matrix.lossyScale;
        //}

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(IDictionary<int, IList<TrackedMarker>> value)
        {
            OnNewPosition(value);
        }
    }
}