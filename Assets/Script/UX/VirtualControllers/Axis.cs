using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Controllers
{
    [CreateAssetMenu(menuName = "Controllers/Axis")]
    /// <summary>
    /// combinacion de middleaxis, eventos para si se mueve un joystick (down) o deja de hacerlo(up), y si se mantiene en movimiento (pressed)
    /// </summary>
    public class Axis : FatherKey, IEventController, IState<Vector2>
    {
        public event Action<Vector2, float> eventDown;
        public event Action<Vector2, float> eventPress;
        public event Action<Vector2, float> eventUp;

        public Vector2 dir { get; private set; }

        public override void Destroy()
        {
            eventDown = null;
            eventUp = null;
            eventPress = null;
        }

        public void OnEnterState(Vector2 param)
        {
            if (!enable || !VirtualControllers.eneable)
                return;
            timePressed = 0;
            //dir = param;
            eventDown?.Invoke(param, timePressed);
        }

        public void OnStayState(Vector2 param)
        {
            if (!enable || !VirtualControllers.eneable)
                return;
            timePressed += Time.deltaTime;
            dir = param;
            eventPress?.Invoke(param, timePressed);
        }

        public void OnExitState(Vector2 param)
        {
            eventUp?.Invoke(param, timePressed);
            timePressed = 0;
            //dir = Vector2.zero;
        }

        public void SuscribeController(IControllerDir controllerDir)
        {
            if (controllerDir == null)
                return;
            eventDown += controllerDir.ControllerDown;
            eventUp += controllerDir.ControllerUp;
            eventPress += controllerDir.ControllerPressed;
        }

        public void DesuscribeController(IControllerDir controllerDir)
        {
            if (controllerDir == null)
                return;

            eventDown -= controllerDir.ControllerDown;
            eventUp -= controllerDir.ControllerUp;
            eventPress -= controllerDir.ControllerPressed;
        }

        private void OnEnable()
        {
            VirtualControllers.keys.Add(this);
        }

        public override void MyAwake()
        {
            enable = true;
        }
    }
}

