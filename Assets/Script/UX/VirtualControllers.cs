using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class VirtualControllers : MonoBehaviour
{
    #region static classes
    static VirtualControllers _instance;

    static List<FatherKey> _keys = new List<FatherKey>();

    /*
    static public Button principal = new Button("Fire1");

    static public Button secondary = new Button("Fire2");

    static public Button terciary = new Button("Fire3");

    static public Button parry = new Button("Parry");
    */
    static public IControlador movement;

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
    public abstract class MiddleKey<T> : FatherKey, IState<T>
    {
        public event System.Action<T> eventDown;
        public event System.Action<T> eventPress;
        public event System.Action<T> eventUp;

        public void OnEnterState(T param)
        {
            eventDown?.Invoke(param);
        }

        public void OnStayState(T param)
        {
            eventPress?.Invoke(param);
        }

        public void OnExitState(T param)
        {
            eventUp?.Invoke(param);
        }
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
    /// clase destinada a instanciar la deteccion de un boton
    /// </summary>
    public class Button : Key<string, bool>
    {
      
        #region general functions
        public override void MyUpdate()
        {
            if (down)
            {
                OnEnterState(0);
            }

            if (pressed)
            {
                timePressed += Time.deltaTime;
                OnStayState(timePressed);
            }

            if (up)
            {
                OnExitState(timePressed);
                timePressed = 0;
            }

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
    /// clase intermedia entre el buttonaxis y el key, encargada de generar el chequeo
    /// </summary>
    public class MiddleAxis : Key<string, float>
    {
        public override void MyUpdate()
        {
            if (pressed!=0)
            {
                timePressed += Time.deltaTime;
                OnStayState(timePressed);
            }
        }

        public MiddleAxis(string k)
        {
            enable = true;
            ChangeKey(k);

            pressFunc = Input.GetAxis;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AxisButton : FatherKey, IControlador
    {
        MiddleAxis horizontal;
        MiddleAxis vertical;
        Button button;

        Vector2 dir;

        public event Action<Vector2, float> down;
        public event Action<Vector2, float> pressed;
        public event Action<Vector2, float> up;

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
                down?.Invoke(Vector2.zero, 0);
            }

            if (vecPressed.sqrMagnitude > 0)
            {
                timePressed += Time.deltaTime;
                pressed?.Invoke(dir, timePressed);
            }

            if (button.up)
            {
                up?.Invoke(dir, timePressed);
                timePressed = 0;
                dir = Vector2.zero;
            }
        }

        public override void Destroy()
        {
            down = null;
            up = null;
            pressed = null;
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

    public class ControllerDivide : IControlador
    {
        IControlador controlador1;
        IControlador contralador2;

        event Action<Vector2, float> IControlador.down
        {
            add
            {
                controlador1.down += value;
                contralador2.down += value;
            }

            remove
            {
                controlador1.down -= value;
                contralador2.down -= value;
            }
        }

        event Action<Vector2, float> IControlador.pressed
        {
            add
            {
                controlador1.pressed += value;
                contralador2.pressed += value;
            }

            remove
            {
                controlador1.pressed -= value;
                contralador2.pressed -= value;
            }
        }

        event Action<Vector2, float> IControlador.up
        {
            add
            {
                controlador1.up += value;
                contralador2.up += value;
            }

            remove
            {
                controlador1.up-= value;
                contralador2.up -= value;
            }
        }

        public ControllerDivide(IControlador controlador1, IControlador contralador2)
        {
            this.controlador1 = controlador1;
            this.contralador2 = contralador2;
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

    public JoyController joyController;

    Button _principal = new Button("Fire1");

    Button _secondary = new Button("Fire2");

    Button _terciary = new Button("Fire3");

    Button _parry = new Button("Parry");

    AxisButton _movement = new AxisButton("Horizontal", "Vertical", "Sprint");


    #region unity functions

    private void Awake()
    {
        _instance = this;
        eneableMove = true;
        movement = new ControllerDivide(_movement, joyController);
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
