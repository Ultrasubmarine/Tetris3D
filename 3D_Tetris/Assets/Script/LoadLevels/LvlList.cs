using UnityEngine;

namespace Script
{
    [CreateAssetMenu(fileName = "LvlList", menuName = "Lvl", order = 0)]
    public class LvlList : ScriptableObject
    {
        public LvlSettings[] lvls;
        
        public int firstRepeatLvl; //index
    }
}