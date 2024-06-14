using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CooldownUI : MonoBehaviour
    {
        [SerializeField]
        Image imageKata;

        [SerializeField]
        Image imageFill;

        [SerializeField]
        Color inUse;

        [SerializeField]
        Color complete;

        [SerializeField]
        Color inComplete;

        ComplexColor complexColor = new ComplexColor();

        TimedCompleteAction timCompleted = null;

        Timer timer;

        Ability ability;

        /// <summary>
        /// Numero comprendido entre 0 y 1
        /// </summary>
        /// <param name="number"></param>
        public void FillAmount(IGetPercentage getPercentage,float number)
        {
            imageFill.fillAmount = getPercentage.Percentage();
            complexColor.Add(inComplete);
            if (imageFill.fillAmount == 0)
                timCompleted.Reset();
        }

        public void SetCooldown(Ability param, ItemBase item)
        {
            if(ability!=null)
            {
                ability.onExit -= AbilityOnExit;
                ability.onEnter -= AbilityOnEnter;
                timer.onChange -= FillAmount;
            }
            
            if(param!=null)
            {
                ability = param;
                ability.onEnter += AbilityOnEnter;
                ability.onExit += AbilityOnExit;

                timer = param.cooldown;
                timer.onChange += FillAmount;
            }

            imageKata.sprite = item?.image;


            gameObject.SetActive(item != null || param != null);
        }

        private void AbilityOnExit()
        {
            complexColor.Remove(inUse);
        }

        private void AbilityOnEnter()
        {
            complexColor.Add(inUse);
        }

        private void ChangeColor(Color save)
        {
            complexColor.multiply = save;
        }

        private void ColorBlink(bool active)
        {
            if (active)
            {
                //parpadeo rapido
                complexColor.Remove(inComplete);
                complexColor.Add(complete);
            }
            else
            {
                //el mantenido
                complexColor.Remove(complete);
                complexColor.Add(inComplete);
            }
        }

        private void ColorBlinkEnd()
        {
            //volver al original
            complexColor.Remove(complete);
            complexColor.Remove(complete);
        }
        
        private void ColorSetter_setter(Color obj)
        {
            imageKata.color = obj;
        }

        private void Awake()
        {
            complexColor.setter += ColorSetter_setter;

            complexColor.Add(imageKata.color);

            timCompleted = TimersManager.Create(0.33f, () => ColorBlink(timCompleted.current< timCompleted.total/3 || timCompleted.current> 2*timCompleted.total/3), ColorBlinkEnd);
        }
    }
}


