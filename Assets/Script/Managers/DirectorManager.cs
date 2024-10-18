using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectorManager : SingletonMono<DirectorManager>
{
    [SerializeField] Character _player;
    [SerializeField] ReSpawner[] _spawners;
    [SerializeField] private Transform _puertacasa;
    
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
                () => GameManager.instance.Load("Hexagonos test"));
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
        if(enemiesKilled % 2 != 0) return;

        BreakRandomAbility();
        BreakRandomKataCombo();

        if (enemiesKilled >= 20)
        {
            _puertacasa.gameObject.SetActive(false);
            UI.Interfaz.instance["Subtitulo"].ShowMsg($"La puerta se ha desbloqueado!".RichTextColor(Color.red));
        }
        //_puertacasa.Translate(_puertacasa.position + Vector3.down * 2 * Time.deltaTime, _puertacasa);

        //_player.TakeDamage(Damage.Create<DamageTypes.Perforation>(100));
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
        _player.caster.actualAbility?.Destroy();

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

    [ContextMenu("Noti coordenadas")]
    public void ConsoleCoords()
    {
        UI.Interfaz.instance["Notificacion"]
            .ShowMsg("Adquiriendo coordenadas de fortaleza......".RichTextColor(Color.cyan));

        TimersManager.Create(10f, () => CoordsTaken());
    }

    void CoordsTaken()
    {
        UI.Interfaz.instance["Notificacion"]
            .ShowMsg("Coordenadas adquiridas!".RichTextColor(Color.cyan));
        
        //TimersManager.Create(10f, () => );
    }
}