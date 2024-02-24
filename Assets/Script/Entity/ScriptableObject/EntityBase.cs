using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BaseData/EntityBase", fileName = "new EntityBase")]
public class EntityBase : ItemBase
{
    [SerializeField]
    List<FlyWeight<EntityBase>> container = new List<FlyWeight<EntityBase>>();

    [Header("Estadistica")]
    public float areaFarming = 1;

    public T GetFlyWeight<T>() where T : FlyWeight<EntityBase>
    {
        for (int i = 0; i < container.Count; i++)
        {
            if (container[i] is T)
                return (T)container[i];
        }

        return default(T);
    }

    protected override System.Type SetItemType()
    {
        return typeof(EntityDiagram);
    }

    public override Pictionarys<string, string> GetDetails()
    {
        Pictionarys<string, string> aux = base.GetDetails();

        foreach (var item in container)
        {
            aux.Add(Lenguages.instance[item], item.ToString());
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


public class FlyWeight<T> : ScriptableObject where T : ScriptableObject
{

}