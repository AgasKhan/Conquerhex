using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackButton : MonoBehaviour
{
    [SerializeReference]
    Image sprite;

    [SerializeReference]
    Color apreto;

    Color _default;

    void Start()
    {
        var aux = GetComponent<JoyController>();

        aux.axisButton.eventUp += Controloador_up;

        aux.axisButton.eventDown += Controloador_down;

        aux.axisButton.eventPress += AxisButton_eventPress;

        _default = sprite.color;
    }

    private void AxisButton_eventPress(Vector2 arg1, float arg2)
    {
        
    }

    private void Controloador_up(Vector2 arg1, float arg2)
    {
        sprite.color = _default;
    }

    private void Controloador_down(Vector2 arg1, float arg2)
    {
        sprite.color = apreto;
    }
}
