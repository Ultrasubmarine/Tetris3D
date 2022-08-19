using UnityEngine;

namespace Script
{
    public class AdsManager : MonoBehaviourSingleton<AdsManager>
    {
        [SerializeField] private bool useSayGamesAdds = true;
        
        public bool isAds { get; private set; } = false;
        public bool ShowAds()
        {
            return true;
        }

    }
}