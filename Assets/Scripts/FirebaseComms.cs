using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.RemoteConfig;
using Firebase.Analytics;
using Firebase.Extensions;
using System;

public class FirebaseComms : MonoBehaviour
{
    public static FirebaseComms firebaseInstance;
    public BoolEvent adDisableEvent;
    private void Start()
    {
        FetchDataAsync();
    }
    public Task FetchDataAsync()
    {
        Debug.Log("Fetching data....");
        Task fetchdata = Firebase.RemoteConfig.FirebaseRemoteConfig.FetchAsync(TimeSpan.Zero);
        return fetchdata.ContinueWithOnMainThread(FetchComplete);
    }

    private void FetchComplete(Task obj)
    {
        FirebaseRemoteConfig.ActivateFetched();
        ConfigInfo configInfo = FirebaseRemoteConfig.Info;

        switch (configInfo.LastFetchFailureReason)
        {
            case FetchFailureReason.Invalid:
                break;
            case FetchFailureReason.Throttled:
                break;
            case FetchFailureReason.Error:
                break;
            default:
                break;
        }
        switch (configInfo.LastFetchStatus)
        {
            case LastFetchStatus.Success:
                GetFetchedData();
                break;
            case LastFetchStatus.Failure:
                break;
            case LastFetchStatus.Pending:
                break;
            default:
                break;
        }


        throw new NotImplementedException();
    }

    void GetFetchedData()
    {
         UnityRemoteConfigs.promoval    = FirebaseRemoteConfig.GetValue("bonus_promo").StringValue;
        print("bonusPromo " + UnityRemoteConfigs.promoval);

        UnityRemoteConfigs.adskey = FirebaseRemoteConfig.GetValue("remove_Ads").StringValue;
        print("adsStatus " + UnityRemoteConfigs.adskey);
        bool en = UnityRemoteConfigs.adskey == "true";
        adDisableEvent.Invoke(en);
        Notifcation(en.ToString());

        UnityRemoteConfigs.lowGoldKey = FirebaseRemoteConfig.GetValue("getlowGold").StringValue;
        UnityRemoteConfigs.highGoldKey = FirebaseRemoteConfig.GetValue("gethighGold").StringValue;

        UnityIAP.instance.InitialiseAfterFetching(UnityRemoteConfigs.lowGoldKey, UnityRemoteConfigs.highGoldKey);
    }
    void Notifcation(string message)
    {
        NotificationView.notifInstance.CreateNotification("Ads disabled " + message);
    }
}
