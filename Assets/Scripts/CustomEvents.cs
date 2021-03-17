using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace Launchship2DTiles
{
    [System.Serializable]
    public class CustomEvents : MonoBehaviour
    {

    }


}
[System.Serializable]
public class IntEvent : UnityEvent<int> { }
[System.Serializable]
public class StringEvent : UnityEvent<string> { }
[System.Serializable] public class BoolEvent : UnityEvent<bool> { }

[System.Serializable]
public class FloatEvent : UnityEvent<float> { }

[System.Serializable]
public class AudioEvent : UnityEvent { }