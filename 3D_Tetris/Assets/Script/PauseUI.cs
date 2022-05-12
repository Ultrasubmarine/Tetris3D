using System;
using System.Collections.Generic;
using DG.Tweening;
using Script.Controller;
using Script.GameLogic.Stars;
using Script.Influence;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    public Action<bool> onPauseStateChange;

    public bool isPause => !_influenceManager.enabled;
    
    [SerializeField] private BottomPanelAnimation _pausePanel;
    [SerializeField] private BottomPanelAnimation _gamePanel;
    [SerializeField] private BottomPanelAnimation _topGamePanel;
    
    private InfluenceManager _influenceManager;
    private MovementJoystick _joystick;
    private TapsEvents _tapsEvents;
    private StarUIAnimation _starUIAnimation;
    
    [SerializeField] private GameObject _pauseIcon;
    [SerializeField] private GameObject _playIcon;

    private Toggle _toggle;
    private List<Tween> pauseT;
    
    private void Awake()
    {
        _influenceManager = RealizationBox.Instance.influenceManager;
        _joystick = RealizationBox.Instance.joystick;
        _tapsEvents = RealizationBox.Instance.tapsEvents;
        _starUIAnimation = RealizationBox.Instance.starUIAnimation;
        _toggle = GetComponent<Toggle>();
       
    }

    public void SetPauseGame(bool isPause)
    {
        _influenceManager.enabled = !isPause;

        _starUIAnimation.Pause(isPause);
        if (isPause)
        {
          //  pauseT = DOTween.PlayingTweens();
          //  DOTween.Pause(pauseT);
         //   _starUIAnimation.Pause(isPause);
            _pausePanel.Show();
            _gamePanel.Hide();
            _topGamePanel.Hide();
            
            _joystick.Hide();
            _joystick.enabled = false;
            _tapsEvents.enabled = false;

            _pauseIcon.SetActive(true);
            _playIcon.SetActive(false);
            _toggle.isOn= true;
            
         //   DOTween.Pause(_starUIAnimation);
        }
        else
        {
           // DOTween.Play(pauseT);
          //  _starUIAnimation.Pause(isPause);
            _topGamePanel.Show();
            _gamePanel.Show();
            _pausePanel.Hide();
            
            _joystick.enabled = true;
            _tapsEvents.enabled = true;
            
            _pauseIcon.SetActive(false);
            _playIcon.SetActive(true);
            _toggle.isOn = false;
        }
        onPauseStateChange?.Invoke(isPause);
    }
}