using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleButtonA : MonoBehaviour
{
    public ButtonA left, right;

    public DoubleButtonA ClearDoubleA()
    {
        ClearSpecificA(true);
        ClearSpecificA(false);
        return this;
    }

    /// <summary>
    /// Limpia un BotonA especifico dependiendo de su booleano. True para izquierda, False para derecha
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public ButtonA ClearSpecificA(bool value)
    {
        if (value)
            return left.ClearButton();
        else
            return right.ClearButton();
    }
}
