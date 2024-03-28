using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{
    public GameObject canvas;

    public List<OldSubMenu> subMenus;

    public TextMeshProUGUI[] textMesh;

    public GameObject subMenuCargado;

    public TMP_InputField[] cargaDatos;

    public TextMeshProUGUI[] muestraDatos;

    // Start is called before the first frame update
    //    bool loadscene = false;

    Hexagone[] arrHexCreados => HexagonsManager.arrHexCreados;

    public void ChangeHex(string number)
    {
        if(number.Length<11)
        {
            ulong.TryParse(number, out ulong hex);

            if (hex < int.MaxValue && hex >= 0)
                PlayerPrefs.SetInt("Hex", int.Parse(hex.ToString()));
            else
                DebugPrint.Log("El numero ingresado es demasiado grande");
        }
        else
            DebugPrint.Log("Mucho texto");
       
    }

    public void ChangeNivelMin(string number)
    {
        int.TryParse(number, out int Nivel);
        PlayerPrefs.SetInt("nivelMin", Nivel);
    }

    public void ChangeNivelMax(string number)
    {
        int.TryParse(number, out int Nivel);
        PlayerPrefs.SetInt("nivelMax", Nivel);
    }

    public IEnumerator CerrarhexagonosCO(System.Action<bool> end, System.Action<string> msg)
    {

        if (GameManager.instance != null)
        {
           
            for (int i = 0; i < arrHexCreados.Length; i++)
            {
                if(arrHexCreados[i]!=null)
                    arrHexCreados[i].gameObject.SetActive(false);
            }
            
            yield return null;

            for (int i = 0; i < arrHexCreados.Length; i++)
            {
                if (arrHexCreados[i] != null && arrHexCreados[i].transform.Find("Jugador") == null)
                    Destroy(arrHexCreados[i]);

                if (i % 100 == 0)
                {
                    msg("Destruidos: " + i + " de " + arrHexCreados.Length);
                    yield return null;
                }
            }
        }
        yield return null;

        end(true);
    }

    public void ShowMenu()
    {
        canvas.SetActive(!canvas.activeSelf);
        Actualizar();
    }
    
    //tenes que encapsular todo lo que vas a sapawnear
    //en un empty, cargarlo en un array o lista y luego realizar la busqueda para spawnearlo
    
    //cargar un menu prefab
    public void ShowMenu(string submenu)
    {
        ShowMenu();
        canvas.transform.GetChild(3).gameObject.SetActive(false);
        if(submenu != "")
        {
            for (int i = 0; i < subMenus.Count; i++)
            {
                if (subMenus[i].name == submenu)
                {
                    if (canvas.transform.childCount == 7)
                    {
                        Destroy(canvas.transform.GetChild(6).gameObject);

                    }
                    //canvas.transform.GetChild(3).gameObject.SetActive(false);


                    subMenuCargado = Instantiate(subMenus[i].prefab, canvas.transform);
                    
                    
                    
                    subMenuCargado.SetActive(true);

                }
            }
        }
        
        canvas.transform.GetChild(4).gameObject.SetActive(true);
        canvas.transform.GetChild(5).gameObject.SetActive(false);
        //canvas.transform.GetChild(5).gameObject.SetActive(false);
    }

    public void ShowMenu(string submenu, bool btn2)
    {
        ShowMenu(submenu);
        canvas.transform.GetChild(5).gameObject.SetActive(btn2);
    }

    public void ShowMenu(string titulo, string parrafo, bool btn1, bool btn2)
    {
        if (canvas.transform.GetChild(6) != null)
        {
            Destroy(canvas.transform.GetChild(6).gameObject);
        }

        textMesh[0].text = titulo;
        textMesh[1].text = parrafo;
        
        ShowMenu("Default");
        
        canvas.transform.GetChild(4).gameObject.SetActive(btn1);
        canvas.transform.GetChild(5).gameObject.SetActive(btn2);


    }

    public void Close()
    {
        Application.Quit();
    }

    private void Awake()
    {
        GameManager.OnPause += () => ShowMenu();

        GameManager.OnPlay += () => ShowMenu();
    }

    private void Start()
    {
        subMenus.Add(new OldSubMenu("Default", canvas.transform.GetChild(3).gameObject));

        if (subMenuCargado != null)
            subMenuCargado = Instantiate(subMenuCargado, canvas.transform);

        /*
        if (!AudioManager.instance.Srch("menu").source.isPlaying)
            AudioManager.instance.Play("menu");
        */
        Actualizar();

        LoadSystem.AddPreLoadCorutine(CerrarhexagonosCO);
    }

    private void OnEnable()
    {
        foreach (TMP_InputField item in cargaDatos)
        {
            item.text = PlayerPrefs.GetInt(item.name, 0).ToString();
            //DebugPrint.Log(item.name);
        }
        //DebugPrint.Print();


    }


    void Actualizar()
    {
        foreach (TextMeshProUGUI item in muestraDatos)
        {
            item.text = PlayerPrefs.GetInt(item.name, 0).ToString();
            //DebugPrint.Log(item.name);
        }
    }

}

[System.Serializable]
public struct OldSubMenu
{
    public string name;
    public GameObject prefab;

    public OldSubMenu(string n, GameObject g)
    {
        name = n;
        prefab = g;
    }
}