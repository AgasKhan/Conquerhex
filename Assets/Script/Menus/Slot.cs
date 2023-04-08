using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class Slot : MonoBehaviour, IDropHandler
{
    //------------------------------------------------------
    public BodyPart SlotOf;
    //------------------------------------------------------

    protected DragItem draggableItem;

    public Action <GameObject> onAcceptDrop;

    Transform Container;

    private void Awake()
    {
        LoadSystem.AddPostLoadCorutine(LoadSlots);
    }

    private void Start()
    {
       
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;

        //dropped.transform.SetAsLastSibling();

        draggableItem = dropped.GetComponent<DragItem>();

        //------------------------------------------------------
        var modObject = dropped.GetComponent<ModsManager>();

        if (modObject != null)
        {
            if (modObject.myPart == SlotOf)
                AcceptedDrop();
            else
                DeclinedDrop();
        }
        else if (draggableItem != null)
            AcceptedDrop();

        //------------------------------------------------------


        /*
        if (Container == draggableItem.originalParent)
        {
            DeclinedDrop();
            Debug.Log("NO se acepto el drop");
        }
        else
        {
            AcceptedDrop();
            Debug.Log("Se acepto el drop");
        }
        */
    }

    public virtual void AcceptedDrop()
    {
        draggableItem.parentAfterDrag = Container;
        draggableItem.lastParent = Container;
        onAcceptDrop(this.gameObject);
    }

    public virtual void DeclinedDrop()
    {
        draggableItem.parentAfterDrag = draggableItem.lastParent;
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

    /*
    public SlotsParent(DragItem draggableItem, Action onAcceptDrop, Transform container)
    {
        this.draggableItem = draggableItem;
        this.onAcceptDrop = onAcceptDrop;
        Container = container;
    }
    */

    IEnumerator LoadSlots(System.Action<bool> end, System.Action<string> msg)
    {
        end(true);

        MenuManager.instance.eventListVoid.AddRange(new Pictionarys<string, System.Action<GameObject>>()
        {

            {"FarmSlot", FarmSlot},
            {"AttackSlot", AttackSlot},
            {"DefendSlot", DefendSlot}

        });

        yield return new WaitForSeconds(1f);

        msg ("Cargando Slots");
        Extensions.SlotEvent(this);

        Container = GetComponentInChildren<VerticalLayoutGroup>().transform;

        yield return null;
    }
}
