using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class AdsBuilding : Building, IUnityAdsListener
{
    [SerializeField] string adToShow = "Rewarded_Android";
    public override string rewardNextLevel => throw new System.NotImplementedException();

    //---------------------------------
    public MenuManager refMenu;
    //---------------------------------

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

        //---------------------------------
        refMenu.eventListVoid.Add("Try4", Try4);
        //---------------------------------
    }
    //---------------------------------
    void Try4(GameObject g)
    {
        myBuildSubMenu.ClearSubMenu();
        myBuildSubMenu.Create();
    }
    //---------------------------------
    void Internal()
    {

    }

    /*
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

[System.Serializable]
public class AdsSubMenu : CreateSubMenu
{
    protected override void InternalCreate()
    {
        subMenu.CreateSection(0, 2);
        subMenu.CreateChildrenSection<ScrollRect>();

        subMenu.CreateSection(2, 6);
        subMenu.AddComponent<DetailsWindow>();
        subMenu.AddComponent<EventsCall>();
    }
}