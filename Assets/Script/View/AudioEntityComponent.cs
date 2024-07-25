using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComponentsAndContainers;

public class AudioEntityComponent : AudioManager, IComponent<Entity>
{
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



    public void OnSetContainer(Entity param)
    {
        container = param;
    }

    public void OnEnterState(Entity entity)
    {
        if (audios.ContainsKey(damagedLifeAudio))
        {
            entity.health.lifeUpdate += Health_lifeUpdate;
        }
        if (audios.ContainsKey(damagedRegenAudio))
        {
            entity.health.regenUpdate += Health_regenUpdate;
        }

        if (TryGetInContainer<InventoryEntityComponent>(out var inventory))
        {
            inventory.onNewItem += Inventory_onNewItem;
            inventory.onLostItem += Inventory_onLostItem;
            foreach (var item in inventory)
            {
                Inventory_onNewItem(item);
            }
        }

        if (TryGetInContainer<MoveEntityComponent>(out var move))
        {
            if (audios.ContainsKey(teleportAudio))
                move.onTeleport += TeleportAudio;
        }
    }

    public void OnStayState(Entity param)
    {
    }

    public void OnExitState(Entity entity)
    {
        if (audios.ContainsKey(damagedLifeAudio))
        {
            entity.health.lifeUpdate -= Health_lifeUpdate;
        }
        if (audios.ContainsKey(damagedRegenAudio))
        {
            entity.health.regenUpdate -= Health_regenUpdate;
        }

        if (TryGetInContainer<InventoryEntityComponent>(out var inventory))
        {
            inventory.onNewItem -= Inventory_onNewItem;
            inventory.onLostItem -= Inventory_onLostItem;
        }

        if (TryGetInContainer<MoveEntityComponent>(out var move))
        {
            if (audios.ContainsKey(teleportAudio))
                move.onTeleport -= TeleportAudio;
        }

        container = null;
    }

    private void Inventory_onLostItem(Item obj)
    {
        if (obj is WeaponKata weaponKata)
        {
            weaponKata.onEquipedWeapon -= WeaponKata_onEquipedWeapon;
        }
    }

    private void Inventory_onNewItem(Item obj)
    {
        if (obj is Ability ability)
        {
            AddAudio($"{ability.itemBase.name}-Cast", ability.itemBase.castAudio);

            foreach (var item in ability.itemBase.auxiliarAudios)
            {
                AddAudio($"{ability.itemBase.name}-{item.key}", item.value);
            }
        }

        if(obj is WeaponKata weaponKata)
        {
            weaponKata.onEquipedWeapon += WeaponKata_onEquipedWeapon;
        }
    }

    private void WeaponKata_onEquipedWeapon(MeleeWeapon obj)
    {
        if (obj == null)
            return;

        if(obj.itemBase.castAudio.clip!= null)
        {
            AddAudio($"{obj.itemBase.name}-Cast", obj.itemBase.castAudio);
        }

        foreach (var item in obj.itemBase.auxiliarAudios)
        {
            AddAudio($"{obj.itemBase.name}-{item.key}", item.value);
        }
    }

    private void Health_lifeUpdate(IGetPercentage percentage, float number)
    {
        DamagedLifeAudio(number);
    }
    private void Health_regenUpdate(IGetPercentage percentage, float number)
    {
        DamagedRegenAudio(number);
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
