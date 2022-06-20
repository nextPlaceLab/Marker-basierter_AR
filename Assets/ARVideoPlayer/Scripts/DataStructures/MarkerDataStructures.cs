using System;
using System.Collections.Generic;
using UnityEngine;

namespace nextPlace.ARVideoPlayer
{
    public struct ARMarkerPoses
    {
        public ARMarkerPoses(IDictionary<int, IList<Matrix4x4>> markerTransforms, GeoPose deviceOrientation) : this()
        {
            MarkerTransforms = markerTransforms;
            DeviceOrientation = deviceOrientation;
        }

        public IDictionary<int, IList<Matrix4x4>> MarkerTransforms { get; private set; }
        public GeoPose DeviceOrientation { get; private set; }
    }

    public class MarkerSession
    {
        public TrackedMarker Marker;
        public MarkerBehaviour Prefab;

        public MarkerSession(TrackedMarker trackedMarker, MarkerBehaviour markerGO)
        {
            Marker = trackedMarker;
            Prefab = markerGO;
        }

        public double Age => (DateTime.Now - Marker.LastSeen).TotalMilliseconds;

        public void UpdatePosition(GeoPose deviceOrientation, bool isSyncWithGyro)
        {
            Marker.Apply(Prefab.transform, deviceOrientation, isSyncWithGyro);
        }
    }

    public class TrackedMarker
    {
        public int Id;

        public DateTime LastSeen;
        public GeoPose DeviceOrientation;
        public Matrix4x4 Transform;

        public IPoseFilter poseFilter;

        public int bestMatchPoseIndex = -1;

        public Matrix4x4 CurrentTransform(GeoPose deviceOrientation)
        {
            return Transform;
        }

        private Vector3[] filter;
        private int index = 0;

        private Vector3 Filter(Vector3 vec)
        {
            if (filter == null)
                filter = new Vector3[10];

            filter[index] = vec;
            index++;
            index = index % filter.Length;

            var ret = new Vector3();
            for (int i = 0; i < filter.Length; i++)
                ret += filter[i];

            ret /= filter.Length;
            return ret;
        }

        public const bool ApplyRotation = true;
        // note: much better impl. would be to return an updated transformation instead of apply it 
        //testc
            // in unity einstellen, das er die armso auch für die 64 bit variante nehmen soll.
            // vielleicht funktioniert dann die x64  il2cpp version
        public void Apply(Transform t, GeoPose currentOrientation, bool isSyncWithGyro)
        {
            if (!isSyncWithGyro)
            {
                //Log.Info("No gyroSync");

                t.position = Transform.GetPosition();
                if (ApplyRotation)
                    t.rotation = Transform.GetQuaternion();
                t.localScale = Transform.GetScale();
            }
            else
            {
                //Log.Info("GyroSync = {0}", currentOrientation);
                var rot0 = DeviceOrientation.Orientation;
                var rot1 = currentOrientation.Orientation;

                var invRot0 = Quaternion.Inverse(rot0);
                var invRot1 = Quaternion.Inverse(rot1);
                //Quaternion.RotateTowards(rot1,rot0,)
                var dEuler = -rot0.eulerAngles + rot1.eulerAngles;
                //var dEulerUnity = dEuler;
                var dEulerUnity = new Vector3(-dEuler.x, -dEuler.y, -dEuler.z);
                //var dEulerUnity = new Vector3(dEuler.y, -dEuler.x, -dEuler.z);//old

                //var filterEulerUnity = Filter(dEulerUnity);
                //UIDisplay.current.Set("Delta Pose", dEulerUnity);
                //UIDisplay.current.Set("Filter UnityEuler", filterEulerUnity);
                //UIDisplay.current.Set("Diff", filterEulerUnity - dEulerUnity);
                //var rot = Quaternion.Euler(filterEulerUnity);
                var rot = Quaternion.Euler(dEulerUnity);
                //var rot = invRot0 * invRot1;
                //var rot = invRot1 * invRot0;
#if DEBUG
                UIDisplay.current.Set("GyroDelta", rot.eulerAngles);
#endif
                //t.position = Transform.GetPosition();
                //t.rotation = Transform.GetQuaternion();
                //t.localScale = Transform.GetScale();
                //return;
                if (false)
                {
                    t.position = rot * Transform.GetPosition();
                    t.rotation = rot * Transform.GetQuaternion();

                    //t.position = Transform.GetPosition();
                    //t.rotation = Transform.GetQuaternion();
                    t.localScale = Transform.GetScale();
                }
                else
                {
                    var pos = rot * Transform.GetPosition();
                    var rotation = rot * Transform.GetQuaternion();
                    var scale = Transform.GetScale();

                    var m = Matrix4x4.TRS(pos, rotation, scale);

                    Matrix4x4 currentT;
                    if (poseFilter != null)
                        currentT = poseFilter.Update(m);
                    else
                        currentT = Matrix4x4.TRS(pos, rotation, scale);

                    t.position = currentT.GetPosition();
                    if (ApplyRotation)
                        t.rotation = currentT.GetQuaternion();
                    t.localScale = currentT.GetScale();
                }
            }
        }
    }

    public class MarkerOnScene
    {
        public int markerId;
        public DateTime LastSeen;
        public GeoPose DeviceOrientation;

        public Matrix4x4 transform;
        public IPoseFilter poseFilter;

        public int bestMatchPoseIndex = -1;
    }
}