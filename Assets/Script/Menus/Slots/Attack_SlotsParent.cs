using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_SlotsParent : SlotsParent
{
    public override void AcceptedDrop()
    {
        base.AcceptedDrop();
        Debug.Log("Attack");
    }
}
