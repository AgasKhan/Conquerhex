using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StaminaSystem : MonoBehaviour
{
    [SerializeField]
    int maxStamina;

    [SerializeField]
    float minutesToReward;

    [SerializeField]
    Character character;

    DateTime savedTime;

    void Start()
    {
        GiveStamina();
    }


    public void SaveTime()
    {
        savedTime = DateTime.Now;
        SaveWithJSON.SaveInPictionary("SavedTime", savedTime.ToString());
    }

    public float TimePassed()
    {
        if (!SaveWithJSON.BD.ContainsKey("SavedTime"))
            return 0f;

        savedTime = DateTime.Parse(SaveWithJSON.LoadFromPictionary<string>("SavedTime"));

        TimeSpan timePassed = DateTime.Now - savedTime;

        float aux = (float)timePassed.TotalMinutes;

        return aux;
    }

    public void GiveStamina()
    {
        float timePassed = TimePassed();
        int amount = 0;

        while (timePassed > minutesToReward)
        {
            amount++;
            if (amount > maxStamina)
                amount = maxStamina;

            timePassed -= minutesToReward;
        }

        if (amount > 0)
        {
            character.AddOrSubstractItems("PortalFuel", amount);
            Debug.Log("Se le dio al jugador " + amount + " poratl fuels " + " por haber pasado " + TimePassed() + " minutos");
            
            SaveTime();
        }
        else
            Debug.Log("No se le dio stamina al jugador");

    }
}
