using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonB : MonoBehaviour
{
    [SerializeField]
    Image previewImage;

    [SerializeField]
    Button buttonLeft;

    [SerializeField]
    Button buttonRight;


    public ButtonB SetBarSprite(Sprite sprite)
    {
        previewImage.sprite = sprite;
        return this;
    }
    public ButtonB SetLeftButton(System.Action<Image> action)
    {
        AddListener(buttonLeft,action  );
        return this;
    }
    public ButtonB SetRightButton(System.Action<Image> action)
    {
        AddListener(buttonRight, action);
        return this;
    }

    private ButtonB AddListener(Button button,System.Action<Image> action)
    {
        button.onClick.AddListener(() => { action(previewImage); });

        return this;
    }
}
