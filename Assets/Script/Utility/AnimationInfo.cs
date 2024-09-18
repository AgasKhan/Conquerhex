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
        /*
        if (animClips.Count > 0)
        {
            animClips[0].offsetTime = 0;
            //animClips[0].previusIndexAnim = -1;
        }   

        for (int i = 1; i < animClips.Count; i++)
        {
            //animClips[i].offsetTime = animClips[animClips[i].previusIndexAnim].Length;
            animClips[i].offsetTime = animClips[i-1].Length;
        }
        */
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
