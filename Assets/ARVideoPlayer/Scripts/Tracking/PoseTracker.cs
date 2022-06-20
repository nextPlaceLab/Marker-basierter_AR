using System;
using System.Collections.Generic;
using System.Diagnostics;
using UniRx;
using UnityEngine;

namespace nextPlace.ARVideoPlayer
{
    public class PoseTracker : IObservable<IDictionary<int, IList<Matrix4x4>>>
    {
        private const bool _IsDebug = false;
        private List<IObserver<IDictionary<int, IList<Matrix4x4>>>> _observers = new List<IObserver<IDictionary<int, IList<Matrix4x4>>>>();
        private OCVWebCam _camera;
        private DetectorAdapter _detector;
        private bool _isTracking = false;
        private float dfps;
        private Stopwatch sw = new Stopwatch();
        private Config _config;
        public PoseTracker(OCVWebCam camera, DetectorAdapter detector, Config config)
        {
            _camera = camera;
            _detector = detector;
            _config = config;
        }

        public void StartTracking()
        {
            if (_isTracking) return;
            Log.Info("StartTracking");
            _isTracking = true;
            Scheduler.ThreadPool.Schedule(() =>
               {
                   try
                   {
                       var fps = new FPSMeasure("PoseTracker", 30);
                       fps.Start();
                       do
                       {

                           if (_camera != null && _camera.WebCam != null && _camera.WebCam.IsCameraFrameReady && _camera.CurrentFrame != null)
                           {
                               var marker = Track();
                               _observers.ForEach(o => o.OnNext(marker));
                           }

                           fps.Lap();

                       } while (_isTracking);
                   }
                   catch (Exception e)
                   {
                       Log.Error("Exception raised: {0}", e);
                   }
               });
        }

        public void StopTracking()
        {
            _isTracking = false;
        }

        private long trackedFrameId;

        private IDictionary<int, IList<Matrix4x4>> Track()
        {
            WebCamFrame frame;

            while (_camera.CurrentFrame.FrameID == trackedFrameId && _isTracking) ;

            frame = _camera.CurrentFrame;
            trackedFrameId = frame.FrameID;

            var marker = _detector.Detect(frame);
            if (_IsDebug)
                Log.Info("Track frame -> #marker = {0}", marker.Count);
            return marker;
        }

        public IDisposable Subscribe(IObserver<IDictionary<int, IList<Matrix4x4>>> observer)
        {
            _observers.Add(observer);
            return Disposable.Empty;
        }
    }
}