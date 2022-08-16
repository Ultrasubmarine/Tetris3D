using UnityEngine;

namespace Script
{
    public class AdsManager : MonoBehaviourSingleton<AdsManager>
    {
        [SerializeField] private bool useSayGamesAdds = true;

        public bool ShowAds()
        {
            return true;
        }

    }
}