using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UI
{
    public class HealthUI : MonoBehaviour
    {
        public ImageWidth vida;
        public TextMeshProUGUI textVida;
        public ImageWidth regen;
        public ImageWidth regenMax;

        public TextMeshProUGUI textRegen;
        public ImageWidth regenTimeMax;
        public ImageWidth regenTime;

        public void healthBarUpdate(Health health)
        {
            float nextRegenLifePercentage = health.nextRegenLife / health.maxLife;

            float healthPercentage = health.actualLife / health.maxLife;

            float cooldownRegen = (1 - health.actualCoolDownRegen / health.MaxCoolDownRegen);

            textVida.text = ((int)health.actualLife).ToString();

            regen.FillAmount = health.actualRegen / 100f;

            regenMax.FillAmount = health.maxRegen / 100f;

            textRegen.text = ((int)(health.actualRegen / 100 * health.maxLife)).ToString();

            regenTime.FillAmount = cooldownRegen * nextRegenLifePercentage;

            if (cooldownRegen == 0 || cooldownRegen > 0.05f)
            {
                var aux = (Mathf.Clamp((cooldownRegen - (0.05f)) * 10, 0, 1));

                regenTimeMax.FillAmount = Mathf.Clamp((health.nextRegenLife * aux) / health.maxLife, healthPercentage, 1);
            }

            if (vida.FillAmount > healthPercentage)
            {
                if (cooldownRegen < 0.05f)
                {
                    vida.FillAmount = Mathf.Lerp(vida.FillAmount, healthPercentage, Time.deltaTime);
                }
                else
                {
                    vida.FillAmount = Mathf.Lerp(vida.FillAmount, healthPercentage, Time.deltaTime * 10);
                }
            }
            else
            {
                vida.FillAmount = healthPercentage;
            }
        }
    }

}

