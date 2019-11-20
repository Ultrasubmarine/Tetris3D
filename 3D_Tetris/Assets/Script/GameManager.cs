using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _losePanel;
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
        _losePanel.SetActive(true);
    }
    
    private void OnWinGame()
    {
        _winPanel.SetActive(true);
    }
    
    public void ReplayGame()
    {
        RealizationBox.Instance.elementCleaner.DeleteAllElements();
        RealizationBox.Instance.matrix.Clear();
        
        _fsm.StartFSM();
    }
}