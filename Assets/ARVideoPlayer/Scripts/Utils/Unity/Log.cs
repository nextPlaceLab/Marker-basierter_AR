#define MY_FIELD

using System.Diagnostics;
using System.Text;
using System.Threading;
using Debug = UnityEngine.Debug;
using Object = System.Object;

namespace nextPlace.ARVideoPlayer

{
    public static class Log
    {
#if DEBUG
        private const bool isActive = true;

        private const bool isLogInfo = true;
        private const bool isLogWarning = isLogInfo;
        private const bool isLogError = isLogInfo;

#else
        private const bool isActive = false;

        private const bool isLogInfo = false;
        private const bool isLogWarning = false;
        private const bool isLogError = false;

#endif

        private const bool isAddStackInfo = true;
        //private const bool isLogInfo = true;
        //private const bool isLogWarning = true;
        //private const bool isLogError = true;

        
        private static Stopwatch timeSinceStartup = Stopwatch.StartNew();

        public static void Info(string msg, params Object[] args)
        {
            if (isActive && isLogInfo )
            {
                var displayMsg = string.Format(AddMetaData(msg), args);

                if (isLogInfo)
                    Debug.Log(displayMsg);
            }
        }

        public static void Warning(string msg, params Object[] args)
        {
            if (isActive && isLogWarning)
            {
                var displayMsg = string.Format(AddMetaData(msg), args);

                if (isLogWarning)
                    Debug.LogWarning(displayMsg);
            }
        }

        public static void Error(string msg, params Object[] args)
        {
            if (isActive && isLogError)
            {
                var displayMsg = string.Format(AddMetaData(msg), args);

                if (isLogError)
                    Debug.LogError(displayMsg);
            }
            //if (isLogError && isActive)
            //    Debug.LogErrorFormat(AddMetaData(msg), args);
        }

        private static string AddMetaData(string msg)
        {
            var sb = new StringBuilder();

            var threadId = Thread.CurrentThread.ManagedThreadId;

            sb.Append("[");

            sb.Append(string.Format("{0:F3}", timeSinceStartup.ElapsedMilliseconds / 1000f));

            sb.Append("] ");
            sb.Append("[");
            sb.Append(string.Format("{0:D3}", threadId));
            sb.Append("] ");

            if (isAddStackInfo)
            {
                var method = new StackFrame(2).GetMethod(); // each stackframe needs to be created
                sb.Append("[");
                sb.Append(method.DeclaringType.Name);
                sb.Append(".");
                sb.Append(method.Name);
                sb.Append("()]: ");
            }

            sb.Append(msg);
            msg = sb.ToString();
            return msg;
        }

        //private static string FormatMsg(string msg)
        //{
        //    var sb = new StringBuilder();

        // var threadId = Thread.CurrentThread.ManagedThreadId;

        // sb.Append("[Time: ");

        // sb.Append(string.Format("{0:F3}", timeSinceStartup.ElapsedMilliseconds / 1000f));

        // sb.Append("] "); sb.Append("[Thread: "); sb.Append(string.Format("{0:D3}", threadId));
        // sb.Append("] ");

        // if (isAddInfo) { var method = new StackFrame(2).GetMethod(); // each stackframe needs to
        // be created sb.Append("[Call: "); sb.Append(method.DeclaringType.Name); sb.Append(".");
        // sb.Append(method.Name); sb.Append("()]: "); }

        //    sb.Append(msg);
        //    msg = sb.ToString();
        //    return msg;
        //}
    }
}