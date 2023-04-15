using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PopUp : MonoBehaviour
{
    //List<DoubleString> messages = new List<DoubleString>();

    TextMeshProUGUI myText;
    Button boton;

    void SetText(DoubleString text)
    {
        string aux = "";

        aux += text.superior.RichText("size", "45").RichText("color", "yellow") + "\n" + "\n";

        aux += text.inferior.RichText("size", "35");

        myText.text = aux;
    }


    void OnClickBack()
    {
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        myText = GetComponentInChildren<TextMeshProUGUI>();
        boton = GetComponentInChildren<Button>();
        boton.onClick.AddListener(OnClickBack);
    }

}
