using System;
using Helper.Patterns.Messenger;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Score : MonoBehaviour {

   [SerializeField] int _ScoreForWin;
   [SerializeField] int _CurrentScore;

    [FormerlySerializedAs("ScoreText")] [SerializeField] Text _ScoreText;

//    StateMachine _machine;

    void Awake() {
//        _machine = FindObjectOfType<StateMachine>();
    }

    // Use this for initialization
	void Start () {
//        Messenger<int>.AddListener(GameEvent.DESTROY_LAYER.ToString(), ScoreIncrement);
//        Messenger.AddListener(StateMachine.StateMachineKey + EMachineState.NotActive, ClearScore);
        
        _ScoreText.text = _CurrentScore.ToString() + "/" + _ScoreForWin.ToString() + " m";
    }

    void OnDestroy()
    {
//        Messenger<int>.RemoveListener(GameEvent.DESTROY_LAYER.ToString(), ScoreIncrement);
//        Messenger.RemoveListener(StateMachine.StateMachineKey + EMachineState.NotActive, ClearScore);
    }

    void ScoreIncrement(int layer)
    {
        _CurrentScore += 9;
        _ScoreText.text = _CurrentScore.ToString() + "/" + _ScoreForWin.ToString() + " m";
        Messenger<int>.Broadcast(GameEvent.CURRENT_SCORE.ToString(), _CurrentScore);
//        CheckWin();
    }

    public bool CheckWin()
    {
        return _CurrentScore >= _ScoreForWin;
//        _machine.ChangeState(EMachineState.Win);
    }

    private void ClearScore() {
        _CurrentScore = 0;
        _ScoreText.text = _CurrentScore.ToString() + "/" + _ScoreForWin.ToString() + " m";
    }
}
