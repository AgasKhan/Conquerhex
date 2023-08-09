using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LavaHazard : MonoBehaviour
{
    [SerializeField]
    DrawFullscreenFeature postProcess;

    [SerializeField]
    FadeOnOff myfadeOnOff;

    Character character;

    [SerializeField]
    float lavaDmgSecs;

    Timer lavaDmg;

    void Awake()
    {
        //EventManager.events.SearchOrCreate<EventGeneric>(LifeType.life).action += PlayerPostProcess_LifeRegen;

        myfadeOnOff.alphas += FadeRegen_alphas_Regen;

        myfadeOnOff.Init();

        postProcess.SetActive(false);

        lavaDmg = TimersManager.Create(lavaDmgSecs, LavaDamage).Stop();
    }

    void LavaDamage()
    {
        if (character == null)
            return;
        
        Damage dmg = new Damage();

        dmg.typeInstance = (ClassDamage)Manager<ShowDetails>.pic["Perforation"];

        dmg.amount = 10;

        character.TakeDamage(dmg);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            character = collision.GetComponent<Character>();
            PlayerPostProcess_LifeRegen(character);

            lavaDmg.Reset();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player"))
        {
            myfadeOnOff.FadeOff();
            myfadeOnOff.end += FadeRegen_end;


            lavaDmg.Stop();
            character = null;
        }
    }

    private void FadeRegen_alphas_Regen(float obj)
    {
        postProcess.settings.blitMaterial.SetFloat("_Fade", obj);
    }

    private void PlayerPostProcess_LifeRegen(params object[] param)
    {
        if (!postProcess.isActive)
        {
            postProcess.SetActive(true);

            myfadeOnOff.FadeOn();
        }
        
        else if (postProcess.isActive && myfadeOnOff.fadeFinish)
        {
            myfadeOnOff.FadeOff();

            myfadeOnOff.end += FadeRegen_end;
        }
    }

    private void FadeRegen_end()
    {
        postProcess.SetActive(false);

        myfadeOnOff.end -= FadeRegen_end;
    }
}
