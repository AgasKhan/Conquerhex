using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StaticEntity : Entity
{
    public Pictionarys<string,LogicActive> interact; //funciones de un uso

    public Pictionarys<string,WorkEntiy> work; //funciones en el update

    public List<Item> inventory;

    [SerializeField]
    Pictionarys<string, LogicActive> actions; //funciones de un uso

    FSMWork fsmWork;

    public void ChangeWork(string key)
    {
        fsmWork.CurrentState = work[key];
    }

    public void CancelWork()
    {
        fsmWork.CurrentState = fsmWork.voiid;
    }

    private void Awake()
    {
        fsmWork = new FSMWork(this);
    }

    private void Update()
    {
       fsmWork.UpdateState();
    }
}

public class FSMWork : FSM<FSMWork, StaticEntity>
{
    public IState<FSMWork> voiid = new WorkEntiyNull();

    public FSMWork(StaticEntity reference) : base(reference)
    {
        Init(voiid);
    }
}

public class WorkEntiyNull : IState<FSMWork>
{
    public void OnEnterState(FSMWork param)
    {  
    }

    public void OnExitState(FSMWork param)
    {
    }

    public void OnStayState(FSMWork param)
    {
    }
}

public abstract class WorkEntiy : MonoBehaviour, IState<FSMWork>
{
    public abstract void OnEnterState(FSMWork param);
    public abstract void OnExitState(FSMWork param);
    public abstract void OnStayState(FSMWork param);
}


