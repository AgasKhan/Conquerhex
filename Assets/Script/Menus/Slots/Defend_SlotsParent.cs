using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defend_SlotsParent : SlotsParent
{
    public override void AcceptedDrop()
    {
        base.AcceptedDrop();
        Debug.Log("Defend");
    }
}
