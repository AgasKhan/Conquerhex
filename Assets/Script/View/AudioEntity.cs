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
            entity.health.lifeUpdate += Health_lifeUpdate;
        }
        if (audios.ContainsKey(damagedRegenAudio))
        {
            entity.health.regenUpdate += Health_regenUpdate;
        }

        if(entity is DynamicEntity)
        {
            var aux = (DynamicEntity)entity;

            if(audios.ContainsKey(teleportAudio))
                aux.move.onTeleport += TeleportAudio;
        }
    }

    private void Health_regenUpdate(IGetPercentage percentage, float number)
    {
        DamagedRegenAudio(number);
    }

    private void Health_lifeUpdate(IGetPercentage percentage, float number)
    {
        DamagedLifeAudio(number);
    }

    private void TeleportAudio(Hexagone teleport, int lado)
    {
        Play(teleportAudio);
    }

    void DamagedLifeAudio(float obj)
    {
        if (obj < 0)
            Play(damagedLifeAudio);
    }

    void DamagedRegenAudio(float obj)
    {
        if (obj < 0)
            Play(damagedRegenAudio);
    }
}
