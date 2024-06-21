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

    [SerializeField]
    LoadSystem loadSystem;

    float fade=1;

    public void Progress(float percentage, string message)
    {
        if(percentage>=0)
            pantallaCarga.fillAmount = percentage / 100;

        if(message!=string.Empty)
            textoCarga.text = message;
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

        loadSystem.onStartLoad += Open;
        loadSystem.onFeedbackLoad += Progress;
        loadSystem.onFinishtLoad += Close;
    }

    private void Update()
    {
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, fade, Time.unscaledDeltaTime* transition);
        if(fade==0 && canvasGroup.alpha<0.01f)
            gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        loadSystem.onStartLoad -= Open;
        loadSystem.onFeedbackLoad -= Progress;
        loadSystem.onFinishtLoad -= Close;
    }
}
