using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class VirtualControllers : MonoBehaviour
{
    #region static classes
    static VirtualControllers _instance;

    static List<FatherKey> _keys = new List<FatherKey>();

    public static AxisButton movement = new AxisButton("Horizontal", "Vertical", "Sprint");

    static public AxisButton principal = new AxisButton("Mouse X", "Mouse Y", "Fire1");

    static public AxisButton secondary = new AxisButton("Mouse X", "Mouse Y", "Fire2");

    static public AxisButton terciary = new AxisButton("Mouse X", "Mouse Y", "Fire3");

    static public Button parry = new Button("Parry");

    static public T Search<T>(KeyInput e) where T :FatherKey
    {
        return _keys[(int)e] as T;
    }

    #endregion

    #region class

    /// <summary>
    /// clase base de la deteccion de teclas
    /// </summary>
    public abstract class FatherKey
    {
        public float timePressed;

        public abstract void MyUpdate();

        public abstract void Destroy();
    }

    /// <summary>
    /// clase destinada a generar los eventos de suscripcion
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MiddleKey<T> : FatherKey
    {
        public System.Action<T> eventDown;
        public System.Action<T> eventPress;
        public System.Action<T> eventUp;

        public override void Destroy()
        {
            eventDown = null;
            eventPress = null;
            eventUp = null;
        }
    }

    /// <summary>
    /// Clase destinada a detectar la tecla
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="B"></typeparam>
    public abstract class Key<T, B> : MiddleKey<float>
    {
        public bool enable;

        public System.Func<T, B> pressFunc;
        public System.Func<T, B> downFunc;
        public System.Func<T, B> upFunc;

        public T detects;

        public B pressed
        {
            get
            {
                return CheckKey(detects, pressFunc);
            }
        }

        public B down
        {
            get
            {
                return CheckKey(detects, downFunc);
            }
        }

        public B up
        {
            get
            {
                return CheckKey(detects, upFunc);
            }
        }

        protected B CheckKey(T p, System.Func<T, B> func)
        {
            if (enable && eneable)
            {
                return func(p);
            }
            else
                return default;
        }

        public void ChangeKey(T k)
        {
            detects = k;
        }
    }

    /// <summary>
    /// manager de eventos destinados a teclas, boton pulsado(down), mientras lo presiono (pressed), y cuando lo suelto(up)
    /// </summary>
    public class Button : Key<string, bool>, IState
    {
      
        #region general functions
        public override void MyUpdate()
        {
            if (down)
            {
                OnEnterState();
            }

            if (pressed)
            {
                OnStayState();
            }

            if (up)
            {
                OnExitState();
            }

        }

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

        public Button(string k)
        {
            enable = true;
            ChangeKey(k);

            pressFunc = Input.GetButton;
            downFunc = Input.GetButtonDown;
            upFunc = Input.GetButtonUp;

            _keys.Add(this);
        }

        #endregion
    }

    /// <summary>
    /// clase intermedia entre el buttonaxis y el key, encargada de generar el chequeo del axis
    /// </summary>
    public class MiddleAxis : Key<string, float>
    {
        public override void MyUpdate()
        {
        }

        public MiddleAxis(string k)
        {
            enable = true;
            ChangeKey(k);

            pressFunc = Input.GetAxis;
        }
    }

    /// <summary>
    /// combinacion de boton + axis, eventos para si se pulsa(down) o suelta un boton(up), y cual es el valor de un joystick (pressed)
    /// </summary>
    public class AxisButton : FatherKey, IEventController, IState<Vector2>
    {
        public event Action<Vector2, float> eventDown;
        public event Action<Vector2, float> eventPress;
        public event Action<Vector2, float> eventUp;

        MiddleAxis horizontal;
        MiddleAxis vertical;
        Button button;

        Vector2 dir;

        float multiply= 0.5f;

        public bool enable
        {
            get
            {
                return horizontal.enable;
            }

            set
            {
                horizontal.enable = value;
                vertical.enable = value;
                button.enable = value;
            }

        }

        Vector2 vecPressed
        {
            get
            {
                dir = new Vector2(horizontal.pressed, vertical.pressed);

                if (dir.sqrMagnitude > 1)
                    dir.Normalize();

                return dir;
            }
        }

        public override void MyUpdate()
        {
            if (button.down)
            {
                multiply = 1;
                OnEnterState(dir * multiply);
            }

            if (vecPressed.sqrMagnitude > 0)
            {
                OnStayState(dir*multiply);
            }

            if (button.up)
            {
                OnExitState(dir * multiply);
                dir = Vector2.zero;
                multiply = 0.5f;
            }
        }

        public override void Destroy()
        {
            eventDown = null;
            eventUp = null;
            eventPress = null;
        }

        public void OnEnterState(Vector2 param)
        {
            eventDown?.Invoke(param, 0);
        }

        public void OnStayState(Vector2 param)
        {
            timePressed += Time.deltaTime;
            eventPress?.Invoke(param, timePressed);
        }

        public void OnExitState(Vector2 param)
        {
            eventUp?.Invoke(param, timePressed);
            timePressed = 0;
        }

        public void SuscribeController(IControllerDir controllerDir)
        {
            eventDown += controllerDir.ControllerDown;
            eventUp += controllerDir.ControllerUp;
            eventPress += controllerDir.ControllerPressed;
        }

        public void DesuscribeController(IControllerDir controllerDir)
        {
            eventDown -= controllerDir.ControllerDown;
            eventUp -= controllerDir.ControllerUp;
            eventPress -= controllerDir.ControllerPressed;
        }

        public AxisButton(string strHorizontal, string strVertical, string strButton)
        {
            horizontal = new MiddleAxis(strHorizontal);
            vertical = new MiddleAxis(strVertical);
            button = new Button(strButton);

            horizontal.enable = true;
            vertical.enable = true;
            button.enable = true;

            horizontal.ChangeKey(strHorizontal);
            vertical.ChangeKey(strVertical);
            button.ChangeKey(strButton);

            vertical.pressFunc = Input.GetAxis;
            horizontal.pressFunc = Input.GetAxis;

            button.downFunc = Input.GetButtonDown;
            button.upFunc = Input.GetButtonUp;

            _keys.Add(this);
        }

    }

    #endregion

    static public bool eneable
    {
        set
        {

            _instance.enabled = value;
        }
        get
        {
            return _instance.enabled;
        }
    }

    static public bool eneableMove;


    #region unity functions

    private void Awake()
    {
        _instance = this;
        eneableMove = true;
    }

    private void OnDestroy()
    {
        foreach (var item in _keys)
        {
            item.Destroy();
        }
    }

    void Update()
    {
        foreach (var item in _keys)
        {
            item.MyUpdate();
        }
    }
    #endregion
}

public enum KeyInput
{
    movement,
    principal,
    secondary,
    terciary,
    parry
}
