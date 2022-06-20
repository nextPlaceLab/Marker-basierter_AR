using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using static OpenCvSharp.Unity;

namespace nextPlace.ARVideoPlayer
{
    public class OCVWebCam : MonoBehaviour, IObservable<WebCamFrame>
    {
        public bool UseSimulatorCam;

        //public WebCamBehaviour WebCam;
        public WebCam WebCam;

        public Camera SimulatorCam;
        public UnityEvent OnCameraSetup = new UnityEvent();

        /// <summary>
        /// WebCam texture parameters to compensate rotations, flips etc.
        /// </summary>
        public TextureConversionParams TextureParameters { get; private set; }

        public WebCamFrame CurrentFrame { get; private set; }

        private List<IObserver<WebCamFrame>> _observers = new List<IObserver<WebCamFrame>>();

        private void Start()
        {
            WebCam.Subscribe((tex) => OnNextWebCamTexture(tex));
            WebCam.OnFirstFrame.AddListener(OnCameraReady);
            //OnCameraChanged();
            if (UseSimulatorCam)
            {
                OnCameraReady();
            }
        }

        private void OnCameraReady()
        {
            ReadTextureConversionParameters();
            if (UseSimulatorCam)
                TextureParameters.FlipHorizontally = false;

            OnCameraSetup.Invoke();
        }

        private void ReadTextureConversionParameters()
        {
            Log.Info("Update TextureConversasionParameters");
            TextureConversionParams parameters = new TextureConversionParams();

            // frontal camera - we must flip around Y axis to make it mirror-like
            if (!UseSimulatorCam)
                parameters.FlipHorizontally = /*forceFrontalCamera ||*/ WebCam.IsFrontFacing;

            // TODO:
            // actually, code below should work, however, on our devices tests every device except iPad
            // returned "false", iPad said "true" but the texture wasn't actually flipped

            // compensate vertical flip
            parameters.FlipVertically = WebCam.CamTexture.videoVerticallyMirrored;
            if (Application.platform != RuntimePlatform.Android)
            {
                parameters.FlipVertically = true;
            }
            // deal with rotation
            if (!UseSimulatorCam && 0 != WebCam.CamTexture.videoRotationAngle)
                parameters.RotationAngle = WebCam.CamTexture.videoRotationAngle; // cw -> ccw

            // apply
            TextureParameters = parameters;

            //UnityEngine.Debug.Log (string.Format("front = {0}, vertMirrored = {1}, angle = {2}", webCamDevice.isFrontFacing, webCamTexture.videoVerticallyMirrored, webCamTexture.videoRotationAngle));
        }

        private FPSMeasure fps = new FPSMeasure("OCVCamFPS", 30, true);

        private void OnNextWebCamTexture(WebCamTexture value)
        {
            if (UseSimulatorCam)
            {
                var tex = TextureUtils.RTImage(SimulatorCam, Screen.width, Screen.height);
                CurrentFrame = new WebCamFrame(tex);
            }
            else
            {
                TextureParameters.RotationAngle = WebCam.CamTexture.videoRotationAngle; // cw -> ccw
                CurrentFrame = new WebCamFrame(value, TextureParameters);
            }
            _observers.ForEach(o => o.OnNext(CurrentFrame));
            fps.Lap();
        }

        public IDisposable Subscribe(IObserver<WebCamFrame> observer)
        {
            _observers.Add(observer);
            return Disposable.Empty;
        }
    }
}