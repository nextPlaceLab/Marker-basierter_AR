using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace nextPlace.ARVideoPlayer
{
    class TextureUtils
    {
        public static Texture2D RTImage(Camera cam, int mWidth, int mHeight)
        {
            Rect rect = new Rect(0, 0, mWidth, mHeight);
            RenderTexture renderTexture = new RenderTexture(mWidth, mHeight, 24);
            Texture2D screenShot = new Texture2D(mWidth, mHeight, TextureFormat.RGBA32, false);

            cam.targetTexture = renderTexture;
            cam.Render();

            RenderTexture.active = renderTexture;
            screenShot.ReadPixels(rect, 0, 0);

            cam.targetTexture = null;
            RenderTexture.active = null;

            UnityEngine.Object.Destroy(renderTexture);
            renderTexture = null;
            return screenShot;
        }
    }
}
