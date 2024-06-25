using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Salida : Building
{
    //public Menu menu;
    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            GameManager.instance.Menu(true);
    */
    //var aux = collision.GetComponent<Character>();
    //MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(true).SetWindow("Victoria", "Has logrado llegar hasta el dirigible para escapar \n\n"+ "¿Deseas volver a la base con todo lo recolectado?".RichText("color", "#00ffffff"))


    /*.AddButton("Si", () => { aux.inventory.AddOrSubstractItems("Coin", 10); LoadSystem.instance.LoadAndSavePlayer("Base", true); })
    .AddButton("No", () => { MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false); GameManager.instance.Menu(false);})*/
    /*;

    //menu.ShowMenu("Victoria!", "Has logrado llegar hasta el dirigible para escapar", false, true);
    //Time.timeScale = 0;
}
}*/
    [SerializeField]
    SpriteRenderer sprite;

    protected override void Config()
    {
        base.Config();

        MyAwakes += MyAwake;
    }

    void MyAwake()
    {
        //interactComp.OnInteract += SetRewards;
        health.noLife += Health_noLife;
        health.death += Health_noLife;
    }

    private void Health_noLife()
    {
        interactComp.ChangeInteract(true);
        team = Team.noTeam;

        sprite.color = Color.yellow;
    }

    public void Victory()
    {
        GameManager.instance.Menu(true);
        MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(true)
               .SetWindow("Demo terminada", "Gracias por pasarte la demo de Arrange the Heaven \n\n¿Deseas cerrar el juego?")
               .AddButton("Si", Application.Quit)
               .AddButton("No", () => { GameManager.instance.Menu(false); MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(false); });

    }
}
