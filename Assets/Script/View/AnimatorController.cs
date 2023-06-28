using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    [SerializeField]
    Animator animator;

    [SerializeField]
    string attackNameAnim = "Attack";

    [SerializeField]
    string moveNameAnim = "Move";

    void Awake()
    {
        var ia = GetComponent<IAFather>();

        animator = GetComponentInChildren<Animator>();

        ia.onAttack += Ia_onAttack;

        ia.onIdle += Ia_onIdle;

        ia.onMove += Ia_onMove;
    }

    private void Ia_onMove(Vector2 obj)
    {
        animator.SetBool(moveNameAnim, true);
    }

    private void Ia_onIdle()
    {
        animator.SetBool(moveNameAnim, false);
    }

    private void Ia_onAttack()
    {
        animator.SetTrigger(attackNameAnim);
    }
}
