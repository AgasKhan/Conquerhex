using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ControlPJ : MonoBehaviour
{
    public float velocidad = 50;//velocidad en pixeles por segundo

    public float valorEnfriamiento;

    public float valorDuracion;

    public Menu menu;

    Rebote scriptParry;

    Movement mov;
    // Update is called once per frame

    Animator controller;

    SpriteRenderer rend;

    Timers enf;
    Timers dur;

    private void Start()
    {
        scriptParry = GetComponentInChildren<Rebote>();

        mov = GetComponent<Movement>();

        controller= GetComponentInChildren<Animator>();

        rend = controller.gameObject.GetComponent<SpriteRenderer>();

        enf = CoolDown.CreateCd("enfriamientoParry", valorEnfriamiento);
        dur = CoolDown.CreateCd("duracionParry", valorDuracion);
    }

    void Update()
    {
        Vector2 movimiento = Vector2.right * Input.GetAxis("Horizontal") + Vector2.up * Input.GetAxis("Vertical");

        float mult = 1;

        if (Input.GetButtonDown("Cancel"))
        {
            GameManager.instance.Pause();
            
        }

        if (Input.GetButtonDown("Tutorial"))
        {

            menu.ShowMenu("Tutorial",
                "Movimiento: WASD o flechitas" +
                "\n" +
                "Defenza: F (refleja proyectiles mientras dura)" +
                "\n" +
                "Sprint: Shift" +
                "\n" +
                "\n" +
                "Para ganar tienes que escapar del espacio fragmentado atravez de un dirigible como muestra la imagen", true, false);
            
        }


        if (Input.GetButton("Sprint"))
            mult = 1.5f;
        
        if (movimiento.sqrMagnitude>0)
        {
            mov.SetVectorDT(velocidad * Time.deltaTime * mult, movimiento);
            controller.SetBool("Move",true);
            controller.SetFloat("Sprint", mult);
        }
        else
        {
            controller.SetBool("Move", false);
        }

        if (movimiento.x<0)
        {
            rend.flipX = true;
        }
        else if(movimiento.x > 0)
        {
            rend.flipX = false;
        }

        if (Input.GetButton("Parry") && enf.CheckAndSub())
        {
            controller.SetBool("Atack", true);
            scriptParry.parry = true;

            enf.RestartTimer();
            dur.RestartTimer();

        }

        if (dur.CheckAndSub() && scriptParry.parry)
        {
            controller.SetBool("Atack", false);
            scriptParry.parry = false;
        }
        
    }

}
