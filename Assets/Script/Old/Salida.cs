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
            GameManager.instance.Pause(true);

            var aux = collision.GetComponent<Character>();
            MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(true).SetWindow("Victoria", "Has logrado llegar hasta el dirigible para escapar \n\n"+ "¿Deseas volver a la base con todo lo recolectado?".RichText("color", "#00ffffff"))
            .AddButton("Si", () => { aux.AddOrSubstractItems("Coin", 10); LoadSystem.instance.LoadAndSavePlayer("Base", true); })
            .AddButton("No", () => { MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false); GameManager.instance.Pause(false);});

            //menu.ShowMenu("Victoria!", "Has logrado llegar hasta el dirigible para escapar", false, true);
            //Time.timeScale = 0;
        }
    }
    
}
