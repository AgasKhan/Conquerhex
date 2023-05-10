using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEntity : AudioManager
{
    // Start is called before the first frame update

    [SerializeField]
    string damagedLifeAudio = "DamagedLife";

    [SerializeField]
    string damagedRegenAudio = "DamagedRegen";


    [SerializeField]
    string teleportAudio = "TeleportAudio";

    void Start()
    {
        var entity = GetComponent<Entity>();

        if (audios.ContainsKey(damagedLifeAudio))
        {
            entity.health.lifeUpdateAmount += DamagedLifeAudio;
        }
        if (audios.ContainsKey(damagedRegenAudio))
        {
            entity.health.regenUpdateAmount += DamagedRegenAudio;
        }    

        if(entity is DinamicEntity)
        {
            var aux = (DinamicEntity)entity;

            if(audios.ContainsKey(teleportAudio))
                aux.move.onTeleport += TeleportAudio;
        }    
    }

    private void TeleportAudio()
    {
        Play(teleportAudio);
    }

    void DamagedLifeAudio(float obj)
    {
        if (obj > 0)
            Play(damagedLifeAudio);
    }

    void DamagedRegenAudio(float obj)
    {
        if (obj > 0)
            Play(damagedRegenAudio);
    }
}
