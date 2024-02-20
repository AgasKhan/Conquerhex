using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComponentsAndContainers;

public class AudioEntityComponent : AudioManager, IComponent<Entity>
{
    // Start is called before the first frame update

    [SerializeField]
    string damagedLifeAudio = "DamagedLife";

    [SerializeField]
    string damagedRegenAudio = "DamagedRegen";


    [SerializeField]
    string teleportAudio = "TeleportAudio";

    public Entity container {get; private set;}

    public T GetInContainer<T>() where T : IComponent<Entity> => container.GetInContainer<T>();

    public void RemoveInContainer<T>() where T : IComponent<Entity> => container.RemoveInContainer<T>();

    public void AddInContainer<T>(T component) where T : IComponent<Entity> => container.AddInContainer(component);

    public bool TryGetInContainer<T>(out T component) where T : IComponent<Entity> => container.TryGetInContainer(out component);

    public void OnEnterState(Entity entity)
    {
        container = entity;

        if (audios.ContainsKey(damagedLifeAudio))
        {
            entity.health.lifeUpdate += Health_lifeUpdate;
        }
        if (audios.ContainsKey(damagedRegenAudio))
        {
            entity.health.regenUpdate += Health_regenUpdate;
        }

        if (entity.TryGetInContainer<MoveEntityComponent>(out var move))
        {
            if (audios.ContainsKey(teleportAudio))
                move.move.onTeleport += TeleportAudio;
        }
    }

    public void OnExitState(Entity param)
    {
        container = null;
    }

    public void OnStayState(Entity param)
    {
        throw new System.NotImplementedException();
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
