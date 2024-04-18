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

    public string currentSlot = "Slot 1";

    public Pictionarys<ShowDetails, string> savedGames = new Pictionarys<ShowDetails, string>();

    bool TimeSlicing => true;

    [SerializeField]
    List<SaveObject> saveData = new List<SaveObject>();

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

    public void LoadGame(string slot)
    {
        if (PlayerPrefs.HasKey(slot))
        {
            saveData = JsonUtility.FromJson<AuxClassField<List<SaveObject>>>(PlayerPrefs.GetString(slot)).value;
            LoadAll();
        }
        else
            Debug.LogError("EL slot de nombre:" + slot + ". No existe");
    }

    public void LoadAll(Transform parent = null)
    {
        foreach (var item in saveData)
        {
            GameManager.instance.StartCoroutine(LoadObjectDataAsync(item, parent, (obj) => { Debug.Log("Termino la carga de: " + obj.gameObject); }));
        }
    }

    IEnumerator LoadObjectAsync(SaveObject saveObject, Transform parent, System.Action<SaveObject> endAction)
    {
        if (prefabs == null)
            prefabs = prefabsPic.ToDictionary();

        var aux = Instantiate(prefabs[saveObject.gameObject], saveObject.pos, Quaternion.Euler(saveObject.rotation), parent);
        yield return null;

        foreach (var item in aux.GetComponents<ISaveObject>())
        {
            for (int i = 0; i < saveObject.dataComponent.Length; i++)
            {
                if (item.GetType().Name == saveObject.dataComponent[i].name)
                {
                    item.Load(saveObject.dataComponent[i].data);

                    break;
                }

                yield return null;
            }

            yield return null;
        }

        var childsArray = JsonUtility.FromJson<AuxClassField<SaveObject[]>>(saveObject.childs);
        if (childsArray != null)
        {
            foreach (var item in childsArray.value)
            {
                yield return GameManager.instance.StartCoroutine(LoadObjectAsync(item, aux.transform, endAction));

                yield return null;
            }
        }

        endAction?.Invoke(saveObject);
    }
    IEnumerator LoadObjectDataAsync(SaveObject saveObject, Transform parent, System.Action<SaveObject> endAction)
    {
        GameObject aux = parent.gameObject;
        
        foreach (var item in aux.GetComponents<ISaveObject>())
        {
            for (int i = 0; i < saveObject.dataComponent.Length; i++)
            {
                if (item.GetType().Name == saveObject.dataComponent[i].name)
                {
                    item.Load(saveObject.dataComponent[i].data);

                    break;
                }
                yield return null;
            }

            yield return null;
        }

        var childsArray = JsonUtility.FromJson<AuxClassField<SaveObject[]>>(saveObject.childs);
        if (childsArray != null)
        {
            var childs = childsArray.value;
            for (int i = 0; i < childs.Length; i++)
            {
                yield return GameManager.instance.StartCoroutine(LoadObjectDataAsync(childs[i], parent.GetChild(i).transform, endAction));

                yield return null;
            }
        }

        endAction?.Invoke(saveObject);
    }

    public void SaveObject(GameObject saveObject)
    {
        var aux = new AuxClassField<SaveObject>(new SaveObject());

        GameManager.instance.StartCoroutine(SaveObjectDataAsync(saveObject, (svObj)=> saveData.Add(svObj)));
    }

    IEnumerator SaveObjectAsync(GameObject saveObject, System.Action<SaveObject> endAction)
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
                
                for (int j = 0; j < prefabsPic.Count; j++)
                {

                    if (saveObject.transform.GetChild(i).gameObject.name == prefabsPic.keys[j])
                    {
                        
                        yield return GameManager.instance.StartCoroutine(SaveObjectAsync(saveObject.transform.GetChild(i).gameObject, aux));
                
                        childs.Add(bufferAux);
                        
                    }
                }

                yield return null;
            }

            AuxClassField<SaveObject[]> auxClass = new AuxClassField<SaveObject[]>(childs.ToArray());

            svObject.childs = JsonUtility.ToJson(auxClass);
        }

        endAction.Invoke(svObject);
    }

    IEnumerator SaveObjectDataAsync(GameObject saveObject, System.Action<SaveObject> endAction)
    {
        SaveObject svObject = new SaveObject();

        svObject.gameObject = saveObject.transform.GetHashCode().ToString();

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
                yield return GameManager.instance.StartCoroutine(SaveObjectDataAsync(saveObject.transform.GetChild(i).gameObject, aux));
                childs.Add(bufferAux);
               
                yield return null;
            }

            AuxClassField<SaveObject[]> auxClass = new AuxClassField<SaveObject[]>(childs.ToArray());

            svObject.childs = JsonUtility.ToJson(auxClass);
        }

        endAction.Invoke(svObject);
    }

    public void SaveCurrentGame()
    {
        SaveGame(currentSlot);
    }

    public void SaveGame(string slotName)
    {
        var data = JsonUtility.ToJson(new AuxClassField<List<SaveObject>>(saveData));
        savedGames[SearchSlot(slotName)] = data;
        PlayerPrefs.SetString(slotName, data);
    }

    int SearchSlot(string slotName)
    {
        for (int i = 0; i < savedGames.Count; i++)
        {
            if (savedGames.keys[i].nameDisplay == slotName)
                return i;
        }
        return -1;
    }

    public void DeleteCurrentGame()
    {
        saveData.Clear();
        DeleteGame(currentSlot);
    }

    public void DeleteGame(string slotName)
    {
        savedGames[SearchSlot(slotName)] = "";
        PlayerPrefs.DeleteKey(slotName);
    }

    public void DeleteAll()
    {
        saveData.Clear();
        foreach (var item in savedGames)
        {
            item.value = "";
        }
        PlayerPrefs.DeleteAll();
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