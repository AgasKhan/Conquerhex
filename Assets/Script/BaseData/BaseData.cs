using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BaseData/BaseData", fileName = "Base Data")]
public class BaseData : SingletonScript<BaseData>
{
    public static readonly string path = "Prefabs/";
    public static string pathProps => path + "Props/";
    public static CarlitoEntity Carlitos => instance.carlitos;

    public CarlitoEntity carlitos;

    bool TimeSlicing => true;

    [SerializeField]
    Pictionarys<string, GameObject> prefabsPic = new Pictionarys<string, GameObject>();

    Dictionary<string, GameObject> prefabs;


    [ContextMenu("Cargar los prefabs")]
    void ChargePrefabs()
    {
        prefabsPic.Clear();

        foreach (var item in LoadSystem.LoadAssets<GameObject>(path))
        {
            prefabsPic.Add(item.name, item);
        }
    }



    void LoadObject(string name,params Data[] datas)
    {
        if (prefabs == null)
            prefabs = prefabsPic.ToDictionary();

        var aux = Instantiate(prefabs[name]);

        foreach (var item in aux.GetComponentsInChildren<ISaveObject>())
        {
            for (int i = 0; i < datas.Length; i++)
            {
                if(item.GetType().Name == datas[i].name)
                {
                    item.Load(datas[i].data);

                    break;
                }
            }
        }
    }


}

public struct SaveObject
{
    public string gameObject;//prefab

    public Data[] dataComponent;
}


public struct Data
{
    public string name;
    public string data;
}


public interface ISaveObject
{
    string Save();

    void Load(string str);
}