using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretKey : MonoBehaviour
{
    [SerializeField]
    StatisticsSubMenu statisticsSubMenu;
    bool showStatistics = false;

    public GameObject[] myObjects;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            for (int i = 0; i < myObjects.Length; i++)
            {
                myObjects[i].SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(true)
                .SetWindow("", "¿Seguro que deseas cerrar el juego?")
                .AddButton("Si", Application.Quit)
                .AddButton("No", () => MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(false));
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!showStatistics)
            {
                statisticsSubMenu.Create(MenuManager.instance.character);
                showStatistics = true;
            }
            else
            {
                statisticsSubMenu.Exit();
                showStatistics = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
            GameManager.instance.Reload();

    }
}
