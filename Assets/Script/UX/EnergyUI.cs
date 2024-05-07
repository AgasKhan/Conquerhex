using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class EnergyUI : MonoBehaviour
    {
        public Slider energy;
        public Image requirementLeft;
        public Image requirementRight;
        public Transform textPopEnergy;
        public TextMeshProUGUI actualEnergy;

        Timer leftEnergy;
        Timer rightEnergy;

        public void EnergyBarUpdate((float energyValue, float diference, float energyActual) str)
        {
            if (Mathf.Abs(str.diference) > 0.5f)
            {
                PoolManager.SpawnPoolObject(Vector2Int.up * 2, out TextPop textEnergy);

                str.diference *= -1;

                string diference = str.diference.ToStringFixed();

                if (str.diference < 0)
                {
                    diference = diference.RichTextColor(Color.red);
                }
                else
                {
                    diference = ("+" + diference).RichTextColor(Color.blue);
                }

                diference = diference.RichText("size", actualEnergy.fontSize.ToStringFixedComma(1));

                textEnergy.SetText(textPopEnergy, diference, Vector2.up * 0.5f * Mathf.Sign(-str.diference), false);
            }

            energy.value = str.energyValue;

            actualEnergy.text = str.energyActual.ToStringFixedComma(1);
        }

        public void EnergyLeft(float energyValue)
        {
            requirementLeft.fillAmount = energyValue;
            leftEnergy.Reset();
        }

        public void EnergyRight(float energyValue)
        {
            requirementRight.fillAmount = energyValue;
            rightEnergy.Reset();
        }

        private void Awake()
        {
            leftEnergy = TimersManager.Create(0.3f, () => requirementLeft.enabled = leftEnergy.current < leftEnergy.total / 3 || leftEnergy.current > 2 * (leftEnergy.total / 3), () => requirementLeft.enabled = false).Stop();
            rightEnergy = TimersManager.Create(0.3f, () => requirementRight.enabled = rightEnergy.current < rightEnergy.total / 3 || rightEnergy.current > 2 * (rightEnergy.total / 3), () => requirementRight.enabled = false).Stop();
        }

    }

}

