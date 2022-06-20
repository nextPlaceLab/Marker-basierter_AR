using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace nextPlace.ARVideoPlayer
{
    public class WebCam : MonoBehaviour, IObservable<WebCamTexture>
    {
        [Tooltip("-1: ShowSelection, 0: first backCam, device.count-1 = front cam")]
        public int InitCameraId = -1;

        public bool ForceBackface = true;

        public bool IsEditorSimulator = false;

        public Material CameraMaterial = null;

        public bool IsCameraFrameReady { get; private set; }

        public WebCamTexture CamTexture { get; private set; } = null;
        public float AspectRatio => CamTexture.width / (float)CamTexture.height;

        public bool IsFrontFacing => (_IsCameraSelected && ActiveCam.isFrontFacing);
        public bool IsAutoFocusSupported => (_IsCameraSelected && ActiveCam.isAutoFocusPointSupported);
        public WebCamDevice[] CamDevices { get; private set; }
        public WebCamDevice ActiveCam { get; private set; }

        public UnityEvent OnFirstFrame = new UnityEvent();

        private int m_framesPerSecond = 0;

        private int m_frameCount = 0;

        private DateTime m_timerFrames = DateTime.MinValue;

        private int m_indexDevice = -1;

        private bool _IsCameraSelected;
        private List<IObserver<WebCamTexture>> _observers = new List<IObserver<WebCamTexture>>();
        private FPSMeasure fps = new FPSMeasure("CamFPS", 30, true);
        private AsyncOperation ao;
        private long _updateCount = 0;

        private void Start()
        {
            //Log.Info("AllowThreadedTexCreation = {0}", WebCamTexture.allowThreadedTextureCreation);
            if (CameraMaterial == null)
                throw new ApplicationException("Missing camera material reference");

            ao = Application.RequestUserAuthorization(UserAuthorization.WebCam);

            if (IsEditorSimulator)
                ForceBackface = false;

            if (InitCameraId >= 0)
            {
                StartCoroutine(WaitForDevice(InitCameraId, ForceBackface, 5f));
            }
        }

        private IEnumerator WaitForDevice(int index, bool forceBackface, float timeOutSeconds)
        {
            var startTime = DateTime.Now;
            do
            {
                if (!ao.isDone)
                    yield return null;

                ScanForDevices();
                yield return null;
                var id = index;
                if (forceBackface)
                    id = FindNextBackfaceCamera(index);

                if (SetActiveCamera(id))
                    break;

                yield return null;
            } while ((DateTime.Now - startTime).TotalSeconds < timeOutSeconds);
        }

        private int FindNextBackfaceCamera(int index)
        {
            for (int i = 0; i < CamDevices.Length; i++)
            {
                var camIndex = (index + i) % CamDevices.Length;
                if (!CamDevices[camIndex].isFrontFacing)
                    return camIndex;
            }
            return -1;
        }

        private void Update()
        {
            // render update and camera update needs some "sync"
            if ((CamTexture != null
                && CamTexture.didUpdateThisFrame
                && CamTexture.width > 100))
            {
                //_isCameraAvailable = m_texture != null && m_texture.width > 100;
                if (!IsCameraFrameReady)
                {
                    IsCameraFrameReady = true;
                    OnFirstFrame.Invoke();
                    Log.Info("Invoked OnFirstFrame");
                }

                CameraMaterial.mainTexture = CamTexture;
                _observers.ForEach(o => o.OnNext(CamTexture));
                fps.Lap();
                //Log.Info("WebCamTexure({0}) okay", _updateCount);
            }
            else
            {
                var texNull = CamTexture == null;
                var didUpdate = false;
                var size = Vector2.zero;
                if (!texNull)
                {
                    didUpdate = CamTexture.didUpdateThisFrame;
                    size = new Vector2(CamTexture.width, CamTexture.height);
                }
                if (!texNull && size.x > 100)
                {
                    // render fps higher than camera -> skip
                }
                else
                {
                    //Log.Info("WebCamTexure({3}): is null = {0}, has updated = {1}, size = {2}", texNull, didUpdate, size, _updateCount);
                }
            }
            _updateCount++;
        }

        private void ScanForDevices()
        {
            //Log.Info("Scanning for camera devices");
            CamDevices = WebCamTexture.devices;
        }

        private void OnGUI()
        {
            //if (_IsCameraSelected) return;
            if (InitCameraId >= 0 || _IsCameraSelected) return;

            ScanForDevices();

            if (m_timerFrames < DateTime.Now)
            {
                m_framesPerSecond = m_frameCount;
                m_frameCount = 0;
                m_timerFrames = DateTime.Now + TimeSpan.FromSeconds(1);
            }
            ++m_frameCount;

            GUILayout.Label(string.Format("Frames per second: {0}", m_framesPerSecond));

            if (m_indexDevice >= 0 && CamDevices.Length > 0)
            {
                GUILayout.Label(string.Format("Selected Device: {0}, front-facing = {1}", CamDevices[m_indexDevice].name, CamDevices[m_indexDevice].isFrontFacing));
            }

            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                GUILayout.Label("Has WebCam Authorization");
                if (CamDevices == null)
                {
                    GUILayout.Label("Null web cam devices");
                }
                else
                {
                    GUILayout.Label(string.Format("{0} web cam devices", CamDevices.Length));
                    for (int index = 0; index < CamDevices.Length; ++index)
                    {
                        var deviceName = CamDevices[index].name;
                        if (string.IsNullOrEmpty(deviceName))
                        {
                            GUILayout.Label("unnamed web cam device");
                            continue;
                        }

                        if (GUILayout.Button(string.Format("web cam device {0}{1}{2}{3}",
                                                            m_indexDevice == index
                                                           ? "["
                                                           : string.Empty,
                                                           deviceName,
                                                           m_indexDevice == index ? "]" : string.Empty, CamDevices[index].isFrontFacing),
                                             GUILayout.MinWidth(200),
                                             GUILayout.MinHeight(50)))
                        {
                            if (!SetActiveCamera(index))
                                Log.Error("Could not set camera index {0}", index);
                        }
                    }
                }
            }
            else
            {
                GUILayout.Label("Pending WebCam Authorization...");
            }
        }

        private bool SetActiveCamera(int index)
        {
            // stop playing
            if (CamTexture != null && CamTexture.isPlaying)
                CamTexture.Stop();

            // destroy the old texture
            if (CamTexture != null)
                UnityEngine.Object.DestroyImmediate(CamTexture, true);

            IsCameraFrameReady = false;
            if (index < 0 || index > CamDevices.Length - 1)
            {
                m_indexDevice = -1;
                ActiveCam = default;
                _IsCameraSelected = false;
                return false;
            }

            m_indexDevice = index;
            ActiveCam = CamDevices[index];

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
            CamTexture = new WebCamTexture(ActiveCam.name, Screen.width, Screen.height);
            //WebCamTexture = new WebCamTexture(ActiveCam.name);
            Log.Info("Requested webcamTex size = ({0}, {1})", Screen.width, Screen.height);
            // start playing
            CamTexture.Play();

            // assign the texture
            CameraMaterial.mainTexture = CamTexture;

            //// update cam parameter
            //IsFrontFacing = ActiveCam.isFrontFacing;
            //IsAutoFocusSupported = ActiveCam.isAutoFocusPointSupported;

            // notifiy on new camera
            //OnCameraSetup.Invoke();
            _IsCameraSelected = true;
            _updateCount = 0;

            return true;
        }

        public IDisposable Subscribe(IObserver<WebCamTexture> observer)
        {
            _observers.Add(observer);
            return Disposable.Empty;
        }
    }
}