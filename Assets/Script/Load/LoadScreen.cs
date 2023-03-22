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

    float fade;

    public void Progress(float percentage, string message)
    {
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

    private void Update()
    {
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, fade, Time.deltaTime);
        if(fade==0 && canvasGroup.alpha<0.01f)
            gameObject.SetActive(false);
    }
}
