using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Euler;

#if UNITY_EDITOR 
using UnityEditor;
#endif

public class ResourceItemContainer : SuperScriptableObject
{
    [SerializeField]
    TextAsset[] _textAssets;

    
    [SerializeField]
    public Pictionarys<string, ResourcesBase_ItemBase[]> allItems { get; private set; }
    

    public string[] types;

    public ResourcesBase_ItemBase[] items;

    public int Length => types.Length;

    public string this[int index]
    {
        get
        {
            return allItems.keys[index];
        }
    }

    public ResourcesBase_ItemBase[] this[string type]
    {
        get
        {
            return allItems[type];
        }
    }

#if UNITY_EDITOR
    /*
    public event System.Action<ResourceItemContainer> OnFinishSet;

    [ContextMenu("Cargar textos")]
    void LoadTexts()
    {
        DeleteAll();

        Parse _parse = new Parse(_textAssets);

        types = new types[_parse.DataUser.Count];

        List<ResourcesBase_ItemBase> items = new List<ResourcesBase_ItemBase>();

        string debug = string.Empty;

        int index = 0;

        foreach (var user in _parse.DataUser)
        {
            List<PDO<string, string>> data = new List<PDO<string, string>>();

            debug += user + "\n";

            for (int i = _parse.DataOrdered.Count - 1; i >= 0; i--)
            {
                debug += "\t" + _parse.DataOrdered[i].ToString() + "\n";

                if (_parse.DataOrdered[i][1] == user)
                {
                    data.Insert(0, _parse.DataOrdered[i]);
                    _parse.DataOrdered.RemoveAt(i);
                }
            }

            types[index] = MakeNew<User>(user, (userClass) => userClass.Initilize(data)); //creo en disco el scriptable

            items.AddRange(types[index].comments);

            index++;
        }

        items.Sort(Sort);

        this.comments = items.ToArray();

        Debug.Log(debug);

        OnFinishSet?.Invoke(this);

        OnFinishSet = null;
    }

    [ContextMenu("Borrar tablas relacionadas")]
    void DeleteAll()
    {
        types?.Delete();
    }

    int Sort(Comment x, Comment y)
    {
        return int.Parse(x.name) <= int.Parse(y.name) ? -1 : 1;
    }
    */
#endif
}
