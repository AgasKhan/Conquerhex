using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ControlPJ : MonoBehaviour
{
    public float velocidad = 50;//velocidad en pixeles por segundo

    public float valorEnfriamiento;

    public float valorDuracion;

    public Health health;

    public Menu menu;

    Rebote scriptParry;

    Movement mov;
    // Update is called once per frame

    Animator controller;

    SpriteRenderer rend;

    Timer enf;
    Timer dur;

    private void Start()
    {
        scriptParry = GetComponentInChildren<Rebote>();

        mov = GetComponent<Movement>();

        controller= GetComponentInChildren<Animator>();

        rend = controller.gameObject.GetComponent<SpriteRenderer>();

        enf = TimersManager.Create(valorEnfriamiento);
        dur = TimersManager.Create(valorDuracion);

        health.Init();
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

        if (Input.GetButton("Parry") && enf.Chck)
        {
            controller.SetBool("Atack", true);
            scriptParry.parry = true;

            enf.Reset();
            dur.Reset();

        }

        if (dur.Chck && scriptParry.parry)
        {
            controller.SetBool("Atack", false);
            scriptParry.parry = false;
        }
        
    }

}
