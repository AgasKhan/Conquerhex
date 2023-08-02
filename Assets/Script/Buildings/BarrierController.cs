using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarrierController : BuildingsController
{
    public BarrierBuild barrier;

    protected override void Config()
    {
        base.Config();
        MyAwakes += MyAwake;
    }

    private void MyAwake()
    {
        barrier.health.noLife += barrier.DestroyBarrier;
        barrier.originalFlyweight = barrier.flyweight as TurretStructure;
    }
    
    public override void EnterBuild()
    {
        if(barrier.currentLevel == barrier.maxLevel)
        {
            MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(true).SetWindow("", "La barrera alcanzó el nivel máximo").AddButton("Cerrar", () => MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false));
            return;
        }

        if (barrier.upgradesRequirements[barrier.currentLevel].CanCraft(barrier.character))
        {
            barrier.upgradesRequirements[barrier.currentLevel].Craft(barrier.character);
            barrier.UpgradeLevel();
            MenuManager.instance.modulesMenu.ObtainMenu<SubMenus>().SetActiveGameObject(false);
        }
        else
            MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(true).SetWindow("", "No tienes los recursos necesarios").AddButton("Cerrar", () => MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false));
    }

    public override void UpgradeLevel()
    {
        barrier.ChangeStructure(barrier.barrierStructure.damagesUpgrades[0]);
        barrier.ChangeSprite(barrier.flyweight.image);
        barrier.ResetLife();
        barrier.currentLevel++;

        foreach (var item in barrier.interact)
        {
            if (item.key == "Construir")
                item.key = "Nivel Máximo";
        }
    }
}