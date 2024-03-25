using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BaseData/Interact", fileName = "New Interact")]
public class InteractBase : FlyWeight<EntityBase>
{
    public Pictionarys<string, LogicActive> interact;
}
