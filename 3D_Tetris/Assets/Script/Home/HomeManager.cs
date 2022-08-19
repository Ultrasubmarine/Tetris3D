using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Script.PlayerProfile
{
    public class HomeManager : MonoBehaviour
    {
        [SerializeField] string _lvlSceneName;
        [SerializeField] private TextMeshProUGUI _lvlText;

        [SerializeField] private bool isCheat;
        [SerializeField] private List<GameObject> _cheatObjects;
        [SerializeField] private List<GameObject> _adsObjects;
        
        [SerializeField] public GameObject _skipLvl;

        [SerializeField] public Button _incrementStars;
        [SerializeField] public Button _incrementCoins;

        [SerializeField] public Toggle _soundTogle;
        
        private void Start()
        {
            PlayerSaveProfile.instance.CheckWin();
            int lvl = PlayerSaveProfile.instance._lvl;

            _incrementCoins.onClick.AddListener(OfferIncrementCoins);
            _incrementStars.onClick.AddListener(OfferIncrementStars);
            
            SetLvlText();
            SetSkipLvl();
            
            CheatSet();
            AdsSet();
            
            SetAudio();
        }

        public void SetSkipLvl()
        {
            _skipLvl.SetActive(PlayerSaveProfile.instance.canSkipLvl);
        }

        public void Skip()
        {
            PlayerSaveProfile.instance.IncrementLvl();
            SetLvlText();
            SetSkipLvl();
        }
        
        public void CheatSet()
        {
            foreach (var c in _cheatObjects)
            {
                c.SetActive(isCheat);
            }
        }
        
        public void AdsSet()
        {
            foreach (var c in _adsObjects)
            {
                c.SetActive(AdsManager.instance.isAds);
            }
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

        public void IncrementLvlData()
        {
            PlayerSaveProfile.instance.IncrementLvl();
            SetLvlText();
        }

        public void DecrementLvlData()
        {
            PlayerSaveProfile.instance.DecrementLvl();
            SetLvlText();
        }

        public void OfferIncrementStars()
        {
            PlayerSaveProfile.instance.AddStars(10);
        }

        public void OfferIncrementCoins()
        {
            PlayerSaveProfile.instance.AddCoins(50);
        }
        public void SetLvlText()
        {
            if (PlayerSaveProfile.instance._lvl > PlayerSaveProfile.instance._lvlData && isCheat)
                _lvlText.text = "level " + (PlayerSaveProfile.instance._lvl+1).ToString() + " (" + (PlayerSaveProfile.instance._lvlData+1) + ")";
            else
                _lvlText.text = "level " + (PlayerSaveProfile.instance._lvl + 1).ToString();
        }

        public void SoundChange(bool active)
        {
            PlayerSaveProfile.instance.SoundChange(!active);
            SetAudio();
        }

        public void SetAudio()
        {
            AudioListener.pause = PlayerSaveProfile.instance.muteAudio;
            _soundTogle.isOn = !PlayerSaveProfile.instance.muteAudio;
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus)
            {
                DOTween.KillAll();
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}