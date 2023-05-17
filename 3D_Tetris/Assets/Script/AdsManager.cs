using System;
using UnityEngine;

namespace Script
{
    public class AdsManager : MonoBehaviourSingleton<AdsManager>
    {
        [SerializeField] private bool useSayGamesAdds = true;
        
        public bool isAds { get; private set; } = true;
        
        public bool isShowingAds { get; private set; } = false;
        
        public void ShowInterstitial(Action callBack)
        {
           // if (SayKit.isInterstitialAvailable())
           // {
           //     isShowingAds = true;
               // SayKit.showInterstitial(() =>
                // {
                //     callBack.Invoke();
                //     Invoke(nameof(ShowingFinish), 1f);
                // });
          //  }
            // else
            // {
            //     callBack.Invoke();
            // }
        }

        public void ShowRewarded(Action<bool> callBack)
        {
            // if (SayKit.isRewardedAvailable())
            // {
            //     isShowingAds = true;
            //     SayKit.showRewarded((b) =>
            //     {
            //         callBack.Invoke(b);
            //         Invoke(nameof(ShowingFinish), 1f);
            //     });
            // }
        }

        public void ShowingFinish()
        {
            isShowingAds = false;
        }
    }
}