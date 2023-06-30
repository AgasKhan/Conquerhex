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

    [SerializeField]
    string deathNameAnim = "Death";

    void Awake()
    {
        var ia = GetComponent<Character>();

        animator = GetComponentInChildren<Animator>();

        ia.onAttack += Ia_onAttack;

        ia.move.onIdle += Ia_onIdle;

        ia.move.onMove += Ia_onMove;

        ia.health.death += Ia_onDeath;
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
    private void Ia_onDeath()
    {
        animator.SetTrigger(deathNameAnim);
    }
}
