using UnityEngine;

namespace nextPlace.ARVideoPlayer
{
    internal class WebLinkBehavior : MonoBehaviour
    {
        public bool _enabled = false;
        public string URL;
        public string DefaultUrl = "https://essig-fabrik.de/";

        private void OnMouseDown()
        {
            if (_enabled)
                OpenURL();
        }

        public void OpenURL()
        {
            if (URL == "")
                Application.OpenURL(DefaultUrl);
            else
                Application.OpenURL(URL);
        }
    }
}