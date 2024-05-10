using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretKey : MonoBehaviour
{
    [SerializeField]
    StatisticsSubMenu statisticsSubMenu;
    [SerializeField]
    GameObject submenuRef;
    bool isInStatistics = false;

    public GameObject[] myObjects;
    public GameObject leverCorderito;

    public Character minion;

    Vector3 originalMinionPos;
    private void Awake()
    {
        originalMinionPos = minion.transform.position;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            leverCorderito.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
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
            if (!submenuRef.activeSelf && !isInStatistics)
            {
                statisticsSubMenu.Create(MenuManager.instance.character);
                isInStatistics = true;
            }
            else if (isInStatistics)
            {
                statisticsSubMenu.Exit();
            }
        }

        if (isInStatistics && !submenuRef.activeSelf)
            isInStatistics = false;

        if (Input.GetKeyDown(KeyCode.P))
            GameManager.instance.Reload();

    }

    public void ReviveMinion()
    {
        if (!minion.gameObject.activeSelf)
        {
            minion.health.Revive();
            minion.transform.position = originalMinionPos;
            minion.SetActiveGameObject(true);
        }
    }
}
