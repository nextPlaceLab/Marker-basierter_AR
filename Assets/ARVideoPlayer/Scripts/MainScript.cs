namespace nextPlace.ARVideoPlayer
{
    using UniRx;
    using UnityEngine;

    public class MainScript : MonoBehaviour
    {
        public Camera Camera;

        public MobileDevice MobileDevice;

        public GameObject Simulation;

        public ARMarkerVisualizer MarkerVisualizer;

        public Config Config;

        private DetectorAdapter _markerDetector;
        private ARPoseDetector _arDetector;
        private ARMarkerTracker _markerTracker;

        public Material DebugCamTex;

        protected void Awake()
        {
            // init RX
            Scheduler.SetDefaultForUnity();
#if !DEBUG
            Config.ReadPrefs();
#endif
            //ApplyConfig();
        }

        private void OnValidate()
        {
            Log.Info("Validate");
            ApplyConfig();
        }

        private void Start()
        {
            UnitySettings();

            _markerDetector = new DetectorAdapter(MobileDevice.Camera.TextureParameters, Config);

            _markerTracker = new ARMarkerTracker(MobileDevice, Config);

            if (Config.UseDetectorThread)
            {
                _arDetector = new ARPoseDetector(MobileDevice, _markerDetector);

                _arDetector
                    .SubscribeOn(Scheduler.ThreadPool)
                    .ObserveOn(Scheduler.ThreadPool)
                    .Subscribe(_markerTracker);

                _markerTracker
                    .SubscribeOn(Scheduler.ThreadPool)
                    .ObserveOn(Scheduler.MainThread)
                    .Subscribe(MarkerVisualizer);
            }
            else
            {
                MobileDevice.Camera.Subscribe(_markerDetector);
                _markerDetector.Subscribe((matrix) =>
                {
                    var markerPoses = new ARMarkerPoses(matrix, MobileDevice.CurrentPose);
                    _markerTracker.OnNext(markerPoses);
                });
                _markerTracker
                    .SubscribeOn(Scheduler.ThreadPool)
                    .ObserveOn(Scheduler.MainThread)
                    .Subscribe(MarkerVisualizer);
            }
            ApplyConfig();
            MobileDevice.Camera.OnCameraSetup.AddListener(OnCameraStreamReady);
        }

        private void ApplyConfig()
        {
            if (MobileDevice != null)
            {
                MobileDevice.UseGyroEKF = Config.UseGyroEKF;
                MobileDevice.Camera.WebCam.IsEditorSimulator = Config.IsEditorSimulation;

                // if editor simulation checked, setup simulated marker
                MobileDevice.Camera.UseSimulatorCam = Config.IsEditorSimulation;
            }
            Simulation.SetActive(Config.IsEditorSimulation);
            if (_markerTracker != null)
            {
                var filter = ARMarkerTracker.FilterType.None;
                if (Config.UseEKF) filter = ARMarkerTracker.FilterType.EKF;
                if (Config.UseKF) filter = ARMarkerTracker.FilterType.KF;
                _markerTracker.Filter = filter;
            }
            if (MarkerVisualizer != null)
            {
                MarkerVisualizer.UseGyroSync = Config.UseGyroSync;
            }
        }

        private void Update()
        {
            //DebugCamTex.mainTexture = _markerDetector.DebugCamTex;
            if (Input.GetKey(KeyCode.Home))
            {
                Log.Info("key = home");
            }

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Log.Info("key = escape");
                if (Application.platform == RuntimePlatform.Android)
                {
                    AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                    activity.Call<bool>("moveTaskToBack", true);
                }
                else
                {
                    Application.Quit();
                }
            }
        }

        private void OnCameraStreamReady()
        {
            Log.Info("OnCameraStream");
            if (Config.UseDetectorThread)
                _arDetector.StartTracking();

            MobileDevice.Camera.OnCameraSetup.RemoveListener(OnCameraStreamReady);
        }

        private void UnitySettings()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            //Application.RequestUserAuthorization(UserAuthorization.WebCam);

            QualitySettings.vSyncCount = 0;
            // default for mobile devices are 30 fps
            //Application.targetFrameRate = 300;

            var vSyncCount = QualitySettings.vSyncCount;
            var targetFrameRate = Application.targetFrameRate;

            Log.Info("vSnyc = {0}, targetFPS = {1}", vSyncCount, targetFrameRate);

            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
        }

        private void OnDestroy()
        {
            Config.WritePrefs();
        }
    }
}