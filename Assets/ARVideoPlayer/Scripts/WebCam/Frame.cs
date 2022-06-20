using OpenCvSharp;
using UnityEngine;
using UtyMap.Unity;
using static OpenCvSharp.Unity;

namespace nextPlace.ARVideoPlayer
{
    public class WebCamFrame
    {
        public WebCamTexture Tex { get; private set; }
        public bool IsSimulated { get; private set; }
        public Color32[] Pixels { get; private set; }
        public Vector2Int Size { get; private set; }
        public long FrameID { get; private set; }

        public Mat Mat
        {
            get
            {
                if (_mat == null)
                    _mat = ConvertToMat(Pixels, Size);
                return _mat;
            }
        }

        private Mat _mat;
        private TextureConversionParams _texParams;

        private static long _FrameCount = 0;

        protected WebCamFrame(WebCamFrame frame)
        {
            Tex = frame.Tex;
            Pixels = frame.Pixels;
            Size = frame.Size;
            _texParams = frame._texParams;
            _mat = null;

            FrameID = frame.FrameID;
        }

        public WebCamFrame(WebCamTexture tex, TextureConversionParams texParams)
        {
            Tex = tex;
            Pixels = tex.GetPixels32();
            //Pixels = new Color32[0];
            Size = new Vector2Int(tex.width, tex.height);
            _mat = null;
            _texParams = texParams;

            FrameID = _FrameCount++;
        }
        // for simulation camera

        public WebCamFrame(Texture2D tex)
        {
            Tex = null;
            IsSimulated = true;
            Pixels = tex.GetPixels32();
            //Pixels = new Color32[0];
            Size = new Vector2Int(tex.width, tex.height);
            _mat = null;
            _texParams = null;
            FrameID = _FrameCount++;
        }

        private Mat ConvertToMat(Color32[] pixels, Vector2Int size)
        {
            if (pixels.Length != size.x * size.y)
            {
                Log.Error("Pixel.lengt != size.w * size.h");
                return new Mat(0, 0, MatType.CV_8UC3);
            }
            return OpenCvSharp.Unity.PixelsToMat(pixels, size, _texParams);
        }

        public override string ToString()
        {
            return string.Format("Size = {0}", Size);
        }
    }

    public struct GeoPose
    {
        public GeoCoordinate GeoCoordinate;
        public float Altitude;
        public float Heading;
        public Quaternion Orientation;

        public override string ToString()
        {
            return string.Format("Geo. = {0}, alt.= {1:F2}, head. = {2:F2}, orient. = {3}", GeoCoordinate, Altitude, Heading, Orientation);
        }
    }

    public class ARFrame : WebCamFrame
    {
        public GeoPose DeviceOrientation;

        public ARFrame(GeoPose devicePose, WebCamTexture tex, TextureConversionParams texParams) : base(tex, texParams)
        {
            DeviceOrientation = devicePose;
        }

        public ARFrame(WebCamFrame frame, GeoPose devicePose) : base(frame)
        {
            DeviceOrientation = devicePose; 
        }

        public override string ToString()
        {
            return string.Format("{0} at {1}", base.ToString(), DeviceOrientation);
        }
    }
}