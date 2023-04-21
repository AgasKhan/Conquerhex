using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotsFunctions : MonoBehaviour
{
    private void Awake()
    {
        LoadSystem.AddPostLoadCorutine(LoadSlots);
    }


    #region FuncionesSlots

    public void FarmSlot(GameObject g)
    {
        Debug.Log("FarmingSlot");
    }

    public void AttackSlot(GameObject g)
    {
        Debug.Log("AttackSlot");
    }

    public void DefendSlot(GameObject g)
    {
        Debug.Log("DefendSlot");
    }


    //------------------------------------------------------
    public void HeadSlot(GameObject g)
    {
        Debug.Log("HeadSlot");
    }
    public void ArmSlot(GameObject g)
    {
        Debug.Log("ArmSlot");
    }
    public void LegSlot(GameObject g)
    {
        Debug.Log("LegSlot");
    }
    public void TailSlot(GameObject g)
    {
        Debug.Log("TailSlot");
    }
    //------------------------------------------------------

    #endregion

    void LoadSlots()
    {
        MenuManager.instance.eventListVoid.AddRange(new Pictionarys<string, System.Action<GameObject>>()
        {

            {"FarmSlot", FarmSlot},
            {"AttackSlot", AttackSlot},
            {"DefendSlot", DefendSlot},


            {"HeadSlot", HeadSlot},
            {"ArmSlot", ArmSlot},
            {"LegSlot", LegSlot},
            {"TailSlot", TailSlot}

        });
    }

}
