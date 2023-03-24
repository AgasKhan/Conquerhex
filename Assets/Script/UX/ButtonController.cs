using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : Controlador
{

    Vector3 dir;

    public void Right()
    {
        dir.x = 1;
    }

    public void Left()
    {
        dir.x = -1;
    }

    public void Up()
    {
        dir.y = 1;
    }

    public void Down()
    {
        dir.y = -1;
    }

    public void Stop()
    {
        dir = Vector3.zero;
    }

    public override Vector3 MoveDir()
    {
        return dir;
    }
}
