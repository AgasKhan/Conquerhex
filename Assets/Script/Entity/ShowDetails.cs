using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BaseData/DetailsObject", fileName = "Details")]
public class ShowDetails : ScriptableObject, IShowDetails
{
    [Header("ShowDetails")]
    [SerializeField]
    protected string _nameDisplay;

    [SerializeField]
    Sprite _image;

    [Space]
    [SerializeField]
    [TextArea(3, 6)]
    string _details;

    public virtual string nameDisplay => _nameDisplay;

    public Sprite image => _image;

    public override string ToString()
    {
        return nameDisplay + "\n\n" + GetDetails().ToString(": ", "\n\n");
    }

    public virtual Pictionarys<string, string> GetDetails()
    {
        return new Pictionarys<string, string>() { { "Descripción".RichText("color", "#f6f1c2"), _details } };
    }

    private void OnEnable()
    {
        MyEnable();
    }

    private void OnDisable()
    {
        MyDisable();
    }

    private void OnDestroy()
    {
        MyDisable();
    }

    protected virtual void MyDisable()
    {
        Manager<ShowDetails>.pic.Remove(nameDisplay);
    }

    protected virtual void MyEnable()
    {
        Manager<ShowDetails>.pic.Add(nameDisplay, this);
    }
}

public interface IShowDetails
{
    string nameDisplay { get; }
    Sprite image { get; }
    Pictionarys<string, string> GetDetails();
}