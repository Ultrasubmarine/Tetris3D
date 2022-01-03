using System;
using DG.Tweening;
using Script.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _losePanels;
    
    [SerializeField] private BottomPanelAnimation _settingsPanel;
    [SerializeField] private BottomPanelAnimation _gameplayBottomPanel;
    
    [SerializeField] private CanvasGroup _winPanel;
    
    private TetrisFSM _fsm;

    public event Action OnReplay;
    
    private void Start()
    {
        _fsm = RealizationBox.Instance.FSM;
        Invoke( nameof(LastStart), 1f);
    }

    private void LastStart()
    {
        _fsm.AddListener(TetrisState.LoseGame, OnLoseGame);
    }

    public void StartGame()
    {
        _fsm.StartFSM();
    }

    private void OnLoseGame()
    {
        foreach (var panel in _losePanels)
        {
            panel.SetActive(true);
        }
        _settingsPanel.Show();
        _gameplayBottomPanel.Hide();
    }
    
    private void OnWinGame()
    {
        _winPanel.gameObject.SetActive(true);
    }

    public void ClearPlace()
    {
        RealizationBox.Instance.projectionLineManager.Clear();
        RealizationBox.Instance.projection.Destroy();
        
        RealizationBox.Instance.elementCleaner.DeleteAllElements();
        RealizationBox.Instance.matrix.Clear();
        
        RealizationBox.Instance.slowManager.DeleteAllSlows();
        OnReplay?.Invoke();
        RealizationBox.Instance.haightHandler.CalculateHeight();
        RealizationBox.Instance.gameCamera.SetPositionWithoutAnimation();
        RealizationBox.Instance.speedChanger.ResetSpeed();
        RealizationBox.Instance.generatorChanger.ResetGenerator();
        RealizationBox.Instance.starsManager.Clear();
       
        //Add boosters
    }

    public void ShowWinPanel()
    {
        _winPanel.gameObject.SetActive(true);
        _winPanel.DOFade(1, 0.4f);
        _winPanel.transform.DOMoveY(_winPanel.transform.position.y, 0.4f).From(_winPanel.transform.position.y - 250);
    }
}