using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectorManager : SingletonMono<DirectorManager>
{
    [SerializeField] Character _player;
    [SerializeField] ReSpawner[] _spawners;

    int enemiesKilled;
    private int blocked;

    private Hexagone _initial;

    private Timer t;

    public void MyAwake()
    {
        Spawning();
        _player.health.death += () =>
        {
            t = TimersManager.Create(1f,
                () => UnityEngine.SceneManagement.SceneManager.LoadScene("Hexagonos test"));
        };
    }

    void Spawning()
    {
        Debug.Log("Spawn");
        foreach (var item in _spawners)
        {
            Entity e = item.TryRespawn();
            e.health.death += BreakItems;
            e.GetComponent<IA_GenericEnemy>().attackDetection.maxRadius = 100;
            
        }
    }

    [ContextMenu("BreakItems")]
    void BreakItems()
    {
        BreakRandomCombo();
        
        
        enemiesKilled++;
        Debug.Log(enemiesKilled);
        if(enemiesKilled % 2 != 0) return;

        BreakRandomAbility();
        BreakRandomKataCombo();
        
        if(enemiesKilled >= 20) _player.TakeDamage(Damage.Create<DamageTypes.Perforation>(100));
    }

    public void BreakRandomAbility()
    {
        int ran = Random.Range(2, _player.caster.abilities.Count);

        if (_player.caster.abilities[ran].isBlocked == true) return;
        
        UI.Interfaz.instance["Notificacion"]
            .ShowMsg(
                $"La habilidad {_player.caster.abilities[ran].defaultItem?.nameDisplay} se ha roto!"
                    .RichTextColor(Color.red));
        _player.caster.abilities[ran].isBlocked = true;
        _player.caster.abilities[ran].equiped?.Unequip();
    }

    public void BreakRandomCombo()
    {
        int ran = Random.Range(1, _player.caster.combos.Count);
        if (ran == 5 || ran == 10) ran++; //Hardcodeado para que los combos del click izq no se bloqueen

        if (_player.caster.combos[ran].isBlocked == true) return;

        _player.caster.combos[ran].isBlocked = true;
        _player.caster.combos[ran].equiped?.Unequip();
        UI.Interfaz.instance["Notificacion"].ShowMsg($"Un combo se ha perdido!".RichTextColor(Color.red));
    }

    [ContextMenu("BreakWeapons")]
    public void BreakRandomWeapon()
    {
        int ran = Random.Range(1, _player.caster.weapons.Count);

        //Ver como eliminar un arma del inventario
        _player.caster.actualWeapon?.Unequip();
        _player.caster.weapons[ran].inventoryComponent[0].Destroy();

        UI.Interfaz.instance["Notificacion"]
            .ShowMsg($"el arma {_player.caster.actualWeapon.nameDisplay} se ha roto!".RichTextColor(Color.red));
    }

    public void BreakRandomKataCombo()
    {
        int ran = Random.Range(1, _player.caster.katas.Count);

        if (_player.caster.katas[ran].isBlocked == true) return;

        _player.caster.katas[ran].isBlocked = true;
        _player.caster.katas[ran].equiped?.Unequip();
        UI.Interfaz.instance["Notificacion"]
            .ShowMsg(
                $"la kata {_player.caster.katas[ran].defaultItem?.nameDisplay} se ha roto!".RichTextColor(Color.red));
    }
}