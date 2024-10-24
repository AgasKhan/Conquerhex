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

    [SerializeField]
    LayoutGroup layoutGroup;

    public DetailsWindow SetTexts(DoubleString d)
    {
        SetTexts(d.superior, d.inferior);

        //Utilitys.LerpInTime(() => instance.scrollbar.value, 1, 0.3f, Mathf.Lerp, (save) => { instance.scrollbar.value = save; });

        return this;
    }
    public DetailsWindow SetTexts(string title, string description)
    {
        myTitle.text = title;
        myDescription.text = description;

        myTitle.SetActiveGameObject(title!="");

        myDescription.SetActiveGameObject(description != "");

        GameManager.RetardedOn((_bool)=> layoutGroup.SetActive(_bool));

        return this;
    }

    public DetailsWindow SetAlignment(TextAlignmentOptions alignmentOptions)
    {
        return SetAlignment(alignmentOptions, alignmentOptions);
    }

    public DetailsWindow SetAlignment(TextAlignmentOptions titleAlignment, TextAlignmentOptions descriptionAlignment)
    {
        myTitle.alignment = titleAlignment;

        myDescription.alignment = descriptionAlignment;

        return this;
    }

    public DetailsWindow SetImage(Sprite sprite = null)
    {
        previewImage.SetActiveGameObject(sprite != null);

        if (sprite != null)
            previewImage.sprite = sprite;

        GameManager.RetardedOn((_bool) => layoutGroup.SetActive(_bool));

        return this;
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

    private void OnDestroy()
    {
        fadeMenu.Stop();
    }

}
