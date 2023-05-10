using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

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

    public static void SaveGame()
    {
        if (Application.platform == RuntimePlatform.Android)
            SaveGameAndroid();
        else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            SaveGameWindows();
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
        string save = "";

        save += File.ReadAllText(savePath);

        if(save != "")
            BD = JsonUtility.FromJson<Pictionarys<string, string>>(save);
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
        SaveGame();
    }

    public static void SaveClassInPictionary<T>(string id, T data)
    {
        string json = JsonUtility.ToJson(data, true);

        Debug.Log(json);

        if (BD.ContainsKey(id, out int index))
            BD[index] = json;
        else
            BD.Add(id, json);
    }


    public static void SaveInPictionary<T>(string id, T data)
    {
        string json = JsonUtility.ToJson(new AuxClass<T>(data), true);

        Debug.Log(json);

        if (BD.ContainsKey(id, out int index))
            BD[index] = json;
        else
            BD.Add(id, json);
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

        if(Application.platform == RuntimePlatform.Android)
        {
            //savePath = Application.persistentDataPath + "/saveData.json";
            savePath = Path.Combine(Application.persistentDataPath, "saveData.json");

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

public interface IBDSave
{
    void JsonToObj(string Json);
}

public interface IBDLoad
{
    string ObjToJson();
}