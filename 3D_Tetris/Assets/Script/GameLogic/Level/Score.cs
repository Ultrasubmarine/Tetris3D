using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [FormerlySerializedAs("_ScoreForWin")] [SerializeField] private int _scoreForWin;
    
    [FormerlySerializedAs("_ScoreText")] [SerializeField] private Text _scoreText;
    
    private int _currentScore = 0;
    
    
    public bool CheckWin()
    {
        return _currentScore >= _scoreForWin;
    }
    
    private void Start()
    {
        _scoreText.text = _currentScore+ "/" + _scoreForWin + " m";
        
        RealizationBox.Instance.matrix.OnDestroyLayer += ScoreIncrement;
        RealizationBox.Instance.elementCleaner.onDeleteAllElements += ClearScore;
    }
    
    private void ScoreIncrement(int layer)
    {
        _currentScore += 9;
        _scoreText.text = _currentScore + "/" + _scoreForWin + " m";
    }

    private void ClearScore()
    {
        _currentScore = 0;
        _scoreText.text = _currentScore + "/" + _scoreForWin + " m";
    }
}