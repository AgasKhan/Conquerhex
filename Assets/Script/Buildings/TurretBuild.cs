using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Model
public class TurretBuild : BarrierBuild
{
    //public AttackEntityComponent attack;
    public InventoryEntityComponent invent;

    public string originalAbility = "";

    public override Sprite Image 
    { get 
        { 
            if (currentLevel == 0) return flyweight.image; 
            else return ((TurretStructure)flyweight).possibleAbilities[originalAbility][currentLevel - 1];
        } 
    }

    protected override void Config()
    {
        base.Config();
        MyStarts = null;
    }

    public override void DestroyConstruction()
    {
        base.DestroyConstruction();

        attack.ActualKata(0).indexEquipedItem = -1;
        attack.ActualKata(1).indexEquipedItem = -1;
        attack.ActualKata(2).indexEquipedItem = -1;

        transform.GetComponent<Collider2D>().isTrigger = true;
        invent.inventory.Clear();
        originalAbility = "";
    }

    public void SetKataCombo(int index)
    {
        //attack.SetWeaponKataCombo(index);
    }
    public void ChangeSprite(Sprite sprite)
    {
        GetComponentInChildren<ViewShadowController>().sprite = sprite;
    }

    public override void UpgradeLevel()
    {
        base.UpgradeLevel();
        transform.GetComponent<Collider2D>().isTrigger = false;
    }
}