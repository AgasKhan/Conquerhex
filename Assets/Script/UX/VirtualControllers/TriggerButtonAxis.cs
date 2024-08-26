using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    [CreateAssetMenu(menuName = "Controllers/TriggerButtonAxis")]
    public class TriggerButtonAxis : TriggerDetection
    {
        [SerializeField]
        string button;

        /*
        [SerializeField, Tooltip("en caso de ser falso leera el axis ingresado\nen caso de ser verdadero utilizara la posicion del mouse")]
        bool mouseOrMovement = true;
        */

        [SerializeField]
        ButtonAxis movementDetect;

        public override void Update()
        {
            //UpdateAxis();
            if(movementDetect!=null)
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

        private void Axis_onSwitchGetDir(ButtonAxis axis)
        {
            movementDetect = axis;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            axis.onSwitchGetDir += Axis_onSwitchGetDir;
        }
    }

}


