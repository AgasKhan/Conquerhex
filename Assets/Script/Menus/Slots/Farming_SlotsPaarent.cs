using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farming_SlotsPaarent : SlotsParent
{
    public override void AcceptedDrop()
    {
        base.AcceptedDrop();
        Debug.Log("Farming");
    }
}
