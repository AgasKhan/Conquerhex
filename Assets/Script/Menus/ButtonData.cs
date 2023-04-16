using UnityEngine;

[CreateAssetMenu (fileName = "New Button Data", menuName = "Button Data")]
public class ButtonData : ScriptableObject
{
    public DoubleString text;
    public Sprite sprite;

    [HideInInspector]
    public DetailsWindow detailWindow;
}
