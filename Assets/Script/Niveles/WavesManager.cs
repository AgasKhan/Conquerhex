using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavesManager : SingletonMono<WavesManager>
{
    //[SerializeField]
    //public List<GameObject[]> spawners = new List<GameObject[]>();
    [SerializeField]
    Character character;

    [SerializeField]
    float secondsForVictory;

    [SerializeField]
    float secondsForWaves;

    [SerializeField]
    Pictionarys<int, GameObject[]> spawners = new Pictionarys<int, GameObject[]>();

    Timer waves;
    Timer victory;
    int currentWave = 0;

    protected override void Awake()
    {
        waves = TimersManager.Create(secondsForWaves, NextWave).Stop();
        victory = TimersManager.Create(secondsForVictory, VictoryTD).Stop();
    }

    public void StartWaves()
    {
        waves.Start();
        victory.Start();
    }

    public void NextWave()
    {
        Debug.Log("Current: " + currentWave + " Count: " + spawners.values.Length);
        currentWave++;
        if(currentWave >= spawners.values.Length)
        {
            Debug.Log("-------------------Waves Stop");
            waves.Stop();
            return;
        }
        Debug.Log("---------------------Set Active");
        for (int i = 0; i < spawners[currentWave].Length ; i++)
        {
            spawners[currentWave][i].SetActive(true);
        }

        waves.Reset();
    }

    void VictoryTD()
    {
        GameManager.instance.Pause(true);

        MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(true).SetWindow("Victoria", "Has logrado sobrevivir a todas las oleadas \n\n" + "¿Deseas jugar de nuevo?".RichText("color", "#00ffffff"))
        .AddButton("Si", () => { LoadSystem.instance.Load("Simulation_Final", true); })
        .AddButton("No", () => { LoadSystem.instance.Load("MainMenu"); });

    }

}
