//using System;
//using System.Collections.Generic;
//using UnityEngine;

//namespace nextPlace.ARVideoPlayer
//{
//    public class MarkerVisualizer : MonoBehaviour, IObserver<IDictionary<int, IList<MarkerOnScene>>>
//    {
//        public int HideDelayMS = 300;
//        public Camera ARCamera;
//        public GameObject PrefabParent;
//        public List<MarkerGOPreset> MarkerPresets;

//        private Dictionary<int, MarkerGOPreset> _prefabs = new Dictionary<int, MarkerGOPreset>();
//        private Dictionary<int, MarkerSession> _sessionPrefabs = new Dictionary<int, MarkerSession>();
//        private const bool _IsDebug = false;

//        //private class MarkerSession
//        //{
//        //    public int markerId { get; private set; }
//        //    public GameObject go { get; private set; }
//        //    public DateTime LastSeen { get; set; }

//        //    public MarkerSession(int markerId, GameObject go)
//        //    {
//        //        this.markerId = markerId;
//        //        this.go = go;
//        //    }
//        //}

//        private void Start()
//        {
//            foreach (var item in MarkerPresets)
//                _prefabs.Add(item.MarkerId, item);
//        }

//        public void OnCompleted()
//        {
//        }

//        public void OnError(Exception error)
//        {
//        }

//        public void OnNext(IDictionary<int, IList<MarkerOnScene>> value)
//        {
//            OnNewPosition(value);
//        }

//        private void OnNewPosition(IDictionary<int, IList<MarkerOnScene>> markerPoses)
//        {
//            //Log.Info("dict.count = {0}", markerPoses.Count);
//            //foreach (var item in markerPoses)
//            //{
//            //    Log.Info("Item update, markerId = {0}, count = {1}", item.Key, item.Value.Count);
//            //}
//            //return;
//            foreach (var marker in markerPoses)
//            {
//                //Log.Info("Item update, markerId = {0}, count = {1}", marker.Key, marker.Value.Count);
//                if (!_prefabs.ContainsKey(marker.Key))
//                {
//                    Log.Error("No prefab assigned for markerID = {0}", marker.Key);
//                    continue;
//                }

//                if (marker.Value.Count > 0)
//                {
//                    //continue;

//                    if (!_sessionPrefabs.ContainsKey(marker.Key))
//                    {
//                        var go = InstantiateMakerPrefab(marker.Key);
//                        var mb = go.GetComponent<MarkerBehaviour>();
//                        if (mb != null)
//                        {
//                            // create new MarkerSession
//                            var ms = new MarkerSession(marker.Key, mb);
//                            // pass in dependencies
//                            mb.ARCamera = ARCamera;
//                            mb.MarkerSession = ms;
//                            _sessionPrefabs.Add(ms.Marker.Id, ms);
//                        }
//                        else
//                        {
//                            Log.Error("MarkerBehaviour component not found");
//                        }
//                    }

//                    // take always the first marker of given id -> [0]!
//                    _sessionPrefabs[marker.Key].LastSeen = marker.Value[0].LastSeen;
//                    if (_IsDebug)
//                        Log.Info("MarkerId = {1}, Pose Age = {0}", _sessionPrefabs[marker.Key].Age, marker.Key);
//                    ApplyTransform(_sessionPrefabs[marker.Key].GameObject.gameObject, marker.Value[0].transform);
//                }
//            }
//        }

//        private void Update()
//        {
//            foreach (var item in _sessionPrefabs)
//            {
//                var isVisible = (DateTime.Now - item.Value.LastSeen).TotalMilliseconds < HideDelayMS;
//                item.Value.GameObject.SetVisible(isVisible);

//                //if (item.Value.MarkerGO.gameObject.activeInHierarchy != isVisible)
//                //    item.Value..SetActive(isVisible);
//            }
//        }

//        private GameObject InstantiateMakerPrefab(int markerId)
//        {
//            var go = Instantiate(_prefabs[markerId].Prefab);
//            go.transform.parent = PrefabParent.transform;
//            //// das sollte das preab selbst machen...ist ja bei jedem anders
//            //var moveScript = go.GetComponent<MoveToFrontBehavior>();
//            //if (moveScript != null) moveScript.Camera = ARCamera;

//            return go;
//        }

//        private static void ApplyTransform(GameObject gameObject, Matrix4x4 matrix)
//        {
//            gameObject.transform.localPosition = MatrixHelper.GetPosition(matrix);
//            gameObject.transform.localRotation = MatrixHelper.GetQuaternion(matrix);
//            gameObject.transform.localScale = MatrixHelper.GetScale(matrix);

//            //gameObject.transform.localPosition = matrix.GetColumn(3);
//            //gameObject.transform.localRotation = matrix.rotation;
//            //gameObject.transform.localScale = matrix.lossyScale;
//        }
//    }
//}