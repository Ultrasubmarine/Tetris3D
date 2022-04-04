using UnityEngine;

namespace Script
{
    
    //[Destr]
    public class LvlLoader : Singleton<LvlLoader>
    {
        public LvlSettings lvlSettings => _lvlSettings;
        
        private LvlSettings _lvlSettings;
        
        protected override void Init()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        public void Select(LvlSettings lvl)
        {
            _lvlSettings = lvl;
        }
        
    }
}