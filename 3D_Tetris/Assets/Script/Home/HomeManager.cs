using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.PlayerProfile
{
    public class HomeManager : MonoBehaviour
    {
        [SerializeField] string _lvlSceneName;
        [SerializeField] private TextMeshProUGUI _lvlText;
        
        private void Start()
        {
            PlayerSaveProfile.instance.CheckWin();
            int lvl = PlayerSaveProfile.instance._lvl;
            _lvlText.text = (PlayerSaveProfile.instance._lvl+1).ToString() + " lvl";
            
        }

        public void StartLvl()
        {
            SelectLvlSettings(PlayerSaveProfile.instance.GetCurrentLvlData());
            SceneManager.LoadScene(_lvlSceneName);
        }

        private void SelectLvlSettings(LvlSettings lvl)
        { 
            DOTween.KillAll();
            LvlLoader.instance.Select(lvl);
        }
    }
}