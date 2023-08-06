using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAAnimator : IAFather
{
    public IGetEntity enemy;

    public AutomaticAttack automatick;

    public Detect<IGetEntity> detectEntities;

    Animator anim;

    public override void OnEnterState(Character param)
    {
        base.OnEnterState(param);

        //timerStun.Set(((BodyBase)character.flyweight).stunTime);

        automatick = new AutomaticAttack(_character, 2);
    }

    public override void OnStayState(Character param)
    {
        base.OnStayState(param);

        var entities = detectEntities.AreaWithRay(transform, (entity) => entity.visible && entity.GetEntity().team != Team.recursos && entity.GetEntity().team != character.team);

        if (entities.Count > 0)
        {
            enemy = entities[0];
        }
        else
        {
            enemy = null;
        }
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
}