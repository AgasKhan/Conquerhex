using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlEnemigo : MonoBehaviour
{
    //public GameObject[] player;

    readonly List<Transform> player = new List<Transform>();

    public float nivel;

    public GameObject proyectil;

    public float velocidad;

    public float deteccion;

    public float velocidadProyectil;

    public float danio;

    public float enfriamientoDanio;//dejarlo por defecto

    public float enfriamientoDisparoConst;

    public Transform fuego;

    public LayerMask layerMask;

    Movement scriptMov;

    Timer enf;

    DispararProyectil dispararScript;

    public Collider2D collPj;

    public Collider2D collFire;

    Vector2 der = Vector2.zero;

    Vector2 coord; //POSicion del centro del jugador

    Vector3 pos;//Posicion del centro del enemigo (no el jugador)

    Vector2 apuntar;//vector de direccion desde el enemigo, hacia donde apunta (el jugador)

    Animator anim;

    //AudioSource audioSource;

    private void Start()
    {
        scriptMov = this.GetComponent<Movement>();

        anim = GetComponentInChildren<Animator>();

        /*
        audioSource = AudioManager.instance.CreateAuSc(AudioManager.instance.Srch("disparo"), gameObject);


        audioSource.maxDistance = 5;
        audioSource.spatialBlend = 1;

        */
        
        //health = this.GetComponent<vida>();

        dispararScript = this.GetComponent<DispararProyectil>();

        enf = TimersManager.Create(enfriamientoDisparoConst);

        player.AddRange(LoadMap.instance.carlitos);

        player.Add(GameManager.instance.player.transform);

    }
    void FixedUpdate()
    {
        float aux=deteccion*deteccion;

        pos = transform.position;

        pos.y += 0.6f;

        apuntar = Vector2.zero;
        Vector2 arma = Vector2.zero; //vector de direccion desde el arma hacia al jugador
        coord=Vector2.zero;

        //audioSource.volume = PlayerPrefs.GetFloat("Efecto");
        foreach (var item in player)
        {
            Vector2 coordAux = new Vector2(item.transform.position.x, item.transform.position.y);
            if (aux > (coordAux - (transform.position).Vect3To2()).sqrMagnitude)
            {
                apuntar = new Vector2(item.transform.position.x - pos.x, item.transform.position.y + 0.7f - pos.y);
                arma = new Vector2(item.transform.position.x - fuego.position.x, item.transform.position.y+0.7f - fuego.position.y);
                aux = arma.sqrMagnitude;
                coord = item.transform.position;
            }
        }
                
        if (aux < deteccion*deteccion)
        {
            /*
            if ((apuntar.x>0 && transform.localScale.x<0 ) || (apuntar.x < 0 && transform.localScale.x > 0))
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            */
            RaycastHit2D[] apuntadoArr= new RaycastHit2D[2];
            collFire.Raycast(arma, apuntadoArr, deteccion, layerMask);

            RaycastHit2D apuntado = apuntadoArr[1];

            collPj.Raycast(apuntar, apuntadoArr, deteccion, layerMask);

            RaycastHit2D mira = apuntadoArr[1];

            Debug.DrawRay(pos.Vect3To2().Vec2to3(5), apuntar.normalized*deteccion, Color.gray);

            Debug.DrawRay(fuego.position.Vect3To2().Vec2to3(5), arma.normalized * deteccion, Color.gray);

            if (apuntado.collider!= null)
            {
                bool cubierto = true;

                for (int i = 0; i < 2; i++)
                {
                    
                    if (apuntado.collider.CompareTag("Player"))
                    {
                        der = Vector2.zero;
                        anim.SetFloat("move", 1);
                        if (aux > 16)
                        {
                            scriptMov.SetVectorDT(velocidad * Time.deltaTime, apuntar);
                        }
                        else
                        {
                            scriptMov.SetVectorDT(velocidad * Time.deltaTime, apuntar * -1);
                        }
                        if (enf.Chck)
                        {
                            anim.SetTrigger("attack");
                            enf.Reset();
                            //audioSource.Play();
                            dispararScript.Disparar(danio, arma, velocidadProyectil, fuego.position);
                        }
                        cubierto = false;
                        break;
                    }
                    
                    else if(mira.collider.CompareTag("Player"))
                    {
                        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                        collFire.Raycast(arma, apuntadoArr, deteccion, layerMask);
                        apuntado = apuntadoArr[1];
                    }
                }
               
                if(cubierto)// (der > 5)
                {
                    if(der==Vector2.zero)
                    {
                        Flanqueo();
                    }
                }
            }
        }

        if (der != Vector2.zero)
        {
            Debug.DrawRay(pos, der, Color.blue);

            scriptMov.SetVectorDT(velocidad * Time.deltaTime, der);

            if (der.sqrMagnitude < 4)
            {
                der = Vector2.zero;
            }
        }


        if (scriptMov.rb2d.velocity.sqrMagnitude>0)
        {
            anim.SetFloat("move", 1);
        }
        else
        {
            anim.SetFloat("move", -1);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
       
        if (collision.CompareTag("Props") || collision.CompareTag("Enemigo"))
        {
            if (der == Vector2.zero)
            {
                scriptMov.SetVectorDT(velocidad * 1.5f * Time.deltaTime, (collision.transform.position - transform.position) * -1);
            }
            else
            {
                scriptMov.SetVectorDT(velocidad * 0.5f * Time.deltaTime, (collision.transform.position - transform.position) * -1);
                Flanqueo();
            }
        }
        else
        {
            if(collision.CompareTag("Untagged"))
            {
                scriptMov.SetVectorDT(velocidad * 3f * Time.deltaTime, (collision.transform.position - transform.position) * -1);
            }
        }
    }


    void Flanqueo()
    {
        RaycastHit2D[] rayArr = new RaycastHit2D[2];
        RaycastHit2D ray;

        float distancia = deteccion;

        for (int i = 1; i <= 24; i++)
        {
            collPj.Raycast(Utilitys.VecFromDegs(Utilitys.DifAngulosVectores(Vector2.right, apuntar) + 15 * i), rayArr, deteccion * 2, layerMask);

            ray = rayArr[1];

            Debug.DrawRay(pos, Utilitys.VecFromDegs(Utilitys.DifAngulosVectores(Vector2.right, apuntar) + 15 * i), Color.magenta);

            RaycastHit2D ray2;

            collPj.Raycast(coord - ray.point, rayArr, Mathf.Infinity, layerMask);

            ray2 = Physics2D.Raycast(ray.point, coord - ray.point, layerMask);

            Debug.DrawRay(ray.point.Vec2to3(5), coord - ray.point, Color.cyan);

            //DebugPrint.Log(ray2.collider.name);

            if (ray2.collider != null)
                if ((ray2.collider.name == "Jugador" || ray2.collider.name == "Circle") && distancia * 4 > ray2.distance)//&& distancia*4 > ray2.distance
                {
                    Debug.DrawRay((ray.point.Vec2to3(5)), coord - ray.point, Color.blue,1);
                    der = ray.point - (pos.Vect3To2());
                    distancia = ray2.distance;
                }
        }

        if (der == Vector2.zero)
        {
            distancia = 0;
            for (int i = 1; i <= 24; i++)
            {
                collPj.Raycast(Utilitys.VecFromDegs(Utilitys.DifAngulosVectores(Vector2.right, apuntar) + 15 * i), rayArr, Mathf.Infinity, layerMask);

                ray = rayArr[1];
                //Debug.DrawRay(transform.position, Euler.VecFromDegs(Euler.DifAngulosVectores(Vector2.right, movimiento) + 15 * i), Color.red);
                //DebugPrint.Log("2d bucle");
                if (ray.collider != null)
                    if (distancia < ray.distance)
                    {
                        der = ray.point - pos.Vect3To2();
                        distancia = ray.distance;
                        Debug.DrawRay(pos, der, Color.red, 5);
                    }
            }
        }
    }

    private void OnDestroy()
    {
        if (enf != null)
            TimersManager.Destroy(enf);
        
        //Destroy(audioSource);

    }
}
