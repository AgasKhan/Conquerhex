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

        TimedCompleteAction timDisponibility = null;

        Timer timer;

        Ability ability;

        bool noDisponibility;

        /// <summary>
        /// Numero comprendido entre 0 y 1
        /// </summary>
        /// <param name="number"></param>
        public void FillAmount(IGetPercentage getPercentage,float number)
        {
            imageFill.fillAmount = getPercentage.Percentage();                
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
            complexColor.Remove(inComplete);
            complexColor.Remove(complete);
        }
        
        private void ColorSetter_setter(Color obj)
        {
            imageKata.color = obj;
        }

        private void AbilityDisponibility((float percentage, float diference, float energy) obj)
        {
            if (!timDisponibility.Chck)
                return;

            if(ability.caster.EnergyComprobation(ability.CostExecution, out float energyBuff, out float percentage))
            {
                complexColor.Remove(inComplete);
                if(noDisponibility)
                    timDisponibility.Reset();

                noDisponibility = false;
            }
            else
            {
                complexColor.Add(inComplete);
                noDisponibility = true;
            }
        }


        public void SetCooldown(Ability param, ItemBase item)
        {
            if (ability != null)
            {
                ability.onExit -= AbilityOnExit;
                ability.onEnter -= AbilityOnEnter;
                timer.onChange -= FillAmount;
                ability.caster.energyUpdate -= AbilityDisponibility;
            }

            if (param != null)
            {
                ability = param;
                ability.onEnter += AbilityOnEnter;
                ability.onExit += AbilityOnExit;

                timer = param.cooldown;
                timer.onChange += FillAmount;

                ability.caster.energyUpdate += AbilityDisponibility;
            }

            imageKata.sprite = item?.image;


            gameObject.SetActive(item != null || param != null);
        }


        private void Awake()
        {
            complexColor.setter += ColorSetter_setter;

            complexColor.Add(imageKata.color);

            timDisponibility = TimersManager.Create(0.33f, () => ColorBlink(timDisponibility.current< timDisponibility.total/3 || timDisponibility.current> 2*timDisponibility.total/3), ColorBlinkEnd);
        }
    }
}


