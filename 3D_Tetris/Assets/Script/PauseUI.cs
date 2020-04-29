using Script.Influence;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private BottomPanelAnimation _pausePanel;
    
    [SerializeField] private BottomPanelAnimation _gamePanel;

    private MoveTouchController _moveTouchController;
    private InfluenceManager _influenceManager;

    private void Awake()
    {
        _influenceManager = RealizationBox.Instance.influenceManager;
        _moveTouchController = RealizationBox.Instance.moveTouchController;
    }

    public void SetPauseGame(bool isPause)
    {
        _influenceManager.enabled = !isPause;
        
        if (isPause)
        {
            _pausePanel.Show();
            _gamePanel.Hide();
            
            _moveTouchController.OnBreakOn();
            _moveTouchController.enabled = false;
        }
        else
        {
            _gamePanel.Show();
            _pausePanel.Hide();
            
            _moveTouchController.enabled = true;
        }
        
    }
}