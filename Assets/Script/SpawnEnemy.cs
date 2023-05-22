using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public MyEnemy enemyPref;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            enemyPref.Clone();
        }


    }
}
