using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class CSVReader : SingletonMono<CSVReader>
{

}
/*{
    /// <summary>
    /// El archivo CSV que se manejara
    /// </summary>
    //public TextAsset textAssetData;

    
    public Pictionarys<string, string> baseData = new Pictionarys<string, string>();


    public int maximumGames;

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



    [System.Serializable]
    public class AuxClass<T>
    {
        public T value;

        public AuxClass(T o)
        {
            value = o;
        }
    }



    [System.Serializable]
    public class TestPlayerList
    {
        //public AuxClass[] player;
    }

    public TestPlayerList myTestPlayerList = new TestPlayerList();

    protected override void Awake()
    {
        base.Awake();
    */
/*
if (instance != null)
{
    Destroy(gameObject);
    return;
}
*/

//ReadCSV(textAssetData);

/*
SaveInPictionary("Scene_1_Dialogue_1", "TestNumberOne");
SaveInPictionary("Scene_1_Dialogue_2", "TestNumberTwo");
SaveInPictionary("Scene_1_Dialogue_3", "TestNumberThree");
SaveInPictionary("Scene_1_Dialogue_4", "TestNumberFour");
*/
//WriteCSV(textAssetData, baseData);

/*

    for (int i = 0; i < maximumGames; i++)
    {
        PlayerPrefs.SetString("GameSlot_" + i.ToString(), "");
    }

    SaveProgress(0, baseData);

    print(LoadProgress(0));

    DontDestroyOnLoad(this);
*/
/*
LobbyManager.playerPoints = LoadFromPictionary<int>("PlayerPoints", 100);
BaseData.currentLevel = LoadFromPictionary<int>("CurrentLevel", 1);
BaseData.lastLevelUnlocked = LoadFromPictionary<int>("LastUnlockedLevel", 1);

if (Abilities.Abilitieslist == null)
    Abilities.Abilitieslist = LoadClassFromPictionary("Abilities", new Pictionarys<Type, Abilities.Ability>());

if (Quests.incomplete == null)
{
    Quests.incomplete = LoadFromPictionary<List<Quests.Mission>>("QuestsIncomplete", new List<Quests.Mission>());
    Quests.complete = LoadFromPictionary<List<Quests.Mission>>("QuestsComplete", new List<Quests.Mission>());

    if (Quests.incomplete == null || Quests.incomplete.Count == 0 && Quests.complete.Count == 0)
    {
        Quests.CreateQuests();
    }
}
*/
/*
}

void SaveProgress(int indexSlot, Pictionarys<string, string> myChanges)
{
if (indexSlot >= 0 && indexSlot < maximumGames)
    PlayerPrefs.SetString("GameSlot_" + indexSlot, myChanges.ToString());
else
    print("There is not a game slot whit that index");
}

string LoadProgress(int indexSlot)
{
if (indexSlot >= 0 && indexSlot < maximumGames)
    return PlayerPrefs.GetString("GameSlot_" + indexSlot);
else
{
    print("There is not a game slot whit that index");
    return null;
}
}


#region TextManagment
void ReadCSV(TextAsset myCSVFile)
{
string[] dataCSV;

dataCSV = ReadString(myCSVFile.name + ".csv").Split('\n');

for (int i = 0; i < dataCSV.Length - 1; i++)
{
    string[] reglonCSV = dataCSV[i].Split(';');

    baseData.Add(reglonCSV[0], reglonCSV[1]);
}

}

void WriteCSV(TextAsset myCSVFile, Pictionarys<string, string> myChanges)
{
WriteString(myCSVFile.name + ".csv", myChanges.ToString(";"));
}

static void WriteString(string file, string data)
{
string path = "Assets/Resources/" + file;

StreamWriter writer = new StreamWriter(path, false);
writer.Write(data);
writer.Close();
}

static string ReadString(string file)
{
string aux;
string path = "Assets/Resources/" + file;

StreamReader reader = new StreamReader(path);
aux = (reader.ReadToEnd());
reader.Close();

return aux;
}
#endregion

#region SaveAndLoadJson

/// <summary>
/// Convierte un objeto en un json y lo almacena o reemplaza en el id que le pases
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="id"></param>
/// <param name="data"></param>
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

#endregion

}*/

/*
public interface IBDSave
{
    void JsonToObj(string Json);
}

public interface IBDLoad
{
    string ObjToJson();
}
*/
