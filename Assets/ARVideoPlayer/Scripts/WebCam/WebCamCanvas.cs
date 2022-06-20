using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace nextPlace.ARVideoPlayer
{
    public class WebCamCanvas : MonoBehaviour
    {
        public RawImage background;
        public AspectRatioFitter aspectFitter;
        public TextMeshProUGUI text;

        [SerializeField]
        private WebCam _webCam;
        //private WebCamBehaviour _webCamBehaviour;

        private Texture defaultBackground;

        private void Start()
        {
            defaultBackground = background.texture;
            _webCam.OnFirstFrame.AddListener(OnCameraSetup);
            background.material = _webCam.CameraMaterial;
            background.transform.localScale = Vector3.one;
            background.transform.localPosition= Vector3.zero;
            background.transform.localRotation= Quaternion.identity;
        }

        private void OnCameraSetup()
        {
            background.SetMaterialDirty();
        }

        private void Update()
        {
            if (!_webCam.IsCameraFrameReady) return;
            aspectFitter.aspectRatio = _webCam.AspectRatio;

            //float scaleY = backCam.videoVerticallyMirrored ? -1f : 1f;
            //background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);
            if (_webCam.CamTexture.videoVerticallyMirrored)
                background.uvRect = new Rect(1, 0, -1, 1);  // means flip on vertical axis
            else
                background.uvRect = new Rect(0, 0, 1, 1);  // means no flip

            int orient = -_webCam.CamTexture.videoRotationAngle;
            background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);

            var info = string.Format("aspect ratio = {0}, orient = {1}, camTex.size(w,h) = {2}, {3}",
                _webCam.AspectRatio, orient, _webCam.CamTexture.width, _webCam.CamTexture.height);
            text.text = info;
        }
    }
}