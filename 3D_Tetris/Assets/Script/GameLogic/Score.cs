using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Score : MonoBehaviour {

    [FormerlySerializedAs("ScoreForWin")] [SerializeField] int _ScoreForWin;
    [FormerlySerializedAs("CurrentScore")] [SerializeField] int _CurrentScore;

    [FormerlySerializedAs("ScoreText")] [SerializeField] Text _ScoreText;

	// Use this for initialization
	void Start () {
        Messenger<int>.AddListener(GameEvent.DESTROY_LAYER.ToString(), ScoreIncrement);
        Messenger.AddListener(GameManager.CLEAR_ALL, ClearScore);
        
        _ScoreText.text = _CurrentScore.ToString() + "/" + _ScoreForWin.ToString() + " m";
    }

    void OnDestroy()
    {
        Messenger<int>.RemoveListener(GameEvent.DESTROY_LAYER.ToString(), ScoreIncrement);
        Messenger.RemoveListener(GameManager.CLEAR_ALL, ClearScore);
    }

    void ScoreIncrement(int layer)
    {
        _CurrentScore += 9;
        _ScoreText.text = _CurrentScore.ToString() + "/" + _ScoreForWin.ToString() + " m";
        Messenger<int>.Broadcast(GameEvent.CURRENT_SCORE.ToString(), _CurrentScore);
        CheckWin();
    }

    void CheckWin()
    {
        if (_CurrentScore < _ScoreForWin) return;
        Messenger.Broadcast(GameEvent.WIN_GAME.ToString());
        Debug.Log("WIN");
    }

    private void ClearScore() {
        _CurrentScore = 0;
        _ScoreText.text = _CurrentScore.ToString() + "/" + _ScoreForWin.ToString() + " m";
    }
}
