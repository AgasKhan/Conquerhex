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
    public TurretStructure myStructure => flyweight as TurretStructure;

    [HideInInspector]
    public TurretStructure originalFlyweight;

    public SpriteRenderer constructSprite;

    public Node node;

    protected override void Config()
    {
        base.Config();
        MyAwakes += MyAwake;
    }

    public void MyAwake()
    {
        originalFlyweight = flyweight as TurretStructure;
        constructSprite = GetComponent<SpriteRenderer>();
    }

    public virtual void DestroyConstruction()
    {
        flyweight = originalFlyweight;
        visible = false;
        currentLevel = 0;

        ResetLife();

        if (node != null)
            node.cost = 1;

        foreach (var item in interact)
        {
            if (item.key == "Mejorar" || item.key == "Nivel Máximo")
                item.key = "Construir";
        }

        constructSprite.enabled = true;
        transform.GetChild(0).SetActiveGameObject(false);
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
        constructSprite.enabled = false;
        transform.GetChild(0).SetActiveGameObject(true);
        if(node!= null)
            node.cost = -1;
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
