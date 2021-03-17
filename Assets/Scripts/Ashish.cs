using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
using Firebase.RemoteConfig;

public class Ashish : MonoBehaviour
{
    public BoolEvent enableAdEvent;
    private void Awake()
    {
        FetchDataAsync();

    }
    public Task FetchDataAsync()
    {
        Debug.Log("Fetching data...");
        System.Threading.Tasks.Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.FetchAsync(
            TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }

    void FetchComplete(Task task)
    {
        //if (task.IsCanceled)
        //{
        //    Debug.Log("fetching Cancelled");
        //}
        //else if (task.IsFaulted)
        //{
        //    Debug.Log("fetching Error");
        //}
        //else if (task.IsCompleted)
        //{
        //    Debug.Log("fetching Success");
        //}
        //ConfigInfo info = FirebaseRemoteConfig.Info;
        //switch (info.LastFetchFailureReason)
        //{
        //    case FetchFailureReason.Error:
        //        break;
        //    case FetchFailureReason.Invalid:
        //        break;
        //    case FetchFailureReason.Throttled:
        //        break;
        //    default:
        //        break;

        //}
        //switch (info.LastFetchStatus)
        //{
        //    case LastFetchStatus.Success:
        //        Debug.Log("Fetching success");
        //        FirebaseRemoteConfig.ActivateFetched();
        //        Getdata();
        //        break;
        //    case LastFetchStatus.Failure:
        //        Debug.Log("Fetching Failed");
        //        break;
        //    case LastFetchStatus.Pending:
        //        Debug.Log("Fetching Pending");
        //        break;
        //    default:
        //        break;
        //}
    }
    void Getdata()
    {
        ConfigValue cv = FirebaseRemoteConfig.GetValue("RemoveAd");
        bool s = cv.StringValue == "true";
        enableAdEvent.Invoke(s);
        // Notification(s.ToString());
    }
    void Notification(string message)
    {
        NotificationView.notifInstance.CreateNotification("enabled: " + message);
    }
}
