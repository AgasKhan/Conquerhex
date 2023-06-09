using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsListener
{
    [SerializeField] string adToShow = "Rewarded_Android";

    private void Awake()
    {
        Advertisement.AddListener(this);
        Advertisement.Initialize("5307781", true);
    }

    public void ShowAd()
    {
        if(Advertisement.IsReady())
        {
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
        if (placementId != "Rewarded_Android") return;

        if (ShowResult.Finished == showResult)
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
