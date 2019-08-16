using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

enum GameState {
    Play,
    Replay,
    Win,
    End,
}

public class GameManager : MonoBehaviour {
    
    [SerializeField] StateMachine _Machine;
    [SerializeField] GameObject _Controller;

    static public string CLEAR_ALL = "CLEAR_ALL";
    
    public void StartGame() {

        _Controller.SetActive(true);
        _Machine.ChangeState(EMachineState.Empty);
    }

    public void ReplayGame() {
        _Machine.ChangeState(EMachineState.NotActive);
        Messenger.Broadcast(CLEAR_ALL);
    }

    public void End() {
        _Controller.SetActive(false);
    }
    
}