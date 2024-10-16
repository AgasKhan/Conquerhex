using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectorManager : SingletonMono<DirectorManager>
{
    [SerializeField] Character _player;
    [SerializeField] ReSpawner[] _spawners;

    int enemiesKilled;

    Timer t;

    public void MyAwake()
    {
       Spawning();
    }

    void Spawning()
    {
        Debug.Log("Spawn");
        foreach (var item in _spawners)
        {
            Entity e = item.TryRespawn();
            e.health.death += BreakItems;
        }
    }

    [ContextMenu("BreakItems")]
    void BreakItems()
    {
        enemiesKilled++;
        if(enemiesKilled % 2 != 0) return;

        BreakRandomAbility();
        BreakRandomCombo();
        BreakRandomKataCombo();
    }

    public void BreakRandomAbility()
    {
        int ran = Random.Range(2, _player.caster.abilities.Count);

        _player.caster.abilities[ran].isBlocked = true;
        _player.caster.abilities[ran].equiped?.Unequip();
        Debug.Log("Habilidad rota!");
    }

    public void BreakRandomCombo()
    {
        int ran = Random.Range(1, _player.caster.combos.Count);

        _player.caster.combos[ran].isBlocked = true;
        _player.caster.combos[ran].equiped?.Unequip();
        Debug.Log("Combo roto!");
    }

    [ContextMenu("BreakWeapons")]
    public void BreakRandomWeapon()
    {
        int ran = Random.Range(1, _player.caster.weapons.Count);

        _player.caster.actualWeapon?.Unequip();
        _player.inventory.SubstracItems(_player.caster.actualWeapon.itemBase, 1);
        
        Debug.Log("Arma rota!");
    }

    public void BreakRandomKataCombo()
    {
        int ran = Random.Range(1, _player.caster.katas.Count);

        _player.caster.katas[ran].isBlocked = true;
        _player.caster.katas[ran].equiped?.Unequip();
        Debug.Log("Kata rota!");
    }


}
