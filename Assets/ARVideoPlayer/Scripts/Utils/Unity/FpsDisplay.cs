using nextPlace.ARVideoPlayer;
using System.Diagnostics;
using UnityEngine;

public class FpsDisplay : MonoBehaviour
{
    private float deltaTime = 0.0f;
    private float deltaTime1 = 0.0f;

    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    private void OnGUI()
    {
        int w = Screen.width, h = Screen.height;
        GUIStyle style = new GUIStyle();
        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperRight;
        style.fontSize = h * 2 / 100 * 2;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }
}

public class FPSMeasure
{
    private Stopwatch sw = new Stopwatch();
    private int _interval;
    private int _count;
    private bool _log;
    private string _name;

    public float FPS { get; private set; }

    public FPSMeasure(string name, int interval = 30, bool start = true, bool log = true)
    {
        _name = name;
        _interval = interval;
        _log = log;
        if (start)
            sw.Start();
    }
    public void Start()
    {
        sw.Restart();
    }
    public void Lap()
    {
        _count++;
        if (_count == _interval)
        {
            if (sw.ElapsedMilliseconds > 0)
            {
                FPS = 1000f / sw.ElapsedMilliseconds;
            }
            else
            {
                FPS = 0;
            }
            FPS *= _interval;

            if (_log)
                Log.Info("FPS({0}) = {1:F2}", _name, FPS);

            _count = 0;
            sw.Restart();
        }
    }
}