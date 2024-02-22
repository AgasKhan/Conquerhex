using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NexusController : TurretController
{
    protected override void Config()
    {
        base.Config();
        MyStarts = null;
    }

    public override void EnterBuild()
    {
        MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(true).SetWindow("Empezar Partida", "¿Estas seguro de querer empezar partida?").AddButton("Si", () => StartGame()).AddButton("No", () => MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false));
        turret.originalAbility = "Cut";
        turret.currentLevel++;
    }

    void StartGame()
    {
        for (int i = 0; i < turret.attack.flyweight.kataCombos.Length; i++)
        {
            turret.SetKataCombo(i);
        }
        //turret.interact.Remove("Construir");

        UpgradeLevel();

        MenuManager.instance.modulesMenu.ObtainMenu<SubMenus>().SetActiveGameObject(false);
        MenuManager.instance.modulesMenu.ObtainMenu<PopUp>().SetActiveGameObject(false);

        WavesManager.instance.StartWaves();
    }

    public override void UpgradeLevel()
    {
        turret.visible = true;
        turret.constructSprite.enabled = false;
        transform.GetChild(0).SetActiveGameObject(true);
        turret.ChangeSprite(turret.myStructure.possibleAbilities["Cut"][0]);
    }

    protected override void DestroyTurret()
    {
        base.DestroyTurret();
        turret.currentLevel = 0;

        TimersManager.Create(1f, 0f, 2, Mathf.Lerp, (save) => Time.timeScale = save).AddToEnd(() =>
        {
            GameManager.instance.Pause(true);
            MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(true).SetWindow("Derrota", "¿Deseas volver a jugar?").AddButton("Si", () => LoadSystem.instance.Reload()).AddButton("No", () => LoadSystem.instance.Load("MainMenu"));
        }).SetUnscaled(true);
    }
}
