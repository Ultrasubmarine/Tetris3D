using Script.Influence;
using Script.UI;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private PausePanel _pausePanel;

    
    [SerializeField] private GameObject _pauseIcon;

    public void SetPauseGame(bool isPause)
    {
        _pauseIcon.SetActive(!isPause);
        _pausePanel.SetPauseGame(isPause);
    }
}