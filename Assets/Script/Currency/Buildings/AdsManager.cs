using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : BuildingBase, IUnityAdsListener
{
    [SerializeField] string adToShow = "Rewarded_Android";

    protected override void InternalAction()
    {
        base.InternalAction();

        buttonsFuncs.AddRange(new Pictionarys<string, System.Action>()
        {
            {"Open", Internal}
        });
    }

    void Internal()
    {

    }

    /*
    private void Awake()
    {
        Advertisement.AddListener(this);
        if (Application.platform == RuntimePlatform.Android)
            Advertisement.Initialize("5307781");
        else
            Advertisement.Initialize("5307781", true);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            ShowAd();
        }
    }
    */

    public void ShowAd()
    {
        if(!Advertisement.IsReady())
        {
            //Pop Up
            Debug.Log("No hay Ad");
            return;
        }

        Advertisement.Show(adToShow);
    }


    public void OnUnityAdsDidError(string message)
    {

    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (placementId != "Rewarded_Android") 
            return;

        if (showResult == ShowResult.Finished)
            Debug.Log("Te doy una recompensa");
        else
            Debug.Log("No te doy nada");
    }

    public void OnUnityAdsDidStart(string placementId)
    {

    }

    public void OnUnityAdsReady(string placementId)
    {

    }
}
