using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsGiver : LogicActive<Entity>
{
    public List<Ingredient> items = new List<Ingredient>();

    public override void Activate(Entity genericParams)
    {
        for (int i = 0; i < items.Count; i++)
        {
            genericParams.GetInContainer<InventoryEntityComponent>().AddItem(items[i].Item, items[i].Amount);
        }
    }

    public void GiveWeapon()
    {
        GameManager.instance.playerCharacter.GetInContainer<InventoryEntityComponent>().AddItem(items[0].Item, 1);

        MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(true)
                .SetWindow("Felicidades", "Has obtenido: \n" + items[0].Item.nameDisplay)
                .AddButton("Aceptar", () => { GameManager.instance.Menu(false); MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(false); });

    }
}
