using System;
using UnityEngine;
using UnityEngine.Video;

namespace nextPlace.ARVideoPlayer
{
    [Serializable]
    public class MarkerGOPreset
    {
        public int MarkerId;
        public Texture2D Image;
        public GameObject Prefab;
        public VideoClip Clip;
        public string VideoURL;
        public string SurveyURL;
    }

    public class MarkerBehaviour : MonoBehaviour
    {
        public Camera ARCamera;
        public MarkerSession MarkerSession;
        public MarkerGOPreset Preset;
        public virtual void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

    }
}