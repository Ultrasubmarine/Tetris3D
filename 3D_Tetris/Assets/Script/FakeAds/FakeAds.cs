using System;
using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;
using UnityEngine.UI;

public class FakeAds : MonoBehaviour
{
    
    [SerializeField] private TimerView _timerView;
    [SerializeField] private GameObject _adsPanel;
    [SerializeField] private GameObject _closeButtonObj;
    [SerializeField] private Button _closeButton;
    
    [SerializeField] private float _delay = 10;
    private Action _callBack;

    private void Start()
    {
        _closeButton.onClick.AddListener(CloseAds);
        AdsManager.instance._fakeAds = this;
    }

    private void OnDestroy()
    {
        AdsManager.instance._fakeAds = null;
    }

    private void CloseAds()
    {
        _callBack?.Invoke();
        _adsPanel.SetActive(false);
    }

    public void StartAds( Action callBack)
    {
        _timerView.Reset();
        _closeButtonObj.SetActive(false);
        _adsPanel.SetActive(true);
        _callBack = callBack;
        
        var timer = TimersKeeper.Schedule(_delay);
        timer.onStateChanged += OnTimerStateChanged;
        _timerView.SetTimer(ref timer);
    }

    private void OnTimerStateChanged( TimerState newState)
    {
        switch (newState)
        {
            case TimerState.Completed:
            {
                _closeButtonObj.SetActive(true);
                break;
            }
            default: break;
        }
    }
}
