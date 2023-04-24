using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttk : IControllerDir
{
 
    public virtual void ControllerDown(Vector2 dir, float tim)
    {
        
    }

    public virtual void ControllerPressed(Vector2 dir, float tim)
    {
        //Ataque 2
    }

    public virtual void ControllerUp(Vector2 dir, float tim)
    {
        //Ataque 3
    }
}
