using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class SaveWithJSON : SingletonMono<SaveWithJSON>
{
    
    public Pictionarys<string, string> baseData = new Pictionarys<string, string>();

    public static Pictionarys<string, string> BD
    {
        get
        {
            return instance.baseData;
        }
        set
        {
            instance.baseData = value;
        }
    }


    private string path;

    public int maximumGames;

    //BaseData json;


    [System.Serializable]
    public class AuxClass<T>
    {
        public T value;

        public AuxClass(T o)
        {
            value = o;
        }
    }

    
    //private void Start()
    //{
       // path = Application.persistentDataPath + "/saveData.json";


        //Para Computadora
        /*
        var directoryPath = Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\","/")) + "/" + Application.productName;
        
        if (Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);
        */

        //if (File.Exists(path))
           // LoadGame();
       // else
           // SaveGame();
    //}
    
    void SaveGame()
    {
        File.WriteAllText(path, BD.ToString());
    }

    public void LoadGame()
    {
        string save = File.ReadAllText(path);
        LoadFromPictionary<Pictionarys<string, string>>(save);
    }
    public void DeleteData()
    {
        //File.Delete(path);

        //json = new BaseData();
        //SaveGame();

        BD.Clear();
        SaveGame();

    }

    public static void SaveClassInPictionary<T>(string id, T data)
    {
        string json = JsonUtility.ToJson(data);

        if (BD.ContainsKey(id))
            BD[id] = json;
        else
            BD.Add(id, json);
    }


    public static void SaveInPictionary<T>(string id, T data)
    {
        string json = JsonUtility.ToJson(new AuxClass<T>(data));

        if (BD.ContainsKey(id))
            BD[id] = json;
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

}


public interface IBDSave
{
    void JsonToObj(string Json);
}

public interface IBDLoad
{
    string ObjToJson();
}