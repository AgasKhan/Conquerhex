using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventMediator : MonoBehaviour
{
    [SerializeField]
    UnityEvent[] unityEvents;


    public void TriggerEvent(int index)
    {
        unityEvents[Mathf.Clamp(index, 0, unityEvents.Length - 1)].Invoke();
    }
}
