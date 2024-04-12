using ComponentsAndContainers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class InteractEntityComponent : ComponentOfContainer<Entity>
{
    public bool interactuable = true;
    public Sprite Image;

    public Pictionarys<Type, InteractAction> interact => _interact;

    Pictionarys <Type, InteractAction> _interact = new Pictionarys<Type, InteractAction>();

    public GenericSubMenu genericMenu;

    public virtual void ShowMenu(Character character)
    {
        if (!interactuable)
            return;
        genericMenu.Init();
        MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(false);
        genericMenu.Create(character);
    }

    public virtual T Interact<T>() where T : InteractAction
    {
        if (!interactuable)
            return null;

        return (T)_interact[typeof(T)];
    }

    public override void OnEnterState(Entity param)
    {
        Image = param.flyweight.image;

        foreach (var item in GetComponents<InteractAction>())
        {
            _interact.Add(item.GetType(), item);
        }

        genericMenu = new GenericSubMenu(this);

        Action<SubMenus> menuAction =
        (subMenu) =>
        {
            subMenu.CreateSection(0, 2);
            subMenu.CreateChildrenSection<ScrollRect>();

            foreach (var item in genericMenu.interactComponent.interact)
            {
                item.value.InteractInit(genericMenu.interactComponent);
                subMenu.AddComponent<EventsCall>().Set(item.key.Name, () => { genericMenu.DestroyLastButtons(); item.value.ShowMenu(genericMenu.myCharacter); }, "").rectTransform.sizeDelta = new Vector2(300, 75);
            }

            subMenu.CreateSection(2, 6);
            subMenu.CreateChildrenSection<ScrollRect>();
            genericMenu.detailsWindow = subMenu.AddComponent<DetailsWindow>().SetTexts("", genericMenu.interactComponent.container.flyweight.GetDetails()["Description"]).SetImage(genericMenu.interactComponent.container.flyweight.image);
            
            subMenu.CreateTitle(genericMenu.interactComponent.container.flyweight.nameDisplay);

        };

        genericMenu.SetCreateAct(menuAction);

        //Interact<CraftingAction>().Activate(param.);

        //interact = param.flyweight.GetFlyWeight<InteractBase>().interact;
    }

    public override void OnExitState(Entity param)
    {
        throw new System.NotImplementedException();
    }

    public override void OnStayState(Entity param)
    {
        throw new System.NotImplementedException();
    }
}
