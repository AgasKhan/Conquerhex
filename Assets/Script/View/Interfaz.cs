using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Interfaz : MonoBehaviour
{
    static public Interfaz instance;

    public List<TextCompleto> textC = new List<TextCompleto>();

    [Header("Dialogo")]

    [SerializeField] Image dialogoImage;
    TextCompleto dialogoText;

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
        textVida.text = ((int)getPercentage.actual).ToString();
    }

    void UpdateRegen(params object[] param)
    {
        IGetPercentage getPercentage = param[0] as IGetPercentage;
        regen.fillAmount = getPercentage.Percentage();
        textRegen.text = ((int)getPercentage.actual).ToString();
    }

    private void Awake()
    {
        instance = this;

        foreach (var item in textC)
        {
            item.Init();
        }

        dialogoText = SearchTitle("Subtitulo");
    }

    void Start()
    {
        SearchTitle("Titulo secundario").ClearMsg();

        dialogoText.on += () => enabled = true;
        dialogoText.ClearMsg();

        widthDiag = dialogoImage.rectTransform.rect.width;
        heightDiag=dialogoImage.rectTransform.rect.height;

        dialogoImage.rectTransform.sizeDelta = Vector2.zero;
       
        LoadSystem.AddPostLoadCorutine(MyCoroutine);
    }

    IEnumerator MyCoroutine(System.Action<bool> end, System.Action<string> msg)
    {
        end(true);
        yield return null;
        EventManager.events.SearchOrCreate<EventGeneric>(LifeType.life).action += UpdateLife;
        EventManager.events.SearchOrCreate<EventGeneric>(LifeType.regen).action += UpdateRegen;
    }

    void Update()
    {       
        if(dialogoText.text!="" || dialogoImage.rectTransform.rect.width > 0)
        {
            float aux1 = 0;
            float aux2 = dialogoImage.rectTransform.rect.width;

            if(dialogoText.text != "")
            {
                aux1 = widthDiag;
                aux2 = widthDiag - dialogoImage.rectTransform.rect.width;
                dialogoImage.enabled = true;
            }

            dialogoImage.rectTransform.sizeDelta= new Vector2( Mathf.Lerp ( dialogoImage.rectTransform.rect.width, aux1 , Time.deltaTime * (widthDiag / aux2) ), heightDiag);
            //dialogoText.texto.rectTransform.sizeDelta = new Vector2(dialogoImage.rectTransform.sizeDelta.x - 100, dialogoText.texto.rectTransform.sizeDelta.y);

            if (Mathf.Approximately(dialogoImage.rectTransform.rect.width, 0))
            {
                dialogoImage.enabled = false;
                enabled = false;
            }   
        }   
    }
    

}

[System.Serializable]
public class TextCompleto : Init
{
    [Header("Seteo")]

    [SerializeField] TextMeshProUGUI texto;

    
    [field: SerializeField] [field: TextArea(3, 6)] public string final { get; private set; }

    [SerializeField] float tiempoEntreLetras;

    [SerializeField] float tiempoParaDesaparecer = 3;

    [SerializeField]
    FadeOnOff fadeMenu;

    [Header("Para debug")]
    [SerializeReference]
    Timer timer;
    [SerializeReference]
    Timer letras;

    public event System.Action on;

    public event System.Action off;

    public string name => texto.name;

    public string text => texto.text;

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
        fadeMenu.end -= FadeMenu_end;
        SetFade(1);
        timer.Reset();
        letras.Start();
        on?.Invoke();
    }

    void SetFade(float end)
    {
        fadeMenu.SetFade(texto.color.a, end);
    }

    void ChecktOverflowing()
    {
        if (texto.isTextOverflowing && fadeMenu.timerOn.Chck)
        {
            var aux = new char[] { ' ', '\t', '\n' };
            final = final.Substring(final.IndexOfAny(aux) + 1);
            texto.text = texto.text.Substring(texto.text.IndexOfAny(aux) + 1);
        }
    }

    void Write()
    {
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

        fadeMenu.Init();

        letras = TimersManager.Create(tiempoEntreLetras, ChecktOverflowing ,Write).SetLoop(true).Stop();

        timer = TimersManager.Create(tiempoParaDesaparecer, () => {

            fadeMenu.end += FadeMenu_end;
            SetFade(0);
            letras.Stop();

            });
    }

    private void FadeMenu_end()
    {
        texto.text = "";
        off?.Invoke();
    }
}
