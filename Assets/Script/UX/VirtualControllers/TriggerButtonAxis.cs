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
        
        [SerializeField, Tooltip("En caso de ser verdadero, leera el ultimo input registrado, en caso de ser falso leera el input del frame")]
        bool LastOrFrameDirect = true;        

        [SerializeField]
        ButtonAxis movementDetect;

        IDir moveDetect;

        protected override void InternalUpdate()
        {
            //UpdateAxis();
            if(moveDetect != null)
            {
                if(LastOrFrameDirect)
                    dir = moveDetect.LastDir;
                else
                    dir = moveDetect.FrameDir;
            }
                

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

        private void Axis_onSwitchGetDir(IDir axis)
        {
            moveDetect = axis;
        }

        protected override void OnEnable()
        {
            moveDetect = movementDetect;
            base.OnEnable();
            axis.onSwitchGetDir += Axis_onSwitchGetDir;
        }
    }

}


