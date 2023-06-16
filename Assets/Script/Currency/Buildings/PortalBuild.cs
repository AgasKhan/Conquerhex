using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalBuild : Building
{
    public  override string rewardNextLevel => throw new System.NotImplementedException();

    protected override void Config()
    {
        base.Config();

        MyAwakes += MyAwake;
    }
    void MyAwake()
    {
        
    }

    void Internal()
    {

    }
}

public class PortalSubMenu : CreateSubMenu
{
    public Building buildingBase;

    protected override void InternalCreate()
    {
        subMenu.ClearBody();

        subMenu.CreateSection(1, 5);
        subMenu.AddComponent<DetailsWindow>().SetTexts(buildingBase.structureBase.nameDisplay, buildingBase.structureBase.GetDetails().ToString());
        subMenu.AddComponent<DetailsWindow>().SetTexts("", "Costo del viaje: ");
        subMenu.AddComponent<EventsCall>().Set("Viajar al mundo aleatorio",null,"");
        subMenu.AddComponent<EventsCall>().Set("Viajar a mazmorras", null, "");

    }
}
