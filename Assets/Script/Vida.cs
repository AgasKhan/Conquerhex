using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Vida : MonoBehaviour
{
    public float maxHp;
    public float hp;

    TextCompleto UIdanio;

    [SerializeReference]
    SpriteRenderer rend;
    Color colorDamage = new Color(1, 0, 0);
    Color normal;

    public MonoBehaviour[] controlador;
    
    Timer enf;

    Animator anim;

    Collider2D[] coll;

    void Start()
    {
        hp = maxHp;

        enf= TimersManager.Create();

        rend = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();

        /*
        if (child == null)
        {
            rend = GetComponent<SpriteRenderer>();
            anim = GetComponent<Animator>();
        }
        else
        {
            rend = child.gameObject.GetComponent<SpriteRenderer>();
            anim = child.gameObject.GetComponent<Animator>();
        }
        */
        
        if(rend!=null)
            normal = rend.color;

        UIdanio = Interfaz.TitleSrchByName("Danio");

        coll = GetComponentsInChildren<Collider2D>();
    }

    void Update()
    {
        if (enf.Chck&& rend.color != normal)
        {
            rend.color= normal;
        }
    }

    public void SetHp(float num)
    {
        maxHp = num;
        hp = num;
    }

    public void RestarHp(float danio, float enf2)
    {
        if (enf.Chck)
        {
            hp -= danio;

            enf.Set(enf2);

            rend.color= colorDamage;
            anim.SetTrigger("Damage");

            //pantallas.Add("El objeto " + this.name + " recibio " + danio + " de daño");

            UIdanio.Message(this.name + " recibio " + danio + " de daño");

            if (hp <= 0)
            {
                anim.SetTrigger("Muerte");

                for (int i = 0; i < controlador.Length; i++)
                    controlador[i].enabled = false;
                

                for (int i = 0; i < coll.Length; i++)
                    coll[i].enabled = false;
                
            }

            if (this.CompareTag("Player"))
            {
                Interfaz.health.fillAmount = hp / maxHp;

                if (hp <= 0)
                {
                    Interfaz.TitleSrchByName("Titulo").Message("Segui participando" + "\n" + "Maquinola");
                }
            }


                  
            }
    }

    private void OnDestroy()
    {
        TimersManager.Destroy(enf);
    }
}