using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsBuildController : BuildingsController, IUnityAdsListener
{
    [SerializeField] string adToShow = "Rewarded_Android";

    protected override void Config()
    {
        base.Config();

        MyAwakes += MyAwake;
    }
    void MyAwake()
    {
        Advertisement.AddListener(this);
        if (Application.platform == RuntimePlatform.Android)
            Advertisement.Initialize("5307781");
        else
            Advertisement.Initialize("5307781", true);
    }

    public void ShowAd()
    {
        if (!Advertisement.IsReady())
        {
            Debug.Log("No hay Ad");
            return;
        }

        Advertisement.Show(adToShow);
    }

    public override void EnterBuild()
    {
        ShowAd();
    }

    public void OnUnityAdsDidError(string message)
    {

    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (placementId != "Rewarded_Android")
            return;

        if (showResult == ShowResult.Finished)
        {
            Debug.Log("Te doy una recompensa");
            GameManager.instance.playerCharacter.AddOrSubstractItems("PortalFuel", 10);
        }

        else
        {
            Debug.Log("No te doy nada");
            MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(true).SetWindow("", "No conseguiste la recompensa por saltar el anuncio").AddButton("Cerrar", () => MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false));
        }

    }

    public void OnUnityAdsDidStart(string placementId)
    {

    }

    public void OnUnityAdsReady(string placementId)
    {

    }

}