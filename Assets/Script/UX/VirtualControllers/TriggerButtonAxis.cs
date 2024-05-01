using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    [CreateAssetMenu(menuName = "Controllers/TriggerButtonAxis")]
    public class TriggerButtonAxis : TriggerAxis
    {
        [SerializeField]
        string button;

        public override void MyUpdate()
        {
            //UpdateAxis();

            dir = Input.mousePosition - new Vector3(Screen.width/2,Screen.height/2,0);

            if (Input.GetButtonDown(button))
            {
                axis.OnEnterState(dir);
                press = true;
            }

            if (Input.GetButtonUp(button))
            {
                axis.OnExitState(dir);
                press = false;
            }

            if (press)
            {
                axis.OnStayState(dir);
            }
        }
    }

}


