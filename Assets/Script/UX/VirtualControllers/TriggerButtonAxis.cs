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

        [SerializeField, Tooltip("en caso de ser falso leera el axis ingresado\nen caso de ser verdadero utilizara la posicion del mouse")]
        bool mouseOrMovement = true;

        [SerializeField]
        Axis movementDetect;

        public override void Update()
        {
            //UpdateAxis();
            if (mouseOrMovement)
            {
                dir = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2, 0);
                dir.Normalize();
            }
            else
                dir = movementDetect.dir;

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


