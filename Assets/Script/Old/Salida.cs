using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Salida : MonoBehaviour
{
    //public Menu menu;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            //MenuManager.instance.ShowWindow("Victory");
            //menu.ShowMenu("Victoria!", "Has logrado llegar hasta el dirigible para escapar", false, true);
            Time.timeScale = 0;
        }
    }
    
}
