using System;
using System.Collections.Generic;
using System.Diagnostics;
using UniRx;

namespace nextPlace.ARVideoPlayer
{
    public class ARPoseDetector : IObservable<ARMarkerPoses>
    {
        private const bool _IsDebug = false;
        private List<IObserver<ARMarkerPoses>> _observers = new List<IObserver<ARMarkerPoses>>();
        private MobileDevice _mobileDevice;
        private DetectorAdapter _detector;
        private bool _isTracking = false;
        private float dfps;
        private Stopwatch sw = new Stopwatch();

        public ARPoseDetector(MobileDevice mobileDevice, DetectorAdapter detector)
        {
            _mobileDevice = mobileDevice;
            _detector = detector;
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
                       var fps = new FPSMeasure("ARPoseTracker");
                       fps.Start();
                       do
                       {
                           if (_mobileDevice.IsDeviceReady)
                           {
                               var marker = Track();
                               if (!marker.Equals(default))
                                   _observers.ForEach(o => o.OnNext(marker));
                           }

                           fps.Lap();
                       } while (_isTracking);
                   }
                   catch (Exception e)
                   {
                       Log.Error("Exception raised: {0},{1},{2}", e.StackTrace, e.Message, e.HelpLink);
                   }
               });
        }

        public void StopTracking()
        {
            _isTracking = false;
        }

        private long trackedFrameId = 0;

        private ARMarkerPoses Track()
        {
            ARFrame frame;
            // horrible, but won't occur on mobile devices (camera fps >> detection fps)

            //while ((_mobileDevice.CurrentFrame == null || _mobileDevice.CurrentFrame.FrameID == trackedFrameId) && _isTracking) ;

            try
            {
                do
                {
                    frame = _mobileDevice.CurrentFrame;
                } while (frame == null || frame.FrameID == trackedFrameId);
            }
            catch (Exception e)
            {
                Log.Error("Track Exception: {0},\ntrace: {1}", e.Message, e.StackTrace);
                return default;
            }

            frame = _mobileDevice.CurrentFrame;
            trackedFrameId = frame.FrameID;

            var marker = _detector.Detect(frame);

            //detected pose are relative to the current device orientation
            return new ARMarkerPoses(marker, frame.DeviceOrientation);
        }

        public IDisposable Subscribe(IObserver<ARMarkerPoses> observer)
        {
            _observers.Add(observer);
            return Disposable.Empty;
        }
    }
}