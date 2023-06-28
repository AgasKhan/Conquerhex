using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBuilding : AudioManager
{
    [SerializeField]
    string craftAudio = "CraftAudio";

    private void Start()
    {
        var building = GetComponent<CraftingBuild>();

        if (audios.ContainsKey(craftAudio))
        {
            building.createSubMenu.onCraft += CraftSound;
        }

    }
    private void CraftSound()
    {
        Play(craftAudio);

        Debug.Log("----------Se reprodujo el sonido--------------");
    }
}
