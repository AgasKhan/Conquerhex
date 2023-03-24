using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackJoy : MonoBehaviour
{
    [SerializeReference]
    Image sprite;

    [SerializeReference]
    Color apreto;

    Color _default;

    void Start()
    {
        VirtualControllers.movement.down += Controloador_down;
        VirtualControllers.movement.up += Controloador_up;

        _default = sprite.color;
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
