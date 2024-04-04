using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SaveSystem;

[CreateAssetMenu(menuName = "BaseData/BaseData", fileName = "Base Data")]
public class BaseData : SingletonScript<BaseData>
{
    public static readonly string path = "Prefabs/";
    public static string pathProps => path + "Props/";
    public static CarlitoEntity Carlitos => instance.carlitos;

    public CarlitoEntity carlitos;

    bool TimeSlicing => true;

    [SerializeField]
    AuxClass<List<SaveObject>> saveData = new AuxClass<List<SaveObject>>(new List<SaveObject>());

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

    public void LoadAll()
    {

    }

    void LoadObject(SaveObject saveObject, Transform parent = null)
    {
        if (prefabs == null)
            prefabs = prefabsPic.ToDictionary();

        var aux = Instantiate(prefabs[saveObject.gameObject], saveObject.pos, Quaternion.Euler(saveObject.rotation), parent);

        foreach (var item in aux.GetComponents<ISaveObject>())
        {
            for (int i = 0; i < saveObject.dataComponent.Length; i++)
            {
                if(item.GetType().Name == saveObject.dataComponent[i].name)
                {
                    item.Load(saveObject.dataComponent[i].data);

                    break;
                }
            }
        }

        foreach (var item in JsonUtility.FromJson<AuxClass<SaveObject[]>>(saveObject.childs).value)
        {
            LoadObject(item, aux.transform);
        }
    }

    public void SaveObject(GameObject saveObject)
    {
        var aux = new AuxClass<SaveObject>(new SaveObject());

        GameManager.instance.StartCoroutine(SaveObjectAsync(saveObject, (svObj)=> saveData.value.Add(svObj)));
    }

    IEnumerator SaveObjectAsync(GameObject saveObject, System.Action<SaveObject> action)
    {
        SaveObject svObject = new SaveObject();

        svObject.gameObject = saveObject.name;

        svObject.pos = saveObject.transform.position;

        svObject.rotation = saveObject.transform.rotation.eulerAngles;

        svObject.dataComponent = saveObject.GetComponents<ISaveObject>().Select(

            (svObj) =>
            {
                return new Data()
                {
                    name = svObj.GetType().Name,

                    data = svObj.Save()
                };
            }

            ).ToArray();

        yield return null;

        List<SaveObject> childs = new List<SaveObject>();

        SaveObject bufferAux = new SaveObject();

        System.Action<SaveObject> aux = (_svObj) => bufferAux = _svObj;

        if (saveObject.transform.childCount > 0)
        {
            for (int i = 0; i < saveObject.transform.childCount; i++)
            {
                yield return GameManager.instance.StartCoroutine(SaveObjectAsync(saveObject.transform.GetChild(i).gameObject, aux));

                childs.Add(bufferAux);

                yield return null;
            }

            AuxClass<SaveObject[]> auxClass = new AuxClass<SaveObject[]>(childs.ToArray());

            svObject.childs = JsonUtility.ToJson(childs);
        }

        action.Invoke(svObject);
    }
}

public interface ISaveObject
{
    string Save();

    void Load(string str);
}

namespace SaveSystem
{
    [System.Serializable]
    public struct SaveObject
    {
        public string gameObject;//prefab

        public Vector3 pos;

        public Vector3 rotation;

        public Data[] dataComponent;

        public string childs;
    }


    [System.Serializable]
    public struct Data
    {
        public string name;
        public string data;
    }
}