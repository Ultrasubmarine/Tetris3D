using System;
using Helper.Patterns.Messenger;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] private int _ScoreForWin;
    [SerializeField] private int _CurrentScore;

    [FormerlySerializedAs("ScoreText")] [SerializeField]
    private Text _ScoreText;

//    StateMachine _machine;

    private void Awake()
    {
//        _machine = FindObjectOfType<StateMachine>();
    }

    // Use this for initialization
    private void Start()
    {
//        Messenger<int>.AddListener(GameEvent.DESTROY_LAYER.ToString(), ScoreIncrement);
//        Messenger.AddListener(StateMachine.StateMachineKey + EMachineState.NotActive, ClearScore);

        _ScoreText.text = _CurrentScore.ToString() + "/" + _ScoreForWin.ToString() + " m";
    }

    private void OnDestroy()
    {
//        Messenger<int>.RemoveListener(GameEvent.DESTROY_LAYER.ToString(), ScoreIncrement);
//        Messenger.RemoveListener(StateMachine.StateMachineKey + EMachineState.NotActive, ClearScore);
    }

    private void ScoreIncrement(int layer)
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

    private void ClearScore()
    {
        _CurrentScore = 0;
        _ScoreText.text = _CurrentScore.ToString() + "/" + _ScoreForWin.ToString() + " m";
    }
}