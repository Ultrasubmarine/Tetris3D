using System;
using TMPro;
using UnityEngine;

namespace Script.PlayerProfile
{
    public class HomeManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _lvlText;
        
        [SerializeField] private LvlList _lvlList;
        private void Start()
        {
            int lvl = PlayerSaveProfile.instance._lvl;
            
            _lvlText.text = PlayerSaveProfile.instance._lvl.ToString();
            
        }

        private void IncrementLvl()
        {
            if(PlayerSaveProfile.instance.lvl)
        }
        
    }
}