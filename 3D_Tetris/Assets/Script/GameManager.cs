using Script.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _losePanels;
    
    [SerializeField] private BottomPanelAnimation _settingsPanel;
    [SerializeField] private BottomPanelAnimation _gameplayBottomPanel;
    
    [SerializeField] private GameObject _winPanel;
    
    private TetrisFSM _fsm;

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
        _winPanel.SetActive(true);
    }

    public void ClearPlace()
    {
        RealizationBox.Instance.elementCleaner.DeleteAllElements();
        RealizationBox.Instance.matrix.Clear();
        
        RealizationBox.Instance.haightHandler.CalculateHeight();
        RealizationBox.Instance.gameCamera.SetPositionWithoutAnimation();
    }
}