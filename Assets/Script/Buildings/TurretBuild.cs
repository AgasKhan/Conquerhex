using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Model
public class TurretBuild : BarrierBuild
{
    public string originalAbility = "";

    protected override void Config()
    {
        base.Config();
        MyStarts = null;
    }

    public override void DestroyConstruction()
    {
        base.DestroyConstruction();

        ActualKata(0).indexEquipedItem = -1;
        ActualKata(1).indexEquipedItem = -1;
        ActualKata(2).indexEquipedItem = -1;

        inventory.Clear();
        originalAbility = "";
    }

    public void SetKataCombo(int index)
    {
        SetWeaponKataCombo(index);
    }
    public void ChangeSprite(Sprite sprite)
    {
        GetComponentInChildren<AnimPerspecitve>().sprite = sprite;
    }
}