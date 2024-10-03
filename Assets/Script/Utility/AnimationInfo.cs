using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Animation with independent events
/// </summary>
[CreateAssetMenu(menuName = "Scriptables/Animation", fileName = "new AnimationInfo")]
public class AnimationInfo : ScriptableObject
{
    enum MirrorState
    {
        NoMirror,
        Mirror,
        Random
    }

    [System.Serializable]
    public class Data
    {
        public AnimatorController.DefaultActions defaultAction;
        public AnimationClip animationClip;
        public bool inLoop;
        public float velocity = 1;
        public int nextIndex = -1;
        
        [SerializeReference]
        public Data nextAnimation;

        [SerializeField]
        MirrorState mirrorType;

        public bool mirror => mirrorType == MirrorState.NoMirror ? false : (mirrorType == MirrorState.Mirror ? true : Random.Range(0, 2) == 0);
        //public float offsetTime;
        //public int previusIndexAnim=-1;
        public float Length => animationClip.length;

        public Pictionarys<string, float> events = new();

        public void SetTimers(Dictionary<string, Timer> timers)
        {
            timers["End"].Set(Length);

            foreach (var item in events)
            {
                timers[item.key].Set(item.value);
            }
        }
    }

    public Pictionarys<string, Data> animClips = new();

    private void OnValidate()
    {
        foreach (var item in animClips)
        {
            if(item.value.nextIndex >= 0 && item.value.nextIndex < animClips.Count && item.value != animClips[item.value.nextIndex])
            {
                item.value.nextAnimation = animClips[item.value.nextIndex];
            }
        }
    }

    [ContextMenu("Bake anims events")]
    void Bake()
    {

        //AnimationClip prev = null;
        foreach (var clip in animClips)
        {
            clip.value.events.Clear();

            foreach (var evnt in clip.value.animationClip.events)
            {
                string name = evnt.stringParameter;
                //float timeOffset = prev != null ? prev.length : 0;
                if (name == "Cast")
                    name = "Action";

                clip.value.events.CreateOrSave(name, evnt.time /*+ timeOffset*/);
            }

            //prev = clip.value.animationClip;
        }
    }

    [ContextMenu("Delete all old events")]
    void Delete()
    {
        for (int i = 1; i < animClips.Count; i++)
        {
            animClips[i].animationClip.events = new AnimationEvent[0];
        }
    }
}
