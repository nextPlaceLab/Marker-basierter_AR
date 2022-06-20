using UniRx;
using UnityEngine;
using UnityEngine.Android;
using UtyMap.Unity;

namespace nextPlace.ARVideoPlayer
{
    public class MobileDevice : MonoBehaviour
    {
        public OCVWebCam Camera;

        public GeoPose CurrentPose => LastPose;

        public bool UseGyroEKF { get; set; } //= true;

        //public GeoPose CurrentPose => GetDevicePose();
        public GeoPose LastPose { get; private set; }

        public ARFrame CurrentFrame;

        public bool IsDeviceReady { get; private set; }
        public GenericEvent<ARFrame> OnNewFrame = new GenericEvent<ARFrame>();
        public GenericEvent OnDeviceReady = new GenericEvent();
        private FPSMeasure _camFPS = new FPSMeasure("MobileDeviceCam");
        private const bool _IsDebug = false;
        private QuaternionEKF gyroFilter = new QuaternionEKF();

        private Quaternion fixRot;
        private void Awake()
        {
            //ao = Application.RequestUserAuthorization(UserAuthorization.l);
            if (Application.platform == RuntimePlatform.Android )
                Permission.RequestUserPermission(Permission.FineLocation);
        }

        private void Start()
        {
            Camera.OnCameraSetup.AddListener(OnCameraSetup);
            Camera.Subscribe(OnNewCamFrame);
        }

        private void OnCameraSetup()
        {
            InitDevice();
            //_isDeviceReady = true;
            OnDeviceReady.Invoke();
            Log.Info("Mobile device ready");
        }

        [ContextMenu("Init LocationServices")]
        private void InitDevice()
        {
            if (Input.location.isEnabledByUser)
            {
                Input.location.Start();
                //Input.compass.enabled = true;
                Input.compass.enabled = false;
                Input.gyro.enabled = true;
                //DeviceRotation.Get();
                //Input.gyro.updateInterval = -1;
                Input.gyro.updateInterval = 0.01f;
                Log.Info("LocationServices enabled!, gyro update = {0}", Input.gyro.updateInterval);
            }
            else
            {
                Log.Error("LocationServices not enabled!");
            }
        }

        private void OnNewCamFrame(WebCamFrame webCamFrame)
        {
            if (webCamFrame.Tex == null && webCamFrame.IsSimulated == false)
            {
                Log.Error("Tex == null!!! WTF");
                return;
            }
            //Log.Info("Current Frame = {0}", webCamFrame.FrameID);
            // NOTE TODO wait until frame.width >1000!!!!
            //Ich depp
            if (webCamFrame.IsSimulated == false && webCamFrame.Tex.width < 100)
            {
                Log.Error("Wtf? Frame.width < 100");
                return;
            }
            CurrentFrame = new ARFrame(webCamFrame, GetDevicePose());
            IsDeviceReady = true;
            OnNewFrame.Invoke(CurrentFrame);
            _camFPS.Lap();
        }

        private void Update()
        {
            GetDevicePose();
            if (_IsDebug)
            {
                //var test = Input.gyro.attitude;
                //Log.Info("t={0}, d={1}", test.eulerAngles, LastPose.Orientation.eulerAngles);
                if (Input.GetMouseButton(0))
                {
                    fixRot = LastPose.Orientation;
                }
                var dEuler = fixRot.eulerAngles - LastPose.Orientation.eulerAngles;
                UIDisplay.current.Set("delta = {0}", dEuler);
            }
        }

        private GeoPose GetDevicePose()
        {
            if (!IsDeviceReady || Input.location.status != LocationServiceStatus.Running) return default(GeoPose);

            // update location/gps

            var location = Input.location.lastData;
            var geoCoordinate = new GeoCoordinate(location.latitude, location.longitude);
            var altitude = location.altitude;
            var heading = Input.compass.trueHeading;

            // Update Gyroscope data
            //GyroModifyCamera();
            //var orientation = GyroToUnity(Input.gyro.attitude);
            var orientation = ReadGyroscopeRotation();
            //var euler = orientation.eulerAngles;
            //var val = string.Format("({0:F0}, {1:F0}, {2:F0})", euler.x, euler.y, euler.z);
            //UIDisplay.current.Set("Device", val);
            Quaternion filteredOrientation = Quaternion.identity;

            if (UseGyroEKF)
            {
                filteredOrientation = gyroFilter.Update(orientation);
                //euler = filteredOrientation.eulerAngles;
                //val = string.Format("({0:F0}, {1:F0}, {2:F0})", euler.x, euler.y, euler.z);
                //UIDisplay.current.Set("DeviceFiltered", val);

                //euler = filteredOrientation.eulerAngles - orientation.eulerAngles;
                //val = string.Format("({0:F3}, {1:F3}, {2:F3})", euler.x, euler.y, euler.z);
                //UIDisplay.current.Set("Delta = ", val);
            }

            LastPose = new GeoPose()
            {
                GeoCoordinate = geoCoordinate,
                Altitude = altitude,
                Heading = heading,
                Orientation = UseGyroEKF ? filteredOrientation : orientation
                //Orientation = orientation
            };
            return LastPose;
        }

        /********************************************/

        private static Quaternion ReadGyroscopeRotation()
        {
            //(0.5,0.5,-0.5,0.5) -> (90, 90 , 0), (0,0,1,0) -> (0, 0, 180)
            return new Quaternion(0.5f, 0.5f, -0.5f, 0.5f) * Input.gyro.attitude * new Quaternion(0, 0, 1, 0);
        }

        // The Gyroscope is right-handed.  Unity is left handed.
        // Make the necessary change to the camera.
        private void GyroModifyCamera()
        {
            transform.rotation = GyroToUnity(Input.gyro.attitude);
        }

        private static Quaternion GyroToUnity(Quaternion q)
        {
            return new Quaternion(q.x, q.y, -q.z, -q.w);
        }
    }
}