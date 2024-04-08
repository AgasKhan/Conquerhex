using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum EnumController
{
    movement,
    principal,
    secondary,
    terciary,
    interact
}

public class VirtualControllers : MonoBehaviour
{
    #region static classes
    static VirtualControllers _instance;

    static List<FatherKey> _keys = new List<FatherKey>();

    static public Axis movement = new Axis("Horizontal", "Vertical");

    static public AxisButton principal = new AxisButton("Mouse X", "Mouse Y", "Fire1");

    static public AxisButton secondary = new AxisButton("Mouse X", "Mouse Y", "Fire2");

    static public AxisButton terciary = new AxisButton("Horizontal", "Vertical", "Sprint");

    static public AxisButton interact = new AxisButton("Mouse X", "Mouse Y", "Interact");

    static public Axis Search(EnumController e)
    {
        return _keys[(int)e] as Axis;
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
    /// combinacion de middleaxis, eventos para si se mueve un joystick (down) o deja de hacerlo(up), y si se mantiene en movimiento (pressed)
    /// </summary>
    public class Axis : FatherKey, IEventController, IState<Vector2>
    {
        public event Action<Vector2, float> eventDown;
        public event Action<Vector2, float> eventPress;
        public event Action<Vector2, float> eventUp;

        MiddleAxis horizontal;
        MiddleAxis vertical;

        protected bool press = false;

        protected Vector2 dir;
        public virtual bool enable
        {
            get
            {
                return horizontal.enable;
            }

            set
            {
                horizontal.enable = value;
                vertical.enable = value;
            }
        }

        protected Vector2 vecPressed
        {
            get
            {
                UpdateAxis(true);

                return dir;
            }
        }

        protected void UpdateAxis(bool b = false)
        {
            float h = horizontal.pressed;
            float v = vertical.pressed;

            if (b || (h!=0&&v!=0))
                dir.Set(h, v);

            if (dir.sqrMagnitude > 1)
                dir.Normalize();
        }


        public override void MyUpdate()
        {
            if (vecPressed.sqrMagnitude > 0 && !press)
            {
                OnEnterState(dir);
                press = true;
            }

            else if(press)
            {
                if (dir.sqrMagnitude == 0)
                {
                    OnExitState(dir);
                    press = false;
                }
                else
                    OnStayState(dir);
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
            timePressed = 0;
            eventDown?.Invoke(param, timePressed);
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
            dir = Vector2.zero;
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

        public Axis(string strHorizontal, string strVertical)
        {
            horizontal = new MiddleAxis(strHorizontal);
            vertical = new MiddleAxis(strVertical);

            horizontal.enable = true;
            vertical.enable = true;

            _keys.Add(this);
        }
    }

    /// <summary>
    /// combinacion de boton + axis, eventos para si se pulsa(down) o suelta un boton(up), y cual es el valor de un joystick (pressed)
    /// </summary>
    public class AxisButton : Axis
    {

        Button button;

        public override void MyUpdate()
        {
            UpdateAxis();

            if (button.down)
            {
                OnEnterState(dir);
                press = true;
            }

            if (button.up)
            {
                OnExitState(dir);
                press = false;
            }

            if (press)
            {
                OnStayState(dir);
            }
        }
        
        public AxisButton(string strHorizontal, string strVertical, string strButton) : base(strHorizontal, strVertical)
        {
            button = new Button(strButton);

            button.enable = true;
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


