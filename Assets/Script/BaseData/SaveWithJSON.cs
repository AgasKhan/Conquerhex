using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

[System.Serializable]
public class SaveWithJSON : SingletonClass<SaveWithJSON>, Init
{
    public Pictionarys<string, string> _baseData = new Pictionarys<string, string>();

    public static Pictionarys<string, string> BD
    {
        get
        {
            return instance._baseData;
        }
        set
        {
            instance._baseData = value;
        }
    }


    public static string savePath;

    public int gamesSlots;

    public static Action OnSave;

    public static Action OnLoad;

    public static void SaveGame()
    {
        SaveGameWindows();
        OnSave?.Invoke();

        /*
        if (Application.platform == RuntimePlatform.Android)
            SaveGameAndroid();
        else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            SaveGameWindows();
        */
    }

    public static void SaveGameAndroid()
    {
        File.WriteAllText(savePath, JsonUtility.ToJson(BD));
    }

    public static void SaveGameWindows()
    {
        PlayerPrefs.SetString("GameData", JsonUtility.ToJson(BD));
    }

    public static void LoadGameAndroid()
    {
        /*
        string save = "";

        save += File.ReadAllText(savePath);

        if(save != "")
            BD = JsonUtility.FromJson<Pictionarys<string, string>>(save);
        */

        BD = JsonUtility.FromJson<Pictionarys<string, string>>(File.ReadAllText(savePath));
    }

    public static void LoadGameWindows()
    {
        if (PlayerPrefs.HasKey("GameData"))
            BD = JsonUtility.FromJson <Pictionarys<string, string>> (PlayerPrefs.GetString("GameData"));
    }

    public static void DeleteData()
    {
        //File.Delete(path);

        //json = new BaseData();
        //SaveGame();
        BD.Clear();
        instance._baseData.Clear();
        
        SaveGame();
    }

    public static void SaveClassInPictionary<T>(string id, T data)
    {
        string json = JsonUtility.ToJson(data, true);

        int index;

        for (int i = 0; i < LoadSystem.instance.noSerailizeProperties.Length; i++)
        {
            if((index = json.IndexOf("\""+LoadSystem.instance.noSerailizeProperties[i])) > -1)
            {
                json = json.Substring(0, index) + json.Substring(json.IndexOf("},", index) + 2);
            }
        }

        Debug.Log(json);

        BD.CreateOrSave(id, json);
    }


    public static void SaveInPictionary<T>(string id, T data)
    {
        string json = JsonUtility.ToJson(new AuxClass<T>(data), true);

        //string json = JsonConvert.SerializeObject(data);

        Debug.Log(json);
        BD.CreateOrSave(id, json);
    }

    public static bool CheckKeyInBD(string key)
    {
        return BD.ContainsKey(key);
    }

    /// <summary>
    /// Busca un json dentro del pictionary a partir de un id
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="id"></param>
    /// <returns></returns>
    public static T LoadClassFromPictionary<T>(string id, T failed = default)
    {
        if (BD.ContainsKey(id))
        {
            var obj = JsonUtility.FromJson<T>(BD[id]);
            return obj;
        }
        else
        {
            Debug.Log(id + " Not found on Base Data");
            return failed;
        }
    }

    public static void LoadClassFromPictionary<T>(string id, ref T overrideClass)
    {
        if (BD.ContainsKey(id))
        {
            JsonUtility.FromJsonOverwrite(BD[id], overrideClass);
            if (overrideClass is Init)
            {
                ((Init)overrideClass).Init();
            }
                
        }
        else
        {
            Debug.Log(id + " Not found on Base Data");
        }
    }

    public static T LoadFromPictionary<T>(string id, T failed = default)
    {
        if (BD.ContainsKey(id))
        {
            var obj = JsonUtility.FromJson<AuxClass<T>>(BD[id]);
            return obj.value;
        }
        else
        {
            Debug.Log(id + " Not found on Base Data");
            return failed;
        }
    }

    public void Init(params object[] param)
    {
        Debug.Log("BD: \n" + BD.ToString());

        if (PlayerPrefs.HasKey("GameData"))
        {
            LoadGameWindows();
        }

        /*
        if (Application.platform == RuntimePlatform.Android)
        {
            savePath = Application.persistentDataPath + "/saveData.json";
            //savePath = Path.Combine(Application.persistentDataPath, "saveData.json");
            //Directory.CreateDirectory(Environment.)
            File.Create(savePath);
            SaveGameAndroid();
            LoadGameAndroid();
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            Debug.Log("Estas en Windows");

            if(PlayerPrefs.HasKey("GameData"))
            {
                LoadGameWindows();
            }
        }
        */



        //Para Computadora
        /*
        var directoryPath = Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\","/")) + "/" + Application.productName;
        
        if (Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);
        */

        /*
        if (File.Exists(path))
            LoadGame();
        else
            SaveGame();
        */
    }

    //-------------------------

    public static void SaveParams(Type classID, params object[] variables)
    {
        SaveInPictionary(classID.ToString(), true);

        foreach (var item in variables)
        {
            SaveInPictionary(classID + item.ToString(), item);
        }
    }

    public static object LoadParams(Type classID, object variable)
    {
        if (BD.ContainsKey(classID + variable.ToString()))
            return LoadFromPictionary<object>(classID + variable.ToString());
        else
            return default;
    }
}

[System.Serializable]
public class AuxClass<T>
{
    [SerializeReference]
    public T value;

    public AuxClass(T o)
    {
        value = o;
    }
}