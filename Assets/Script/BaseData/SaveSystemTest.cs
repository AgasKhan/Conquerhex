using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystemTest : MonoBehaviour
{
    public GameObject objToSave;

    public BaseData baseData;

    public Transform parent;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("Guardadndo objeto: " + objToSave.name);
            baseData.SaveObject(objToSave);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("Cargando objeto: " + objToSave.name);
            baseData.LoadAll(parent);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Eliminando todos los archivos" );
            baseData.DeleteAll();
        }

    }



}
