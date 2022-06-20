using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using UniRx;
using UnityEngine;
using static OpenCvSharp.Unity;

namespace nextPlace.ARVideoPlayer
{
    public class DetectorAdapter : ISubject<WebCamFrame, IDictionary<int, IList<Matrix4x4>>>
    {
        private const bool _IsDebug = false;

        private List<IObserver<IDictionary<int, IList<Matrix4x4>>>> _observer = new List<IObserver<IDictionary<int, IList<Matrix4x4>>>>();
        private MarkerDetector _markerDetector;
        private TextureConversionParams _textureParameters;
        private Config _config;
        public Texture2D DebugCamTex;

        public DetectorAdapter(OpenCvSharp.Unity.TextureConversionParams textureParameters, Config config)
        {
            _markerDetector = new MarkerDetector();
            _textureParameters = textureParameters;
            _config = config;
        }

        public void OnNext(WebCamFrame frame)
        {
            Detect(frame);
        }

        public IDictionary<int, IList<Matrix4x4>> Detect(WebCamFrame frame)
        {
            Stopwatch sw;
            if (_IsDebug)
                sw = Stopwatch.StartNew();

            var currentMarker = new Dictionary<int, IList<Matrix4x4>>();
            if (frame != null && frame.Pixels != null)
            {
                //detect marker
                var mat = frame.Mat;
                Log.Info("Size before = {0}", mat.Size());
                Cv2.Resize(mat, mat, new Size(mat.Cols / 2, mat.Rows / 2));
                Log.Info("Size after = {0}", mat.Size());
                Timer.Restart("detect");
                var markerIds = _markerDetector.Detect(mat, mat.Cols, mat.Rows);
                //Thread.Sleep(100);
                Timer.Stop("detect");
                
                UIDisplay.current.Set("Detection", Timer.Time("detect"));

#if UNITY_EDITOR
                //Log.Info("Detector sleep...");
                //Thread.Sleep(2000);
#endif

                var sb = new StringBuilder();
                if (markerIds.Count > 0)
                {
                    for (int i = 0; i < markerIds.Count - 1; i++)
                    {
                        sb.Append(markerIds[i]);
                        sb.Append(", ");
                    }
                    sb.Append(markerIds[markerIds.Count - 1]);
                }
                sb.Append(string.Format(" ({0})", markerIds.Count));
                UIDisplay.current.Set("Marker", sb.ToString());

                // write output mat to debug texture
                if (Thread.CurrentThread.ManagedThreadId == 1)
                    DebugCamTex = MatToTexture(mat);

                // add poses to dictionary
                for (int i = 0; i < markerIds.Count; i++)
                {
                    if (!currentMarker.ContainsKey(markerIds[i]))
                        currentMarker.Add(markerIds[i], new List<Matrix4x4>());

                    currentMarker[markerIds[i]].Add(_markerDetector.TransfromMatrixForIndex(i));
                }
            }

            if (_IsDebug)
                Log.Info("Detected marker in {0} ms", sw.ElapsedMilliseconds);
            // notify observer
            if (currentMarker.Count > 0)
                _observer.ForEach(o => o.OnNext(currentMarker));

            return currentMarker;
        }

        public IDisposable Subscribe(IObserver<IDictionary<int, IList<Matrix4x4>>> observer)
        {
            _observer.Add(observer);
            return Disposable.Empty;
        }

        public void OnCompleted()
        {
            Log.Info("OnCompleted");
        }

        public void OnError(Exception error)
        {
            Log.Error("Error: {0}", error);
        }
    }
}