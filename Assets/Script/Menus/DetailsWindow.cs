using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DetailsWindow : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI myTitle;

    [SerializeField]
    TextMeshProUGUI myDescription;

    [SerializeField]
    Image previewImage;

    [SerializeField]
    FadeOnOff fadeMenu;

    public void ShowOrHide(bool condition)
    {
        gameObject.SetActive(condition);
    }

    public void ModifyTexts(DoubleString d)
    {
        myTitle.text = d.superior;
        myDescription.text = d.inferior;

        //Utilitys.LerpInTime(() => instance.scrollbar.value, 1, 0.3f, Mathf.Lerp, (save) => { instance.scrollbar.value = save; });

    }
    public void ModifyTexts(string title, string description)
    {
        myTitle.text = title;
        myDescription.text = description;

        //Utilitys.LerpInTime(() => instance.scrollbar.value, 1, 0.3f, Mathf.Lerp, (save) => { instance.scrollbar.value = save; });
    }

 
    public void PreviewImage(bool active, Sprite sprite = null)
    {
        previewImage.gameObject.SetActive(active);

        if (sprite != null)
            previewImage.sprite = sprite;
    }

    public void SetWindow(Sprite sprite, DoubleString ds)
    {
        PreviewImage(true, sprite);
        ModifyTexts(ds);
    }

    public void SetWindow(Sprite sprite, string title, string description)
    {
        PreviewImage(sprite!=null, sprite);
        ModifyTexts(title, description);
        ShowOrHide(false);
        ShowOrHide(true);
    }

    private void Awake()
    {
        fadeMenu.alphas += Fade_Event;
        fadeMenu.Init();
    }

    private void Fade_Event(float obj)
    {
        myTitle.color = myTitle.color.ChangeAlphaCopy(obj);
        myDescription.color = myDescription.color.ChangeAlphaCopy(obj);
        previewImage.color = previewImage.color.ChangeAlphaCopy(obj);
    }

    private void OnEnable()
    {
        fadeMenu.FadeOn();
    }

}
