namespace nextPlace.ARVideoPlayer
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using UnityEngine.Video;

    public class VideoProgressBar : MonoBehaviour, IDragHandler, IPointerDownHandler
    {
        [SerializeField]
        private bool PlayOnStart = true;

        [SerializeField]
        private VideoPlayer videoPlayer;

        [SerializeField]
        private Image progress;

        [SerializeField]
        private Button btnPlay;

        [SerializeField]
        private Button btnPause;

        private bool hasPlayed;
        public GenericEvent OnStopPlaying = new GenericEvent();

        private void Awake()
        {
            btnPlay.onClick.AddListener(OnPlay);
            btnPause.onClick.AddListener(OnPause);
            //progress = GetComponent<Image>();
        }

        private void Start()
        {
            videoPlayer.playbackSpeed = 1;
            if (PlayOnStart)
                videoPlayer.Play();
        }

        private void Update()
        {
            if (videoPlayer.frameCount > 0 && hasPlayed)
                progress.fillAmount = videoPlayer.frame / (float)videoPlayer.frameCount;
            else
            {
                progress.fillAmount = 0;
            }
            if (videoPlayer.isPlaying && !hasPlayed)
            {
                // hack
                hasPlayed = videoPlayer.frame < 30;
                Log.Info("first update, frame => {0}", videoPlayer.frame);
            }
            if (!videoPlayer.isPlaying && hasPlayed)
            {
                OnStopPlaying.Invoke();
            }
        }

        private void OnPause()
        {
            videoPlayer.Pause();
        }

        private void OnPlay()
        {
            if (!hasPlayed)
            {
                videoPlayer.frame = 0;
            }
            videoPlayer.Play();
        }

        public void OnDrag(PointerEventData eventData)
        {
            TrySkip(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            TrySkip(eventData);
        }

        private void TrySkip(PointerEventData eventData)
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                progress.rectTransform, eventData.position, null, out localPoint))
            {
                float pct = Mathf.InverseLerp(progress.rectTransform.rect.xMin, progress.rectTransform.rect.xMax, localPoint.x);
                SkipToPercent(pct);
            }
        }

        private void SkipToPercent(float pct)
        {
            var frame = videoPlayer.frameCount * pct;
            videoPlayer.frame = (long)frame;
        }
    }
}