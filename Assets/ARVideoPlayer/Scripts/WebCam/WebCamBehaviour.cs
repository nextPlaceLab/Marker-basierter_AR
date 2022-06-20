using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace nextPlace.ARVideoPlayer
{
    public class WebCamBehaviour : MonoBehaviour, IObservable<WebCamTexture>
    {
        [Tooltip("-1: ShowSelection, 0: first backCam, device.count-1 = front cam")]
        public int CameraId = -1;

        /// <summary>
        /// Meta reference to the camera
        /// </summary>
        public Material CameraMaterial = null;

        /// <summary>
        /// The number of frames per second
        /// </summary>
        private int m_framesPerSecond = 0;

        /// <summary>
        /// The current frame count
        /// </summary>
        private int m_frameCount = 0;

        /// <summary>
        /// The frames timer
        /// </summary>
        private DateTime m_timerFrames = DateTime.MinValue;

        /// <summary>
        /// The selected device index
        /// </summary>
        private int m_indexDevice = -1;

        private List<IObserver<WebCamTexture>> _observers = new List<IObserver<WebCamTexture>>();
        private bool _IsCameraSelected;

        public bool IsCameraAvailable { get; private set; }

        public WebCamTexture WebCamTexture { get; private set; } = null;
        public float AspectRatio => WebCamTexture.width / (float)WebCamTexture.height;

        public bool IsFrontFacing { get; private set; }
        public bool IsAutoFocusSupported { get; private set; }

        public UnityEvent OnCameraSetup = new UnityEvent();

        // Use this for initialization
        private void Start()
        {
            //CameraMaterial = background.material;
            if (null == CameraMaterial)
            {
                throw new ApplicationException("Missing camera material reference");
            }

            Application.RequestUserAuthorization(UserAuthorization.WebCam);
        }

        private void OnGUI()
        {
            if (_IsCameraSelected) return;

            if (m_timerFrames < DateTime.Now)
            {
                m_framesPerSecond = m_frameCount;
                m_frameCount = 0;
                m_timerFrames = DateTime.Now + TimeSpan.FromSeconds(1);
            }
            ++m_frameCount;

            GUILayout.Label(string.Format("Frames per second: {0}", m_framesPerSecond));

            if (m_indexDevice >= 0 && WebCamTexture.devices.Length > 0)
            {
                GUILayout.Label(string.Format("Selected Device: {0}", WebCamTexture.devices[m_indexDevice].name));
            }

            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                GUILayout.Label("Has WebCam Authorization");
                if (null == WebCamTexture.devices)
                {
                    GUILayout.Label("Null web cam devices");
                }
                else
                {
                    GUILayout.Label(string.Format("{0} web cam devices", WebCamTexture.devices.Length));
                    for (int index = 0; index < WebCamTexture.devices.Length; ++index)
                    {
                        var device = WebCamTexture.devices[index];
                        if (string.IsNullOrEmpty(device.name))
                        {
                            GUILayout.Label("unnamed web cam device");
                            continue;
                        }

                        if (GUILayout.Button(string.Format("web cam device {0}{1}{2}",
                                                            m_indexDevice == index
                                                           ? "["
                                                           : string.Empty,
                                                           device.name,
                                                           m_indexDevice == index ? "]" : string.Empty),
                                             GUILayout.MinWidth(200),
                                             GUILayout.MinHeight(50)))
                        {
                            m_indexDevice = index;

                            // stop playing
                            if (null != WebCamTexture)
                            {
                                if (WebCamTexture.isPlaying)
                                {
                                    WebCamTexture.Stop();
                                }
                            }

                            // destroy the old texture
                            if (null != WebCamTexture)
                            {
                                UnityEngine.Object.DestroyImmediate(WebCamTexture, true);
                            }

                            //var availableResolutions = device.availableResolutions;
                            //if (availableResolutions != null)

                            //{
                            //    var resStr = "";
                            //    foreach (var res in availableResolutions)
                            //    {
                            //        resStr += res + "\n";
                            //    }
                            //    MainScript.DisplayInfo(resStr);
                            //}
                            //else
                            //{
                            //    Log.Info("Resolution = null");
                            //}

                            // use the device name
                            //m_texture = new WebCamTexture(device.name);

                            //m_texture = new WebCamTexture(device.name, 1920, 1080);
                            WebCamTexture = new WebCamTexture(device.name, Screen.width, Screen.height);

                            // start playing
                            WebCamTexture.Play();

                            // assign the texture
                            CameraMaterial.mainTexture = WebCamTexture;

                            // update cam parameter
                            IsFrontFacing = device.isFrontFacing;
                            IsAutoFocusSupported = device.isAutoFocusPointSupported;

                            // notifiy on new camera
                            //OnCameraSetup.Invoke();
                            _IsCameraSelected = true;
                        }
                    }
                }
            }
            else
            {
                GUILayout.Label("Pending WebCam Authorization...");
            }
        }

        private FPSMeasure fps = new FPSMeasure("CamFPS", 30, true);

        private void Update()
        {
            if (WebCamTexture != null
                && WebCamTexture.didUpdateThisFrame
                && WebCamTexture.width > 100)
            {
                //_isCameraAvailable = m_texture != null && m_texture.width > 100;
                if (!IsCameraAvailable)
                {
                    IsCameraAvailable = true;
                    OnCameraSetup.Invoke();
                }

                CameraMaterial.mainTexture = WebCamTexture;
                _observers.ForEach(o => o.OnNext(WebCamTexture));
                fps.Lap();
            }
        }

        public IDisposable Subscribe(IObserver<WebCamTexture> observer)
        {
            _observers.Add(observer);
            return Disposable.Empty;
        }
    }
}