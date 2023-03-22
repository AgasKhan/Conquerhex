using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CoolDownUpdate : MonoBehaviour
{
    private void Update()
    {
        CoolDown.Update();
    }
}

static public class CoolDown
{
    readonly static List<Timers> timers = new List<Timers>();

    static public void Update()
    {
        for (int i = 0; i < timers.Count; i++)
        {
            timers[i].CheckAndSub(Time.deltaTime);
            //DebugPrint.Log(timers[i].name + timers[i].CheckAndSub());
        }
    }

    static public Timers CreateCd(string name, float time = 0)
    {
        Timers aux = SrchCd(name, false);

        if (aux == null)
        {
            timers.Add(new Timers(time, name));
            DebugPrint.Log("Se creo el timer " + name + " :D");
            return timers[timers.Count - 1];
        }
        
        DebugPrint.Warning("No se creo el timer "+name+ " por que ya existe");
        return aux;

    }

    static public void DestroyCd(string name)
    {
        timers.Remove(SrchCd(name));
    }

    static public Timers SrchCd(string name, bool falla=true)
    {
        for (int i = 0; i < timers.Count; i++)
        {
            if (timers[i].name == name)
            {
                return timers[i];
            }
        }
        if(falla)
            DebugPrint.Warning("No encontro al objeto "+name);
        return null;
    }
}

public class Timers
{
    public string name;

    float tiempo;

    float original;

    public bool CheckAndSub(float t = 0)
    {
        if (tiempo > 0)
        {

            if ((tiempo -= t) <= 0)
                return true;

            else
                return false;
        }
        else
            return true;
    }

    public void AddTimer(float num)
    {
        tiempo += num;
    }
    
    public void DefaultTimer(float num)
    {
        tiempo = num;
        original = num;
    }

    public void RestartTimer()
    {
        tiempo = original;
    }

    public Timers(float tie, string nombre="")
    {
        DefaultTimer(tie);
        name = nombre;
    }

}
