using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavesManager : SingletonMono<WavesManager>
{
    [SerializeField]
    float secondsForVictory;

    [SerializeField]
    float secondsForWaves;

    [SerializeField]
    Pictionarys<int, ReSpawner[]> spawners = new Pictionarys<int, ReSpawner[]>();

    Timer waves;
    Timer victory;
    int currentWave = 0;

    string victoria;
    string oleada;

    protected override void Awake()
    {
        base.Awake();
        waves = TimersManager.Create(secondsForWaves, NextWave).Stop();
        victory = TimersManager.Create(secondsForVictory, VictoryTD).Stop();
    }
    private void Start()
    {
        victory.onChange += Victory_onChange;
        waves.onChange += Waves_onChange;

        waves.Start();
    }

    private void Waves_onChange(IGetPercentage arg1, float arg2)
    {
        var minutos = waves.current / 60;
        var segundos = waves.current % 60;
        oleada = ((int)minutos) + ":" + ((int)segundos);

        RefreshUI();
    }

    void RefreshUI()
    {
        Interfaz.SearchTitle("Tiempo").ShowMsg("Victoria: \t\t".RichText("color", "yellow") + victoria + "\n" + "Siguiente oleada: \t".RichText("color", "red") + oleada);
    }

    private void Victory_onChange(IGetPercentage arg1, float arg2)
    {
        var minutos = victory.current / 60;
        var segundos = victory.current % 60;
        victoria = ((int)minutos) + ":" + ((int)segundos);

        RefreshUI();
    }


    public void StartWaves()
    {
        victory.Start();
    }

    public void NextWave()
    {
        currentWave++;
        if(currentWave >= spawners.values.Length)
        {
            waves.Stop();
            return;
        }
        for (int i = 0; i < spawners[currentWave].Length ; i++)
        {
            spawners[currentWave][i].SetActiveGameObject(true);
            spawners[currentWave][i].Init();
        }

        Interfaz.SearchTitle("Titulo").AddMsg("Oleada " + currentWave);
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
