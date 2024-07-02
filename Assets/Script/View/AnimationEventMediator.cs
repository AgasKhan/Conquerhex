using UnityEngine;
using UnityEngine.Events;

public class AnimationEventMediator : MonoBehaviour
    {
        public AnimatorController reference;
        [SerializeField]
        UnityEvent[] unityEvents;

        public void TriggerEvent(int index)
        {
            unityEvents[Mathf.Clamp(index, 0, unityEvents.Length - 1)].Invoke();
        }
        public void AnimEvent(string str)
        {
            if (str == string.Empty)
              return;

         //Debug.Log("AnimEvent: " + str);
            reference?.animationEventMediator[str]?.Invoke();
        }
    }

