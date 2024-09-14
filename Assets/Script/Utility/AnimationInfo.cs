using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Animation with independent events
/// </summary>
[CreateAssetMenu(menuName = "Scriptables/Animation", fileName = "new AnimationInfo")]
public class AnimationInfo : ScriptableObject
{
    public AnimationClip animationClip;

    public Pictionarys<float, string> events = new Pictionarys<float, string>();

    private void OnValidate()
    {
        if(events.Count==0)
            Bake();
    }

    [ContextMenu("Bake anims events")]
    void Bake()
    {
        foreach (var item in animationClip.events)
        {
            events.CreateOrSave(item.time, item.stringParameter);
        }
    }

    [ContextMenu("Delete all old events")]
    void Delete()
    {
        animationClip.events = new AnimationEvent[0];
    }
}
