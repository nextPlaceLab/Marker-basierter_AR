using UnityEngine;
using UnityEngine.Video;

namespace nextPlace.ARVideoPlayer
{
    public class VideoMarkerBehaviour : MarkerBehaviour
    {
        public bool togglePos;
        public VideoPlayer VideoPlayer;
        public Billboard billboard; 
        private GameObject VideoCanvas;
        private long frame;
        private Vector3 scale;
        private MoveToFrontBehaviour moveScript;
        private const bool _IsDebug = false;
        private bool _IsStartCalled;
        private bool _isInit;

        private void Start()
        {
            billboard.Eye = ARCamera.transform;
            if (VideoPlayer != null)
            {
                // set video source
                if (Preset.Clip != null)
                {
                    VideoPlayer.clip = Preset.Clip;
                }
                else
                {
                    VideoPlayer.url = Preset.VideoURL;
                }

                VideoPlayer.Prepare();
                VideoPlayer.prepareCompleted += VideoPlayer_prepareCompleted;

                VideoCanvas = VideoPlayer.gameObject;
                scale = VideoCanvas.transform.localScale;

                moveScript = VideoCanvas.GetComponent<MoveToFrontBehaviour>();
                if (moveScript != null)
                {
                    moveScript.Camera = ARCamera;
                    moveScript.MarkerSession = MarkerSession;

                    moveScript.gameObject.SetActive(true);
                }
                else
                {
                    Log.Error("Missing component: MoveToFrontBehaviour");
                }
            }
            else
            {
                Log.Error("VideoPlayer not assigned. Check Prefab!");
            }

            _IsStartCalled = true;
        }

        private void VideoPlayer_prepareCompleted(VideoPlayer source)
        {
            LoadIdleFrame(30, source);
            source.prepareCompleted -= VideoPlayer_prepareCompleted;
        }

        private void Update()
        {
            if (togglePos)
            {
                togglePos = false;
                moveScript.TogglePosition();
            }
        }

        private static void LoadIdleFrame(int frame, VideoPlayer vp)
        {
            frame = (int)Mathf.Min(frame, vp.frameCount);
            Log.Info("Set idle frame = {0}", frame);
            vp.Play();
            vp.frame = frame;
            vp.Pause();
        }

        public override void SetVisible(bool isVisible)
        {
            if (!_IsStartCalled) return;

            //if (gameObject.activeInHierarchy != isVisible)
            //{
            if (_IsDebug)
                Log.Info("Videoplayer.Play = {0}", isVisible);

            if (isVisible || moveScript.IsInFront)
            {
                VideoCanvas.transform.localScale = scale;
                //VideoPlayer.Play();
            }
            else
            {
                VideoPlayer.Pause();
                VideoCanvas.transform.localScale = Vector3.zero;
            }

            //}
        }
    }
}