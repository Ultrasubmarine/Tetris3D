using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] private int _ScoreForWin;
    
    [SerializeField] private Text _ScoreText;
    
    
    private int _CurrentScore = 0;
    
    
    public bool CheckWin()
    {
        return _CurrentScore >= _ScoreForWin;
    }
    
    private void Start()
    {
        _ScoreText.text = _CurrentScore+ "/" + _ScoreForWin + " m";
        
        RealizationBox.Instance.matrix.OnDestroyLayer += ScoreIncrement;
        RealizationBox.Instance.elementCleaner.onDeleteAllElements += ClearScore;
    }
    
    private void ScoreIncrement(int layer)
    {
        _CurrentScore += 9;
        _ScoreText.text = _CurrentScore + "/" + _ScoreForWin + " m";
    }

    private void ClearScore()
    {
        _CurrentScore = 0;
        _ScoreText.text = _CurrentScore + "/" + _ScoreForWin + " m";
    }
}