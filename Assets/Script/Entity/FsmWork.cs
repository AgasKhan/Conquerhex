using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class FSMWork : FSMSerialize<FSMWork, InventoryEntityComponent>
{
    public IState<FSMWork> voiid = new WorkEntiyNull();

    [SerializeField]
    public Pictionarys<string, WorkEntiy> work = new Pictionarys<string, WorkEntiy>();

    public void ChangeWork(string key)
    {
        CurrentState = work[key];
    }

    public void CancelWork()
    {
        CurrentState = voiid;
    }

    public override void Init(InventoryEntityComponent reference)
    {
        base.Init(reference);
        Init(voiid);
    }
}

public class WorkEntiyNull : IState<FSMWork>
{
    public void OnEnterState(FSMWork param) { }
    public void OnExitState(FSMWork param) { }
    public void OnStayState(FSMWork param) { }
}

public abstract class WorkEntiy : MonoBehaviour, IState<FSMWork>
{
    public abstract void OnEnterState(FSMWork param);
    public abstract void OnExitState(FSMWork param);
    public abstract void OnStayState(FSMWork param);
}