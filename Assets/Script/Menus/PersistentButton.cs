using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentButton : MonoBehaviour
{
    private void OnEnable()
    {
        var textChild = transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
        var imageChild = GetComponent<UnityEngine.UI.Image>();
        
        if(SaveWithJSON.CheckKeyInBD(name))
        {
            if (!SaveWithJSON.LoadFromPictionary<bool>(name))
            {
                textChild.text = "MUTE";
                imageChild.color = Color.gray;
            }
        }
    }
}
