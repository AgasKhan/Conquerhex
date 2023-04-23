using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackButton : MonoBehaviour, IControllerDir
{
    [SerializeReference]
    Image sprite;

    [SerializeReference]
    Color apreto;

    Color _default;

    void Start()
    {
        var aux = GetComponent<JoyController>();

        aux.axisButton.SuscribeController(this);

        _default = sprite.color;
    }
    public void ControllerDown(Vector2 dir, float tim)
    {
        sprite.color = apreto;
    }

    public void ControllerPressed(Vector2 dir, float tim)
    {
       
    }

    public void ControllerUp(Vector2 dir, float tim)
    {
        sprite.color = _default;
    }
}
