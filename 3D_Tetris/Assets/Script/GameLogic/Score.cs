using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Score : MonoBehaviour {

    [FormerlySerializedAs("ScoreForWin")] [SerializeField] int _ScoreForWin;
    [FormerlySerializedAs("CurrentScore")] [SerializeField] int _CurrentScore;

    [FormerlySerializedAs("ScoreText")] [SerializeField] Text _ScoreText;

	// Use this for initialization
	void Start () {
        Messenger<int>.AddListener(GameEvent.DESTROY_LAYER.ToString(), ScoreeIncrement);
        _ScoreText.text = _CurrentScore.ToString() + "/" + _ScoreForWin.ToString() + " m";
    }

    void OnDestroy()
    {
        Messenger<int>.RemoveListener(GameEvent.DESTROY_LAYER.ToString(), ScoreeIncrement);
    }

    public void ScoreeIncrement(int layer)
    {
        _CurrentScore += 9;
        _ScoreText.text = _CurrentScore.ToString() + "/" + _ScoreForWin.ToString() + " m";
        Messenger<int>.Broadcast(GameEvent.CURRENT_SCORE.ToString(), _CurrentScore);
        CheckWin();
    }

    public void CheckWin()
    {
        if(_CurrentScore >= _ScoreForWin)
        {
            Messenger.Broadcast(GameEvent.WIN_GAME.ToString());
            Debug.Log("WIN");
        }
    }
}
