using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StaminaSystem : MonoBehaviour
{
    [SerializeField] Character character;

    [SerializeField] int maxStamina = 10;
    int currentStamina;
    DateTime nextStaminaTime;
    DateTime lastStaminaTime;
    [SerializeField] float timeToCharge = 10f;

    bool recharging;

    int notifID;

    private void Start()
    {
        if (PlayerPrefs.HasKey("currentStamina"))
        {
            Load();
        }
        else
        {
            currentStamina = maxStamina;
            Save();
        }

        UpdateUI();
        StartCoroutine(UpdateStamina());

        #if UNITY_ANDROID
        if (currentStamina < maxStamina)
        {
            notifID = NotificationsSystem.SendNotification(AddDuration(lastStaminaTime, timeToCharge * (maxStamina - currentStamina)),
                                     "Volvé a jugar", "Se te recargó toda la energía", "stamina_ch");
        }
        #endif
    }


    IEnumerator UpdateStamina()
    {
        UpdateTimer();
        recharging = true;
        while (currentStamina < maxStamina)
        {
            DateTime currentTime = DateTime.Now;
            DateTime nextTime = nextStaminaTime;


            bool addingStamina = false;
            while (currentTime > nextTime)
            {
                if (currentStamina >= maxStamina) break;


                currentStamina += 1;
                addingStamina = true;
                DateTime timeToAdd = nextTime;

                if (lastStaminaTime > nextTime)
                    timeToAdd = lastStaminaTime;

                nextTime = AddDuration(timeToAdd, timeToCharge);
            }

            if (addingStamina)
            {
                nextStaminaTime = nextTime;
                lastStaminaTime = DateTime.Now;
            }

            UpdateUI();
            UpdateTimer();
            Save();

            yield return new WaitForEndOfFrame();
        }
        recharging = false;
    }

    void UpdateTimer()
    {
        if (currentStamina >= maxStamina)
        {
            return;
        }
    }

    public void RechargeStamina(int staminaToAdd)
    {
        currentStamina += staminaToAdd;

        UpdateUI();
        Save();
        if (currentStamina >= maxStamina)
        {
            if (recharging)
            {
                recharging = false;
                StopAllCoroutines();
            }
        }
    }


    DateTime AddDuration(DateTime date, float duration)
    {
        return date.AddSeconds(duration);
    }

    bool HasEnoughStamina(int stamina) => currentStamina >= stamina;

    void UpdateUI()
    {
        character.AddOrSubstractItems("PortalFuel", currentStamina);
    }

    void Save()
    {
        PlayerPrefs.SetInt("currentStamina", currentStamina);
        PlayerPrefs.SetString("nextStaminaTime", nextStaminaTime.ToString());
        PlayerPrefs.SetString("lastStaminaTime", lastStaminaTime.ToString());
    }

    void Load()
    {
        currentStamina = PlayerPrefs.GetInt("currentStamina");
        nextStaminaTime = StringToDateTime(PlayerPrefs.GetString("nextStaminaTime"));
        lastStaminaTime = StringToDateTime(PlayerPrefs.GetString("lastStaminaTime"));
    }

    DateTime StringToDateTime(string date)
    {
        if (string.IsNullOrEmpty(date))
        {
            return DateTime.Now;
        }

        return DateTime.Parse(date);
    }


    /*
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

    }*/
}
