using UnityEngine;

namespace Script
{
    
    //[Destr]
    public class LvlLoader : MonoBehaviourSingleton<LvlLoader>
    {
        public LvlSettings lvlSettings => _lvlSettings;
        
        private LvlSettings _lvlSettings;

        public void Select(LvlSettings lvl)
        {
            _lvlSettings = lvl;
        }
        
    }
}