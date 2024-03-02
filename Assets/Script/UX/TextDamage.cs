using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UI
{
    public class TextDamage : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI textMesh;

        public static float colorTime = 1f;

        public static float velocity = 2f;

        public static float distance = 200;

        Vector3 originalPos;

        Timer movement;

        //Transform follow;

        Vector3 direction;

        Vector3 offset;

        Color original;//no lo estoy usando

        Camera main;

        public void SetText(Transform follow ,string text)
        {
            originalPos = follow.position;

            offset = Vector3.zero;

            textMesh.color = original;

            textMesh.text = text;

            gameObject.SetActive(true);

            direction = Random.insideUnitCircle.normalized * distance;

            movement.Reset();
        }        

        void Movement()
        {
            offset +=  (Mathf.Exp( -5 * movement.InversePercentage())) * Time.deltaTime * direction;
        }

        private void LateUpdate()
        {
            transform.position = offset + main.WorldToScreenPoint(originalPos);
            /*        
                if(movement.InversePercentage() <= 0.01f)
                    textMesh.color = Color.Lerp(Color.white, Color.white.ChangeAlphaCopy(0), movement.InversePercentage() * 100);
            */
        }

        private void Awake()
        {
            main = Camera.main;
            movement = TimersManager.Create(velocity, Movement, ()=> gameObject.SetActive(false)).Stop();
            original = textMesh.color;
            gameObject.SetActive(false);        
        }
    }
}