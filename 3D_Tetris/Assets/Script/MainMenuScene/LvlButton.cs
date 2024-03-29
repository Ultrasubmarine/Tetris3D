﻿using DG.Tweening;
using Script;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LvlButton : MonoBehaviour
{
    [SerializeField] string _lvlName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenLvl()
    {
        SceneManager.LoadScene(_lvlName);
    }
    
    public void OpenLvlAfterAds()
    {
        AdsManager.instance.ShowInterstitial(OpenLvl);
    }
    
    public void SelectLvlSettings(LvlSettings lvl)
    { 
        DOTween.KillAll();
       LvlLoader.instance.Select(lvl);
    }
}
