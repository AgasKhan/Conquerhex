using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Interfaz : MonoBehaviour
{
    static  public  List<TextCompleto>  titulosC = new List<TextCompleto>();

            public  TextMeshProUGUI[]   titulos;

            public  float               tiempoEntreLetrasPublic;

            public  Image               Dialogo;

                    float               widthDiag;
                    float               heightDiag;

    public Image vida;

    static public Image health;

    TextCompleto tiempo;
    TextCompleto subtitulo;

    private void Awake()
    {
        titulosC.Clear();

        health = vida;

        foreach (var item in titulos)
        {
            item.text = "";
            titulosC.Add(new TextCompleto(item, tiempoEntreLetrasPublic));
        }
    }
    void Start()
    {
        //defino la propiedad de vida
        tiempo = TitleSrchByName("Tiempo");
        tiempo.fade = false;

        TitleSrchByName("Titulo secundario").timer.DefaultTimer(6);
        TitleSrchByName("Titulo secundario").Message("Presiona T para ver el tutorial");

        subtitulo = TitleSrchByName("Subtitulo");
        subtitulo.timer.DefaultTimer(6);

        widthDiag =Dialogo.rectTransform.rect.width;
        heightDiag=Dialogo.rectTransform.rect.height;

        Dialogo.rectTransform.sizeDelta = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var item in titulosC)
        {            
            
            if (item.Write() && item.fade && item.timer.CheckAndSub())
            {
                item.Fade(Time.deltaTime,0);
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

    static public TextCompleto TitleSrchByName(string name)
    {
        TextCompleto aux = null;
        //inicializo si no esta inicializada la lista
        //Saque la inicializacion y la puse en el awake
        //la recorro y devuelvo el resultado de la busqueda
        foreach (var item in titulosC)
        {
            if(item.name==name)
            {
                aux = item;
            }
        }
        return aux;
    }

}


public class TextCompleto
{
    public string name;
    public TextMeshProUGUI texto;
    public Timers timer;
    public Timers letras;
    public string final;
    public bool fade;

    public void Show(string palabra)
    {
        texto.color = new Color(texto.color.r, texto.color.g, texto.color.b, 1);
        texto.text = palabra;
        final = palabra;
    }

    public void Message(string palabra)
    {

        if (final=="")
            final = (texto.text + "\n" + palabra).Trim();
        
        else
            final += "\n" + palabra;

        if(fade)
            texto.color = new Color(texto.color.r, texto.color.g, texto.color.b, 1);
        
    }

    public bool Write()
    {
        

        if(texto.isTextOverflowing)
        {
            final = final.Substring(final.IndexOf("\n")+1);
            texto.text = texto.text.Substring(texto.text.IndexOf("\n")+1);
        }

        if (texto.text == final && final!="")
        {
            final = "";            
        }
        else if(final != "")
        {
            if (letras.CheckAndSub())
            {
                texto.text += final[texto.text.Length];

                AudioManager.instance.Play("tec"+Random.Range(1,4));

                letras.RestartTimer();
                timer.RestartTimer();
            }
            return false;
        }
        return true;
    }

    public void Fade(float t, float a)
    {

        if((texto.color.a - a) != 0)
        {
            if (Mathf.Abs(texto.color.a - a) * 100 < 1)
                texto.color = new Color(texto.color.r, texto.color.g, texto.color.b, a);
            else if (Mathf.Abs(texto.color.a - a) * 100 < 15)
                t *= 4;
            else if (Mathf.Abs(texto.color.a - a) * 100 < 30)
                t *= 2;
        }
    
        if (texto.color.a!=a)
        {
            texto.color = new Color(texto.color.r, texto.color.g, texto.color.b, Mathf.Lerp(texto.color.a, a, t));
        }
        else if (a==0)
        {
            texto.text = "";
            final = "";
        }
    }

    public TextCompleto(TextMeshProUGUI o, float t=0 , bool fadeActivacion=true,string palabraInicial="")
    {
        name = o.name;
        texto = o;
        letras = CoolDown.CreateCd("letras-"+name + o.GetInstanceID(), t);
        final = palabraInicial;
        fade = fadeActivacion;
        timer = CoolDown.CreateCd("timer-" + name + o.GetInstanceID(), 3);
    }
}
