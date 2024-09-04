using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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

    public GameObject newMenu;

    public EventControllerMediator escapeEventMediator;

    private void Awake()
    {
        if(minion!=null)
            originalMinionPos = minion.transform.position;

        escapeEventMediator = new EventControllerMediator();
        escapeEventMediator.eventDown += EscapeEventMediator_eventDown;

        VirtualControllers.Escape.SuscribeController(escapeEventMediator);
        

        //var timeToOff = TimersManager.Create(0.1f, () => gameObject.SetActive(false));
    }

    private void EscapeEventMediator_eventDown(Vector2 arg1, float arg2)
    {
        if(submenuRef.activeSelf)
        {
            statisticsSubMenu.TriggerMyOnClose();
        }
        else
        {
            MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(true)
                .SetWindow("", "�Seguro que deseas cerrar el juego?")
                .AddButton("Si", Application.Quit)
                .AddButton("No", () => { MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(false); });
        }
    }

    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Comma) && leverCorderito != null)
        {
            leverCorderito.SetActive(true);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha0) && myObjects != null)
        {
            for (int i = 0; i < myObjects.Length; i++)
            {
                myObjects[i].SetActive(true);
            }
        }
        */
        /*
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(true)
                .SetWindow("", "�Seguro que deseas cerrar el juego?")
                .AddButton("Si", Application.Quit)
                .AddButton("No", () => { GameManager.instance.Menu(false); MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(false); });
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
                statisticsSubMenu.TriggerMyOnClose();
            }
        }

        if (isInStatistics && !submenuRef.activeSelf)
            isInStatistics = false;

        if (Input.GetKeyDown(KeyCode.P))
            GameManager.instance.Reload();

        if(Input.GetKeyDown(KeyCode.Alpha9))
        {
            if (SceneManager.GetActiveScene().name == "Hexagonos test")
            {
                GameManager.instance.Load("PruebaTesis");
            }
            else
            {
                GameManager.instance.Load("Hexagonos test");
            }
        }
        */

        if (SceneManager.GetActiveScene().name == "MainMenu" && Input.GetKeyDown(KeyCode.P))
        {
            GameManager.instance.Load("DummyPractice");
        }


        if (SceneManager.GetActiveScene().name == "DummyPractice" && Input.GetKeyDown(KeyCode.P))
        {
            GameManager.instance.Load("MainMenu");
        }

        if (SceneManager.GetActiveScene().name != "MainMenu" && UIE_MenusManager.instance != null && Input.GetKeyDown(KeyCode.Tab))
        {
            if (!UIE_MenusManager.instance.isInMenu)
                UIE_MenusManager.instance.EnableMenu(UIE_MenusManager.instance.EquipmentMenu);
            else
                UIE_MenusManager.instance.TriggerOnClose();
        }

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

    GenericSubMenu genMenu;
    public InteractEntityComponent interComp;

    [ContextMenu("TestMenu")]
    public void TestMenus()
    {
        genMenu = new GenericSubMenu(interComp);
        genMenu.createAction = (subMenu) =>
        {
            subMenu.ClearBody();
            subMenu.CreateSection(0, 20);
            subMenu.AddComponent<ListNavBarModule>();
            //subMenu.CreateChildrenSection<ScrollRect>();
            //subMenu.AddComponent<DetailsWindow>().SetTexts("B�sicos", "Ataque b�sico: click izq \nHabilidad basica: Click der\nHabilidad Alternativa: shift izquierdo\nAlgunas habilidades apuntaran en direccion del mouse y otras dependeran del movimiento");

            //subMenu.CreateSection(4, 6);
            //subMenu.CreateChildrenSection<ScrollRect>();
            //subMenu.AddComponent<DetailsWindow>().SetTexts("Titulo", "Descripcion detallada lo suficientemente larga como para que ocupeena parte significativa de la pantralla y asi probar la posicion y capacidad de texto de la text box");

            //subMenu.CreateSection(4, 5);
            //subMenu.CreateChildrenSection<ScrollRect>();
            //subMenu.AddComponent<DetailsWindow>().SetTexts("Habilidades", "Combinaci�n de teclas (movimiento) +  Click der");
            

            //subMenu.CreateSection(5, 6);
            //subMenu.CreateChildrenSection<ScrollRect>();
            //subMenu.AddComponent<DetailsWindow>().SetTexts("(Katas) Movimientos ofensivos", "Combinacion de teclas (movimiento) +  Click izq");
            

            //subMenu.OnClose += Exit;
        };

        genMenu.Create(MenuManager.instance.character);
    }

}
