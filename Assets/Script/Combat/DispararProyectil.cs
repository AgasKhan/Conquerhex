using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DispararProyectil : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject proyectil;

    public float enfriamientoDanio;

    public void Disparar(float danio, Vector2 movimiento, float velocidadProyectil, Vector2 pos)
    {
        GameObject spawnP = Instantiate(proyectil);

        DanioColision damageColision = spawnP.GetComponent<DanioColision>();

        Movement mov = spawnP.GetComponent<Movement>();

        if (pos == null)
            pos = this.gameObject.transform.position;

        damageColision.owner = "";

        damageColision.enfriamiento = enfriamientoDanio;

        damageColision.danio = danio;
        
        spawnP.transform.SetPositionAndRotation(
            pos.Vec2to3(5),
            Quaternion.Euler(0, 0, Mathf.Acos(movimiento.normalized.x) * Mathf.Sign(movimiento.y) * Mathf.Rad2Deg - 90)
        );

        mov.restaLineal = 0;

        mov.SetVectorDT(velocidadProyectil, movimiento);

        spawnP.GetComponent<SpriteRenderer>().color =new Color(danio / 5f, 1- danio / 5f, 1- danio / 5f);        
    }
}
