//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace nextPlace.ARVideoPlayer
//{
//    public static class DeviceRotation
//    {
//        private static bool gyroInitialized = false;

//        public static bool HasGyroscope
//        {
//            get
//            {
//                return SystemInfo.supportsGyroscope;
//            }
//        }

//        public static Quaternion Get()
//        {
//            if (!gyroInitialized)
//            {
//                InitGyro();
//            }

//            return HasGyroscope
//                ? ReadGyroscopeRotation()
//                : Quaternion.identity;
//        }

//        private static void InitGyro()
//        {
//            if (HasGyroscope)
//            {
//                Input.gyro.enabled = true;                // enable the gyroscope
//                Input.gyro.updateInterval = 0.0167f;    // set the update interval to it's highest value (60 Hz)
//            }
//            gyroInitialized = true;
//        }

//        private static Quaternion ReadGyroscopeRotation()
//        {
//            //(0.5,0.5,-0.5,0.5) -> (90, 90 , 0), (0,0,1,0) -> (0, 0, 180)
//            return new Quaternion(0.5f, 0.5f, -0.5f, 0.5f) * Input.gyro.attitude * new Quaternion(0, 0, 1, 0);
//        }
//    }
//}
