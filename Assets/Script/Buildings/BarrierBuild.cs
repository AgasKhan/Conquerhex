using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierBuild : Building
{
    public override bool visible { get => myVisibility; set => myVisibility = value; }
    [SerializeField]
    private bool myVisibility;

    public override string rewardNextLevel => throw new System.NotImplementedException();

    [HideInInspector]
    public TurretStructure barrierStructure => flyweight as TurretStructure;

    [HideInInspector]
    public TurretStructure originalFlyweight;

    public void DestroyBarrier()
    {
        flyweight = originalFlyweight;
        visible = false;
        currentLevel = 0;

        ChangeSprite(barrierStructure.image);

        foreach (var item in interact)
        {
            if (item.key == "Mejorar" || item.key == "Nivel Máximo")
                item.key = "Construir";
        }
    }
    public void ResetLife()
    {
        health.TakeLifeDamage(-1000);
        health.TakeRegenDamage(-100);
    }

    public override void UpgradeLevel()
    {
        base.UpgradeLevel();
        visible = true;
    }
    public void ChangeSprite(Sprite sprite)
    {
        GetComponentInChildren<AnimPerspecitve>().sprite = sprite;
    }
    public void ChangeStructure(StructureBase newStructure)
    {
        flyweight = newStructure;
    }
    public override void Interact(Character character)
    {
        base.Interact(character);

        interact["Información"].Activate(this);
    }
}
