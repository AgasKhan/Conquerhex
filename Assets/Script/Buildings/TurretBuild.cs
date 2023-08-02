using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Model
public class TurretBuild : Building
{
    //[HideInInspector]
    //public Sprite baseSprite;

    public override bool visible { get => myVisibility; set => myVisibility = value; }

    [SerializeField]
    private bool myVisibility;

    public override string rewardNextLevel => throw new System.NotImplementedException();

    public string originalAbility = "";

    [HideInInspector]
    public TurretStructure turretStructure => flyweight as TurretStructure;

    protected override void Config()
    {
        base.Config();

        MyAwakes += MyAwake;

        MyStarts = null;
    }
    void MyAwake()
    {
        //baseSprite = GetComponentInChildren<AnimPerspecitve>().sprite;
    }

    public void DestroyTurret()
    {
        GetComponentInChildren<AnimPerspecitve>().sprite = flyweight.image;

        //Que la vida se resetee
        health.TakeLifeDamage(-1000);
        health.TakeRegenDamage(-100);

        //Que las habilidades se desequipen
        ActualKata(0).indexEquipedItem = -1;
        ActualKata(1).indexEquipedItem = -1;
        ActualKata(2).indexEquipedItem = -1;

        inventory.Clear();
        visible = false;

        foreach (var item in interact)
        {
            if (item.key == "Mejorar" || item.key == "Nivel Máximo")
                item.key = "Construir";
        }

        originalAbility = "";

        currentLevel = 0;
        SaveWithJSON.SaveInPictionary(flyweight.nameDisplay + "Level", currentLevel);
    }

    public override void UpgradeLevel()
    {
        base.UpgradeLevel();
        visible = true;
    }

    public void ChangeStructure(StructureBase newStructure)
    {
        flyweight = newStructure;
    }

    public void SetKataCombo(int index)
    {
        SetWeaponKataCombo(index);
    }

    public override void Interact(Character character)
    {
        base.Interact(character);

        interact["Información"].Activate(this);
    }

    public void ChangeSprite(Sprite sprite)
    {
        GetComponentInChildren<AnimPerspecitve>().sprite = sprite;
    }

}