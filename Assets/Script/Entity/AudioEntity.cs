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
            entity.health.lifeUpdate += Health_lifeUpdate; ;
        }
        if (audios.ContainsKey(damagedRegenAudio))
        {
            entity.health.regenUpdate += Health_regenUpdate;
        }

        if(entity is DinamicEntity)
        {
            var aux = (DinamicEntity)entity;

            if(audios.ContainsKey(teleportAudio))
                aux.move.onTeleport += TeleportAudio;
        }
    }

    private void Health_regenUpdate(IGetPercentage arg1, float arg2, float arg3)
    {
        DamagedRegenAudio(arg3);
    }

    private void Health_lifeUpdate(IGetPercentage arg1, float arg2, float arg3)
    {
        DamagedLifeAudio(arg3);
    }

    private void TeleportAudio(Hexagone teleport, int lado)
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
