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

        public Vector2 LastDir { get=> _dir; }

        public Vector2 FrameDir
        {
            get => _frameDir;
            private set
            {
                _frameDir = value;
                if(value != Vector2.zero)
                {
                    _dir = _frameDir;
                }
            }
        }

        Vector2 _frameDir;

        Vector2 _dir;


        public override void Destroy()
        {
            eventDown = null;
            eventUp = null;
            eventPress = null;
        }

        public void SwitchGetDir(ButtonAxis axis)
        {
            onSwitchGetDir?.Invoke(axis);
        }

        public void OnEnterState(Vector2 param)
        {
            if (!enable || !VirtualControllers.eneable)
                return;

            timePressed = 0;

            FrameDir = param;

            eventDown?.Invoke(param, timePressed);
        }

        public void OnStayState(Vector2 param)
        {
            if (!enable || !VirtualControllers.eneable)
                return;

            timePressed += Time.deltaTime;

            FrameDir = param;

            eventPress?.Invoke(param, timePressed);
        }

        public void OnExitState(Vector2 param)
        {
            eventUp?.Invoke(param, timePressed);

            timePressed = 0;

            FrameDir = Vector2.zero;
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

