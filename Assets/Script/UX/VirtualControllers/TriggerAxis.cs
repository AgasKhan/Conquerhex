using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    [CreateAssetMenu(menuName = "Controllers/TriggerAxis")]
    public class TriggerAxis : ScriptableObject
    {
        [SerializeField]
        protected Axis axis;

        [SerializeField]
        string vertical;

        [SerializeField]
        string horizontal;

        protected bool press;

        protected Vector2 dir;

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
            float h = Input.GetAxis(horizontal);
            float v = Input.GetAxis(vertical);

            if (b || (h != 0 && v != 0))
                dir.Set(h, v);

            if (dir.sqrMagnitude > 1)
                dir.Normalize();
        }


        // Update is called once per frame
        public virtual void MyUpdate()
        {
            if (vecPressed.sqrMagnitude > 0 && !press)
            {
                axis.OnEnterState(dir);
                press = true;
            }

            else if (press)
            {
                if (dir.sqrMagnitude == 0)
                {
                    axis.OnExitState(dir);
                    press = false;
                }
                else
                    axis.OnStayState(dir);
            }
        }
    }
}


