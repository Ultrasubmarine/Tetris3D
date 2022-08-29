using System;
using UnityEngine;

namespace Script
{
    public class AdsManager : MonoBehaviourSingleton<AdsManager>
    {
        [SerializeField] private bool useSayGamesAdds = true;
        
        public bool isAds { get; private set; } = true;
        
        public void ShowInterstitial(Action callBack)
        {
            callBack.Invoke();
        }

        public void ShowRewarded(Action<bool> callBack)
        {
            callBack.Invoke(true);
        }
        
        
    }
}