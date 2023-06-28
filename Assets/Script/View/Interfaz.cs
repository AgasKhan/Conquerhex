using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Interfaz : MonoBehaviour
{
    static public Interfaz instance;

    public List<TextCompleto> textC = new List<TextCompleto>();

    public  Image   Dialogo;

    float   widthDiag;
    float   heightDiag;


    [Header("vida")]
    public Image vida;
    public TextMeshProUGUI textVida;
    public Image regen;
    public TextMeshProUGUI textRegen;

    public TextCompleto this[string name]
    {
        get
        {
            foreach (var item in textC)
            {
                if (item.name == name)
                    return item;
            }

            return null;
        }
    }

    static public TextCompleto SearchTitle(string name)
    {
        return instance[name];
    }

    /// <summary>
    /// el primer parametro debe de ser un IGetPercentage
    /// </summary>
    /// <param name="param"></param>
    void UpdateLife(params object[] param)
    {
        IGetPercentage getPercentage = param[0] as IGetPercentage;
        vida.fillAmount = getPercentage.Percentage();
        textVida.text = ((int)(float)param[1]).ToString();
    }

    void UpdateRegen(params object[] param)
    {
        IGetPercentage getPercentage = param[0] as IGetPercentage;
        regen.fillAmount = getPercentage.Percentage();
        textRegen.text = ((int)(float)param[1]).ToString();
    }

    private void Awake()
    {
        instance = this;

        foreach (var item in textC)
        {
            item.Init();
        }
    }

    void Start()
    {
        //defino la propiedad de vida
        //TitleSrchByName("Titulo secundario").timer.Set(6);
        SearchTitle("Titulo secundario").AddMsg("Presiona T para ver el tutorial");

        widthDiag = Dialogo.rectTransform.rect.width;
        heightDiag=Dialogo.rectTransform.rect.height;

        Dialogo.rectTransform.sizeDelta = Vector2.zero;
       
        LoadSystem.AddPostLoadCorutine(MyCoroutine);
 
    }

    IEnumerator MyCoroutine(System.Action<bool> end, System.Action<string> msg)
    {
        end(true);
        yield return null;
        EventManager.events.SearchOrCreate<EventGeneric>(LifeType.life).action += UpdateLife;
        EventManager.events.SearchOrCreate<EventGeneric>(LifeType.regen).action += UpdateRegen;
    }

    // Update is called once per frame
    /*
    void Update()
    {
        foreach (var item in titulosC)
        {            
            
            if (item.value.Write() && item.value.fade && item.value.timer.Chck)
            {
                item.value.Fade(Time.deltaTime,0);
            }
        }
        
        if(subtitulo.texto.text!="" || Dialogo.rectTransform.rect.width > 0)
        {
            float aux1 = 0;
            float aux2 = Dialogo.rectTransform.rect.width;

            if(subtitulo.texto.text != "")
            {
                aux1 = widthDiag;
                aux2 = widthDiag - Dialogo.rectTransform.rect.width;
                Dialogo.enabled = true;
            }

            Dialogo.rectTransform.sizeDelta= new Vector2( Mathf.Lerp ( Dialogo.rectTransform.rect.width, aux1 , Time.deltaTime * (widthDiag / aux2) ), heightDiag);
            subtitulo.texto.rectTransform.sizeDelta = new Vector2(Dialogo.rectTransform.sizeDelta.x - 100, subtitulo.texto.rectTransform.sizeDelta.y);

            if(Mathf.Approximately(Dialogo.rectTransform.sizeDelta.x, 0))
            {
                Dialogo.enabled = false;
            }   
        }   
    }
    */

}

[System.Serializable]
public class TextCompleto : Init
{
    public string name => texto.name;
    public TextMeshProUGUI texto;
    [SerializeReference]
    public Timer timer;
    [SerializeReference]
    public Timer letras;
    public string final;

    public float tiempoEntreLetras;

    [SerializeField]
    FadeOnOff fadeMenu;

    private void FadeMenu_alphas(float obj)
    {
        texto.color = texto.color.ChangeAlphaCopy(obj);
    }

    public void ClearMsg()
    {
        final = "";
        texto.text = "";
    }

    public void ShowMsg(string msg)
    {
        final = msg;
        texto.text = msg;
        On();
    }

    public void AddMsg(string msg)
    {
        if (final == "")
            msg = texto.text + msg;

        final += msg + "\n";

        On();
    }

    void On()
    {
        SetFade(1);
        timer.Reset();
        letras.Start();
    }

    void SetFade(float end)
    {
        fadeMenu.SetFade(texto.color.a, end);
    }

    void Write()
    {
        if (texto.isTextOverflowing)
        {
            final = final.Substring(final.IndexOf("\n") + 1);
            texto.text = texto.text.Substring(texto.text.IndexOf("\n") + 1);
        }
        if (texto.text == final && final != "")
        {
            final = "";
        }
        else if (final != "")
        {
            texto.text += final[texto.text.Length];
            timer.Reset();
        }
    }

    public void Init(params object[] param)
    {
        fadeMenu.alphas += FadeMenu_alphas;

        fadeMenu.end += FadeMenu_end;

        fadeMenu.Init();

        letras = TimersManager.Create(tiempoEntreLetras, Write).SetLoop(true).Stop();

        timer = TimersManager.Create(3, () => {

            SetFade(0);
            letras.Stop();

            });
    }

    private void FadeMenu_end()
    {
        if(final=="")
        {
            texto.text = "";
        }
    }
}
