using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    [CreateAssetMenu(menuName = "Controllers/Button")]
    /// <summary>
    /// manager de eventos destinados a teclas, boton pulsado(down), mientras lo presiono (pressed), y cuando lo suelto(up)
    /// </summary>
    public class Button : FatherKey, IState
    {

        public System.Action<float> eventDown;
        public System.Action<float> eventPress;
        public System.Action<float> eventUp;

        public override void Destroy()
        {
            eventDown = null;
            eventPress = null;
            eventUp = null;
        }

        #region general functions


        public void OnEnterState()
        {
            eventDown?.Invoke(0);
        }

        public void OnStayState()
        {
            timePressed += Time.deltaTime;
            eventPress?.Invoke(timePressed);
        }

        public void OnExitState()
        {
            eventUp?.Invoke(timePressed);
            timePressed = 0;
        }

        public void SuscribeController(IController controllerDir)
        {
            eventDown += controllerDir.ControllerDown;
            eventUp += controllerDir.ControllerUp;
            eventPress += controllerDir.ControllerPressed;
        }

        public void DesuscribeController(IController controllerDir)
        {
            eventDown -= controllerDir.ControllerDown;
            eventUp -= controllerDir.ControllerUp;
            eventPress -= controllerDir.ControllerPressed;
        }
        #endregion


        #region constructor
        private void OnEnable()
        {
            VirtualControllers.keys.Add(this);
        }

        public override void MyAwake()
        {
            enable = true;
        }
        #endregion
    }
}