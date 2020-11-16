using Script.Controller;
using Script.Influence;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private BottomPanelAnimation _pausePanel;
    
    [SerializeField] private BottomPanelAnimation _gamePanel;

    private InfluenceManager _influenceManager;
    private MovementJoystick _joystick;
    private TapsEvents _tapsEvents;
    
    [SerializeField] private GameObject _pauseIcon;
    [SerializeField] private GameObject _playIcon;

    private Toggle _toggle;
    
    private void Awake()
    {
        _influenceManager = RealizationBox.Instance.influenceManager;
        _joystick = RealizationBox.Instance.joystick;
        _tapsEvents = RealizationBox.Instance.tapsEvents;
        _toggle = GetComponent<Toggle>();
    }

    public void SetPauseGame(bool isPause)
    {
        _influenceManager.enabled = !isPause;
        
        if (isPause)
        {
            _pausePanel.Show();
            _gamePanel.Hide();
            
            _joystick.Hide();
            _joystick.enabled = false;
            _tapsEvents.enabled = false;

            _pauseIcon.SetActive(true);
            _playIcon.SetActive(false);
            _toggle.isOn = true;
        }
        else
        {
            _gamePanel.Show();
            _pausePanel.Hide();
            
            _joystick.enabled = true;
            _tapsEvents.enabled = true;
            
            _pauseIcon.SetActive(false);
            _playIcon.SetActive(true);
            _toggle.isOn = false;
        }
    }
}