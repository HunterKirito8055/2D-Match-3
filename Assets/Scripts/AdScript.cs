using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdScript : MonoBehaviour
{

    string appKey = "ebda7c31";

    [SerializeField] IntEvent extraTimeRewardEvent;
    private void OnEnable()
    {
        IronSourceEvents.onInterstitialAdReadyEvent += ADisReady;



        IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
        IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
        IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent;
        IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
        IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
        IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;

        //banner
        IronSourceEvents.onBannerAdLoadFailedEvent += BannerAdLoadFailed;
        IronSourceEvents.onBannerAdLoadedEvent += BannerAdLoad;


        //rewards
        //IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
        //IronSourceEvents.onRewardedVideoAdClickedEvent += RewardedVideoAdClickedEvent;
        //IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
        //IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
        //IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
        //IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
        IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
        //IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;

    }

    void Start()
    {
        IronSource.Agent.validateIntegration();
        IronSource.Agent.init(appKey);
        InitializeAds();
    }
    private void InitializeAds()
    {
        IronSource.Agent.loadInterstitial();
        IronSource.Agent.loadBanner(IronSourceBannerSize.SMART, IronSourceBannerPosition.BOTTOM);
       
    }

    #region Interstitial Ads
    public void ShowAdOnGameOver()
    {
        if (IronSource.Agent.isInterstitialReady())
        {
            IronSource.Agent.showInterstitial();
        }
        else
        {
            print("interstitial ad is not ready");
            IronSource.Agent.loadInterstitial();
        }
    }

    void InterstitialAdLoadFailedEvent(IronSourceError error)
    {
        print("AD failed : " + error.ToString());
    }
    //Invoked right before the Interstitial screen is about to open.
    void InterstitialAdShowSucceededEvent()
    {
        IronSource.Agent.loadInterstitial();
    }
    //Invoked when the ad fails to show.
    //@param description - string - contains information about the failure.
    void InterstitialAdShowFailedEvent(IronSourceError error)
    {
        Debug.Log("ad failed " + error.ToString());
    }
    // Invoked when end user clicked on the interstitial ad
    void InterstitialAdClickedEvent()
    {
    }
    //Invoked when the interstitial ad closed and the user goes back to the application screen.
    void InterstitialAdClosedEvent()
    {
    }
    //Invoked when the Interstitial is Ready to shown after load function is called
    void InterstitialAdReadyEvent()
    {
    }
    //Invoked when the Interstitial Ad Unit has opened
    void InterstitialAdOpenedEvent()
    {
    }
    #endregion
    #region Banner Ads
    void BannerAdLoadFailed(IronSourceError obj)
    {
        IronSource.Agent.loadBanner(IronSourceBannerSize.SMART, IronSourceBannerPosition.BOTTOM);
    }
    void BannerAdLoad()
    {
        ShowBannerAd();
    }
    public void ShowBannerAd()
    {
        if (disableAdEvents == true)
        {
            return;
        }
        IronSource.Agent.displayBanner();
    }

    public void HideBannerAd()
    {
        IronSource.Agent.hideBanner();
    }

    #endregion
    void ADisReady()
    {
        print("AD ready");
    }

    #region Reward Ads

    //Invoked when the RewardedVideo ad view has opened.
    //Your Activity will lose focus. Please avoid performing heavy 
    //tasks till the video ad will be closed.
    void RewardedVideoAdOpenedEvent()
    {
    }
    void RewardedVideoAdClickedEvent(IronSourcePlacement ironSourcePlacement)
    {

    }
    //Invoked when the RewardedVideo ad view is about to be closed.
    //Your activity will now regain its focus.
    void RewardedVideoAdClosedEvent()
    {

    }
    //Invoked when there is a change in the ad availability status.
    //@param - available - value will change to true when rewarded videos are available. 
    //You can then show the video by calling showRewardedVideo().
    //Value will change to false when no videos are available.
    void RewardedVideoAvailabilityChangedEvent(bool available)
    {
        //Change the in-app 'Traffic Driver' state according to availability.
        rewardedVideoAvailability = available;
        print("availability event " + rewardedVideoAvailability);
    }

    //Invoked when the user completed the video and should be rewarded. 
    //If using server-to-server callbacks you may ignore this events and wait for 
    // the callback from the  ironSource server.
    //@param - placement - placement object which contains the reward data
    void RewardedVideoAdRewardedEvent(IronSourcePlacement placement)
    {
        extraTimeRewardEvent.Invoke(15);
    }
    //Invoked when the Rewarded Video failed to show
    //@param description - string - contains information about the failure.
    void RewardedVideoAdShowFailedEvent(IronSourceError error)
    {
    }


    //Invoked when the video ad starts playing. 
    void RewardedVideoAdStartedEvent()
    {
    }
    //Invoked when the video ad finishes playing. 
    void RewardedVideoAdEndedEvent()
    {
    }
    bool rewardedVideoAvailability;
    bool disableAdEvents;
    public void ShowRewardedVideo()
    {
        if (disableAdEvents == true)
        {
            return;
        }
        if (rewardedVideoAvailability)
        {
            print("reward ad running...");
            extraTimeRewardEvent.Invoke(15);

            IronSource.Agent.showRewardedVideo();

        }
    }
    public void DisableAds(bool val) // given this func to event in adscript inspector
    {
        disableAdEvents = val;
        rewardedVideoAvailability = !val;

    }
    #endregion

    private void OnDisable()
    {
        IronSourceEvents.onInterstitialAdReadyEvent -= ADisReady;

        //banner
        IronSourceEvents.onBannerAdLoadFailedEvent -= BannerAdLoadFailed;
        IronSourceEvents.onBannerAdLoadedEvent -= BannerAdLoad;

        IronSourceEvents.onInterstitialAdLoadFailedEvent -= InterstitialAdLoadFailedEvent;
        IronSourceEvents.onInterstitialAdShowSucceededEvent -= InterstitialAdShowSucceededEvent;
        IronSourceEvents.onInterstitialAdShowFailedEvent -= InterstitialAdShowFailedEvent;
        IronSourceEvents.onInterstitialAdClickedEvent -= InterstitialAdClickedEvent;
        IronSourceEvents.onInterstitialAdOpenedEvent -= InterstitialAdOpenedEvent;
        IronSourceEvents.onInterstitialAdClosedEvent -= InterstitialAdClosedEvent;



        //IronSourceEvents.onRewardedVideoAdOpenedEvent -= RewardedVideoAdOpenedEvent;
        //IronSourceEvents.onRewardedVideoAdClickedEvent -= RewardedVideoAdClickedEvent;
        //IronSourceEvents.onRewardedVideoAdClosedEvent -= RewardedVideoAdClosedEvent;
        //IronSourceEvents.onRewardedVideoAvailabilityChangedEvent -= RewardedVideoAvailabilityChangedEvent;
        //IronSourceEvents.onRewardedVideoAdStartedEvent -= RewardedVideoAdStartedEvent;
        //IronSourceEvents.onRewardedVideoAdEndedEvent -= RewardedVideoAdEndedEvent;
        IronSourceEvents.onRewardedVideoAdRewardedEvent -= RewardedVideoAdRewardedEvent;
        //IronSourceEvents.onRewardedVideoAdShowFailedEvent -= RewardedVideoAdShowFailedEvent;
    }
}
