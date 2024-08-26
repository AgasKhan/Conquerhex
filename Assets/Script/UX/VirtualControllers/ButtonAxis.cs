using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Controllers
{
    /// <summary>
    /// Evento de tecla que puede representar: <br/>
    /// Axis<br/>
    /// Botones<br/>
    /// Botones + axis<br/>
    /// Posee eventos para cuando comienza a presionarse/moverse (down) o deja de hacerlo (up), y si se mantiene (pressed)
    /// </summary>
    [CreateAssetMenu(menuName = "Controllers/ButtonAxis")]
    public class ButtonAxis : FatherKey, IEventController, IState<Vector2>
    {
        public event Action<Vector2, float> eventDown;
        public event Action<Vector2, float> eventPress;
        public event Action<Vector2, float> eventUp;

        public event Action<ButtonAxis> onSwitchGetDir;

        [field: SerializeField]
        public Vector2 dir { get; private set; }

        public override void Destroy()
        {
            eventDown = null;
            eventUp = null;
            eventPress = null;
        }

        public void SitchGetDir(ButtonAxis axis)
        {
            onSwitchGetDir?.Invoke(axis);
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

        public override void MyAwake()
        {
            enable = true;
        }
    }
}

