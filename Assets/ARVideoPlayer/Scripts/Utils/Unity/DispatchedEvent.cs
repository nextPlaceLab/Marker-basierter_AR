using System;
using UnityEngine.Events;

namespace nextPlace.ARVideoPlayer
{
    [Serializable]
    public class GenericEvent : UnityEvent { }

    [Serializable]
    public class GenericEvent<T0> : UnityEvent<T0> { }

    [Serializable]
    public class GenericEvent<T0, T1> : UnityEvent<T0, T1> { }

    [Serializable]
    public class GenericEvent<T0, T1, T2> : UnityEvent<T0, T1, T2> { }

    [Serializable]
    public class GenericEvent<T0, T1, T2, T3> : UnityEvent<T0, T1, T2, T3> { }
}