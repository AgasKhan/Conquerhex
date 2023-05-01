using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAHunter : IAFather
{
    [SerializeField]
    public Pictionarys<string, SteeringWithTarger> steerings;
    [SerializeField]
    protected Detect<Entity> detectCordero;

    Algo a;
    
    float energy = 0;

    

    private void Start()
    {
        a = new Algo(this);
    }

    public override void OnEnterState(Character param)
    {
        
    }

    public override void OnExitState(Character param)
    {

    }

    public override void OnStayState(Character param)
    {
        float distance = float.PositiveInfinity;
        var corderos = detectCordero.Area(param.transform.position, (target) => { return Team.hervivoro == target.team; });

        Entity lamb = null;
        for (int i = 0; i < corderos.Count; i++)
        {
            if (distance > (corderos[i].transform.position - param.transform.position).sqrMagnitude)
            {
                lamb = corderos[i];
                distance = (corderos[i].transform.position - param.transform.position).sqrMagnitude;
            }
        }



        foreach (var itemInPictionary in steerings)
        {
            for (int i = 0; i < itemInPictionary.value.Count; i++)
            {
                param.move.Acelerator(itemInPictionary.value[i]);
            }
        }
    }

}

public class Algo : FSM<Algo, IAHunter>
{
    
    public Algo(IAHunter reference) : base(reference)
    {
    }
}

public class InternHunter : IState<Algo>
{
    [SerializeField] public Timer time;


    public void OnEnterState(Algo param)
    {
        //time = TimersManager.Create(10);
        time.Substract(-time.deltaTime);
    }

    public void OnExitState(Algo param)
    {
        time.Start();
    }

    public void OnStayState(Algo param)
    {
        time.Substract(time.deltaTime);
        //time = TimedAction()
    }
}

