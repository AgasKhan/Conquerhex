using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BaseData/DetailsObject", fileName = "Details")]
public class ShowDetails : ScriptableObject, IShowDetails
{
    [SerializeField]
    string _nameDisplay;

    [SerializeField]
    Sprite _image;

    [Space]
    [SerializeField]
    [TextArea(3, 6)]
    string _details;

    public string nameDisplay => _nameDisplay;

    public Sprite image => _image;

    public override string ToString()
    {
        return nameDisplay + "\n\n" + GetDetails().ToString(": ", "\n\n");
    }

    public virtual Pictionarys<string, string> GetDetails()
    {
        return new Pictionarys<string, string>() { { "Description", _details } };
    }
}

public interface IShowDetails
{
    string nameDisplay { get; }
    Sprite image { get; }
    string details => GetDetails()[0];
    Pictionarys<string, string> GetDetails();
}