using System;
using Script.Influence;
using Script.UI;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private PausePanel _pausePanel;

    [SerializeField] private BottomPanelAnimation _pausePanel2;
    
    [SerializeField] private BottomPanelAnimation _gamePanel;
    
    [SerializeField] private GameObject _pauseIcon;

    private InfluenceManager _influenceManager;

    private void Awake()
    {
        _influenceManager = RealizationBox.Instance.influenceManager;
    }

    public void SetPauseGame(bool isPause)
    {
        /*_pauseIcon.SetActive(!isPause);
        _pausePanel.SetPauseGame(isPause);*/

        _influenceManager.enabled = !isPause;
        
        if (isPause)
        {
            _pausePanel2.Show();
            _gamePanel.Hide();
        }
        else
        {
            _gamePanel.Show();
            _pausePanel2.Hide();
        }
        
    }
}