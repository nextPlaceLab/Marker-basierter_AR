using System;
using UnityEngine;

namespace nextPlace.ARVideoPlayer
{
    [Serializable]
    public class Config
    {
#if !DEBUG
        public const bool IsEditorSimulation = false;
#else
        public bool IsEditorSimulation;
#endif

        //public bool IsDetectMaker= true;

        public bool UseDetectorThread = true;
        public bool UseEKF = false;
        public bool UseKF = true;
        public bool UseGyroSync = true;
        public bool UseGyroEKF = true;

        private const string _detectorThread = "detectorThread";
        private const string _usePoseEKF = "poseEKF";
        private const string _usePoseKF = "poseKF";
        private const string _useGyroSync = "gyroSync";
        private const string _useGyroEKF = "gyroEKF";
        private const string _init = "init";

        public void WritePrefs()
        {
            Log.Info("WRITE prefs, poseKF = {0}, poseEKF = {1}", UseKF, UseEKF);

            PlayerPrefs.SetInt(_detectorThread, UseDetectorThread ? 1 : 0);
            PlayerPrefs.SetInt(_usePoseEKF, UseEKF ? 1 : 0);
            PlayerPrefs.SetInt(_usePoseKF, UseKF ? 1 : 0);
            PlayerPrefs.SetInt(_useGyroSync, UseGyroSync ? 1 : 0);
            PlayerPrefs.SetInt(_useGyroEKF, UseGyroEKF ? 1 : 0);
            PlayerPrefs.SetInt(_init, 1);
            PlayerPrefs.Save();
        }

        public void ReadPrefs()
        {
            if (PlayerPrefs.GetInt(_init, 0) == 0)
            {
                // write default values
                WritePrefs();
                return;
            }
            Log.Info("READ prefs, poseKF = {0}, poseEKF = {1}", UseKF, UseEKF);

            UseDetectorThread = PlayerPrefs.GetInt(_detectorThread) == 1 ? true : false;
            UseEKF = PlayerPrefs.GetInt(_usePoseEKF) == 1 ? true : false;
            UseKF = PlayerPrefs.GetInt(_usePoseKF) == 1 ? true : false;
            UseGyroSync = PlayerPrefs.GetInt(_useGyroSync) == 1 ? true : false;
            UseGyroEKF = PlayerPrefs.GetInt(_useGyroEKF) == 1 ? true : false;
        }
    }
}