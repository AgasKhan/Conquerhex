using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsGiver : LogicActive<Entity>
{
    public List<Ingredient> items = new List<Ingredient>();

    public override void Activate(Entity genericParams)
    {
        foreach (var item in items)
        {
            genericParams.GetInContainer<InventoryEntityComponent>().AddItem(item.Item, item.Amount);
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
