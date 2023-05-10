using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentButton : MonoBehaviour
{
    private void OnEnable()
    {
        var textChild = transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
        var imageChild = GetComponent<UnityEngine.UI.Image>();
        
        if(SaveWithJSON.CheckKeyInBD(transform.parent.name))
        {
            if (SaveWithJSON.LoadFromPictionary<float>(transform.parent.name) <= 0.0001f)
            {
                textChild.text = "MUTE";
                imageChild.color = Color.gray;
            }
        }
    }
}
