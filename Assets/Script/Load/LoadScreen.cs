using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadScreen : MonoBehaviour
{
    public Image pantallaCarga;

    public TextMeshProUGUI textoCarga;

    public CanvasGroup canvasGroup;

    [Range(0.1f,10)]
    public float transition=1;

    float fade=1;

    public void Progress(float percentage, string message)
    {
        //revisar
        //if(pantallaCarga != null)
            pantallaCarga.fillAmount = percentage / 100;

        textoCarga.text = message;
    }

    public void Progress(string message)
    {
        textoCarga.text = message;
    }

    public void Progress(float percentage)
    {
        pantallaCarga.fillAmount = percentage / 100;
    }

    public void Open()
    {
        fade = 1;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        fade = 0;
    }

    /*
    private void Awake()
    {
        var aux = GetComponentsInChildren<Image>();
        pantallaCarga = aux[aux.Length - 1];
    }
    */
    public IEnumerator LoadImage(System.Action<bool> end, System.Action<string> msg)
    {
        msg("LoadLoadSystem...");

        while(fade != 0 && canvasGroup.alpha < 0.95f)
        {
            //canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, fade, Time.deltaTime * 10);
            yield return null;
        }

        end(true);
    }

    private void Awake()
    {
        canvasGroup.alpha = fade;
    }

    private void Update()
    {
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, fade, Time.deltaTime* transition);
        if(fade==0 && canvasGroup.alpha<0.01f)
            gameObject.SetActive(false);
    }
}
