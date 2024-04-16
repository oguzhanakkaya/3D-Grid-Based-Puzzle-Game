using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using Unity.Services.RemoteConfig;
using Newtonsoft.Json;
using System.Text.Json;

[Serializable]
public class VersionCheckJSON
{
    public string currentVersion;
    public string shopLinkDRD;
    public string shopLinkIOS;

    public VersionCheckJSON(string currentVersion, string shopLinkDRD, string shopLinkIOS)
    {
        this.currentVersion = currentVersion;
        this.shopLinkDRD = shopLinkDRD;
        this.shopLinkIOS = shopLinkIOS;
    }
}
public class RemoteManager : MonoBehaviour
{

    public static RemoteManager instance;

    public int NoAdsPopUpFreq = 3;
    public int NoAdsPopUpEndLevel = 30;
    public int FreezeTimePrice = 20;
    public int PassangerBoosterPrice = 20;
    public int levelFailedAddExtraTime=15;
    public int FreezeTimeBoosterValue=5;
    public LevelJSON levelData;
    public ShopJSON shopData;


    public struct userAttributes { }
    public struct appAttributes { }

    private void Awake()
    {
        instance = this;

        Start();
 }

    public async Task Start()
    {
        if (Utilities.CheckForInternetConnection())
        {
            await InitializeRemoteConfigAsync();
        }

        RemoteConfigService.Instance.FetchCompleted += ApplyRemoteConfig;
        await RemoteConfigService.Instance.FetchConfigsAsync(new userAttributes(), new appAttributes());


    }
    public async Task CheckAfterConnection()
    {
        if (Utilities.CheckForInternetConnection())
        {
            await InitializeRemoteConfigAsync();
        }

        await RemoteConfigService.Instance.FetchConfigsAsync(new userAttributes(), new appAttributes());


    }
    async Task InitializeRemoteConfigAsync()
    {

        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    void ApplyRemoteConfig(ConfigResponse configResponse)
    {

        switch (configResponse.requestOrigin)
        {
            case ConfigOrigin.Default:
                Debug.LogError("No settings loaded this session and no local cache file exists; using default values.");
                break;
            case ConfigOrigin.Cached:
                Debug.LogError("No settings loaded this session; using cached values from a previous session.");
                break;
            case ConfigOrigin.Remote:
                Debug.LogError("New settings loaded this session; update values accordingly.");
                Debug.LogError("RemoteConfigService.Instance.appConfig fetched: " + RemoteConfigService.Instance.appConfig.config.ToString());



                NoAdsPopUpFreq = RemoteConfigService.Instance.appConfig.GetInt("no_ads_pop_up_freq");
                NoAdsPopUpEndLevel = RemoteConfigService.Instance.appConfig.GetInt("no_ads_pop_up_end_level");
                FreezeTimePrice = RemoteConfigService.Instance.appConfig.GetInt("freeze_time_price");
                PassangerBoosterPrice = RemoteConfigService.Instance.appConfig.GetInt("passanger_booster_price");
                levelFailedAddExtraTime = RemoteConfigService.Instance.appConfig.GetInt("level_failed_add_extra_time");
                FreezeTimeBoosterValue = RemoteConfigService.Instance.appConfig.GetInt("freeze_time_booster_value");

                shopData = JsonUtility.FromJson<ShopJSON>(RemoteConfigService.Instance.appConfig.GetJson("shop_data"));
                break;
        }

    }


}
