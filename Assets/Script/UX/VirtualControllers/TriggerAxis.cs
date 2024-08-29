using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    [CreateAssetMenu(menuName = "Controllers/TriggerAxis")]
    public class TriggerAxis : TriggerDetection
    {
        [SerializeField]
        string vertical;

        [SerializeField]
        string horizontal;

        [SerializeField]
        float velocity;

        [SerializeField]
        bool isSmooth;

        [SerializeField]
        float smoothVelocity;

        [Tooltip("en caso de ser verdadero leera el axis ingresado\nen caso de ser falso utilizara la posicion del mouse")]
        public bool mouseOverride = false;

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
            {
                if (mouseOverride)
                {
                    dir.Set(Input.mousePosition.x - Screen.width / 2, Input.mousePosition.y - Screen.height / 2);

                    dir.Normalize();
                }
                else
                {
                    dir.Set(h, v);
                }

                dir *= velocity;

                if (!isSmooth)
                    return;

                dir.x = Mathf.Lerp(0, dir.x, Time.deltaTime * smoothVelocity);

                dir.y = Mathf.Lerp(0, dir.y, Time.deltaTime * smoothVelocity);
            }

            //if (dir.sqrMagnitude > 1)
        }


        // Update is called once per frame
        public override void Update()
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


