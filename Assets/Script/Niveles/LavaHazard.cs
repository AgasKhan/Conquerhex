using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LavaHazard : MonoBehaviour
{
    [SerializeField]
    DrawFullscreenFeature regen;

    [SerializeField]
    FadeOnOff fadeLifeRegen;

    void Awake()
    {
        EventManager.events.SearchOrCreate<EventGeneric>(LifeType.life).action += PlayerPostProcess_LifeRegen;

        fadeLifeRegen.alphas += FadeRegen_alphas_Regen;

        fadeLifeRegen.Init();

        regen.SetActive(false);
    }

    private void FadeRegen_alphas_Regen(float obj)
    {
        regen.settings.blitMaterial.SetFloat("_fade", obj);
    }

    private void PlayerPostProcess_LifeRegen(params object[] param)
    {
        var percentage = param[0] as IGetPercentage;
        var addLife = (float)param[1];

        if (addLife > 0 && percentage.Percentage() != 1 && !regen.isActive)
        {
            regen.SetActive(true);

            fadeLifeRegen.FadeOn();
        }

        else if ((addLife <= 0 || percentage.Percentage() == 1) && regen.isActive && fadeLifeRegen.fadeFinish)
        {
            fadeLifeRegen.FadeOff();

            fadeLifeRegen.end += FadeRegen_end;
        }
    }

    private void FadeRegen_end()
    {
        regen.SetActive(false);

        fadeLifeRegen.end -= FadeRegen_end;
    }
}
