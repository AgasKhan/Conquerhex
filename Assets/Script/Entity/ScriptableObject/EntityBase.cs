using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BaseData/EntityBase", fileName = "new EntityBase")]
public class EntityBase : ItemBase
{
    [SerializeField]
    List<ScriptableObject> container = new List<ScriptableObject>();

    [Header("Estadistica")]
    public float areaFarming = 1;

    public T GetFlyWeight<T>() where T : ScriptableObject
    {
        for (int i = 0; i < container.Count; i++)
        {
            if (container[i] is T)
                return (T)container[i];
        }

        return null;
    }

    protected override void SetCreateItemType()
    {
        _itemType = typeof(EntityDiagram);
    }

    public override Pictionarys<string, string> GetDetails()
    {
        Pictionarys<string, string> aux = base.GetDetails();

        foreach (var item in container)
        {
            aux.Add(Lenguages.instance[item.GetType().Name], item.ToString());
        }

        return aux;
    }
}


public class EntityDiagram : Item<EntityBase>
{
    public override void Init()
    {

    }
}