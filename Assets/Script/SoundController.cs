using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    public string n;

    private void Start()
    {
        
        GetComponent<Slider>().value = PlayerPrefs.GetFloat(n, 0.6f);
        if (n == "menu")
            GetComponent<Slider>().value *= 10;
    }

    public void SaveVolumeValue(float f)
    {
        PlayerPrefs.SetFloat(n, f);

        //AudioManager.instance.UpdateSound();
    }
}
