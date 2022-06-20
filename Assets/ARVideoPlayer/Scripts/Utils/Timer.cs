using System.Collections.Generic;
using System.Diagnostics;

namespace nextPlace.ARVideoPlayer
{
    internal class Timer
    {
        private static Dictionary<string, Stopwatch> _times = new Dictionary<string, Stopwatch>();

        public static void Start(string key)
        {
            if (!_times.ContainsKey(key))
                _times.Add(key, new Stopwatch());
            _times[key].Start();
        }

        public static float Stop(string key)
        {
            if (!_times.ContainsKey(key))
                _times.Add(key, new Stopwatch());
            _times[key].Stop();
            return Time(key);
        }

        public static float Time(string key)
        {
            if (!_times.ContainsKey(key))
                return 0f;
            else
                return _times[key].ElapsedMilliseconds;
        }

        public static void Reset(string key)
        {
            if (!_times.ContainsKey(key))
                _times.Add(key, new Stopwatch());
            _times[key].Reset();
        }

        public static float Restart(string key)
        {
            if (!_times.ContainsKey(key))
                _times.Add(key, new Stopwatch());
            var elapsed = _times[key].ElapsedMilliseconds;

            _times[key].Restart();

            return elapsed;
        }
    }
}