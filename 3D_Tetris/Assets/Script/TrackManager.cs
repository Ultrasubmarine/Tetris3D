using Script.PlayerProfile;

namespace Script
{
    public class TrackManager 
    {
        public static void LvlStart()
        {
            SayKit.trackLevelStarted(PlayerSaveProfile.instance._lvl + 1);
        }

        public static void LvlFailed(int StarsAmount)
        {
            SayKit.trackLevelFailed(PlayerSaveProfile.instance._lvl + 1, StarsAmount);
        }

        public static void LvlCompleted(int StarsAmount)
        {
            SayKit.trackLevelCompleted(PlayerSaveProfile.instance._lvl + 1, StarsAmount); 
        }
        
    }
}