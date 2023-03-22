using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class car : MonoBehaviour
{
    

    // Update is called once per frame
    void Start()
    {
        prueba clase1 = new prueba();
        prueba clase2 = clase1;

        prueba2 estructura1 = new prueba2();
        prueba2 estructura2 = estructura1;

        clase2.Algo();

        estructura2.Algo();

        print("clases: " + clase1.num + " " + clase2.num
            +"\n"+
            "Estructuras: " + estructura1.num + " " + estructura2.num
            );
    }

}
class prueba
{
    public int num;

    public void Algo()
    {
        num = 3;
    }
}

struct prueba2
{
    public int num;

    public void Algo()
    {
        num = 3;
    }
}