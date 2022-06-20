using UnityEngine;
using UnityEngine.UI;

namespace nextPlace.ARVideoPlayer
{
    public class SettingsUI_Behaviour : MonoBehaviour
    {
        public MainScript mainScript;
        private Config _config;

        public Toggle DetectorThread;
        public Toggle GyroEKF;
        public Toggle GyroSync;
        public Toggle PoseNone;
        public Toggle PoseKF;
        public Toggle PoseEKF;

        private void Awake()
        {
            Log.Info("Awake");
            _config = mainScript.Config;

            DetectorThread.onValueChanged.AddListener(SetDedicatedThread);
            GyroEKF.onValueChanged.AddListener(SetGyroFilter);
            GyroSync.onValueChanged.AddListener(SetGyroSync);
            PoseNone.onValueChanged.AddListener(SetPoseNone);
            PoseKF.onValueChanged.AddListener(SetPoseKF);
            PoseEKF.onValueChanged.AddListener(SetPoseEKF);
        }

        private void OnEnable()
        {
            UpdateFromConfig();
            Log.Info("Enabled");
        }

        private void OnDisable()
        {
            _config.WritePrefs();
            Log.Info("Disabled");
        }

        public void UpdateFromConfig()
        {
            Log.Info("Update from config");
            DetectorThread.SetIsOnWithoutNotify(_config.UseDetectorThread);
            GyroEKF.SetIsOnWithoutNotify(_config.UseGyroEKF);
            GyroSync.SetIsOnWithoutNotify(_config.UseGyroSync);
            PoseNone.SetIsOnWithoutNotify(!_config.UseEKF && !_config.UseKF);
            PoseKF.SetIsOnWithoutNotify(_config.UseKF);
            PoseEKF.SetIsOnWithoutNotify(_config.UseEKF);
        }

        public void SetDedicatedThread(bool state)
        {
            _config.UseDetectorThread = state;
        }

        public void SetGyroFilter(bool state)
        {
            _config.UseGyroEKF = state;
        }

        public void SetGyroSync(bool state)
        {
            _config.UseGyroSync = state;
        }

        public void SetPoseNone(bool state)
        {
            Log.Info("poseNone = {0}", state);
            if (state)
            {
                _config.UseEKF = false;
                _config.UseKF = false;
            }
        }

        public void SetPoseKF(bool state)
        {
            Log.Info("poseKF = {0}", state);
            _config.UseKF = state;
        }

        public void SetPoseEKF(bool state)
        {
            Log.Info("poseEKF = {0}", state);
            _config.UseEKF = state;
        }
    }
}